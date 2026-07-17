using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Huskui.Avalonia.Code.Models;

namespace Huskui.Avalonia.Code.Controls;

[TemplatePart(PART_Track, typeof(Control))]
[TemplatePart(PART_Thumb, typeof(Control))]
public class DiffOverviewBar : TemplatedControl
{
    public const string PART_Track = nameof(PART_Track);
    public const string PART_Thumb = nameof(PART_Thumb);
    public const double DEFAULT_WIDTH = 20.0;
    private const double MIN_THUMB_HEIGHT = 12.0;

    public static readonly StyledProperty<IReadOnlyList<DiffMarker>?> MarkersProperty =
        AvaloniaProperty.Register<DiffOverviewBar, IReadOnlyList<DiffMarker>?>(nameof(Markers));

    public static readonly StyledProperty<double> ViewTopRatioProperty =
        AvaloniaProperty.Register<DiffOverviewBar, double>(nameof(ViewTopRatio));

    public static readonly StyledProperty<double> ViewportRatioProperty =
        AvaloniaProperty.Register<DiffOverviewBar, double>(nameof(ViewportRatio));

    public static readonly StyledProperty<int> TotalLinesProperty =
        AvaloniaProperty.Register<DiffOverviewBar, int>(nameof(TotalLines));

    public IReadOnlyList<DiffMarker>? Markers
    {
        get => GetValue(MarkersProperty);
        set => SetValue(MarkersProperty, value);
    }

    public double ViewTopRatio
    {
        get => GetValue(ViewTopRatioProperty);
        set => SetValue(ViewTopRatioProperty, value);
    }

    public double ViewportRatio
    {
        get => GetValue(ViewportRatioProperty);
        set => SetValue(ViewportRatioProperty, value);
    }

    public int TotalLines
    {
        get => GetValue(TotalLinesProperty);
        set => SetValue(TotalLinesProperty, value);
    }

    public event EventHandler<double>? ScrollRequested;

    private Control? _track;
    private Control? _thumb;
    private readonly TranslateTransform _thumbTransform = new();
    private bool _isDragging;
    private double _thumbHeight = MIN_THUMB_HEIGHT;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_track != null)
        {
            _track.PointerPressed -= OnTrackPointerPressed;
            _track.PointerMoved -= OnTrackPointerMoved;
            _track.PointerReleased -= OnTrackPointerReleased;
            _track.PointerCaptureLost -= OnTrackPointerCaptureLost;
        }

        _track = e.NameScope.Find<Control>(PART_Track);
        _thumb = e.NameScope.Find<Control>(PART_Thumb);

        if (_thumb != null)
            _thumb.RenderTransform = _thumbTransform;

        if (_track != null)
        {
            _track.PointerPressed += OnTrackPointerPressed;
            _track.PointerMoved += OnTrackPointerMoved;
            _track.PointerReleased += OnTrackPointerReleased;
            _track.PointerCaptureLost += OnTrackPointerCaptureLost;
        }

        UpdateThumbGeometry();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ViewTopRatioProperty)
            UpdateThumbPosition();
        else if (change.Property == ViewportRatioProperty)
            UpdateThumbGeometry();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var result = base.ArrangeOverride(finalSize);
        UpdateThumbGeometry();
        return result;
    }

    private void OnTrackPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        _isDragging = true;
        e.Pointer.Capture(_track);
        RequestScroll(e);
        e.Handled = true;
    }

    private void OnTrackPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging)
            return;

        RequestScroll(e);
        e.Handled = true;
    }

    private void OnTrackPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private void OnTrackPointerCaptureLost(object? sender, PointerCaptureLostEventArgs e) =>
        _isDragging = false;

    private void RequestScroll(PointerEventArgs e)
    {
        if (_track == null)
            return;

        var height = _track.Bounds.Height;
        if (height <= 0)
            return;

        var position = e.GetPosition(_track);
        var movable = height - _thumbHeight;
        var ratio = movable > 0 ? (position.Y - _thumbHeight / 2.0) / movable : 0;
        ScrollRequested?.Invoke(this, Math.Clamp(ratio, 0.0, 1.0));
    }

    private void UpdateThumbGeometry()
    {
        if (_track == null || _thumb == null)
            return;

        var height = _track.Bounds.Height;
        if (height <= 0)
            return;

        _thumbHeight = Math.Min(height, Math.Max(MIN_THUMB_HEIGHT, ViewportRatio * height));
        _thumb.Height = _thumbHeight;
        UpdateThumbPosition();
    }

    private void UpdateThumbPosition()
    {
        if (_track == null || _thumb == null)
            return;

        var height = _track.Bounds.Height;
        if (height <= 0)
            return;

        var movable = Math.Max(0, height - _thumbHeight);
        _thumbTransform.Y = Math.Clamp(ViewTopRatio, 0.0, 1.0) * movable;
    }
}
