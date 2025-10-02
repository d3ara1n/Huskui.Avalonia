using Avalonia;
using Avalonia.Data.Converters;

namespace Huskui.Avalonia.Converters;

public static class CornerRadiusConverters
{
    public static IValueConverter Upper { get; } = new RelayConverter((v, _) => v switch
    {
        CornerRadius it => new CornerRadius(it.TopLeft, it.TopRight, 0d, 0d),
        _ => v
    });

    public static IValueConverter Lower { get; } = new RelayConverter((v, _) => v switch
    {
        CornerRadius it => new CornerRadius(0d, 0d, it.BottomRight, it.BottomLeft),
        _ => v
    });

    public static IValueConverter Left { get; } = new RelayConverter((v, _) => v switch
    {
        CornerRadius it => new CornerRadius(it.TopLeft, 0d, 0d, it.BottomLeft),
        _ => v
    });

    public static IValueConverter Right { get; } = new RelayConverter((v, _) => v switch
    {
        CornerRadius it => new CornerRadius(0d, it.TopRight, it.BottomRight, 0d),
        _ => v
    });

    public static IValueConverter ToDouble { get; } = new RelayConverter((v, p) =>
    {
        if (v is CornerRadius radius)
        {
            return p?.ToString()?.ToLower() switch
            {
                "topright" => radius.TopRight,
                "topleft" => radius.TopLeft,
                "bottomright" => radius.BottomRight,
                "bottomleft" => radius.BottomLeft,
                _ when radius.IsUniform => radius.BottomLeft,
                _ => v
            };
        }

        return v;
    });

    public static IValueConverter ToInnerRadius { get; } = new RelayConverter((v, p) =>
    {
        if (v is CornerRadius radius)
        {
            double? padding = p switch
            {
                double it => it,
                int it => (double)it,
                string it when double.TryParse(it, out var result) => result,
                string it when int.TryParse(it, out var result) => (double)result,
                Thickness { IsUniform: true } it => it.Bottom,
                _ => null
            };

            if (padding.HasValue)
            {
                return new CornerRadius(radius.TopLeft - padding.Value,
                                        radius.TopRight - padding.Value,
                                        radius.BottomRight - padding.Value,
                                        radius.BottomLeft - padding.Value);
            }
        }

        return v;
    });

    public static IMultiValueConverter ToInnerRadiusMulti { get; } = new RelayMultiConverter((v, _, info) =>
    {
        return v switch
        {
            [CornerRadius c, double p] => new CornerRadius(c.TopLeft - p,
                                                           c.TopRight - p,
                                                           c.BottomRight - p,
                                                           c.BottomLeft - p),
            [CornerRadius c, int p] => new CornerRadius(c.TopLeft - p,
                                                        c.TopRight - p,
                                                        c.BottomRight - p,
                                                        c.BottomLeft - p),
            [CornerRadius c, string p] when double.TryParse(p, out var result) => new CornerRadius(c.TopLeft - result,
                c.TopRight - result,
                c.BottomRight - result,
                c.BottomLeft - result),
            [CornerRadius c, string p] when int.TryParse(p, out var result) => new CornerRadius(c.TopLeft - result,
                c.TopRight - result,
                c.BottomRight - result,
                c.BottomLeft - result),
            [CornerRadius c, Thickness { IsUniform: true } t] => new CornerRadius(c.TopLeft - t.Bottom,
                c.TopRight - t.Bottom,
                c.BottomRight - t.Bottom,
                c.BottomLeft - t.Bottom),

            _ => v
        };
    });
}
