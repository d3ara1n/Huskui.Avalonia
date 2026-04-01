using System;
using System.Text;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using Huskui.Avalonia.Code.Controls;
using Huskui.Avalonia.Controls;
using Markdig;
using Markdig.Extensions.TaskLists;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Huskui.Avalonia.Markdown.Controls;

[TemplatePart(PART_LayoutContainer, typeof(ScrollViewer))]
public class MarkdownViewer : TemplatedControl
{
    public const string PART_LayoutContainer = nameof(PART_LayoutContainer);

    private static MarkdownPipeline markdownPipeline;

    static MarkdownViewer()
    {
        markdownPipeline = new MarkdownPipelineBuilder()
            .UseAutoLinks()
            .UseTaskLists()
            .UseEmphasisExtras()
            .UseGridTables()
            .UsePipeTables()
            .UseListExtras()
            .Build();
    }

    private ScrollViewer? _container;

    public static readonly StyledProperty<string?> MarkdownProperty = AvaloniaProperty.Register<
        MarkdownViewer,
        string?
    >(nameof(Markdown));

    public string? Markdown
    {
        get => GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<
        MarkdownViewer,
        double
    >(nameof(Spacing), 4);

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _container = e.NameScope.Find<ScrollViewer>(PART_LayoutContainer);

        Render(Markdown);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MarkdownProperty)
        {
            Render(change.GetNewValue<string>());
        }
    }

    private void Render(string? markdown)
    {
        if (_container is null)
            return;

        _container.Content = null;
        LogicalChildren.Clear();

        if (string.IsNullOrEmpty(markdown))
            return;

        var document = Markdig.Markdown.Parse(markdown, markdownPipeline);

        var panel = SpawnStack();
        panel.Classes.Set("Markdown", true);
        _container.Content = panel;

        foreach (var block in document)
        {
            var control = RenderBlock(block);
            if (control != null)
            {
                panel.Children.Add(control);
                LogicalChildren.Add(control);
            }
        }
    }

    private struct BlockContext
    {
        public bool ListOrdered { get; set; }
        public int ListDepth { get; set; }

        public int ListIndex { get; set; }

        public BlockContext()
        {
            ListDepth = -1;
        }
    }

    private Control? RenderBlock(Block block, BlockContext context = default)
    {
        Control? control;
        switch (block)
        {
            case HeadingBlock heading:
                {
                    var rv = new TextBlock();
                    rv.Inlines = RenderInlines(heading.Inline);
                    rv.Classes.Set($"Heading{heading.Level}", true);
                    control = rv;
                }
                break;
            case ParagraphBlock paragraph:
                {
                    var rv = new TextBlock();
                    rv.Inlines = RenderInlines(paragraph.Inline);
                    rv.Classes.Set("Paragraph", true);
                    control = rv;
                }
                break;
            case ListBlock list:
                {
                    var panel = SpawnStack();
                    context.ListOrdered = list.IsOrdered;
                    // 遇到第一个 ListBlock 时 -1 + 1 => 0
                    context.ListDepth++;
                    context.ListIndex = 0;
                    foreach (var item in list)
                    {
                        context.ListIndex++;
                        var inner = RenderBlock(item, context);
                        if (inner != null)
                        {
                            panel.Children.Add(inner);
                        }
                    }
                    panel.Classes.Set("List", true);
                    control = panel;
                }
                break;
            case ListItemBlock listItem:
                {
                    var stack = SpawnStack();
                    var dock = SpawnDock();
                    foreach (var item in listItem)
                    {
                        var inner = RenderBlock(item, context);
                        if (inner != null)
                        {
                            stack.Children.Add(inner);
                        }
                    }
                    var header = new TextBlock();
                    header.Text =
                        $"{(context.ListOrdered ? GenerateOrderedListHead(context.ListDepth, context.ListIndex) : GenerateUnorderedListHead(context.ListDepth))}.";
                    DockPanel.SetDock(header, Dock.Left);
                    dock.Children.Add(header);
                    dock.Children.Add(stack);
                    header.Classes.Set("List", true);
                    header.Classes.Set("Header", true);
                    dock.Classes.Set("List", true);
                    dock.Classes.Set("Item", true);
                    stack.Classes.Set("List", true);
                    stack.Classes.Set("Item", true);
                    control = dock;
                }
                break;
            case CodeBlock code:
                {
                    var rv = new CodeViewer();
                    rv.Code = code.Lines.ToString() ?? string.Empty;
                    if (code is FencedCodeBlock fenced)
                        rv.Language = fenced.Info ?? string.Empty;
                    rv.Classes.Set("Code", true);
                    control = rv;
                }
                break;
            case QuoteBlock quote:
                {
                    var rv = new InfoBar();
                    var container = SpawnStack();
                    foreach (var child in quote)
                    {
                        var content = RenderBlock(child, context);
                        if (content is not null)
                        {
                            container.Children.Add(content);
                        }
                    }
                    rv.Content = container;
                    rv.Classes.Set("Quote", true);
                    control = rv;
                }
                break;
            case ThematicBreakBlock:
                {
                    var rv = new Divider() { Orientation = Orientation.Horizontal };
                    rv.Classes.Set("Divider", true);
                    control = rv;
                }
                break;
            default:
                {
                    control = new TextBlock() { Text = block.ToString() };
                }
                break;
        }

        control?.Classes.Set("Markdown", true);
        return control;
    }

    private InlineCollection RenderInlines(ContainerInline? inline)
    {
        var inlines = new InlineCollection();
        if (inline is null)
            return inlines;

        foreach (var child in inline)
        {
            BuildInline(child, inlines);
        }
        foreach (var child in inlines)
        {
            child.Classes.Set("Markdown", true);
        }
        return inlines;
    }

    private struct EmphasisContext
    {
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Deleted { get; set; }
        public bool Underlined { get; set; }
        public bool Highlighted { get; set; }
        public bool Subscripted { get; set; }
        public bool Superscripted { get; set; }
    }

    private void BuildInline(
        Markdig.Syntax.Inlines.Inline inline,
        InlineCollection inlines,
        EmphasisContext context = default
    )
    {
        switch (inline)
        {
            case LiteralInline lit:
                {
                    var run = new Run(lit.Content.ToString());
                    run.Classes.Set("Literal", true);
                    run.Classes.Set("Bold", context.Bold);
                    run.Classes.Set("Italic", context.Italic);
                    run.Classes.Set("Deleted", context.Deleted);
                    run.Classes.Set("Underlined", context.Underlined);
                    run.Classes.Set("Highlighted", context.Highlighted);
                    run.Classes.Set("Subscripted", context.Subscripted);
                    run.Classes.Set("Superscripted", context.Superscripted);

                    inlines.Add(run);
                }
                break;
            case EmphasisInline em when em.FirstChild is not null:
                {
                    context.Bold |= em is { DelimiterChar: '*', DelimiterCount: 2 };
                    context.Italic |= em is { DelimiterChar: '*', DelimiterCount: 1 };
                    context.Deleted |= em is { DelimiterChar: '~', DelimiterCount: 2 };
                    context.Underlined |= em is { DelimiterChar: '+', DelimiterCount: 2 };
                    context.Highlighted |= em is { DelimiterChar: '=', DelimiterCount: 2 };
                    context.Subscripted |= em is { DelimiterChar: '~', DelimiterCount: 1 };
                    context.Superscripted |= em is { DelimiterChar: '^', DelimiterCount: 1 };
                    BuildInline(em.FirstChild, inlines, context);
                }
                break;
            case TaskList task:
                {
                    var rv = new CheckBox() { IsChecked = task.Checked, IsEnabled = false };
                    inlines.Add(rv);
                }
                break;
            case CodeInline code:
                {
                    var rv = new HighlightInline() { Text = code.Content };
                    inlines.Add(rv);
                }
                break;
            case LinkInline link:
                {
                    if (
                        link.IsImage
                        && link.Url is not null
                        && Uri.IsWellFormedUriString(link.Url, UriKind.Absolute)
                    )
                    {
                        var image = new Image();
                        ImageLoader.SetSource(image, link.Url);
                        if (!string.IsNullOrEmpty(link.Label))
                        {
                            ToolTip.SetTip(image, link.Label);
                        }
                        if (!string.IsNullOrEmpty(link.Url))
                        {
                            ToolTip.SetTip(image, link.Url);
                        }
                        // Already marked .Markdown
                        inlines.Add(image);
                    }
                    else
                    {
                        var linkBtn = new HyperlinkButton();
                        linkBtn.NavigateUri =
                            link.Url is not null
                            || Uri.IsWellFormedUriString(link.Url, UriKind.RelativeOrAbsolute)
                                ? new Uri(link.Url)
                                : null;
                        var label = new TextBlock();
                        var labelInlines = new InlineCollection();
                        if (link.FirstChild is not null)
                        {
                            BuildInline(link.FirstChild, labelInlines, context);
                            label.Inlines = labelInlines;
                        }
                        else
                        {
                            label.Text = link.Url;
                        }
                        label.Classes.Set("Hyperlink", true);

                        if (!string.IsNullOrEmpty(link.Label))
                        {
                            ToolTip.SetTip(linkBtn, link.Label);
                        }
                        if (!string.IsNullOrEmpty(link.Url))
                        {
                            ToolTip.SetTip(linkBtn, link.Url);
                        }
                        linkBtn.Content = label;
                        linkBtn.Classes.Set("Hyperlink", true);
                        inlines.Add(linkBtn);
                    }
                }
                break;
        }
    }

    private Panel SpawnStack() => new StackPanel() { Spacing = Spacing };

    private DockPanel SpawnDock() =>
        new DockPanel() { HorizontalSpacing = Spacing, VerticalSpacing = Spacing };

    private char GenerateUnorderedListHead(int depth) =>
        depth switch
        {
            0 => '●',
            1 => '○',
            2 => '■',
            _ => '□',
        };

    private string GenerateOrderedListHead(int depth, int index)
    {
        switch (depth)
        {
            default:
                return index.ToString();
        }
    }
}
