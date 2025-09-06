using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class SidebarTransition : PageTransitionBase
{
    public SidebarTransition() : this(null) { }
    public SidebarTransition(TimeSpan? duration = null) : base(duration) { }

    protected override void Cleanup(Visual? from, Visual? to)
    {
        base.Cleanup(from, to);

        from?.RenderTransform = null;
        to?.RenderTransform = null;
    }

    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        var parent = parentAccessor.Value;
        var slideDistance = parent.Bounds.Width;

        from
           .Animation(new CubicEaseInOut())
           .AddFrame(0d, [(TranslateTransform.XProperty, 0d), (Visual.OpacityProperty, 1d)])
           .AddFrame(0.2d, [(Visual.OpacityProperty, 0.8d)])
           .AddFrame(1d, [(TranslateTransform.XProperty, slideDistance), (Visual.OpacityProperty, 0d)]);


        to
           .Animation(new SplineEasing(0.1, 0.9, 0.2))
           .AddFrame(0d, [(TranslateTransform.XProperty, slideDistance), (Visual.OpacityProperty, 0d)])
           .AddFrame(0.1d, [(Visual.OpacityProperty, 0.6d)])
           .AddFrame(0.4d, [(Visual.OpacityProperty, 1d)])
           .AddFrame(1d, [(TranslateTransform.XProperty, 0d), (Visual.OpacityProperty, 1d)]);
    }
}
