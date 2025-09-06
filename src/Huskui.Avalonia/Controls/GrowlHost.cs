using Avalonia;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.Styling;
using Avalonia.Threading;
using Huskui.Avalonia.Transitions;

namespace Huskui.Avalonia.Controls;

public class GrowlHost : TemplatedControl
{
    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        AvaloniaProperty.Register<GrowlHost, HorizontalAlignment>(nameof(HorizontalContentAlignment));

    public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
        AvaloniaProperty.Register<GrowlHost, VerticalAlignment>(nameof(VerticalContentAlignment));

    public HorizontalAlignment HorizontalContentAlignment
    {
        get => GetValue(HorizontalContentAlignmentProperty);
        set => SetValue(HorizontalContentAlignmentProperty, value);
    }

    public VerticalAlignment VerticalContentAlignment
    {
        get => GetValue(VerticalContentAlignmentProperty);
        set => SetValue(VerticalContentAlignmentProperty, value);
    }


    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<OverlayHost, int>(nameof(ItemCount));

    public int ItemCount
    {
        get => GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }

    public static readonly StyledProperty<ITemplate> ItemsPanelProperty =
        AvaloniaProperty.Register<OverlayHost, ITemplate>(nameof(ItemsPanel),
                                                          new FuncTemplate<StackPanel>(() => new()));

    public ITemplate ItemsPanel
    {
        get => GetValue(ItemsPanelProperty);
        set => SetValue(ItemsPanelProperty, value);
    }

    public static readonly StyledProperty<IPageTransition> TransitionProperty =
        AvaloniaProperty.Register<GrowlHost, IPageTransition>(nameof(Transition), new GrowlTransition());

    public IPageTransition Transition
    {
        get => GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    [Content]
    public AvaloniaList<GrowlItem> Items { get; } = [];

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AddHandler(GrowlItem.DismissRequestedEvent, DismissedRequested);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        RemoveHandler(GrowlItem.DismissRequestedEvent, DismissedRequested);
    }

    private void DismissedRequested(object? sender, RoutedEventArgs e)
    {
        if (e.Source is GrowlItem item)
        {
            Dismiss(item);
            e.Handled = true;
        }
    }


    public void Pop(GrowlItem item)
    {
        Items.Add(item);
        ItemCount = Items.Count;
        LogicalChildren.Add(item);

        Transition.Start(null, item, true, CancellationToken.None);
    }

    public void Dismiss(GrowlItem item)
    {
        Transition
           .Start(item, null, false, CancellationToken.None)
           .ContinueWith(_ => Dispatcher.UIThread.Post(() =>
            {
                {
                    LogicalChildren.Remove(item);
                    Items.Remove(item);
                    ItemCount = Items.Count;
                }
            }));
    }
}
