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

    public static IValueConverter FromDouble { get; } = new RelayConverter((v, _) =>
    {
        return v switch
        {
            double d => new CornerRadius(d),
            int i => new CornerRadius(i),
            _ => v
        };
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
                Thickness { IsUniform: true } it => it.Bottom,
                _ => null
            };

            if (padding.HasValue)
            {
                return new CornerRadius(radius.TopLeft - padding.Value / (radius.TopLeft / padding < 1.5 ? 3d : 1d),
                                        radius.TopRight - padding.Value / (radius.TopRight / padding < 1.5 ? 3d : 1d),
                                        radius.BottomRight
                                      - padding.Value / (radius.BottomRight / padding < 1.5 ? 3d : 1d),
                                        radius.BottomLeft
                                      - padding.Value / (radius.BottomLeft / padding < 1.5 ? 3d : 1d));
            }
        }

        return new CornerRadius(0);
    });

    public static IMultiValueConverter ToInnerRadiusMulti { get; } = new RelayMultiConverter((v, _, _) =>
    {
        var (radius, padding) = v switch
        {
            [CornerRadius c, double p] => (c, p),
            [CornerRadius c, int p] => (c, p),
            [CornerRadius c, string p] when double.TryParse(p, out var result) => (c, result),
            [CornerRadius c, Thickness { IsUniform: true } t] => (c, t.Bottom),
            _ => (new(0), 0)
        };

        if (radius == default && padding == 0)
        {
            return new CornerRadius(0);
        }

        return new CornerRadius(radius.TopLeft - padding / (radius.TopLeft / padding < 1.5 ? 3d : 1d),
                                radius.TopRight - padding / (radius.TopRight / padding < 1.5 ? 3d : 1d),
                                radius.BottomRight - padding / (radius.BottomRight / padding < 1.5 ? 3d : 1d),
                                radius.BottomLeft - padding / (radius.BottomLeft / padding < 1.5 ? 3d : 1d));
    });
}
