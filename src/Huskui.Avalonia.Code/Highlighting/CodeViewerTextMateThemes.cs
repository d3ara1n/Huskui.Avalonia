using System.IO;
using System.Text;
using TextMateSharp.Internal.Themes.Reader;
using TextMateSharp.Themes;

namespace Huskui.Avalonia.Code.Highlighting;

internal static class CodeViewerTextMateThemes
{
    public static IRawTheme Light { get; } = Parse(THEME_LIGHT_JSON);

    public static IRawTheme Dark { get; } = Parse(THEME_DARK_JSON);

    private static IRawTheme Parse(string json)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        using var reader = new StreamReader(stream);
        return ThemeReader.ReadThemeSync(reader);
    }

    private const string THEME_LIGHT_JSON = """
        {
          "name": "Huskui Radix Light",
          "type": "light",
          "colors": {
            "editor.background": "#FFFDFD",
            "editor.foreground": "#FF1C2024",
            "editor.selectionBackground": "#FFD6EFFF",
            "editor.lineHighlightBackground": "#0F0D74CE",
            "editorCursor.foreground": "#FF205D9E",
            "editorLineNumber.foreground": "#FF7E868C",
            "editorLineNumber.activeForeground": "#FF1C2024"
          },
          "tokenColors": [
            {
              "scope": ["comment", "punctuation.definition.comment"],
              "settings": { "foreground": "#FF5A7D48", "fontStyle": "italic" }
            },
            {
              "scope": ["string", "string.quoted", "markup.inline.raw", "markup.fenced_code.block"],
              "settings": { "foreground": "#FF18794E" }
            },
            {
              "scope": ["constant.numeric", "constant.language.boolean", "constant.language.null"],
              "settings": { "foreground": "#FF9E6C00" }
            },
            {
              "scope": ["keyword", "storage", "storage.type", "keyword.operator.word"],
              "settings": { "foreground": "#FF205D9E" }
            },
            {
              "scope": ["entity.name.type", "entity.name.class", "support.class", "support.type"],
              "settings": { "foreground": "#FF5B5BD6" }
            },
            {
              "scope": ["entity.name.function", "support.function", "meta.function-call", "variable.function"],
              "settings": { "foreground": "#FF0F3D7A" }
            },
            {
              "scope": ["entity.name.tag", "meta.tag", "punctuation.definition.tag"],
              "settings": { "foreground": "#FF0D74CE" }
            },
            {
              "scope": ["entity.other.attribute-name", "support.type.property-name", "meta.property-name"],
              "settings": { "foreground": "#FF8D3A12" }
            },
            {
              "scope": ["variable", "meta.definition.variable", "parameter"],
              "settings": { "foreground": "#FFB54512" }
            },
            {
              "scope": ["support.constant", "constant.character.escape", "constant.other.symbol"],
              "settings": { "foreground": "#FF7E868C" }
            },
            {
              "scope": ["keyword.operator", "punctuation", "meta.brace", "meta.delimiter"],
              "settings": { "foreground": "#FF60646C" }
            },
            {
              "scope": ["markup.heading", "markup.heading entity.name"],
              "settings": { "foreground": "#FF0F3D7A", "fontStyle": "bold" }
            },
            {
              "scope": ["markup.bold"],
              "settings": { "foreground": "#FF1C2024", "fontStyle": "bold" }
            },
            {
              "scope": ["markup.italic"],
              "settings": { "foreground": "#FF1C2024", "fontStyle": "italic" }
            },
            {
              "scope": ["markup.list", "markup.quote"],
              "settings": { "foreground": "#FF60646C" }
            },
            {
              "scope": ["invalid", "invalid.illegal"],
              "settings": { "foreground": "#FFFFFFFF", "background": "#FFCD2B31" }
            }
          ]
        }
        """;

    private const string THEME_DARK_JSON = """
        {
          "name": "Huskui Radix Dark",
          "type": "dark",
          "colors": {
            "editor.background": "#FF111113",
            "editor.foreground": "#FFE7E9EC",
            "editor.selectionBackground": "#1A7CC4FA",
            "editor.lineHighlightBackground": "#147CC4FA",
            "editorCursor.foreground": "#FF8EC8FF",
            "editorLineNumber.foreground": "#FF9BA1A6",
            "editorLineNumber.activeForeground": "#FFE7E9EC"
          },
          "tokenColors": [
            {
              "scope": ["comment", "punctuation.definition.comment"],
              "settings": { "foreground": "#FF7FC17E", "fontStyle": "italic" }
            },
            {
              "scope": ["string", "string.quoted", "markup.inline.raw", "markup.fenced_code.block"],
              "settings": { "foreground": "#FF86E1A6" }
            },
            {
              "scope": ["constant.numeric", "constant.language.boolean", "constant.language.null"],
              "settings": { "foreground": "#FFFFD36E" }
            },
            {
              "scope": ["keyword", "storage", "storage.type", "keyword.operator.word"],
              "settings": { "foreground": "#FF8EC8FF" }
            },
            {
              "scope": ["entity.name.type", "entity.name.class", "support.class", "support.type"],
              "settings": { "foreground": "#FFC4B5FD" }
            },
            {
              "scope": ["entity.name.function", "support.function", "meta.function-call", "variable.function"],
              "settings": { "foreground": "#FFB1C9FF" }
            },
            {
              "scope": ["entity.name.tag", "meta.tag", "punctuation.definition.tag"],
              "settings": { "foreground": "#FF7CC4FA" }
            },
            {
              "scope": ["entity.other.attribute-name", "support.type.property-name", "meta.property-name"],
              "settings": { "foreground": "#FFFFB381" }
            },
            {
              "scope": ["variable", "meta.definition.variable", "parameter"],
              "settings": { "foreground": "#FFFFB381" }
            },
            {
              "scope": ["support.constant", "constant.character.escape", "constant.other.symbol"],
              "settings": { "foreground": "#FF9BA1A6" }
            },
            {
              "scope": ["keyword.operator", "punctuation", "meta.brace", "meta.delimiter"],
              "settings": { "foreground": "#FFB0B4BA" }
            },
            {
              "scope": ["markup.heading", "markup.heading entity.name"],
              "settings": { "foreground": "#FFB1C9FF", "fontStyle": "bold" }
            },
            {
              "scope": ["markup.bold"],
              "settings": { "foreground": "#FFE7E9EC", "fontStyle": "bold" }
            },
            {
              "scope": ["markup.italic"],
              "settings": { "foreground": "#FFE7E9EC", "fontStyle": "italic" }
            },
            {
              "scope": ["markup.list", "markup.quote"],
              "settings": { "foreground": "#FFB0B4BA" }
            },
            {
              "scope": ["invalid", "invalid.illegal"],
              "settings": { "foreground": "#FFFFFFFF", "background": "#FFCD2B31" }
            }
          ]
        }
        """;
}
