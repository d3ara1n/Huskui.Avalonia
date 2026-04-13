using System.Collections.Generic;
using System.Text;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using ColorCode;
using ColorCode.Parsing;
using ColorCode.Styling;

namespace Huskui.Avalonia.Code.Highlighting;

public class AvaloniaInlineFormatter : CodeColorizerBase
{
    private InlineCollection? _inlines = null;
    private readonly Dictionary<string, ISolidColorBrush> _brushCache = [];

    public AvaloniaInlineFormatter()
        : this(CodeViewerStyleDictionaries.Dark) { }

    public AvaloniaInlineFormatter(StyleDictionary styles)
        : base(styles, null) { }

    public InlineCollection FormatInlines(string sourceCode, ILanguage language)
    {
        _inlines = [];
        languageParser.Parse(sourceCode, language, Write);
        return _inlines;
    }

    protected override void Write(string parsedSourceCode, IList<Scope> scopes)
    {
        if (scopes is { Count: 0 } || string.IsNullOrEmpty(parsedSourceCode))
        {
            _inlines?.Add(new Run(parsedSourceCode));
            return;
        }

        WriteParsedSourceCode(parsedSourceCode, scopes);
    }

    private void WriteParsedSourceCode(string sourceCode, IList<Scope> scopes)
    {
        var sortedScopes = new List<Scope>(scopes);
        sortedScopes.Sort((a, b) => a.Index.CompareTo(b.Index));

        var currentPosition = 0;

        foreach (var scope in sortedScopes)
        {
            if (scope.Index > currentPosition)
            {
                var plainText = sourceCode.Substring(
                    currentPosition,
                    scope.Index - currentPosition
                );
                _inlines?.Add(new Run(plainText));
            }

            var scopeText = sourceCode.Substring(scope.Index, scope.Length);
            var scopeStyle = GetStyle(scope.Name);

            var run = new Run(scopeText);
            ApplyStyle(run, scopeStyle);

            _inlines?.Add(run);

            currentPosition = scope.Index + scope.Length;
        }

        if (currentPosition < sourceCode.Length)
        {
            var remainingText = sourceCode.Substring(currentPosition);
            _inlines?.Add(new Run(remainingText));
        }
    }

    private Style? GetStyle(string scopeName)
    {
        return Styles.Contains(scopeName) ? Styles[scopeName] : null;
    }

    private void ApplyStyle(Run run, Style? style)
    {
        if (style is null)
            return;

        if (!string.IsNullOrEmpty(style.Foreground))
            run.Foreground = GetCachedBrush(style.Foreground);

        if (style.Bold)
            run.FontWeight = FontWeight.Bold;

        if (style.Italic)
            run.FontStyle = FontStyle.Italic;
    }

    private ISolidColorBrush GetCachedBrush(string colorHex)
    {
        if (_brushCache.TryGetValue(colorHex, out var cached))
            return cached;

        var brush = new SolidColorBrush(
            Color.TryParse(colorHex, out var color) ? color : Colors.White
        );
        _brushCache[colorHex] = brush;
        return brush;
    }
}
