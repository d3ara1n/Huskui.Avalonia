using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using Huskui.Avalonia.Transitions;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":present")]
[TemplatePart(PART_ItemsPresenter, typeof(ItemsPresenter))]
public class OverlayHost : ItemsControl
{
    public const string PART_ItemsPresenter = nameof(PART_ItemsPresenter);

    public static readonly DirectProperty<OverlayHost, bool> IsPresentProperty =
        AvaloniaProperty.RegisterDirect<OverlayHost, bool>(nameof(IsPresent),
                                                           o => o.IsPresent,
                                                           (o, v) => o.IsPresent = v);


    public static readonly DirectProperty<OverlayHost, IPageTransition> TransitionProperty =
        AvaloniaProperty.RegisterDirect<OverlayHost, IPageTransition>(nameof(Transition),
                                                                      o => o.Transition,
                                                                      (o, v) => o.Transition = v);

    public static readonly RoutedEvent<PropertyChangedRoutedEventArgs<bool>> IsPresentChangedEvent =
        RoutedEvent.Register<OverlayHost, PropertyChangedRoutedEventArgs<bool>>(nameof(IsPresentChanged),
                                                                                    RoutingStrategies.Bubble);

    public bool IsPresent
    {
        get;
        set => SetAndRaise(IsPresentProperty, ref field, value);
    }

    public IPageTransition Transition
    {
        get;
        set => SetAndRaise(TransitionProperty, ref field, value);
    } = new PageCoverOverTransition(null, DirectionFrom.Bottom);

    protected override Type StyleKeyOverride => typeof(OverlayHost);

    public event EventHandler<PropertyChangedRoutedEventArgs<bool>>? IsPresentChanged
    {
        add => AddHandler(IsPresentChangedEvent, value);
        remove => RemoveHandler(IsPresentChangedEvent, value);
    }

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

    public void Pop(object control)
    {
        IsVisible = true;
        var item = new OverlayItem { Content = control, Distance = 0 };
        foreach (var i in Items.OfType<OverlayItem>())
            i.Distance++;
        Items.Add(item);


        // Make control attached to visual tree ensuring its parent is valid
        // Make OnApplyTemplate called and _stage bound
        UpdateLayout();
        // if (control is Visual visual) Transition.Start(null, visual, true, CancellationToken.None);
        var transition = item.Transition ?? Transition;
        transition.Start(null, item.ContentPresenter, true, CancellationToken.None);

        if (Items.Count == 1)
        {
            IsPresent = true;
            // ArgumentNullException.ThrowIfNull(_stage);
            // StageInAnimation.RunAsync(_stage);
        }
    }

    public void Dismiss(OverlayItem item)
    {
        var transition = item.Transition ?? Transition;
        transition
           .Start(item.ContentPresenter, null, false, CancellationToken.None)
           .ContinueWith(_ => Dispatcher.UIThread.Post(Clean));
        return;

        void Clean()
        {
            for (var i = 0; i < Items.IndexOf(item); i++)
                if (Items[i] is OverlayItem inner)
                    inner.Distance--;

            Items.Remove(item);
            if (Items.Count == 0)
            {
                IsPresent = false;
                // ArgumentNullException.ThrowIfNull(_stage);
                // StageOutAnimation.RunAsync(_stage).ContinueWith(_ => Dispatcher.UIThread.Post(() => IsVisible = false));
            }
        }
    }

    public void Dismiss()
    {
        if (Items is [.., OverlayItem last])
            Dismiss(last);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        AddHandler(OverlayItem.DismissRequestedEvent, DismissRequestedHandler);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        RemoveHandler(OverlayItem.DismissRequestedEvent, DismissRequestedHandler);
    }

    private void DismissRequestedHandler(object? sender, OverlayItem.DismissRequestedEventArgs e)
    {
        if (e.Container != null)
            Dismiss(e.Container);
    }

    #region StageInAnimation & StageOutAnimation

    private static readonly Animation StageInAnimation = new()
    {
        FillMode = FillMode.Forward,
        Duration = TimeSpan.FromMilliseconds(146),
        Easing = new SineEaseOut(),
        Children =
        {
            new KeyFrame
            {
                Cue = new Cue(0d), Setters = { new Setter { Property = OpacityProperty, Value = 0d } }
            },
            new KeyFrame
            {
                Cue = new Cue(1d), Setters = { new Setter { Property = OpacityProperty, Value = 1d } }
            }
        }
    };

    private static readonly Animation StageOutAnimation = new()
    {
        FillMode = FillMode.Forward,
        Duration = TimeSpan.FromMilliseconds(146),
        Easing = new SineEaseOut(),
        Children =
        {
            new KeyFrame
            {
                Cue = new Cue(0d), Setters = { new Setter { Property = OpacityProperty, Value = 1d } }
            },
            new KeyFrame
            {
                Cue = new Cue(1d), Setters = { new Setter { Property = OpacityProperty, Value = 0d } }
            }
        }
    };

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
}