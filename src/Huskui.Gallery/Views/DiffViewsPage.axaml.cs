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
    }
}
