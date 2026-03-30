using Avalonia;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.VisualTree;
using Huskui.Avalonia.Transitions;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ItemsPresenter, typeof(ItemsPresenter))]
[PseudoClasses(":present")]
public class DrawerHost : TemplatedControl
{
    public const string PART_ItemsPresenter = nameof(PART_ItemsPresenter);

    public static readonly StyledProperty<IPageTransition> TransitionProperty =
        AvaloniaProperty.Register<DrawerHost, IPageTransition>(
            nameof(Transition),
            new DrawerTransition()
        );

    public static readonly StyledProperty<int> ItemCountProperty = AvaloniaProperty.Register<
        DrawerHost,
        int
    >(nameof(ItemCount));

    public static readonly RoutedEvent<DismissRequestedEventArgs> DismissRequestedEvent =
        RoutedEvent.Register<Drawer, DismissRequestedEventArgs>(
            nameof(DismissRequested),
            RoutingStrategies.Bubble
        );

    public static readonly RoutedEvent<BringToFrontRequestedEventArgs> BringToFrontRequestedEvent =
        RoutedEvent.Register<Drawer, BringToFrontRequestedEventArgs>(
            nameof(BringToFrontRequested),
            RoutingStrategies.Bubble
        );

    private readonly Queue<Drawer> _toDismiss = [];
    private readonly Queue<Drawer> _toPops = [];

    public IPageTransition Transition
    {
        get => GetValue(TransitionProperty);
        set => SetValue(TransitionProperty, value);
    }

    public int ItemCount
    {
        get => GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }

    [Content]
    public AvaloniaList<Drawer> Items { get; } = [];

    public event EventHandler<DismissRequestedEventArgs>? DismissRequested
    {
        add => AddHandler(DismissRequestedEvent, value);
        remove => RemoveHandler(DismissRequestedEvent, value);
    }

    public event EventHandler<BringToFrontRequestedEventArgs>? BringToFrontRequested
    {
        add => AddHandler(BringToFrontRequestedEvent, value);
        remove => RemoveHandler(BringToFrontRequestedEvent, value);
    }

    protected override Type StyleKeyOverride => typeof(DrawerHost);

    protected override Size ArrangeOverride(Size finalSize)
    {
        var rv = base.ArrangeOverride(finalSize);

        if (_toPops.Count > 0)
        {
            while (_toPops.TryDequeue(out var drawer))
            {
                Transition.Start(null, drawer, true, CancellationToken.None);
            }
        }

        if (_toDismiss.Count > 0)
        {
            while (_toDismiss.TryDequeue(out var drawer))
            {
                Transition
                    .Start(drawer, null, true, CancellationToken.None)
                    .ContinueWith(
                        _ =>
                        {
                            if (Items.Remove(drawer))
                            {
                                drawer.IsDismissed = true;
                                LogicalChildren.Remove(drawer);
                                ItemCount = Items.Count;
                                UpdatePresentPseudoClass();
                            }
                        },
                        TaskScheduler.FromCurrentSynchronizationContext()
                    );
            }
        }

        return rv;
    }

    public void Pop(Drawer drawer)
    {
        var height = Bounds.Height * 2 / 3;
        var width = Bounds.Width * 1 / 2;
        if (height > drawer.MaxHeight)
        {
            height = drawer.MaxHeight;
        }
        else if (height < drawer.MinHeight)
        {
            height = drawer.MinHeight;
        }

        if (width > drawer.MaxWidth)
        {
            width = drawer.MaxWidth;
        }
        else if (width < drawer.MinWidth)
        {
            width = drawer.MinWidth;
        }

        drawer.Height = height;
        drawer.Width = width;
        drawer.OffsetX = (Bounds.Width - width) / 2;
        drawer.IsDismissed = false;

        Items.Add(drawer);
        ItemCount = Items.Count;
        LogicalChildren.Add(drawer);
        UpdatePresentPseudoClass();

        _toPops.Enqueue(drawer);
        InvalidateArrange();
    }

    public void Dismiss(object control)
    {
        var drawer = control switch
        {
            Drawer it => it,
            Visual visual => visual.FindAncestorOfType<Drawer>(),
            _ => null,
        };

        if (drawer is null || !Items.Contains(drawer) || _toDismiss.Contains(drawer))
        {
            return;
        }

        _toDismiss.Enqueue(drawer);
        InvalidateArrange();
    }

    public void Dismiss()
    {
        if (Items is [.., { } last])
        {
            Dismiss(last);
        }
    }

    public void BringToFront(object control)
    {
        var drawer = control switch
        {
            Drawer it => it,
            Visual visual => visual.FindAncestorOfType<Drawer>(),
            _ => null,
        };

        if (drawer is null)
        {
            return;
        }

        var max = 0;
        foreach (var item in Items)
        {
            if (item.ZIndex > max)
                max = item.ZIndex;
            item.ZIndex--;
        }
        drawer.ZIndex = max;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AddHandler(DismissRequestedEvent, DismissRequestedHandler);
        AddHandler(BringToFrontRequestedEvent, BringToFrontRequestedHandler);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        RemoveHandler(DismissRequestedEvent, DismissRequestedHandler);
        RemoveHandler(BringToFrontRequestedEvent, BringToFrontRequestedHandler);
    }

    private void DismissRequestedHandler(object? sender, DismissRequestedEventArgs e)
    {
        if (e.Drawer != null)
        {
            Dismiss(e.Drawer);
            e.Handled = true;
        }
    }

    private void BringToFrontRequestedHandler(object? sender, BringToFrontRequestedEventArgs e)
    {
        if (e.Drawer != null)
        {
            BringToFront(e.Drawer);
            e.Handled = true;
        }
    }

    private void UpdatePresentPseudoClass()
    {
        if (Items.Count > 0)
        {
            PseudoClasses.Add(":present");
        }
        else
        {
            PseudoClasses.Remove(":present");
        }
    }

    public class DismissRequestedEventArgs(object? source = null)
        : RoutedEventArgs(DismissRequestedEvent, source)
    {
        public Drawer? Drawer { get; set; }
    }

    public class BringToFrontRequestedEventArgs(object? source = null)
        : RoutedEventArgs(BringToFrontRequestedEvent, source)
    {
        public Drawer? Drawer { get; set; }
    }
}
