using Avalonia;
using Avalonia.Controls;

namespace Huskui.Avalonia.Controls;

public class ConstrainedBox : ContentControl
{
    public static readonly StyledProperty<string> AspectRatioProperty =
        AvaloniaProperty.Register<ConstrainedBox, string>(nameof(AspectRatio), "1:1");

    public string AspectRatio
    {
        get => GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var ratio = ParseAspectRatio(AspectRatio);
        var width = availableSize.Width;
        var height = width / ratio;

        if (height > availableSize.Height)
        {
            height = availableSize.Height;
            width = height * ratio;
        }

        var desired = new Size(width, height);
        // if (Content is Control control)
        // {
        //     control.Measure(new Size(width, height));
        //     desired = control.DesiredSize;
        // }
        // 答案是不关心内部成员大小，不 Measure 他们！

        return desired;
    }

    private static double ParseAspectRatio(string ratio)
    {
        if (double.TryParse(ratio, out var result))
        {
            return result;
        }
        else if (ratio.Count(x => x == ':') == 1)
        {
            var split = ratio.Split(':');
            var width = split[0];
            var height = split[1];
            if (double.TryParse(width, out var w) && double.TryParse(height, out var h))
            {
                return w / h;
            }
        }

        throw new FormatException($"Invalid aspect ratio (1.0 or 1:1): {ratio}");
    }
}
