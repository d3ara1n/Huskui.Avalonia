using System;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Huskui.Avalonia.Controls;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Huskui.Avalonia.Markdown.Controls;

[TemplatePart(PART_LayoutContainer, typeof(StackPanel))]
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

    private StackPanel? _container;

    public static readonly StyledProperty<string?> MarkdownProperty = AvaloniaProperty.Register<
        MarkdownViewer,
        string?
    >(nameof(Markdown));

    public string? Markdown
    {
        get => GetValue(MarkdownProperty);
        set => SetValue(MarkdownProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _container = e.NameScope.Find<StackPanel>(PART_LayoutContainer);

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

        _container.Children.Clear();
        LogicalChildren.Clear();

        if (string.IsNullOrEmpty(markdown))
            return;

        var document = Markdig.Markdown.Parse(markdown, markdownPipeline);

        foreach (var block in document)
        {
            var control = RenderBlock(block);
            if (control != null)
            {
                _container.Children.Add(control);
                LogicalChildren.Add(control);
            }
        }
    }

    private Control? RenderBlock(Block block)
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
            case CodeBlock code:
                {
                    var rv = new CodeViewer();
                    rv.Code = code.Lines.ToString() ?? string.Empty;
                    if (code is FencedCodeBlock fenced) rv.Language = fenced.Info ?? string.Empty;
                    rv.Classes.Set("Code", true);
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
            BuildInline(child, inlines, new());
        }
        foreach (var child in inlines)
        {
            child.Classes.Set("Markdown", true);
        }
        return inlines;
    }

    private class EmphasisContext
    {
        public bool Bold { get; set; }
        public bool Italic { get; set; }
        public bool Strikethrough { get; set; }
        public bool Highlighted { get; set; }
    }

    private void BuildInline(
        Markdig.Syntax.Inlines.Inline inline,
        InlineCollection inlines,
        EmphasisContext context
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
                    run.Classes.Set("Strikethrough", context.Strikethrough);
                    run.Classes.Set("Highlighted", context.Highlighted);

                    inlines.Add(run);
                }
                break;
            case EmphasisInline em when em.FirstChild is not null:
                {
                    context.Bold |= em is { DelimiterChar: '*', DelimiterCount: 2 };
                    context.Italic |= em is { DelimiterChar: '*', DelimiterCount: 1 };
                    context.Strikethrough |= em is { DelimiterChar: '~', DelimiterCount: 2 };
                    context.Highlighted |= em is { DelimiterChar: '=', DelimiterCount: 2 };
                    BuildInline(
                        em.FirstChild,
                        inlines,
                        context
                    );
                }
                break;
            case CodeInline code:
                {
                    inlines.Add(new HighlightInline() { Text = code.Content });
                }
                break;
            case LinkInline link:
                {
                    var linkBtn = new HyperlinkButton();
                    linkBtn.NavigateUri =
                        link.Url is not null
                        || Uri.IsWellFormedUriString(link.Url, UriKind.RelativeOrAbsolute)
                            ? new Uri(link.Url)
                            : null;
                    if (link.IsImage)
                    {
                        // TODO:
                    }
                    else
                    {
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
                        linkBtn.Content = label;
                    }
                    if (!string.IsNullOrEmpty(link.Label))
                    {
                        ToolTip.SetTip(linkBtn, link.Label);
                    }
                    if (!string.IsNullOrEmpty(link.Url))
                    {
                        ToolTip.SetTip(linkBtn, link.Url);
                    }

                    linkBtn.Classes.Set("Hyperlink", true);
                    inlines.Add(linkBtn);
                }
                break;
        }
    }

    private string FlattenInline(Markdig.Syntax.Inlines.Inline inline)
    {
        var builder = new StringBuilder();
        FlattenInline(inline, builder);
        return builder.ToString();
    }

    private void FlattenInline(Markdig.Syntax.Inlines.Inline inline, StringBuilder builder)
    {
        switch (inline)
        {
            case LiteralInline lit:
                builder.Append(lit.ToString());
                break;
            case EmphasisInline em:
                var child = em.FirstChild;
                while (child is not null)
                {
                    FlattenInline(child, builder);
                    child = child.NextSibling;
                }
                break;
            case CodeInline code:
                builder.Append(code.Content.ToString());
                break;
            default:
                builder.Append(inline.ToString());
                break;
        }
    }
}
