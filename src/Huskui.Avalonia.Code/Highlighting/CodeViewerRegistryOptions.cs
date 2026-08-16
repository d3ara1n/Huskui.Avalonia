using System;
using System.Collections.Generic;
using TextMateSharp.Internal.Types;
using TextMateSharp.Registry;
using TextMateSharp.Themes;

namespace Huskui.Avalonia.Code.Highlighting;

internal sealed class CodeViewerRegistryOptions : IRegistryOptions
{
    private static readonly Dictionary<string, string> LanguageExtensions = new(
        StringComparer.OrdinalIgnoreCase
    )
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

    private static readonly GrammarManifest Manifest = GrammarManifest.Instance;

    public string? ResolveScopeName(string? language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return null;

        var normalized = language.Trim();

        if (normalized.StartsWith('.'))
        {
            return Manifest.GetScopeByExtension(normalized);
        }

        var extension = ResolveExtension(normalized);
        if (extension is not null)
        {
            var scopeByExtension = Manifest.GetScopeByExtension(extension);
            if (!string.IsNullOrWhiteSpace(scopeByExtension))
                return scopeByExtension;
        }

        foreach (var knownLanguage in Manifest.Languages)
        {
            if (string.Equals(knownLanguage.Id, normalized, StringComparison.OrdinalIgnoreCase))
            {
                return knownLanguage.Scope;
            }

            if (knownLanguage.Aliases is null)
                continue;

            foreach (var alias in knownLanguage.Aliases)
            {
                if (string.Equals(alias, normalized, StringComparison.OrdinalIgnoreCase))
                {
                    return knownLanguage.Scope;
                }
            }
        }

        return null;
    }

    // Theme.ParseInclude only consults the locator when the theme declares an include chain;
    // the Huskui Radix themes do not, so this member is unreachable and mirrors DefaultLocator.
    public IRawTheme GetTheme(string scopeName) => null!;

    public IRawGrammar GetGrammar(string scopeName) => Manifest.GetGrammar(scopeName)!;

    // TextMateSharp.Grammars RegistryOptions returns null here as well.
    public ICollection<string> GetInjections(string scopeName) => null!;

    public IRawTheme GetDefaultTheme() => CodeViewerTextMateThemes.Dark;

    private static string? ResolveExtension(string language)
    {
        if (LanguageExtensions.TryGetValue(language, out var extension))
            return extension;

        return language.Contains('/') ? null : $".{language}";
    }
}
