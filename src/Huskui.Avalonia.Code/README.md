# Huskui.Avalonia.Code

A syntax-highlighted code viewer extension for [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia), powered by [ColorCode.Core](https://github.com/CommunityToolkit/ColorCode).

## Relationship to Huskui.Avalonia

`Huskui.Avalonia.Code` is an **extension library** for Huskui.Avalonia — it cannot be used standalone.

- **Prerequisite**: You must install `Huskui.Avalonia` and use `HuskuiTheme` in your application
- **Auto-loading**: Once the assembly is loaded, `HuskuiTheme` automatically merges the extension's theme resources via the `[HuskuiExtension]` mechanism — no manual style includes needed
- **Shared namespace**: Controls are mapped to `https://github.com/d3ara1n/Huskui.Avalonia`, using the same `husk:` XAML prefix as the core library

## Installation

```bash
dotnet add package Huskui.Avalonia
dotnet add package Huskui.Avalonia.Code
```

## Usage

### Basic

```xml
<husk:CodeViewer Code="{Binding SourceCode}" Language="csharp" />
```

### Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Code` | `string` | `""` | Source code text to display |
| `Language` | `string` | `"xml"` | Language identifier for syntax highlighting |
| `IsLineNumbersVisible` | `bool` | `true` | Show line numbers column |
| `IsCopyButtonVisible` | `bool` | `true` | Show copy button in the header bar |
| `Inlines` | `InlineCollection?` | `null` | Custom inline collection; overrides `Code`-based rendering when set |

### XAML Example

```xml
<UserControl xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
             x:Class="YourApp.CodePage">

    <StackPanel Spacing="16">
        <!-- C# with default settings -->
        <husk:CodeViewer
            Language="csharp"
            Code='public class Hello&#x0a;{&#x0a;    public void Greet() => Console.WriteLine(&quot;Hello!&quot;);&#x0a;}' />

        <!-- XML without line numbers -->
        <husk:CodeViewer
            Language="xml"
            IsLineNumbersVisible="False"
            Code="&lt;Button Content=&quot;Click me&quot; /&gt;" />

        <!-- JavaScript without copy button -->
        <husk:CodeViewer
            Language="js"
            IsCopyButtonVisible="False"
            Code="console.log(&quot;Hello&quot;);" />
    </StackPanel>
</UserControl>
```

### C# Example

```csharp
var viewer = new CodeViewer
{
    Language = "csharp",
    Code = """
        public class Hello
        {
            public void Greet() => Console.WriteLine("Hello!");
        }
        """
};
```

## Language Support

`CodeViewer` uses [ColorCode.Core](https://github.com/CommunityToolkit/ColorCode) for syntax highlighting. Languages are resolved in the following order:

1. `Languages.FindById()` — direct ColorCode language ID lookup
2. `Languages.All` → `HasAlias()` — alias-based lookup
3. Built-in convenience aliases (see table below)

### Supported Languages

| Language | Identifiers |
|----------|------------|
| C# | `csharp`, `cs` |
| C++ | `cpp` |
| CSS | `css` |
| F# | `fsharp`, `fs` |
| HTML | `html` |
| Java | `java` |
| JavaScript | `javascript`, `js` |
| Markdown | `markdown`, `md` |
| PHP | `php` |
| PowerShell | `powershell`, `ps1` |
| Python | `python`, `py` |
| SQL | `sql` |
| TypeScript | `typescript`, `ts`, `tsx` |
| XML | `xml`, `xaml`, `axaml` |
| ASP.NET | `aspx` |
| CoffeeScript | `coffeescript` |
| Ruby | `ruby` |

> **Note**: The full list of supported languages depends on `ColorCode.Core`. If a language is not recognized, the code is rendered as plain text without syntax highlighting.

## Features

- **Syntax Highlighting** — ColorCode-based tokenization with dark theme styling
- **Line Numbers(WIP)** — Optional line number gutter
- **Copy to Clipboard** — Header bar with language label and copy button
- **Selectable Text** — Code content is rendered in a `SelectableTextBlock` for easy text selection
- **Custom Inlines** — Override rendering by providing your own `InlineCollection`
- **Monospace Font** — Defaults to `Cascadia Code, Consolas, Courier New, monospace`

## Dependencies

- [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia) — Core control library (required)
- [ColorCode.Core](https://github.com/CommunityToolkit/ColorCode) — Syntax highlighting engine

## License

[MIT](https://github.com/d3ara1n/Huskui.Avalonia/blob/master/LICENSE)
