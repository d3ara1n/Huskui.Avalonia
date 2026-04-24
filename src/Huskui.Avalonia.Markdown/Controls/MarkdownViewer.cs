using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Media;
using Huskui.Avalonia.Code.Controls;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Markdown.Models;
using Markdig;
using Markdig.Extensions.Alerts;
using Markdig.Extensions.TaskLists;
using Markdig.Extensions.Yaml;
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
                          .UseAlertBlocks()
                          .UseTaskLists()
                          .UseYamlFrontMatter()
                          .UseEmphasisExtras()
                          .UseGridTables()
                          .UsePipeTables()
                          .UseListExtras()
                          .Build();
    }

    private ScrollViewer? _container;

    public static readonly StyledProperty<string?> MarkdownProperty =
        AvaloniaProperty.Register<MarkdownViewer, string?>(nameof(Markdown));

    public string? Markdown
    {
        get => GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<MarkdownViewer, double>(nameof(Spacing), 4);

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public static readonly StyledProperty<FrontMatterRenderMethods> FrontMatterRenderProperty =
        AvaloniaProperty.Register<MarkdownViewer, FrontMatterRenderMethods>(nameof(FrontMatterRender),
                                                                            FrontMatterRenderMethods.Ignore);

    public FrontMatterRenderMethods FrontMatterRender
    {
        get => GetValue(FrontMatterRenderProperty);
        set => SetValue(FrontMatterRenderProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _container = e.NameScope.Find<ScrollViewer>(PART_LayoutContainer);

        Render(Markdown);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == MarkdownProperty
         || change.Property == FrontMatterRenderProperty
         || change.Property == SpacingProperty)
        {
            Render(Markdown);
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
    }

    private Control? RenderBlock(Block block, BlockContext context = default)
    {
        Control? control;
        switch (block)
        {
            case HeadingBlock heading:
            {
                var rv = SpawnText();
                rv.Inlines = RenderInlines(heading.Inline);
                rv.Classes.Set($"Heading{heading.Level}", true);
                control = rv;
            }
                break;
            case ParagraphBlock paragraph:
            {
                var rv = SpawnText();
                rv.Inlines = RenderInlines(paragraph.Inline);
                rv.Classes.Set("Paragraph", true);
                control = rv;
            }
                break;
            case ListBlock list:
            {
                var panel = SpawnStack();
                context.ListOrdered = list.IsOrdered;
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

                var bullet = SpawnText();
                bullet.Text = context.ListOrdered
                                  ? GenerateOrderedListHead(context.ListDepth, context.ListIndex)
                                  : GenerateUnorderedListHead(context.ListDepth);
                DockPanel.SetDock(bullet, Dock.Left);
                dock.Children.Add(bullet);
                dock.Children.Add(stack);
                bullet.Classes.Set("List", true);
                bullet.Classes.Set("Bullet", true);
                dock.Classes.Set("List", true);
                dock.Classes.Set("Item", true);
                stack.Classes.Set("List", true);
                stack.Classes.Set("Item", true);
                control = dock;
            }
                break;
            case YamlFrontMatterBlock yaml:
            {
                switch (FrontMatterRender)
                {
                    case FrontMatterRenderMethods.Plain:
                        var bar = new InfoBar();
                        bar.Content = yaml.Lines.ToString();
                        bar.Classes.Set("FrontMatter", true);
                        control = bar;
                        break;
                    case FrontMatterRenderMethods.Pretty:
                        var viewer = new CodeViewer();
                        viewer.Language = "yaml";
                        viewer.Code = yaml.Lines.ToString();
                        viewer.Classes.Set("FrontMatter", true);
                        control = viewer;
                        break;
                    default:
                        control = null;
                        break;
                }
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

                if (quote is AlertBlock alert)
                {
                    rv.Header = alert.Kind.ToString();
                    rv.Classes.Set(alert.Kind.ToString().ToUpper() switch
                                   {
                                       "TIP" => "Success",
                                       "WARNING" => "Warning",
                                       "CAUTION" => "Danger",
                                       _ => "Primary",
                                   },
                                   true);
                }

                rv.Content = container;
                rv.Classes.Set("Quote", true);
                control = rv;
            }
                break;
            case ThematicBreakBlock:
            {
                var rv = new Divider() { Orientation = Orientation.Horizontal };
                rv.Classes.Set("Rule", true);
                control = rv;
            }
                break;
            default:
            {
                var rv = SpawnText();
                rv.Text = block.ToString();
                rv.Classes.Set("Unknown", true);
                control = rv;
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
            if (child is InlineUIContainer container && container.Child is { } inner)
            {
                inner.Classes.Set("Markdown", true);
            }
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
        EmphasisContext context = default)
    {
        switch (inline)
        {
            case LiteralInline:
            case HtmlEntityInline:
            {
                var run = new Run(inline switch
                {
                    LiteralInline it => it.Content.ToString(),
                    HtmlEntityInline it => it.Transcoded.ToString(),
                    _ => inline.ToString(),
                });
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
                foreach (var innerInline in em)
                {
                    BuildInline(innerInline, inlines, context);
                }
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
                if (link.IsImage && link.Url is not null && Uri.IsWellFormedUriString(link.Url, UriKind.Absolute))
                {
                    var image = new Image();
                    ImageLoader.SetSource(image, link.Url);
                    inlines.Add(image);
                }
                else
                {
                    var linkBtn = new HyperlinkButton();
                    linkBtn.NavigateUri = link.Url is not null && Uri.IsWellFormedUriString(link.Url, UriKind.Absolute)
                                              ? new Uri(link.Url, UriKind.Absolute)
                                              : null;
                    var label = SpawnText();
                    if (link.FirstChild is not null)
                    {
                        var labelInlines = new InlineCollection();
                        foreach (var child in link)
                        {
                            BuildInline(child, labelInlines, context);
                        }

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
            case LineBreakInline:
            {
                var rv = new LineBreak();
                inlines.Add(rv);
            }
                break;
            default:
            {
                var text = SpawnText();
                text.Text = inline.ToString();
                text.Classes.Set("Unknown", true);
                inlines.Add(text);
            }
                break;
        }
    }

    private Panel SpawnStack() => new StackPanel() { Spacing = Spacing };

    private DockPanel SpawnDock() => new DockPanel() { HorizontalSpacing = Spacing, VerticalSpacing = Spacing };

    // 不能是 SelectableTextBlock 因为会吃掉内部 HyperlinkButton 的交互
    private TextBlock SpawnText() => new TextBlock() { TextWrapping = TextWrapping.Wrap };

    private string GenerateUnorderedListHead(int depth) =>
        depth switch
        {
            1 => "●",
            2 => "○",
            3 => "■",
            4 => "□",
            5 => "◆",
            6 => "♢",
            _ => "‥",
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
