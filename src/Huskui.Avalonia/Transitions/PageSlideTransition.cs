using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;

namespace Huskui.Avalonia.Transitions;

public class PageSlideTransition(TimeSpan? duration = null) : PageTransitionBase(duration)
{
    // 使用更长的持续时间以获得更优雅的动画效果，类似现代 Web 应用
    public PageSlideTransition() : this(TimeSpan.FromMilliseconds(297)) { }
    public DirectionFrom Direction { get; set; } = DirectionFrom.Right;

    protected override void Cleanup(Visual? from, Visual? to)
    {
        base.Cleanup(from, to);

        from?.RenderTransform = null;
        to?.RenderTransform = null;
    }

    protected override void Configure(Builder from, Builder to, Lazy<Visual> parentAccessor)
    {
        var parent = parentAccessor.Value;
        var translateProperty = Direction switch
        {
            DirectionFrom.Right or DirectionFrom.Left => TranslateTransform.XProperty,
            DirectionFrom.Top or DirectionFrom.Bottom => TranslateTransform.YProperty,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction))
        };

        // 新页面进入的距离
        var translateDistance = Direction switch
        {
            DirectionFrom.Left => -parent.Bounds.Width,
            DirectionFrom.Right => parent.Bounds.Width,
            DirectionFrom.Top => -parent.Bounds.Height,
            DirectionFrom.Bottom => parent.Bounds.Height,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction))
        };

        // 旧页面退出的距离（相反方向，且距离较短以营造层次感）
        var exitDistance = -translateDistance * 0.25;

        // === 旧页面退出动画 ===
        // 使用 Cubic Ease In 让退出动画开始缓慢，然后加速
        // 结合轻微的缩放和透明度变化，营造"后退"的视觉效果

        // 位移 + 缩放动画
        from
           .Animation(new CubicEaseIn())
           .AddFrame(0d, [
               (translateProperty, 0d),
               (ScaleTransform.ScaleXProperty, 1.0d),
               (ScaleTransform.ScaleYProperty, 1.0d)
           ])
           .AddFrame(0.45d, [
               (translateProperty, exitDistance),
               (ScaleTransform.ScaleXProperty, 0.95d),
               (ScaleTransform.ScaleYProperty, 0.95d)
           ]);

        // 透明度动画（分离以获得更精细的控制）
        from
           .Animation(new SineEaseIn())
           .AddFrame(0d, [(Visual.OpacityProperty, 1d)])
           .AddFrame(0.35d, [(Visual.OpacityProperty, 0d)]);

        // === 新页面进入动画 ===
        // 使用自定义的缓动曲线，类似 Material Design 的 Standard Easing
        // 或 iOS 的流畅曲线：快速启动，平滑减速

        // 位移动画 - 使用延迟让新页面在旧页面开始退出后再进入
        to
           .Animation(new SplineEasing(0.4, 0.0, 0.2, 1.0))
           .WithDelay(TimeSpan.FromMilliseconds(100))
           .AddFrame(0d, [(translateProperty, translateDistance)])
           .AddFrame(1d, [(translateProperty, 0d)]);

        // 透明度动画 - 快速淡入
        to
           .Animation(new SineEaseOut())
           .WithDelay(TimeSpan.FromMilliseconds(100))
           .AddFrame(0d, [(Visual.OpacityProperty, 0d)])
           .AddFrame(0.3d, [(Visual.OpacityProperty, 1d)]);

        // 轻微的缩放效果 - 从稍微放大的状态进入，营造"向前"的感觉
        to
           .Animation(new CubicEaseOut())
           .WithDelay(TimeSpan.FromMilliseconds(100))
           .AddFrame(0d, [
               (ScaleTransform.ScaleXProperty, 1.02d),
               (ScaleTransform.ScaleYProperty, 1.02d)
           ])
           .AddFrame(1d, [
               (ScaleTransform.ScaleXProperty, 1.0d),
               (ScaleTransform.ScaleYProperty, 1.0d)
           ]);
    }
}
