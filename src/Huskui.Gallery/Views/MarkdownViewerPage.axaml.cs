using Huskui.Avalonia.Markdown.Controls;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class MarkdownViewerPage : ControlPage
{
    private const string SampleMarkdown = """
        # Markdown Viewer Showcase

        This is a **MarkdownViewer** component from `Huskui.Avalonia.Markdown`. It is *not* a built-in control, but an extension library component.

        ---

        ## Text Styles

        **Bold text**, *italic text*, ~~strikethrough text~~.

        EmphasisExtras: superscript E=mc^2^, subscript H~2~O, ++inserted text++, ==highlighted text==.

        Combined: ***bold and italic***, **~~bold strikethrough~~**.

        ## Inline Code

        Install via CLI: `dotnet add package Huskui.Avalonia.Markdown`

        ## Fenced Code Block

        ```csharp
        public class MarkdownViewer : TemplatedControl
        {
            public static readonly StyledProperty<string?> MarkdownProperty =
                AvaloniaProperty.Register<MarkdownViewer, string?>(nameof(Markdown));

            public string? Markdown
            {
                get => GetValue(MarkdownProperty);
                set => SetValue(MarkdownProperty, value);
            }
        }
        ```

        ```json
        {
            "name": "Huskui.Avalonia.Markdown",
            "version": "1.0.0",
            "dependencies": ["Avalonia", "Markdig"]
        }
        ```

        ## Links

        Auto link: https://github.com

        Inline link: [Avalonia UI](https://avaloniaui.net)

        ## Ordered Lists

        1. First item
        2. Second item
        3. Third item

        ## Unordered Lists

        - Item one
        - Item two
        - Item three
          - Nested item A
          - Nested item B
            - Deeper nesting

        ## ListExtras — Lettered Lists

        a. Alpha item one
        b. Alpha item two
        c. Alpha item three

        ## Task List

        - [x] Render headings
        - [x] Render emphasis (bold, italic, strikethrough)
        - [x] Render inline code
        - [ ] Render fenced code blocks
        - [ ] Render images
        - [ ] Render tables
        - [ ] Render blockquotes

        ## Pipe Table

        | Feature       | Supported | Extension         |
        |---------------|-----------|-------------------|
        | Headings      | Yes       | Built-in          |
        | Bold / Italic | Yes       | Built-in          |
        | Strikethrough | Yes       | EmphasisExtras    |
        | Superscript   | Yes       | EmphasisExtras    |
        | Subscript     | Yes       | EmphasisExtras    |
        | Inserted      | Yes       | EmphasisExtras    |
        | Marked        | Yes       | EmphasisExtras    |
        | Inline Code   | Yes       | Built-in          |
        | Auto Links    | Yes       | AutoLinks         |
        | Task Lists    | Yes       | TaskLists         |
        | Pipe Tables   | Yes       | PipeTables        |
        | Grid Tables   | Yes       | GridTables        |
        | Lettered Lists| Yes       | ListExtras        |

        ## Grid Table

        +---------+---------+
        | Header 1 | Header 2 |
        +=========+=========+
        | Cell A  | Cell B  |
        +---------+---------+
        | Cell C  | Cell D  |
        +---------+---------+

        ## Blockquote

        > This is a blockquote. It can contain **formatted** text and `code`.
        >
        > > Nested blockquote with *italic* and ~~strikethrough~~.

        ## Image

        ![Avalonia Logo](https://avaloniaui.net/img/logo/avalonia-logo.png)

        ## Math (Raw LaTeX — requires Math extension)

        Inline math: $E = mc^2$

        Block math:

        $$
        \int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
        $$

        ## Footnotes (requires Footnotes extension)

        This sentence has a footnote[^1] and another one[^2].

        [^1]: This is the first footnote content.
        [^2]: This is the second footnote with **bold** text.

        ## Horizontal Rule

        ---

        ## Mixed Content

        > **Note**: The `MarkdownViewer` uses [Markdig](https://github.com/xoofx/markdig) as its parsing engine with the following pipeline extensions enabled:
        >
        > - `UseAutoLinks()` — automatic URL detection
        > - `UseTaskLists()` — `- [x]` / `- [ ]` checkboxes
        > - `UseEmphasisExtras()` — `~sub~`, `^super^`, `++ins++`, `==mark==`
        > - `UseGridTables()` — grid-style tables
        > - `UsePipeTables()` — pipe-style tables
        > - `UseListExtras()` — lettered (`a.`, `b.`) lists

        """;

    public MarkdownViewerPage()
    {
        InitializeComponent();
        Viewer.Markdown = SampleMarkdown;
    }

}
