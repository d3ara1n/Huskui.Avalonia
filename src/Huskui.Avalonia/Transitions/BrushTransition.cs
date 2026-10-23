using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Media.Immutable;

namespace Huskui.Avalonia.Transitions;

// Brush property transition that interpolates premultiplied sRGB, the same space Skia composites in, so
// every frame composites to a color between the two endpoints over any backdrop. Avalonia's own
// BrushTransition lerps straight alpha in linear light and lerps Opacity separately from Color.A, which
// exposes the raw RGB of translucent colors mid-way and shows up as a flash toward black or white.
public class BrushTransition : InterpolatingTransitionBase<IBrush?>
{
    protected override IBrush? Interpolate(double progress, IBrush? from, IBrush? to)
    {
        if (progress <= 0d)
            return from;
        if (progress >= 1d)
            return to;

        if (!TryPremultiply(from, out var a) || !TryPremultiply(to, out var b))
            return progress >= 0.5d ? to : from;

        var alpha = Math.Clamp(Lerp(a.A, b.A, progress), 0d, 1d);
        var alphaByte = (byte)Math.Round(alpha * 255d);
        if (alphaByte == 0)
            return new ImmutableSolidColorBrush(Color.FromArgb(0, 0, 0, 0));

        // NOTE: Divide by the quantized alpha so the stored straight color recomposes to the
        //  interpolated premultiplied value; dividing by the exact alpha drifts at low alpha.
        var quantized = alphaByte / 255d;
        return new ImmutableSolidColorBrush(
            Color.FromArgb(
                alphaByte,
                Unpremultiply(Lerp(a.R, b.R, progress), quantized),
                Unpremultiply(Lerp(a.G, b.G, progress), quantized),
                Unpremultiply(Lerp(a.B, b.B, progress), quantized)
            )
        );
    }

    private static bool TryPremultiply(IBrush? brush, out (double A, double R, double G, double B) value)
    {
        switch (brush)
        {
            case null:
                value = (0d, 0d, 0d, 0d);
                return true;
            case ISolidColorBrush solid:
                var color = solid.Color;
                var alpha = color.A / 255d * Math.Clamp(solid.Opacity, 0d, 1d);
                value = (alpha, color.R / 255d * alpha, color.G / 255d * alpha, color.B / 255d * alpha);
                return true;
            default:
                value = default;
                return false;
        }
    }

    private static double Lerp(double from, double to, double progress) => from + (to - from) * progress;

    private static byte Unpremultiply(double channel, double alpha) =>
        (byte)Math.Round(Math.Clamp(channel / alpha, 0d, 1d) * 255d);
}
