using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public sealed class TrophyTransition() : PageTransitionBase(TimeSpan.FromMilliseconds(300))
{
    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        // Exit: Shrink to bottom
        from
           .Animation(new BackEaseIn())
           .AddFrame(0d, [(ScaleTransform.ScaleXProperty, 1d), (ScaleTransform.ScaleYProperty, 1d)])
           .AddFrame(1d, [(ScaleTransform.ScaleXProperty, 0.5d), (ScaleTransform.ScaleYProperty, 0.5d)]);
        from
           .Animation(new CubicEaseIn())
           .AddFrame(0d, [(Visual.OpacityProperty, 1d), (TranslateTransform.YProperty, 0d)])
           .AddFrame(1d, [(Visual.OpacityProperty, 0d), (TranslateTransform.YProperty, 177d)]);

        // Enter: Spin & Zoom
        to
           .Animation(new CubicEaseOut())
           .AddFrame(0d,
            [
                (Visual.OpacityProperty, 0d), (ScaleTransform.ScaleXProperty, 0d), (ScaleTransform.ScaleYProperty, 0d)
            ])
           .AddFrame(0.5d, [(ScaleTransform.ScaleXProperty, -0.6d), (ScaleTransform.ScaleYProperty, 0.6d)])
           .AddFrame(1d,
            [
                (Visual.OpacityProperty, 1d), (ScaleTransform.ScaleXProperty, 1d), (ScaleTransform.ScaleYProperty, 1d)
            ]);
    }
}
