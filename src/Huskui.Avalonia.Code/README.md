# Huskui.Avalonia.Code

A syntax-highlighted code viewer extension for [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia), powered by TextMate grammars with a lightweight inline renderer.

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

`CodeViewer` uses TextMate grammars through `TextMateSharp.Grammars`. Languages are resolved through built-in aliases and file-extension-style mappings, then fall back to plain text if no grammar is available.

### Supported Languages

| Language | Identifiers |
|----------|------------|
| Bash | `bash`, `sh`, `shell`, `zsh` |
| C# | `csharp`, `cs` |
| C++ | `cpp` |
| C | `c` |
| CSS | `css` |
| F# | `fsharp`, `fs` |
| HTML | `html` |
| Java | `java` |
| JavaScript | `javascript`, `js` |
| JSON | `json` |
| Markdown | `markdown`, `md` |
| Less | `less` |
| Lua | `lua` |
| PHP | `php` |
| PowerShell | `powershell`, `ps1` |
| Python | `python`, `py` |
| Rust | `rust`, `rs` |
| SCSS | `scss` |
| SQL | `sql` |
| TypeScript | `typescript`, `ts`, `tsx` |
| XML | `xml`, `xaml`, `axaml` |
| YAML | `yaml`, `yml` |
| Visual Basic | `visualbasic`, `vb` |
| ASP.NET | `aspx` |
| CoffeeScript | `coffeescript` |
| Ruby | `ruby` |

> **Note**: The full list of supported languages depends on the bundled TextMate grammars. If a language is not recognized, the code is rendered as plain text without syntax highlighting.

## Features

- **Syntax Highlighting** — TextMate-based tokenization with Huskui Radix light/dark token themes
- **Line Numbers** — Optional lightweight line number gutter
- **Copy to Clipboard** — Header bar with language label and copy button
- **Lightweight Rendering** — Code content is rendered as Avalonia inlines without embedding an editor control
- **Custom Inlines** — Override rendering by providing your own `InlineCollection`
- **Monospace Font** — Defaults to `Cascadia Code, Consolas, Courier New, monospace`

## Dependencies

- [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia) — Core control library (required)
- [TextMateSharp.Grammars](https://github.com/danipen/TextMateSharp) — Bundled grammars and themes

## License

[MIT](https://github.com/d3ara1n/Huskui.Avalonia/blob/master/LICENSE)
