using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":active")]
[TemplatePart(PART_ContentPresenter, typeof(ContentPresenter))]
public class OverlayItem : ContentControl
{
    public const string PART_ContentPresenter = nameof(PART_ContentPresenter);

    public static readonly DirectProperty<OverlayItem, int> DistanceProperty =
        AvaloniaProperty.RegisterDirect<OverlayItem, int>(nameof(Distance), o => o.Distance, (o, v) => o.Distance = v);

    public static readonly DirectProperty<OverlayItem, IPageTransition?> TransitionProperty =
        AvaloniaProperty.RegisterDirect<OverlayItem, IPageTransition?>(nameof(Transition),
                                                                       o => o.Transition,
                                                                       (o, v) => o.Transition = v);

    public static readonly RoutedEvent<DismissRequestedEventArgs> DismissRequestedEvent =
        RoutedEvent.Register<OverlayItem, DismissRequestedEventArgs>(nameof(DismissRequested),
                                                                     RoutingStrategies.Bubble);

    public IPageTransition? Transition
    {
        get;
        set => SetAndRaise(TransitionProperty, ref field, value);
    }

    public int Distance
    {
        get;
        set
        {
            if (SetAndRaise(DistanceProperty, ref field, value))
            {
                ZIndex = -value;
            }

            PseudoClasses.Set(":active", value == 0);
        }
    }

    protected override Type StyleKeyOverride => typeof(OverlayItem);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        if (change.Property == ContentProperty && change.NewValue is IPageTransitionOverride @override)
        {
            Transition = @override.TransitionOverride;
        }
    }

    public event EventHandler<DismissRequestedEventArgs>? DismissRequested
    {
        add => AddHandler(DismissRequestedEvent, value);
        remove => RemoveHandler(DismissRequestedEvent, value);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        AddHandler(DismissRequestedEvent, DismissRequestedHandler);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        RemoveHandler(DismissRequestedEvent, DismissRequestedHandler);
    }

    private void DismissRequestedHandler(object? sender, DismissRequestedEventArgs e) =>
        // 捕捉路过的事件并加上容器信息
        e.Container ??= this;

    #region Nested type: DismissRequestedEventArgs

    public class DismissRequestedEventArgs(object? source = null) : RoutedEventArgs(DismissRequestedEvent, source)
    {
        public OverlayItem? Container { get; set; }
    }

    #endregion
}
