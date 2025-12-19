using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Media;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Converters;

internal static class InternalConverters
{
    public static IValueConverter ButtonColumnSpan { get; } = new RelayConverter((v, p) =>
    {
        if (p is int)
        {
            return v is false ? p : 1;
        }

        if (p is string str && int.TryParse(str, out var i))
        {
            return v is false ? i : 1;
        }

        return 1;
    });

    public static IValueConverter ButtonColumn { get; } = new RelayConverter((v, p) =>
    {
        if (p is int)
        {
            return v is true ? p : 0;
        }

        if (p is string str && int.TryParse(str, out var i))
        {
            return v is true ? i : 0;
        }

        return 0;
    });

    public static IMultiValueConverter StringFormat { get; } = new RelayMultiConverter((v, _, info) =>
    {
        if (v is [string format, ..])
        {
            return string.Format(info, format, [.. v.Skip(1)]);
        }

        return v;
    });

    public static IValueConverter KeyGestureToString { get; } = new RelayConverter((v, _) =>
    {
        return v switch
        {
            null => null,
            KeyGesture gesture => gesture.ToString("p", null),
            _ => throw new NotSupportedException()
        };
    });

    public static IValueConverter TrueIfMatch { get; } = new RelayConverter((v, p) => v == p);

    public static IValueConverter TrueIfNotMatch { get; } = new RelayConverter((v, p) => v != p);

    public static IValueConverter ToDisplayIndex { get; } = new RelayConverter((v, _) =>
    {
        if (v is int index)
        {
            return index + 1;
        }

        return v;
    });

    public static IValueConverter CountToArray { get; } = new RelayConverter((v, _) =>
    {
        if (v is int count)
        {
            return Enumerable.Range(0, count).ToArray();
        }

        return v;
    });

    public static IMultiValueConverter OffsetToOpacity { get; } = new RelayMultiConverter((v, _, info) =>
    {
        if (v is [Vector offset, double max])
        {
            return 1.0 - Math.Min(offset.Y, max) / max;
        }

        return v;
    });

    /// <summary>
    ///     Converts a RatingStarFillState to opacity (0 for Empty, 1 for Full/Half).
    /// </summary>
    public static IValueConverter RatingStarFillToOpacity { get; } = new RelayConverter((v, _) =>
    {
        if (v is RatingStarFillState state)
        {
            return state == RatingStarFillState.Empty ? 0.0 : 1.0;
        }

        return 0.0;
    });

    /// <summary>
    ///     Converts a RatingStarFillState and FontSize to a clip geometry for half-star display.
    /// </summary>
    public static IMultiValueConverter RatingStarClipGeometry { get; } = new RelayMultiConverter((v, _, _) =>
    {
        // NOTE: 这里使用 FontSize 当做宽度作对半裁切，如果未来图标的字体大小不再与宽度一致，需要改为 Bounds.Width
        if (v is [RatingStarFillState state, double fontSize])
        {
            return state == RatingStarFillState.Half
                       ?
                       // Clip to left half
                       new RectangleGeometry(new(0, 0, fontSize / 2, fontSize))
                       :
                       // Full star - no clip needed (return a large rect that covers everything)
                       new(new(0, 0, fontSize * 2, fontSize * 2));
        }

        return null;
    });
}
