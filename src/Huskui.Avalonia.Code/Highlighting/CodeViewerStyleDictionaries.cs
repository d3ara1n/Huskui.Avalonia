using ColorCode.Styling;

namespace Huskui.Avalonia.Code.Highlighting;

internal static class CodeViewerStyleDictionaries
{
    public static StyleDictionary Light { get; } = CreateLight();

    public static StyleDictionary Dark { get; } = CreateDark();

    private static StyleDictionary CreateLight()
    {
        var styles = new StyleDictionary();

        Add(styles, "Plain Text", "#FF1C2024");
        Add(styles, "HTML Comment", "#FF5A7D48");
        Add(styles, "Html Tag Delimiter", "#FF0D74CE");
        Add(styles, "HTML Element ScopeName", "#FF0F3D7A");
        Add(styles, "HTML Attribute ScopeName", "#FF8D3A12");
        Add(styles, "HTML Attribute Value", "#FF0C7C59");
        Add(styles, "HTML Operator", "#FF0D74CE");
        Add(styles, "Comment", "#FF5A7D48");
        Add(styles, "XML Doc Tag", "#FF7E868C");
        Add(styles, "XML Doc Comment", "#FF5A7D48");
        Add(styles, "String", "#FF18794E");
        Add(styles, "String (C# @ Verbatim)", "#FF18794E");
        Add(styles, "Keyword", "#FF205D9E");
        Add(styles, "Preprocessor Keyword", "#FFB54512");
        Add(styles, "HTML Entity", "#FFCD2B31");
        Add(styles, "Json Key", "#FF8D3A12");
        Add(styles, "Json String", "#FF0C7C59");
        Add(styles, "Json Number", "#FF9E6C00");
        Add(styles, "Json Const", "#FF5B5BD6");
        Add(styles, "XML Attribute", "#FF8D3A12");
        Add(styles, "XML Attribute Quotes", "#FF60646C");
        Add(styles, "XML Attribute Value", "#FF18794E");
        Add(styles, "XML CData Section", "#FF60646C");
        Add(styles, "XML Comment", "#FF5A7D48");
        Add(styles, "XML Delimiter", "#FF0D74CE");
        Add(styles, "XML Name", "#FF0F3D7A");
        Add(styles, "Class Name", "#FF5B5BD6");
        Add(styles, "CSS Selector", "#FF0F3D7A");
        Add(styles, "CSS Property Name", "#FF8D3A12");
        Add(styles, "CSS Property Value", "#FF18794E");
        Add(styles, "SQL System Function", "#FF5B5BD6");
        Add(styles, "PowerShell Attribute", "#FF0C7C59");
        Add(styles, "PowerShell Operator", "#FF7E868C");
        Add(styles, "PowerShell Type", "#FF0C7C59");
        Add(styles, "PowerShell Variable", "#FFB54512");
        Add(styles, "PowerShell Command", "#FF205D9E");
        Add(styles, "PowerShell Parameter", "#FF7E868C");
        Add(styles, "Type", "#FF0C7C59");
        Add(styles, "Type Variable", "#FF0C7C59", italic: true);
        Add(styles, "Name Space", "#FF205D9E");
        Add(styles, "Constructor", "#FF5B5BD6");
        Add(styles, "Predefined", "#FF205D9E");
        Add(styles, "Pseudo Keyword", "#FF205D9E");
        Add(styles, "String Escape", "#FF7E868C");
        Add(styles, "Control Keyword", "#FF205D9E");
        Add(styles, "Number", "#FF9E6C00");
        Add(styles, "Markdown Header", "#FF0F3D7A", bold: true);
        Add(styles, "Markdown Code", "#FF18794E");
        Add(styles, "Markdown List Item", "#FF60646C", bold: true);
        Add(styles, "Markdown Emphasized", "#FF1C2024", italic: true);
        Add(styles, "Markdown Bold", "#FF1C2024", bold: true);
        Add(styles, "Built In Function", "#FF3F5CB0", bold: true);
        Add(styles, "Built In Value", "#FF8D3A12", bold: true);
        Add(styles, "Attribute", "#FF0D74CE", italic: true);
        Add(styles, "Special Character", "#FF7E868C");

        return styles;
    }

    private static StyleDictionary CreateDark()
    {
        var styles = new StyleDictionary();

        Add(styles, "Plain Text", "#FFE7E9EC");
        Add(styles, "HTML Comment", "#FF7FC17E");
        Add(styles, "Html Tag Delimiter", "#FF7CC4FA");
        Add(styles, "HTML Element ScopeName", "#FFB1C9FF");
        Add(styles, "HTML Attribute ScopeName", "#FFFFB381");
        Add(styles, "HTML Attribute Value", "#FF86E1A6");
        Add(styles, "HTML Operator", "#FF7CC4FA");
        Add(styles, "Comment", "#FF7FC17E");
        Add(styles, "XML Doc Tag", "#FF9BA1A6");
        Add(styles, "XML Doc Comment", "#FF7FC17E");
        Add(styles, "String", "#FF86E1A6");
        Add(styles, "String (C# @ Verbatim)", "#FF86E1A6");
        Add(styles, "Keyword", "#FF8EC8FF");
        Add(styles, "Preprocessor Keyword", "#FFFFB381");
        Add(styles, "HTML Entity", "#FFFF9592");
        Add(styles, "Json Key", "#FFFFB381");
        Add(styles, "Json String", "#FF86E1A6");
        Add(styles, "Json Number", "#FFFFD36E");
        Add(styles, "Json Const", "#FFC4B5FD");
        Add(styles, "XML Attribute", "#FFFFB381");
        Add(styles, "XML Attribute Quotes", "#FFB0B4BA");
        Add(styles, "XML Attribute Value", "#FF86E1A6");
        Add(styles, "XML CData Section", "#FFB0B4BA");
        Add(styles, "XML Comment", "#FF7FC17E");
        Add(styles, "XML Delimiter", "#FF7CC4FA");
        Add(styles, "XML Name", "#FFB1C9FF");
        Add(styles, "Class Name", "#FFC4B5FD");
        Add(styles, "CSS Selector", "#FFB1C9FF");
        Add(styles, "CSS Property Name", "#FFFFB381");
        Add(styles, "CSS Property Value", "#FF86E1A6");
        Add(styles, "SQL System Function", "#FFC4B5FD");
        Add(styles, "PowerShell Attribute", "#FF67D4C0");
        Add(styles, "PowerShell Operator", "#FF9BA1A6");
        Add(styles, "PowerShell Type", "#FF67D4C0");
        Add(styles, "PowerShell Variable", "#FFFFB381");
        Add(styles, "PowerShell Command", "#FF8EC8FF");
        Add(styles, "PowerShell Parameter", "#FF9BA1A6");
        Add(styles, "Type", "#FF67D4C0");
        Add(styles, "Type Variable", "#FF67D4C0", italic: true);
        Add(styles, "Name Space", "#FF8EC8FF");
        Add(styles, "Constructor", "#FFC4B5FD");
        Add(styles, "Predefined", "#FF8EC8FF");
        Add(styles, "Pseudo Keyword", "#FF8EC8FF");
        Add(styles, "String Escape", "#FF9BA1A6");
        Add(styles, "Control Keyword", "#FF8EC8FF");
        Add(styles, "Number", "#FFFFD36E");
        Add(styles, "Markdown Header", "#FFB1C9FF", bold: true);
        Add(styles, "Markdown Code", "#FF86E1A6");
        Add(styles, "Markdown List Item", "#FFB0B4BA", bold: true);
        Add(styles, "Markdown Emphasized", "#FFE7E9EC", italic: true);
        Add(styles, "Markdown Bold", "#FFE7E9EC", bold: true);
        Add(styles, "Built In Function", "#FFB1C9FF", bold: true);
        Add(styles, "Built In Value", "#FFFFB381", bold: true);
        Add(styles, "Attribute", "#FF7CC4FA", italic: true);
        Add(styles, "Special Character", "#FF9BA1A6");

        return styles;
    }

    private static void Add(
        StyleDictionary styles,
        string scopeName,
        string foreground,
        bool bold = false,
        bool italic = false
    )
    {
        styles.Add(
            new Style(scopeName)
            {
                Foreground = foreground,
                Bold = bold,
                Italic = italic,
            }
        );
    }
}
