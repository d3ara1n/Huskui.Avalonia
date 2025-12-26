using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Huskui.Avalonia.Controls;

public class DrawerHost : Panel
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
                var desiredSize = child.DesiredSize;
                
                // Clamp OffsetX to ensure the drawer stays within the host bounds horizontally
                double x = drawer.OffsetX;
                
                // Ensure the drawer doesn't go off the left edge
                if (x < 0) x = 0;
                
                // Ensure the drawer doesn't go off the right edge
                if (x + desiredSize.Width > finalSize.Width) 
                    x = finalSize.Width - desiredSize.Width;
                
                // Double check left edge in case the drawer is wider than the host
                if (x < 0) x = 0; 
                
                // Update drawer's OffsetX if we clamped it, so it stays in sync
                if (Math.Abs(x - drawer.OffsetX) > 1)
                {
                    drawer.SetCurrentValue(Drawer.OffsetXProperty, x);
                }

                // Anchor to bottom
                double y = finalSize.Height - desiredSize.Height;
                
                child.Arrange(new Rect(x, y, desiredSize.Width, desiredSize.Height));
            }
            else
            {
                // Fallback for non-Drawer children
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            }
        }
        return finalSize;
    }
}