using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class SlideUpTransition : PageTransitionBase
{
    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        const double height = 72d;
        from
           .Animation(new CubicEaseIn())
           .AddFrame(0d, [(TranslateTransform.YProperty, 0d), (Visual.OpacityProperty, 1d)])
           .AddFrame(1d, [(TranslateTransform.YProperty, height), (Visual.OpacityProperty, 0d)]);

        to
           .Animation(new CubicEaseOut())
           .AddFrame(0d, [(TranslateTransform.YProperty, height), (Visual.OpacityProperty, 0d)])
           .AddFrame(1d, [(TranslateTransform.YProperty, 0d), (Visual.OpacityProperty, 1d)]);
    }
}
