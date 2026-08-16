using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;

// -----------------------------------------------------------------------------
// Huskui grammar snapshot tool
//
//   dotnet run -- extract <outputDir>   extract gzip assets + manifest from the
//                                      TextMateSharp.Grammars package reference
//   dotnet run -- verify <assetsDir>    compare the self-hosted locator in
//                                      Huskui.Avalonia.Code against upstream
//                                      (scope resolution + full token streams)
//
// The upstream RegistryOptions is the single source of truth: every mapping in
// the manifest is the value upstream itself resolves, so quirks (first-grammar
// scope for extension lookup, cross-definition language matching) carry over
// verbatim instead of being re-implemented here.
// -----------------------------------------------------------------------------

return args[0] switch
{
    "extract" => Extract(args[1]),
    "verify" => Verify(args[1]),
    _ => Fail("usage: extract <outputDir> | verify <assetsDir>"),
};

static int Fail(string message)
{
    Console.Error.WriteLine(message);
    return 1;
}

static int Extract(string outputDir)
{
    var upstream = new RegistryOptions(ThemeName.DarkPlus);
    var assembly = typeof(RegistryOptions).Assembly;
    var resourceNames = assembly.GetManifestResourceNames();

    // scope -> grammar resource, resolved from each extension's package.json exactly like
    // upstream InitializeAvailableGrammars does: the embedding directory is the resource-name
    // prefix, so ./syntaxes/diff.tmLanguage.json in git/ and in diff/ cannot collide.
    var grammars = new Dictionary<string, string>(StringComparer.Ordinal);
    foreach (
        var packageResource in resourceNames
           .Where(n => n.EndsWith(".package.json", StringComparison.OrdinalIgnoreCase))
           .OrderBy(n => n, StringComparer.Ordinal)
    )
    {
        using var stream = assembly.GetManifestResourceStream(packageResource);
        if (stream is null)
            continue;

        using var document = JsonDocument.Parse(stream);
        var contributes = document.RootElement.TryGetProperty("contributes", out var c)
            && c.ValueKind == JsonValueKind.Object
            && c.TryGetProperty("grammars", out var g)
            && g.ValueKind == JsonValueKind.Array
                ? g
                : (JsonElement?)null;
        if (contributes is null)
            continue;

        // "TextMateSharp.Grammars.Resources.Grammars.<dir>.package.json" -> "...Grammars.<dir>."
        var prefix = packageResource[..^"package.json".Length];

        foreach (var grammar in contributes.Value.EnumerateArray())
        {
            if (grammar.ValueKind != JsonValueKind.Object)
                continue;

            string? scope = null;
            string? path = null;
            if (
                grammar.TryGetProperty("scopeName", out var scopeElement)
                && scopeElement.ValueKind == JsonValueKind.String
            )
                scope = scopeElement.GetString();
            if (
                grammar.TryGetProperty("path", out var pathElement)
                && pathElement.ValueKind == JsonValueKind.String
            )
                path = pathElement.GetString();

            if (string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(path))
                continue;

            var trimmedPath = path!.Trim();
            if (trimmedPath.StartsWith("./", StringComparison.Ordinal))
                trimmedPath = trimmedPath[2..];
            var full = prefix + trimmedPath.Replace('/', '.');
            if (!resourceNames.Contains(full))
            {
                // Upstream embeds grammars via a *.json wildcard, so references to non-json files
                // (e.g. Regular Expressions (JavaScript).tmLanguage) dangle — those scopes are
                // unresolvable through upstream as well and are skipped to keep parity.
                Console.WriteLine($"skipping dangling grammar reference {full} in {packageResource}");
                continue;
            }

            if (!grammars.ContainsKey(scope!))
                grammars[scope!] = full;
        }
    }

    var languages = new List<LanguageEntry>(upstream.GetAvailableLanguages().Count);
    foreach (var language in upstream.GetAvailableLanguages())
    {
        languages.Add(
            new LanguageEntry(
                language.Id,
                language.Aliases?.ToList(),
                language.Extensions?.ToList(),
                upstream.GetScopeByLanguageId(language.Id)
            )
        );
    }

    var extensionScopes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
    foreach (var language in upstream.GetAvailableLanguages())
    {
        if (language.Extensions is null)
            continue;
        foreach (var extension in language.Extensions)
        {
            if (extension is null)
                continue;
            var scope = upstream.GetScopeByExtension(extension);
            if (scope is not null)
                extensionScopes[extension.ToLowerInvariant()] = scope;
        }
    }

    Directory.CreateDirectory(outputDir);
    long totalRaw = 0, totalGz = 0;
    var manifestGrammars = new Dictionary<string, string>(StringComparer.Ordinal);
    foreach (var (scope, resourceName) in grammars.OrderBy(p => p.Key, StringComparer.Ordinal))
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
            return Fail($"cannot open resource {resourceName}");

        var fileName = $"{scope}.json.gz";
        using (var output = File.Create(Path.Combine(outputDir, fileName)))
        using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
        {
            stream.CopyTo(gzip);
        }

        totalRaw += stream.Length;
        totalGz += new FileInfo(Path.Combine(outputDir, fileName)).Length;
        manifestGrammars[scope] = fileName;
    }

    var manifest = new SnapshotManifest(
        $"TextMateSharp.Grammars {typeof(RegistryOptions).Assembly.GetName().Version}",
        DateTime.UtcNow,
        languages,
        manifestGrammars,
        extensionScopes
    );
    File.WriteAllText(
        Path.Combine(outputDir, "manifest.json"),
        JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true })
    );

    Console.WriteLine($"extension packages scanned: {resourceNames.Count(n => n.EndsWith(".package.json", StringComparison.OrdinalIgnoreCase))}");
    Console.WriteLine($"grammars embedded:         {manifestGrammars.Count}");
    Console.WriteLine($"languages:                 {languages.Count}");
    Console.WriteLine($"extension mappings:        {extensionScopes.Count}");
    Console.WriteLine($"raw grammar payload:       {totalRaw:N0} bytes");
    Console.WriteLine($"gzip grammar payload:      {totalGz:N0} bytes");
    return 0;
}

static int Verify(string assetsDir)
{
    var upstream = new RegistryOptions(ThemeName.DarkPlus);
    var huskuiType = typeof(Huskui.Avalonia.Code.Controls.CodeViewer).Assembly.GetType(
        "Huskui.Avalonia.Code.Highlighting.CodeViewerRegistryOptions"
    );
    if (huskuiType is null)
        return Fail("CodeViewerRegistryOptions not found in Huskui.Avalonia.Code");
    var huskui = Activator.CreateInstance(huskuiType)!;
    var huskuiLocator = (IRegistryOptions)huskui;
    var resolveScope = huskuiType.GetMethod(
        "ResolveScopeName",
        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
    )!;

    var failures = 0;
    void Check(string what, string? expected, string? actual)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal))
        {
            failures++;
            Console.WriteLine($"MISMATCH {what}: upstream={expected} huskui={actual}");
        }
    }

    // 1. scope resolution parity: ids, aliases, extensions (dotted and bare)
    var inputs = new List<string>();
    foreach (var language in upstream.GetAvailableLanguages())
    {
        inputs.Add(language.Id);
        if (language.Aliases is not null)
            inputs.AddRange(language.Aliases);
        if (language.Extensions is not null)
            foreach (var extension in language.Extensions)
            {
                inputs.Add(extension!);
                inputs.Add(extension!.TrimStart('.'));
            }
    }
    // Huskui alias table (code, not data) exercised as well
    inputs.AddRange(
        new[] { "axaml", "xaml", "bash", "zsh", "sh", "ps1", "md", "csharp", "fsharp", "visualbasic" }
    );

    var upstreamResolve = UpstreamResolveFactory(upstream);
    foreach (var input in inputs.Where(i => !string.IsNullOrWhiteSpace(i)).Distinct())
    {
        var expected = upstreamResolve(input);
        var actual = (string?)resolveScope.Invoke(huskui, new object?[] { input });
        Check($"ResolveScopeName({input})", expected, actual);
    }
    Console.WriteLine($"scope resolution inputs:  {inputs.Distinct().Count()}");

    // 2. grammar loading + token stream parity through the real Registry path
    const string sample =
        "class Foo { int x = 42; }\n"
        + "\"a string\" // comment\n"
        + "<tag attr=\"v\">#macro</tag>\n"
        + "if (a >= 1.0) return null;";

    var scopes = new HashSet<string>(StringComparer.Ordinal);
    foreach (var definition in upstream.GetAvailableGrammarDefinitions())
        foreach (var grammar in definition.Contributes.Grammars)
            if (grammar.ScopeName is not null)
                scopes.Add(grammar.ScopeName);

    var upstreamRegistry = new Registry(upstream);
    var huskuiRegistry = new Registry(huskuiLocator);
    var loaded = 0;
    foreach (var scope in scopes.OrderBy(s => s, StringComparer.Ordinal))
    {
        var expectedGrammar = LoadQuietly(upstreamRegistry, scope);
        var actualGrammar = LoadQuietly(huskuiRegistry, scope);
        if ((expectedGrammar is null) != (actualGrammar is null))
        {
            failures++;
            Console.WriteLine($"MISMATCH grammar availability for {scope}");
            continue;
        }
        if (expectedGrammar is null || actualGrammar is null)
            continue;

        loaded++;
        var expectedTokens = Tokenize(expectedGrammar, sample);
        var actualTokens = Tokenize(actualGrammar, sample);
        if (!expectedTokens.SequenceEqual(actualTokens))
        {
            failures++;
            Console.WriteLine($"MISMATCH token stream for {scope}");
        }
    }
    Console.WriteLine($"grammars compared:        {loaded}/{scopes.Count}");

    // 3. locator surface sanity
    Check("GetInjections", "null", huskuiLocator.GetInjections("source.cs") is null ? "null" : "non-null");
    var defaultTheme = huskuiLocator.GetDefaultTheme();
    if (defaultTheme is null)
    {
        failures++;
        Console.WriteLine("MISMATCH GetDefaultTheme: huskui=null");
    }

    Console.WriteLine(
        failures == 0
            ? $"PASS (assets dir: {assetsDir})"
            : $"FAIL: {failures} mismatches"
    );
    return failures == 0 ? 0 : 1;
}

static Func<string, string?> UpstreamResolveFactory(RegistryOptions upstream)
{
    // Verbatim behavior of the pre-self-hosting CodeViewerRegistryOptions.ResolveScopeName
    var languageExtensions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["axaml"] = ".xml",
        ["bash"] = ".sh",
        ["c"] = ".c",
        ["cpp"] = ".cpp",
        ["cs"] = ".cs",
        ["csharp"] = ".cs",
        ["css"] = ".css",
        ["fs"] = ".fs",
        ["fsharp"] = ".fs",
        ["html"] = ".html",
        ["java"] = ".java",
        ["js"] = ".js",
        ["json"] = ".json",
        ["jsx"] = ".jsx",
        ["less"] = ".less",
        ["lua"] = ".lua",
        ["markdown"] = ".md",
        ["md"] = ".md",
        ["php"] = ".php",
        ["ps1"] = ".ps1",
        ["py"] = ".py",
        ["python"] = ".py",
        ["rb"] = ".rb",
        ["ruby"] = ".rb",
        ["rs"] = ".rs",
        ["rust"] = ".rs",
        ["scss"] = ".scss",
        ["shell"] = ".sh",
        ["sh"] = ".sh",
        ["sql"] = ".sql",
        ["ts"] = ".ts",
        ["tsx"] = ".tsx",
        ["typescript"] = ".ts",
        ["vb"] = ".vb",
        ["visualbasic"] = ".vb",
        ["xaml"] = ".xml",
        ["xml"] = ".xml",
        ["yaml"] = ".yml",
        ["yml"] = ".yml",
        ["zsh"] = ".sh",
    };

    string? ResolveExtension(string language) =>
        languageExtensions.TryGetValue(language, out var extension)
            ? extension
            : language.Contains('/')
                ? null
                : $".{language}";

    return input =>
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var normalized = input.Trim();

        if (normalized.StartsWith('.'))
            return upstream.GetScopeByExtension(normalized);

        var extension = ResolveExtension(normalized);
        if (extension is not null)
        {
            var scopeByExtension = upstream.GetScopeByExtension(extension);
            if (!string.IsNullOrWhiteSpace(scopeByExtension))
                return scopeByExtension;
        }

        foreach (var knownLanguage in upstream.GetAvailableLanguages())
        {
            if (string.Equals(knownLanguage.Id, normalized, StringComparison.OrdinalIgnoreCase))
                return upstream.GetScopeByLanguageId(knownLanguage.Id);

            if (knownLanguage.Aliases is null)
                continue;

            foreach (var alias in knownLanguage.Aliases)
            {
                if (string.Equals(alias, normalized, StringComparison.OrdinalIgnoreCase))
                    return upstream.GetScopeByLanguageId(knownLanguage.Id);
            }
        }

        return null;
    };
}

static IGrammar? LoadQuietly(Registry registry, string scope)
{
    try
    {
        return registry.LoadGrammar(scope);
    }
    catch (Exception)
    {
        return null;
    }
}

static List<string> Tokenize(IGrammar grammar, string sample)
{
    var tokens = new List<string>();
    IStateStack? state = null;
    foreach (var line in sample.Split('\n'))
    {
        var result = grammar.TokenizeLine(line, state, TimeSpan.MaxValue);
        state = result.RuleStack;
        foreach (var token in result.Tokens)
            tokens.Add($"{token.StartIndex}-{token.EndIndex}:{string.Join(" ", token.Scopes)}");
    }

    return tokens;
}

internal record LanguageEntry(
    string Id,
    List<string>? Aliases,
    List<string>? Extensions,
    string? Scope
);

internal record SnapshotManifest(
    string Source,
    DateTime GeneratedAtUtc,
    List<LanguageEntry> Languages,
    Dictionary<string, string> Grammars,
    Dictionary<string, string> ExtensionScopes
);
