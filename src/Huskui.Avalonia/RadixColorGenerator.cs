using Avalonia.Media;

namespace Huskui.Avalonia;

/// <summary>
///     Generates Radix-style 12-step color scales from a base color.
///     Based on perceptual lightness distribution similar to Radix Colors.
/// </summary>
public static class RadixColorGenerator
{
    /// <summary>
    ///     Generates a 12-step color scale for light theme from a base accent color.
    /// </summary>
    public static Color[] GenerateLightScale(Color baseColor)
    {
        var hsl = RgbToHsl(baseColor);

        // Lightness steps for light theme (from lightest to darkest)
        // These values are calibrated to match Radix Colors distribution
        double[] lightnessSteps =
        {
            0.99, // Step 1: Almost white
            0.97, // Step 2: Very light
            0.94, // Step 3: Light
            0.91, // Step 4: Light-medium
            0.87, // Step 5: Medium-light
            0.82, // Step 6: Medium
            0.76, // Step 7: Medium-dark
            0.68, // Step 8: Dark-medium
            hsl.L, // Step 9: Base color (interactive)
            Math.Max(0.05, hsl.L - 0.05), // Step 10: Slightly darker
            Math.Max(0.05, hsl.L - 0.15), // Step 11: High contrast
            Math.Max(0.05, hsl.L - 0.35) // Step 12: Very dark
        };

        // Saturation adjustments (lighter colors need less saturation)
        double[] saturationMultipliers =
        {
            0.20, // Step 1
            0.30, // Step 2
            0.45, // Step 3
            0.60, // Step 4
            0.70, // Step 5
            0.80, // Step 6
            0.90, // Step 7
            0.95, // Step 8
            1.00, // Step 9: Base saturation
            1.00, // Step 10
            1.00, // Step 11
            0.85 // Step 12: Slightly desaturated for readability
        };

        var scale = new Color[12];
        for (var i = 0; i < 12; i++)
        {
            var newHsl = new HslColor(hsl.H,
                                      Math.Clamp(hsl.S * saturationMultipliers[i], 0, 1),
                                      Math.Clamp(lightnessSteps[i], 0, 1));
            scale[i] = HslToRgb(newHsl);
        }

        return scale;
    }

    /// <summary>
    ///     Generates a 12-step color scale for dark theme from a base accent color.
    /// </summary>
    public static Color[] GenerateDarkScale(Color baseColor)
    {
        var hsl = RgbToHsl(baseColor);

        // Lightness steps for dark theme (from darkest to lightest)
        double[] lightnessSteps =
        {
            0.08, // Step 1: Almost black
            0.10, // Step 2: Very dark
            0.13, // Step 3: Dark
            0.16, // Step 4: Dark-medium
            0.20, // Step 5: Medium-dark
            0.25, // Step 6: Medium
            0.32, // Step 7: Medium-light
            0.42, // Step 8: Light-medium
            hsl.L, // Step 9: Base color (interactive)
            Math.Min(0.95, hsl.L + 0.08), // Step 10: Slightly lighter
            Math.Min(0.95, hsl.L + 0.20), // Step 11: High contrast
            Math.Min(0.95, hsl.L + 0.40) // Step 12: Very light
        };

        // Saturation adjustments for dark theme
        double[] saturationMultipliers =
        {
            0.30, // Step 1
            0.40, // Step 2
            0.55, // Step 3
            0.65, // Step 4
            0.75, // Step 5
            0.85, // Step 6
            0.92, // Step 7
            0.98, // Step 8
            1.00, // Step 9: Base saturation
            1.00, // Step 10
            0.95, // Step 11: Slightly desaturated
            0.85 // Step 12: More desaturated for readability
        };

        var scale = new Color[12];
        for (var i = 0; i < 12; i++)
        {
            var newHsl = new HslColor(hsl.H,
                                      Math.Clamp(hsl.S * saturationMultipliers[i], 0, 1),
                                      Math.Clamp(lightnessSteps[i], 0, 1));
            scale[i] = HslToRgb(newHsl);
        }

        return scale;
    }

    #region HSL Conversion Helpers

    private record struct HslColor(double H, double S, double L);

    private static HslColor RgbToHsl(Color rgb)
    {
        var r = rgb.R / 255.0;
        var g = rgb.G / 255.0;
        var b = rgb.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        double h = 0;
        double s = 0;
        var l = (max + min) / 2.0;

        if (delta != 0)
        {
            s = l > 0.5 ? delta / (2.0 - max - min) : delta / (max + min);

            if (Math.Abs(max - r) < double.Epsilon)
            {
                h = ((g - b) / delta + (g < b ? 6 : 0)) / 6.0;
            }
            else if (Math.Abs(max - g) < double.Epsilon)
            {
                h = ((b - r) / delta + 2) / 6.0;
            }
            else
            {
                h = ((r - g) / delta + 4) / 6.0;
            }
        }

        return new(h, s, l);
    }

    private static Color HslToRgb(HslColor hsl)
    {
        double r, g, b;

        if (hsl.S == 0)
        {
            r = g = b = hsl.L;
        }
        else
        {
            var q = hsl.L < 0.5 ? hsl.L * (1 + hsl.S) : hsl.L + hsl.S - hsl.L * hsl.S;
            var p = 2 * hsl.L - q;

            r = HueToRgb(p, q, hsl.H + 1.0 / 3.0);
            g = HueToRgb(p, q, hsl.H);
            b = HueToRgb(p, q, hsl.H - 1.0 / 3.0);
        }

        return Color.FromRgb((byte)Math.Round(r * 255), (byte)Math.Round(g * 255), (byte)Math.Round(b * 255));
    }

    private static double HueToRgb(double p, double q, double t)
    {
        if (t < 0)
        {
            t += 1;
        }

        if (t > 1)
        {
            t -= 1;
        }

        if (t < 1.0 / 6.0)
        {
            return p + (q - p) * 6 * t;
        }

        if (t < 1.0 / 2.0)
        {
            return q;
        }

        if (t < 2.0 / 3.0)
        {
            return p + (q - p) * (2.0 / 3.0 - t) * 6;
        }

        return p;
    }

    #endregion
}
