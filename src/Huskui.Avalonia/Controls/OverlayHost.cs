using Avalonia;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Metadata;
using Avalonia.Styling;
using Huskui.Avalonia.Transitions;
using VisualExtensions = Avalonia.VisualTree.VisualExtensions;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_SmokeMask, typeof(Border))]
[TemplatePart(PART_ItemsPresenter, typeof(ItemsPresenter))]
[PseudoClasses(":present")]
public class OverlayHost : TemplatedControl
{
    public const string PART_ItemsPresenter = nameof(PART_ItemsPresenter);
    public const string PART_SmokeMask = nameof(PART_SmokeMask);

    public static readonly DirectProperty<OverlayHost, bool> IsPresentProperty =
        AvaloniaProperty.RegisterDirect<OverlayHost, bool>(nameof(IsPresent),
                                                           o => o.IsPresent,
                                                           (o, v) => o.IsPresent = v);


    public static readonly StyledProperty<IPageTransition> TransitionProperty =
        AvaloniaProperty.Register<OverlayHost, IPageTransition>(nameof(Transition),
                                                                new PageCoverOverTransition(null,
                                                                    DirectionFrom.Bottom));

    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<OverlayHost, int>(nameof(ItemCount));

    public static readonly RoutedEvent<PropertyChangedRoutedEventArgs<bool>> IsPresentChangedEvent =
        RoutedEvent.Register<OverlayHost, PropertyChangedRoutedEventArgs<bool>>(nameof(IsPresentChanged),
                                                                                    RoutingStrategies.Bubble);

    public static readonly RoutedEvent<MaskPointerPressedEventArgs> MaskPointerPressedEvent =
        RoutedEvent.Register<OverlayHost, MaskPointerPressedEventArgs>(nameof(MaskPointerPressed),
                                                                       RoutingStrategies.Bubble);

    public static readonly RoutedEvent<DismissRequestedEventArgs> DismissRequestedEvent =
        RoutedEvent.Register<OverlayItem, DismissRequestedEventArgs>(nameof(DismissRequested),
                                                                     RoutingStrategies.Bubble);

    public event EventHandler<DismissRequestedEventArgs>? DismissRequested
    {
        add => AddHandler(DismissRequestedEvent, value);
        remove => RemoveHandler(DismissRequestedEvent, value);
    }

    public static readonly StyledProperty<ITemplate> ItemsPanelProperty =
        AvaloniaProperty.Register<OverlayHost, ITemplate>(nameof(ItemsPanel), new FuncTemplate<Panel>(() => new()));

    private readonly Queue<OverlayItem> _toDismiss = [];

    private readonly Queue<OverlayItem> _toPops = [];

    private Border? _smokeMask;

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

    public ITemplate ItemsPanel
    {
        get => GetValue(ItemsPanelProperty);
        set => SetValue(ItemsPanelProperty, value);
    }

    [Content]
    public AvaloniaList<OverlayItem> Items { get; } = [];

    public bool IsPresent
    {
        get;
        set => SetAndRaise(IsPresentProperty, ref field, value);
    }

    protected override Type StyleKeyOverride => typeof(OverlayHost);

    public event EventHandler<MaskPointerPressedEventArgs> MaskPointerPressed
    {
        add => AddHandler(MaskPointerPressedEvent, value);
        remove => RemoveHandler(MaskPointerPressedEvent, value);
    }

    public event EventHandler<PropertyChangedRoutedEventArgs<bool>>? IsPresentChanged
    {
        add => AddHandler(IsPresentChangedEvent, value);
        remove => RemoveHandler(IsPresentChangedEvent, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnregisterHandlers();

        _smokeMask = e.NameScope.Find<Border>(PART_SmokeMask);

        if (_smokeMask != null)
        {
            _smokeMask.PointerPressed += SmokeMask_OnPointerPressed;
        }
    }

    private void UnregisterHandlers()
    {
        if (_smokeMask != null)
        {
            _smokeMask.PointerPressed -= SmokeMask_OnPointerPressed;
        }
    }

    private void SmokeMask_OnPointerPressed(object? sender, PointerPressedEventArgs e) =>
        RaiseEvent(new MaskPointerPressedEventArgs(this, e));

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsPresentProperty)
        {
            PseudoClasses.Set(":present", change.GetNewValue<bool>());
            RaiseEvent(new PropertyChangedRoutedEventArgs<bool>(IsPresentChangedEvent,
                                                                this,
                                                                change.GetOldValue<bool>(),
                                                                change.GetNewValue<bool>()));
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var rv = base.ArrangeOverride(finalSize);

        if (_toPops.Count > 0)
        {
            while (_toPops.TryDequeue(out var item))
            {
                var transition = item.Transition ?? Transition;
                transition.Start(null, item, true, CancellationToken.None);
            }
        }

        if (_toDismiss.Count > 0)
        {
            while (_toDismiss.TryDequeue(out var item))
            {
                var transition = item.Transition ?? Transition;
                transition
                   .Start(item, null, true, CancellationToken.None)
                   .ContinueWith(_ =>
                                 {
                                     for (var i = 0; i < Items.IndexOf(item); i++)
                                     {
                                         if (Items[i] is { } inner)
                                         {
                                             inner.Distance--;
                                         }
                                     }


                                     LogicalChildren.Remove(item);
                                     Items.Remove(item);
                                     ItemCount = Items.Count;
                                     if (Items.Count == 0)
                                     {
                                         IsPresent = false;
                                     }
                                 },
                                 TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        return rv;
    }

    public void Pop(object control)
    {
        var item = new OverlayItem { Content = control, Distance = 0 };
        foreach (var i in Items.OfType<OverlayItem>())
        {
            i.Distance++;
        }

        Items.Add(item);
        LogicalChildren.Add(item);

        ItemCount = Items.Count;
        if (Items.Count == 1)
        {
            IsPresent = true;
        }

        _toPops.Enqueue(item);
        InvalidateArrange();
    }

    public void Dismiss(object control)
    {
        var item = control switch
        {
            OverlayItem it => it,
            Visual visual => VisualExtensions.FindAncestorOfType<OverlayItem>(visual),
            _ => null
        };
        if (item is not null && !_toDismiss.Contains(item))
        {
            _toDismiss.Enqueue(item);
            InvalidateArrange();
        }
    }

    public void Dismiss()
    {
        if (Items is [.., { } last])
        {
            Dismiss(last);
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AddHandler(DismissRequestedEvent, DismissRequestedHandler);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        RemoveHandler(DismissRequestedEvent, DismissRequestedHandler);

        UnregisterHandlers();
    }

    private void DismissRequestedHandler(object? sender, DismissRequestedEventArgs e)
    {
        if (e.Container != null)
        {
            Dismiss(e.Container);

            e.Handled = true;
        }
    }

    #region Nested type: MaskPointerPressedEventArgs

    public class MaskPointerPressedEventArgs(object? source, PointerPressedEventArgs args)
        : RoutedEventArgs(MaskPointerPressedEvent, source)
    {
        public PointerPressedEventArgs Inner => args;
    }

    #endregion

    #region ContentAlignment

    public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
        ContentControl.HorizontalContentAlignmentProperty.AddOwner<OverlayHost>();

    public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
        ContentControl.VerticalContentAlignmentProperty.AddOwner<OverlayHost>();

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

    #endregion


    #region Nested type: DismissRequestedEventArgs

    public class DismissRequestedEventArgs(object? source = null) : RoutedEventArgs(DismissRequestedEvent, source)
    {
        public OverlayItem? Container { get; set; }
    }

    #endregion
}
