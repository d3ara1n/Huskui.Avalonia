using Avalonia;
using Avalonia.Controls;

namespace Huskui.Avalonia.Controls;

public class DrawerPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            child.Measure(availableSize);
        }

        return availableSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            if (child is Drawer drawer)
            {
                var hostWidth = finalSize.Width;
                var hostHeight = finalSize.Height;
                // 先取 Drawer 当前想要的尺寸
                var width = drawer.Width;
                var height = drawer.Height;
                var x = drawer.OffsetX;
                // 如果 Width/Height 是 NaN，就退回到 DesiredSize
                if (double.IsNaN(width))
                    width = drawer.DesiredSize.Width;
                if (double.IsNaN(height))
                    height = drawer.DesiredSize.Height;
                // 基础最小值
                var minWidth = drawer.MinWidth;
                var minHeight = drawer.IsOpen
                    ? Math.Max(drawer.HeaderHeight, drawer.MinHeight)
                    : drawer.HeaderHeight;
                // 基础最大值：不能超过宿主大小
                var maxWidth = hostWidth;
                var maxHeight = drawer.IsOpen ? hostHeight : drawer.HeaderHeight;
                // 如果宿主比 Min 还小，允许降到宿主大小，避免死锁
                minWidth = Math.Min(minWidth, maxWidth);
                minHeight = Math.Min(minHeight, maxHeight);
                // 先钳制尺寸
                width = Clamp(width, minWidth, maxWidth);
                height = Clamp(height, minHeight, maxHeight);
                // 再钳制 X，确保整个 Drawer 在宿主范围内
                x = Clamp(x, 0, Math.Max(0, hostWidth - width));
                // Y 永远贴底
                var y = hostHeight - height;
                if (y < 0)
                    y = 0;
                // 把钳制后的结果写回去，保持控件状态一致
                if (Math.Abs(drawer.OffsetX - x) > 0.1)
                    drawer.SetCurrentValue(Drawer.OffsetXProperty, x);
                if (!double.IsNaN(drawer.Width) && Math.Abs(drawer.Width - width) > 0.1)
                    drawer.Width = width;
                if (!double.IsNaN(drawer.Height) && Math.Abs(drawer.Height - height) > 0.1)
                    drawer.Height = height;
                drawer.Arrange(new Rect(x, y, width, height));
            }
            else
            {
                // Fallback for non-Drawer children
                child.Arrange(new(0, 0, finalSize.Width, finalSize.Height));
            }
        }

        return finalSize;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    }
}
