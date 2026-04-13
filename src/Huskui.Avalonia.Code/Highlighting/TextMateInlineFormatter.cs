using System;
using System.Collections.Generic;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using TextMateSharp.Grammars;
using TextMateSharp.Registry;
using TextMateSharp.Themes;
using AvaloniaFontStyle = Avalonia.Media.FontStyle;
using TextMateFontStyle = TextMateSharp.Themes.FontStyle;

namespace Huskui.Avalonia.Code.Highlighting;

internal sealed class TextMateInlineFormatter
{
    private readonly CodeViewerRegistryOptions _registryOptions;
    private readonly Dictionary<string, IBrush> _brushCache = [];

    private static Registry? registry = null;

    public TextMateInlineFormatter(CodeViewerRegistryOptions registryOptions)
    {
        _registryOptions = registryOptions;
    }

    public InlineCollection? FormatInlines(string sourceCode, string scopeName, IRawTheme theme)
    {
        registry ??= new Registry(_registryOptions);
        registry.SetTheme(theme);

        var grammar = registry.LoadGrammar(scopeName);
        if (grammar is null)
            return null;

        var colors = registry.GetTheme();
        var defaultForeground = GetEditorForeground(colors);
        var inlines = new InlineCollection();
        var lines = NormalizeLineEndings(sourceCode).Split('\n', StringSplitOptions.None);

        IStateStack? state = null;

        for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
        {
            var line = lines[lineIndex];
            var result = grammar.TokenizeLine(line, state, TimeSpan.MaxValue);
            state = result.RuleStack;

            foreach (var token in result.Tokens)
            {
                var start = Math.Min(token.StartIndex, line.Length);
                var end = Math.Min(token.EndIndex, line.Length);
                if (end <= start)
                    continue;

                var run = new Run(line.Substring(start, end - start));
                ApplyStyle(run, colors, token.Scopes, defaultForeground);
                inlines.Add(run);
            }

            if (lineIndex < lines.Length - 1)
                inlines.Add(new LineBreak());
        }

        return inlines;
    }

    private void ApplyStyle(Run run, Theme theme, IList<string> scopes, string defaultForeground)
    {
        var foreground = defaultForeground;
        var fontStyle = TextMateFontStyle.None;
        var foregroundResolved = false;
        var fontStyleResolved = false;

        foreach (var rule in theme.Match(scopes))
        {
            if (!foregroundResolved && rule.foreground != 0)
            {
                foreground = theme.GetColor(rule.foreground) ?? foreground;
                foregroundResolved = true;
            }

            if (!fontStyleResolved && rule.fontStyle > 0)
            {
                fontStyle = rule.fontStyle;
                fontStyleResolved = true;
            }

            if (foregroundResolved && fontStyleResolved)
                break;
        }

        run.Foreground = GetCachedBrush(foreground);

        if ((fontStyle & TextMateFontStyle.Bold) != 0)
            run.FontWeight = FontWeight.Bold;

        if ((fontStyle & TextMateFontStyle.Italic) != 0)
            run.FontStyle = AvaloniaFontStyle.Italic;
    }

    private static string GetEditorForeground(Theme theme)
    {
        var colors = theme.GetGuiColorDictionary();
        return colors.TryGetValue("editor.foreground", out var foreground)
            ? foreground
            : "#FF1C2024";
    }

    private IBrush GetCachedBrush(string color)
    {
        if (_brushCache.TryGetValue(color, out var brush))
            return brush;

        var parsed = Color.TryParse(color, out var value) ? value : Colors.White;
        brush = new SolidColorBrush(parsed);
        _brushCache[color] = brush;
        return brush;
    }

    private static string NormalizeLineEndings(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n');
}
