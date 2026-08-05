using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class SidebarTransition(TimeSpan? duration = null) : PageTransitionBase(duration)
{
    public SidebarTransition()
        : this(TimeSpan.FromMilliseconds(320)) { }

    public DirectionFrom Direction { get; set; } = DirectionFrom.Right;

    protected override void Cleanup(Visual? from, Visual? to)
    {
        base.Cleanup(from, to);

        if (from != null)
        {
            from.Opacity = 1d;
            from.RenderTransform = null;
        }

        if (to != null)
        {
            to.Opacity = 1d;
            to.RenderTransform = null;
        }
    }

    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        var parent = parentAccessor.Value;
        var translateProperty = Direction switch
        {
            DirectionFrom.Right or DirectionFrom.Left => TranslateTransform.XProperty,
            DirectionFrom.Top or DirectionFrom.Bottom => TranslateTransform.YProperty,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction)),
        };

        var offset = Direction switch
        {
            DirectionFrom.Left => -parent.Bounds.Width,
            DirectionFrom.Right => parent.Bounds.Width,
            DirectionFrom.Top => -parent.Bounds.Height,
            DirectionFrom.Bottom => parent.Bounds.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction)),
        };

        var overshoot = offset * -0.035d;
        var settle = offset * 0.012d;

        from.Animation(new SplineEasing(0.25, 0.1, 0.25))
            .AddFrame(0d, [(translateProperty, 0d), (Visual.OpacityProperty, 1d)])
            .AddFrame(0.72d, [(translateProperty, offset * 0.82d), (Visual.OpacityProperty, 1d)])
            .AddFrame(1d, [(translateProperty, offset), (Visual.OpacityProperty, 0d)]);

        to.Animation(new BackEaseOut())
            .AddFrame(0d, [(translateProperty, offset), (Visual.OpacityProperty, 0d)])
            .AddFrame(0.82d, [(translateProperty, overshoot), (Visual.OpacityProperty, 1d)])
            .AddFrame(0.92d, [(translateProperty, settle), (Visual.OpacityProperty, 1d)])
            .AddFrame(1d, [(translateProperty, 0d), (Visual.OpacityProperty, 1d)]);
    }
}
