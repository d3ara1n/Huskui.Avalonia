using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class CodeViewerPage : ControlPage
{
    private const string InstallSnippet =
        "dotnet add package Huskui.Avalonia\ndotnet add package Huskui.Avalonia.Code";

    private const string PrimarySnippet = """
        public sealed record EditorCommand(string Id, string Label, bool IsEnabled);

        public static class EditorCommands
        {
            public static IReadOnlyList<EditorCommand> Defaults { get; } =
            [
                new("copy-path", "Copy Relative Path", true),
                new("open-docs", "Open Documentation", true),
                new("reveal-token", "Reveal Syntax Tokens", false),
            ];

            public static IEnumerable<EditorCommand> GetVisibleCommands(bool includeExperimental)
            {
                return includeExperimental ? Defaults : Defaults.Where(command => command.IsEnabled);
            }
        }
        """;

    private const string CompactSnippet = """
        <husk:Card Padding="16">
            <StackPanel Spacing="8">
                <TextBlock Text="Inline theme include" />
                <husk:CodeViewer
                    Language="axaml"
                    IsCopyButtonVisible="False"
                    IsLineNumbersVisible="False"
                    Code="&lt;husk:Tag Content=&quot;Copied&quot; /&gt;" />
            </StackPanel>
        </husk:Card>
        """;

    private const string AxamlSnippet = """
        <husk:Button
            Classes="Primary"
            Content="Run snippet"
            Command="{Binding RunCommand}" />
        """;

    private const string JsonSnippet = """
        {
          "fileName": "CodeViewerPage.axaml",
          "language": "csharp",
          "copyButton": true,
          "highlighting": "ColorCode"
        }
        """;

    private const string MarkdownSnippet = """
        ## Viewer tips

        - Use `cs`, `axaml`, `ts`, or full ColorCode ids.
        - Keep the default header when users may copy snippets.
        - Disable chrome for compact guide sections.
        """;

    private const string FallbackSnippet = """
        [trace] parser: custom-log is not registered
        [trace] viewer: rendering plain text fallback
        [trace] result: source stays readable without syntax colors
        """;

    private const string UsageXamlSnippet = """
        <husk:CodeViewer
            Language="csharp"
            Code="{Binding SelectedSnippet}"
            IsCopyButtonVisible="True" />
        """;

    private const string UsageCSharpSnippet = """
        var viewer = new CodeViewer
        {
            Language = "csharp",
            Code = GeneratedSource,
            IsCopyButtonVisible = true,
        };
        """;

    public CodeViewerPage()
    {
        InitializeComponent();

        InstallCodeViewer.Code = InstallSnippet;
        PrimaryCodeViewer.Code = PrimarySnippet;
        CompactCodeViewer.Code = CompactSnippet;
        AxamlCodeViewer.Code = AxamlSnippet;
        JsonCodeViewer.Code = JsonSnippet;
        MarkdownCodeViewer.Code = MarkdownSnippet;
        FallbackCodeViewer.Code = FallbackSnippet;
        UsageXamlCodeViewer.Code = UsageXamlSnippet;
        UsageCSharpCodeViewer.Code = UsageCSharpSnippet;
    }
}
