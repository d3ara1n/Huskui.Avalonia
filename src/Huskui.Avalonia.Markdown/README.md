# Huskui.Avalonia.Markdown

A Markdown rendering extension for [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia) that converts Markdown text into native Avalonia control trees using [Markdig](https://github.com/xoofx/markdig).

## Relationship to Huskui.Avalonia

`Huskui.Avalonia.Markdown` is an **extension library** for Huskui.Avalonia — it cannot be used standalone.

- **Prerequisite**: You must install `Huskui.Avalonia` and use `HuskuiTheme` in your application
- **Auto-loading**: Once the assembly is loaded, `HuskuiTheme` automatically merges the extension's theme resources via the `[HuskuiExtension]` mechanism — no manual style includes needed
- **Shared namespace**: Controls are mapped to `https://github.com/d3ara1n/Huskui.Avalonia`, using the same `husk:` XAML prefix as the core library

## Installation

```bash
dotnet add package Huskui.Avalonia
dotnet add package Huskui.Avalonia.Markdown
```

## Usage

### Basic

```xml
<husk:MarkdownViewer Markdown="{Binding MarkdownText}" />
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Markdown` | `string?` | `null` | Raw Markdown text to render |
| `Spacing` | `double` | `4` | Spacing between block-level elements |

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
             x:Class="YourApp.MarkdownPage">
    <husk:MarkdownViewer
        Spacing="8"
        Markdown="# Hello World&#x0a;&#x0a;This is **bold** and *italic* text.&#x0a;&#x0a;- Item 1&#x0a;- Item 2&#x0a;&#x0a;```csharp&#x0a;Console.WriteLine(&quot;Hello&quot;);&#x0a;```" />
</UserControl>
```

### C# Example

```csharp
var viewer = new MarkdownViewer();
viewer.Markdown = """
    # Hello World

    This is **bold** and *italic* text.

    - Item 1
    - Item 2

    ```csharp
    Console.WriteLine("Hello");
    ```
    """;
```

## Styling

`MarkdownViewer` generates **bare controls with semantic CSS classes but no built-in visual styles**. The extension ships a default theme bundle that is auto-loaded by `HuskuiTheme`, but you can override or extend styles by targeting the generated class names.

The class naming convention is `Control.Markdown.Variant`. Here are some examples:

| Selector | Target |
|----------|--------|
| `TextBlock.Markdown.Heading1` | Level-1 heading |
| `TextBlock.Markdown.Heading2` | Level-2 heading |
| `TextBlock.Markdown.Paragraph` | Paragraph block |
| `Run.Markdown.Literal.Bold` | Bold text run |
| `Run.Markdown.Literal.Italic` | Italic text run |
| `Run.Markdown.Literal.Deleted` | Strikethrough text run |
| `Run.Markdown.Literal.Highlighted` | Highlighted text run (==text==) |
| `Run.Markdown.Literal.Subscripted` | Subscript text run |
| `Run.Markdown.Literal.Superscripted` | Superscript text run |
| `DockPanel.Markdown.List.Item` | List item container |
| `husk:CodeViewer.Markdown.Code` | Fenced code block |
| `husk:InfoBar.Markdown.Quote` | Blockquote |
| `husk:Divider.Markdown.Rule` | Horizontal rule |

### Custom Style Example

```xml
<Styles>
    <!-- Make all headings bold -->
    <Style Selector="TextBlock.Markdown.Heading1">
        <Setter Property="FontWeight" Value="Bold" />
        <Setter Property="FontSize" Value="28" />
    </Style>

    <!-- Add left border to blockquotes -->
    <Style Selector="husk|InfoBar.Markdown.Quote">
        <Setter Property="BorderBrush" Value="Blue" />
        <Setter Property="BorderThickness" Value="3,0,0,0" />
        <Setter Property="Padding" Value="12,8" />
    </Style>

    <!-- Custom code block background -->
    <Style Selector="husk|CodeViewer.Markdown.Code">
        <Setter Property="Background" Value="#1e1e1e" />
        <Setter Property="Foreground" Value="#d4d4d4" />
    </Style>

    <!-- Adjust list indentation -->
    <Style Selector="DockPanel.Markdown.List.Item">
        <Setter Property="Margin" Value="16,0,0,0" />
    </Style>
</Styles>
```

## Markdown Support

### Supported

| Syntax | Example | Notes |
|--------|---------|-------|
| Headings | `# H1` ~ `###### H6` | 6 levels, mapped to font size resources |
| Paragraphs | plain text | Supports inline formatting |
| **Bold** | `**bold**` | |
| *Italic* | `*italic*` | |
| ~~Strikethrough~~ | `~~text~~` | |
| ++Inserted++ | `++text++` | EmphasisExtras extension |
| ==Highlighted== | `==text==` | EmphasisExtras extension |
| Subscript | `~text~` | EmphasisExtras extension |
| Superscript | `^text^` | EmphasisExtras extension |
| `Inline code` | `` `code` `` | Rendered as HighlightInline |
| Code blocks | ` ```lang ` | Rendered as CodeViewer |
| Unordered lists | `- item` | Nested; bullet varies by depth (●○■□) |
| Ordered lists | `1. item` | Nested |
| Task lists | `- [x] done` | Rendered as CheckBox |
| Links | `[text](url)` | Rendered as HyperlinkButton |
| Images | `![alt](url)` | Async loading via AsyncImageLoader |
| Blockquotes | `> quote` | Rendered as InfoBar |
| Horizontal rules | `---` | Rendered as Divider |
| Auto-links | `<https://example.com>` | AutoLinks extension |

### Not Yet Supported

| Syntax | Notes |
|--------|-------|
| Tables | Markdig pipeline includes `UsePipeTables` / `UseGridTables`, but the renderer does not yet handle `TableBlock` |
| Math | No math rendering extension integrated |
| Footnotes | Markdig footnotes extension not enabled |
| Syntax highlighting | Code blocks use CodeViewer as plain text — no syntax coloring |

## Dependencies

- [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia) — Core control library
- [Huskui.Avalonia.Code](https://github.com/d3ara1n/Huskui.Avalonia) — Code block rendering (CodeViewer)
- [Markdig](https://github.com/xoofx/markdig) — Markdown parsing
- [AsyncImageLoader.Avalonia](https://github.com/AvaloniaUtils/AsyncImageLoader.Avalonia) — Async image loading

## License

[MIT](https://github.com/d3ara1n/Huskui.Avalonia/blob/master/LICENSE)
