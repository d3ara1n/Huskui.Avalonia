using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Styling;
using Huskui.Avalonia.Code.Models;
using Huskui.Avalonia.Converters;

namespace Huskui.Avalonia.Code.Converters;

internal static class InternalConverters
{
    private static IBrush? _diffAddedBrush;

    private static IBrush? _diffRemovedBrush;

    private static IBrush? _diffEmptyBrush;

    private static IBrush? _diffModifiedBrush;
    private static IBrush? _diffAddedIndicatorBrush;
    private static IBrush? _diffRemovedIndicatorBrush;
    private static IBrush? _diffModifiedIndicatorBrush;
    private static IBrush? _diffAddedForegroundBrush;
    private static IBrush? _diffRemovedForegroundBrush;
    private static IBrush? _diffModifiedForegroundBrush;
    private static IBrush? _diffSecondaryForegroundBrush;
    private static ThemeVariant? _diffTheme;

    private static void EnsureDiffBrushes()
    {
        if (_diffTheme == Application.Current?.ActualThemeVariant)
        {
            return;
        }

        _diffTheme = Application.Current?.ActualThemeVariant;
        _diffAddedBrush =
            Application.Current?.TryGetResource("ControlSuccessTranslucentFullBackgroundBrush", null, out var res1)
         == true
                ? res1 as IBrush
                : new SolidColorBrush(Color.FromArgb(80, 0x40, 0xA0, 0x40));
        _diffRemovedBrush =
            Application.Current?.TryGetResource("ControlDangerTranslucentFullBackgroundBrush", null, out var res2)
         == true
                ? res2 as IBrush
                : new SolidColorBrush(Color.FromArgb(80, 0xA0, 0x40, 0x40));
        _diffEmptyBrush =
            Application.Current?.TryGetResource("ControlTranslucentFullBackgroundBrush", null, out var res3) == true
                ? res3 as IBrush
                : new SolidColorBrush(Color.FromArgb(40, 0x80, 0x80, 0x80));
        _diffModifiedBrush =
            Application.Current?.TryGetResource("ControlAccentTranslucentFullBackgroundBrush", null, out var res4)
         == true
                ? res4 as IBrush
                : new SolidColorBrush(Color.FromArgb(80, 0x40, 0x80, 0xC0));
        _diffAddedIndicatorBrush =
            Application.Current?.TryGetResource("ControlSuccessBackgroundBrush", null, out var res5) == true
                ? res5 as IBrush
                : new SolidColorBrush(Color.FromRgb(0x2B, 0x9A, 0x66));
        _diffRemovedIndicatorBrush =
            Application.Current?.TryGetResource("ControlDangerBackgroundBrush", null, out var res6) == true
                ? res6 as IBrush
                : new SolidColorBrush(Color.FromRgb(0xDC, 0x3E, 0x42));
        _diffModifiedIndicatorBrush =
            Application.Current?.TryGetResource("ControlAccentBackgroundBrush", null, out var res7) == true
                ? res7 as IBrush
                : new SolidColorBrush(Color.FromRgb(0x00, 0x90, 0xFF));
        _diffAddedForegroundBrush =
            Application.Current?.TryGetResource("ControlSuccessForegroundBrush", null, out var res8) == true
                ? res8 as IBrush
                : new SolidColorBrush(Color.FromRgb(0x2B, 0x9A, 0x66));
        _diffRemovedForegroundBrush =
            Application.Current?.TryGetResource("ControlDangerForegroundBrush", null, out var res9) == true
                ? res9 as IBrush
                : new SolidColorBrush(Color.FromRgb(0xDC, 0x3E, 0x42));
        _diffModifiedForegroundBrush =
            Application.Current?.TryGetResource("ControlAccentForegroundBrush", null, out var res10) == true
                ? res10 as IBrush
                : new SolidColorBrush(Color.FromRgb(0x00, 0x90, 0xFF));
        _diffSecondaryForegroundBrush =
            Application.Current?.TryGetResource("ControlSecondaryForegroundBrush", null, out var res11) == true
                ? res11 as IBrush
                : Brushes.Gray;
    }

    public static IValueConverter DiffLineKindToBackground { get; } = new RelayConverter((v, _) =>
    {
        EnsureDiffBrushes();
        return v is DiffLineKind kind
                   ? kind switch
                   {
                       DiffLineKind.Added => _diffAddedBrush,
                       DiffLineKind.Removed => _diffRemovedBrush,
                       DiffLineKind.Empty => _diffEmptyBrush,
                       DiffLineKind.Modified => _diffModifiedBrush,
                       _ => Brushes.Transparent,
                   }
                   : AvaloniaProperty.UnsetValue;
    });

    public static IValueConverter DiffLineKindToIndicatorBrush { get; } = new RelayConverter((v, _) =>
    {
        EnsureDiffBrushes();
        return v is DiffLineKind kind
                   ? kind switch
                   {
                       DiffLineKind.Added => _diffAddedIndicatorBrush,
                       DiffLineKind.Removed => _diffRemovedIndicatorBrush,
                       DiffLineKind.Modified => _diffModifiedIndicatorBrush,
                       _ => Brushes.Transparent,
                   }
                   : AvaloniaProperty.UnsetValue;
    });

    public static IValueConverter DiffLineKindToForeground { get; } = new RelayConverter((v, _) =>
    {
        EnsureDiffBrushes();
        return v is DiffLineKind kind
                   ? kind switch
                   {
                       DiffLineKind.Added => _diffAddedForegroundBrush,
                       DiffLineKind.Removed => _diffRemovedForegroundBrush,
                       DiffLineKind.Modified => _diffModifiedForegroundBrush,
                       _ => _diffSecondaryForegroundBrush,
                   }
                   : AvaloniaProperty.UnsetValue;
    });
}
