using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public sealed class DrawerTransition(TimeSpan? duration = null) : PageTransitionBase(duration)
{
    public DrawerTransition()
        : this(TimeSpan.FromMilliseconds(260)) { }

    protected override void Cleanup(Visual? from, Visual? to)
    {
        base.Cleanup(from, to);

        if (from != null)
        {
            from.RenderTransform = null;
        }

        if (to != null)
        {
            to.RenderTransform = null;
        }
    }

    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        var parent = parentAccessor.Value;
        var offset = Math.Max(120d, parent.Bounds.Height * 0.5d);

        // 微软风格的平滑 EaseOut（接近 SineOut，但尾段更柔和）
        var easing = new SplineEasing(0.06, 0.88, 0.18);

        from.Animation(easing)
            .AddFrame(0d, [(TranslateTransform.YProperty, 0d), (Visual.OpacityProperty, 1d)])
            .AddFrame(1d, [(TranslateTransform.YProperty, offset), (Visual.OpacityProperty, 0d)]);

        to.Animation(easing)
            .AddFrame(0d, [(TranslateTransform.YProperty, offset), (Visual.OpacityProperty, 0d)])
            .AddFrame(1d, [(TranslateTransform.YProperty, 0d), (Visual.OpacityProperty, 1d)]);
    }
}
