using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class CrossFadeTransition : PageTransitionBase
{
    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        from
           .Animation(new QuarticEaseInOut())
           .AddFrame(0d,
            [
                (Visual.OpacityProperty, 1d),
                (ScaleTransform.ScaleXProperty, 1.0d),
                (ScaleTransform.ScaleYProperty, 1.0d)
            ])
           .AddFrame(1d,
            [
                (Visual.OpacityProperty, 0d),
                (ScaleTransform.ScaleXProperty, 0.98d),
                (ScaleTransform.ScaleYProperty, 0.98d)
            ]);
        to
           .Animation(new QuarticEaseInOut())
           .AddFrame(0d,
            [
                (Visual.OpacityProperty, 0d),
                (ScaleTransform.ScaleXProperty, 0.98d),
                (ScaleTransform.ScaleYProperty, 0.98d)
            ])
           .AddFrame(1d,
            [
                (Visual.OpacityProperty, 1d), (ScaleTransform.ScaleXProperty, 1d), (ScaleTransform.ScaleYProperty, 1d)
            ]);
    }
}
