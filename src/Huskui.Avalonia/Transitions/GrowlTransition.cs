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
           .Animation(new CubicEaseInOut())
           .AddFrame(0.3d,
            [
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d),
                (TranslateTransform.XProperty, 0.0d)
            ])
           .AddFrame(0.7d,
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
           .AddFrame(0.0,
            [
                (Visual.OpacityProperty, 0.0d),
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d),
                (TranslateTransform.XProperty, offset)
            ])
           .AddFrame(0.8d,
            [
                (Visual.OpacityProperty, 1.0d),
                (ScaleTransform.ScaleXProperty, 0.95d),
                (ScaleTransform.ScaleYProperty, 0.95d),
                (TranslateTransform.XProperty, 0.0d)
            ])
           .AddFrame(1.0d,
            [
                (Visual.OpacityProperty, 1.0d),
                (ScaleTransform.ScaleXProperty, 1.0d),
                (ScaleTransform.ScaleYProperty, 1.0d)
            ]);
    }
}
