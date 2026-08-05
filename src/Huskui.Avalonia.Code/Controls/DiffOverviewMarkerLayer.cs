using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Huskui.Avalonia.Code.Models;

namespace Huskui.Avalonia.Code.Controls;

public class DiffOverviewMarkerLayer : Control
{
    private const double MARKER_WIDTH = 5.0;
    private const double MIN_MARKER_HEIGHT = 1.0;

    public static readonly StyledProperty<IReadOnlyList<DiffMarker>?> MarkersProperty =
        AvaloniaProperty.Register<DiffOverviewMarkerLayer, IReadOnlyList<DiffMarker>?>(nameof(Markers));

    static DiffOverviewMarkerLayer()
    {
        AffectsRender<DiffOverviewMarkerLayer>(MarkersProperty);
    }

    public IReadOnlyList<DiffMarker>? Markers
    {
        get => GetValue(MarkersProperty);
        set => SetValue(MarkersProperty, value);
    }

    private IBrush? _addedBrush;
    private IBrush? _removedBrush;
    private IBrush? _modifiedBrush;
    private ThemeVariant? _theme;

    private void EnsureBrushes()
    {
        if (_theme == Application.Current?.ActualThemeVariant)
            return;

        _theme = Application.Current?.ActualThemeVariant;
        _addedBrush = TryBrush(
            "ControlSuccessBackgroundBrush",
            new SolidColorBrush(Color.FromRgb(0x2B, 0x9A, 0x66))
        );
        _removedBrush = TryBrush(
            "ControlDangerBackgroundBrush",
            new SolidColorBrush(Color.FromRgb(0xDC, 0x3E, 0x42))
        );
        _modifiedBrush = TryBrush(
            "ControlAccentBackgroundBrush",
            new SolidColorBrush(Color.FromRgb(0x00, 0x90, 0xFF))
        );
    }

    private static IBrush TryBrush(string key, IBrush fallback) =>
        Application.Current?.TryGetResource(key, null, out var res) == true && res is IBrush b
            ? b
            : fallback;

    private IBrush BrushFor(DiffLineKind kind) =>
        kind switch
        {
            DiffLineKind.Added => _addedBrush!,
            DiffLineKind.Removed => _removedBrush!,
            _ => _modifiedBrush!,
        };

    public override void Render(DrawingContext context)
    {
        EnsureBrushes();
        var width = Bounds.Width;
        var height = Bounds.Height;
        if (width <= 0 || height <= 0 || Markers is not { Count: > 0 })
            return;

        var x = (width - MARKER_WIDTH) / 2.0;
        foreach (var marker in Markers)
        {
            var y = marker.YRatio * height;
            var markerHeight = Math.Max(MIN_MARKER_HEIGHT, marker.HeightRatio * height);
            context.DrawRectangle(
                BrushFor(marker.Kind),
                null,
                new(x, y, MARKER_WIDTH, markerHeight)
            );
        }
    }
}
