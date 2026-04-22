using Avalonia;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Huskui.Avalonia.Controls;

public class Page : HeaderedContentControl
{
    public static readonly StyledProperty<bool> CanGoBackProperty = AvaloniaProperty.Register<
        Page,
        bool
    >(nameof(CanGoBack));

    public static readonly DirectProperty<Page, bool> IsHeaderVisibleProperty =
        AvaloniaProperty.RegisterDirect<Page, bool>(
            nameof(IsHeaderVisible),
            o => o.IsHeaderVisible,
            (o, v) => o.IsHeaderVisible = v
        );

    public static readonly DirectProperty<Page, bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.RegisterDirect<Page, bool>(
            nameof(IsBackButtonVisible),
            o => o.IsBackButtonVisible,
            (o, v) => o.IsBackButtonVisible = v
        );

    public static readonly StyledProperty<BoxShadows> BoxShadowProperty = AvaloniaProperty.Register<
        Page,
        BoxShadows
    >(nameof(BoxShadow));

    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }

    public bool IsBackButtonVisible
    {
        get;
        set => SetAndRaise(IsBackButtonVisibleProperty, ref field, value);
    } = true;

    public bool CanGoBack
    {
        get => GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    public bool IsHeaderVisible
    {
        get;
        set => SetAndRaise(IsHeaderVisibleProperty, ref field, value);
    } = true;

    protected override Type StyleKeyOverride => typeof(Page);
}
