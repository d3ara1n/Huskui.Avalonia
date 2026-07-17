using System.Text;

using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class DiffViewsPage : ControlPage
{
    private const string LeftSnippet = """
        {
          "name": "example-project",
          "version": "1.0.0",
          "description": "A sample project for demonstration",
          "dependencies": {
            "react": "^18.2.0",
            "lodash": "^4.17.21"
          },
          "scripts": {
            "start": "node index.js",
            "build": "webpack --mode production"
          },
          "license": "MIT"
        }
        """;

    private const string RightSnippet = """
        {
          "name": "example-project",
          "version": "2.0.0",
          "description": "An updated sample project with new features",
          "dependencies": {
            "react": "^19.0.0",
            "lodash": "^4.17.21",
            "axios": "^1.6.0"
          },
          "scripts": {
            "start": "node app.js",
            "build": "webpack --mode production",
            "test": "jest"
          },
          "license": "Apache-2.0"
        }
        """;

    private const string MonoLeftSnippet = """
        Left
        Unchanged
        Unchanged
        Removed only
        Removed only
        Unchanged
        Unchanged
        Modified
        Modified
        Unchanged
        Unchanged
        Unchanged
        """;

    private const string MonoRightSnippet = """
        Right
        Unchanged
        Unchanged
        Unchanged
        Modified
        Modified
        Added only
        Added only
        Added only
        Unchanged
        Unchanged
        """;

    public DiffViewsPage()
    {
        InitializeComponent();

        PrimaryDiffView.LeftText = LeftSnippet;
        PrimaryDiffView.RightText = RightSnippet;

        EmptyDiffView.LeftText = "Same content on both sides";
        EmptyDiffView.RightText = "Same content on both sides";

        VariousDiffView.LeftText = MonoLeftSnippet;
        VariousDiffView.RightText = MonoRightSnippet;

        var markdown = BuildLongMarkdownDocuments();
        LongMarkdownDiffView.LeftText = markdown.Left;
        LongMarkdownDiffView.RightText = markdown.Right;
    }

    private static (string Left, string Right) BuildLongMarkdownDocuments()
    {
        var left = new StringBuilder();
        var right = new StringBuilder();

        left.AppendLine("# Widget Platform Release Notes");
        left.AppendLine();
        left.AppendLine("This document describes the published configuration, deployment, and migration notes for the widget platform.");
        left.AppendLine();

        right.AppendLine("# Widget Platform Release Notes");
        right.AppendLine();
        right.AppendLine("This document describes the draft configuration, deployment, and migration notes for the widget platform.");
        right.AppendLine();
        right.AppendLine("> Draft: pending final review from infrastructure and support teams.");
        right.AppendLine();

        for (var section = 1; section <= 72; section++)
        {
            AppendMarkdownSection(left, section, isDraft: false);
            AppendMarkdownSection(right, section, isDraft: true);
        }

        left.AppendLine("## Appendix");
        left.AppendLine();
        left.AppendLine("The published document keeps legacy CLI examples until the next minor release.");

        right.AppendLine("## Appendix");
        right.AppendLine();
        right.AppendLine("The draft document removes legacy CLI examples and links to the migration guide instead.");
        right.AppendLine("- Migration guide: docs/migration/widget-platform-vnext.md");

        return (left.ToString(), right.ToString());
    }

    private static void AppendMarkdownSection(StringBuilder builder, int section, bool isDraft)
    {
        builder.AppendLine($"## {section:00}. {SectionTitle(section)}");
        builder.AppendLine();
        builder.AppendLine($"The service group `{ServiceName(section)}` owns this area and reviews changes during the weekly release window.");

        if (isDraft && section % 4 == 0)
            builder.AppendLine("The draft expands this section with rollout guardrails for staged production deployments.");
        else if (!isDraft && section % 6 == 0)
            builder.AppendLine("The published version keeps the shorter operational summary for the current release train.");
        else
            builder.AppendLine("The existing behavior remains compatible with the current package and configuration defaults.");

        builder.AppendLine();
        builder.AppendLine("### Checklist");
        builder.AppendLine($"- Confirm the `{ConfigKey(section)}` setting before rollout.");
        builder.AppendLine("- Verify dashboards after the first deployment wave.");

        if (isDraft && section % 5 == 0)
            builder.AppendLine("- Capture support handoff notes for regional operators.");
        if (!isDraft && section % 7 == 0)
            builder.AppendLine("- Keep the compatibility flag enabled for partner tenants.");

        builder.AppendLine();
        builder.AppendLine("### Example");
        builder.AppendLine("```yaml");
        builder.AppendLine($"service: {ServiceName(section)}");
        builder.AppendLine($"retries: {(isDraft && section % 3 == 0 ? 5 : 3)}");
        builder.AppendLine($"timeout: {(isDraft && section % 8 == 0 ? 45 : 30)}s");
        builder.AppendLine("```\n");
    }

    private static string SectionTitle(int section) =>
        (section % 8) switch
        {
            0 => "Deployment Gates",
            1 => "Configuration Defaults",
            2 => "Telemetry Events",
            3 => "Access Control",
            4 => "Import Pipeline",
            5 => "Export Pipeline",
            6 => "Support Workflow",
            _ => "Migration Notes",
        };

    private static string ServiceName(int section) => $"widget-{section % 9 + 1}";

    private static string ConfigKey(int section) => $"widget.release.{section % 12 + 1}.enabled";
}
