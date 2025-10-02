using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class ModalTransition : PageTransitionBase
{
    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        from
           .Animation(new SineEaseIn())
           .AddFrame(0d,
            [
                (Visual.OpacityProperty, 1d), (ScaleTransform.ScaleXProperty, 1d), (ScaleTransform.ScaleYProperty, 1d)
            ])
           .AddFrame(1d,
            [
                (Visual.OpacityProperty, 0d),
                (ScaleTransform.ScaleXProperty, 0.94d),
                (ScaleTransform.ScaleYProperty, 0.94d)
            ]);

        to
           .Animation(new BackEaseOut())
           .AddFrame(0d, [(ScaleTransform.ScaleXProperty, 0.94d), (ScaleTransform.ScaleYProperty, 0.94d)])
           .AddFrame(1d, [(ScaleTransform.ScaleXProperty, 1d), (ScaleTransform.ScaleYProperty, 1d)]);

        to
           .Animation(new SineEaseOut())
           .AddFrame(0d, [(Visual.OpacityProperty, 0d)])
           .AddFrame(1d, [(Visual.OpacityProperty, 1d)]);
    }
}
