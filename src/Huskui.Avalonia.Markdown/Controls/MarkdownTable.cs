using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Markdown.Controls;

/// <summary>
///     A markdown table container. <see cref="HeaderedItemsControl.Header" /> holds the header row
///     and <see cref="ItemsControl.Items" /> hold the body rows. The control only owns the table
///     chrome (border, corner radius, header background); cell visuals come from external style
///     selectors targeting the <c>Cell</c> class.
/// </summary>
public class MarkdownTable : HeaderedItemsControl
{
    public const string CLASS_NoHeader = ":noheader";

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        PseudoClasses.Set(CLASS_NoHeader, Header is null);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == HeaderProperty)
            PseudoClasses.Set(CLASS_NoHeader, Header is null);
    }
}
