using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class GrowlTransition() : PageTransitionBase(TimeSpan.FromMilliseconds(400))
{
    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        var parent = parentAccessor.Value;
        var offset = parent.Bounds.Width;

        from
           .Animation(new CubicEaseOut())
           .AddFrame(0.3d,
            [
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d),
                (TranslateTransform.XProperty, 0.0d)
            ])
           .AddFrame(0.9d,
            [
                (Visual.OpacityProperty, 1.0d),
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d),
                (TranslateTransform.XProperty, 0.0d)
            ])
           .AddFrame(1.0d,
            [
                (Visual.OpacityProperty, 0.0d),
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d),
                (TranslateTransform.XProperty, offset)
            ]);

        to
           .Animation(new CubicEaseOut())
           .AddFrame(0.0, [(Visual.OpacityProperty, 0d)])
           .AddFrame(0.5d,
            [
                (Visual.OpacityProperty, 0d),
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d)
            ])
           .AddFrame(0.9d,
            [
                (Visual.OpacityProperty, 1.0d),
                (ScaleTransform.ScaleXProperty, 1.05d),
                (ScaleTransform.ScaleYProperty, 1.05d)
            ])
           .AddFrame(1.0d,
            [
                (Visual.OpacityProperty, 1.0d),
                (ScaleTransform.ScaleXProperty, 1.0d),
                (ScaleTransform.ScaleYProperty, 1.0d)
            ]);
    }
}
