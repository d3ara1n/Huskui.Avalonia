﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":indeterminate")]
public class ProgressRing : RangeBase
{
    public const string PART_Indicator = nameof(PART_Indicator);

    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<ProgressRing, bool>(nameof(IsIndeterminate));

    public static readonly StyledProperty<bool> ShowProgressTextProperty =
        AvaloniaProperty.Register<ProgressRing, bool>(nameof(ShowProgressText));

    public static readonly StyledProperty<string> ProgressTextFormatProperty =
        AvaloniaProperty.Register<ProgressRing, string>(nameof(ProgressTextFormat), "{1:0}%");

    public static readonly StyledProperty<double> StrokeWidthProperty =
        AvaloniaProperty.Register<ProgressRing, double>(nameof(StrokeWidth), 4);

    public static readonly StyledProperty<double> TrackStrokeWidthProperty =
        AvaloniaProperty.Register<ProgressRing, double>(nameof(TrackStrokeWidth));

    public static readonly StyledProperty<Thickness> TrackPaddingProperty =
        AvaloniaProperty.Register<ProgressRing, Thickness>(nameof(TrackPadding));


    public static readonly DirectProperty<ProgressRing, double> PercentageProperty =
        AvaloniaProperty.RegisterDirect<ProgressRing, double>(nameof(Percentage), o => o.Percentage);

    public static readonly DirectProperty<ProgressRing, double> AngleProperty =
        AvaloniaProperty.RegisterDirect<ProgressRing, double>(nameof(Angle), o => o.Angle);

    public double Angle
    {
        get;
        private set => SetAndRaise(AngleProperty, ref field, value);
    }

    public double TrackStrokeWidth
    {
        get => GetValue(TrackStrokeWidthProperty);
        set => SetValue(TrackStrokeWidthProperty, value);
    }

    public Thickness TrackPadding
    {
        get => GetValue(TrackPaddingProperty);
        set => SetValue(TrackPaddingProperty, value);
    }

    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    public bool ShowProgressText
    {
        get => GetValue(ShowProgressTextProperty);
        set => SetValue(ShowProgressTextProperty, value);
    }

    public string ProgressTextFormat
    {
        get => GetValue(ProgressTextFormatProperty);
        set => SetValue(ProgressTextFormatProperty, value);
    }

    public double StrokeWidth
    {
        get => GetValue(StrokeWidthProperty);
        set => SetValue(StrokeWidthProperty, value);
    }

    public double Percentage
    {
        get;
        private set => SetAndRaise(PercentageProperty, ref field, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsIndeterminateProperty)
            PseudoClasses.Set(":indeterminate", change.GetNewValue<bool>());

        if (change.Property == ValueProperty
         || change.Property == MinimumProperty
         || change.Property == MaximumProperty
         || change.Property == IsIndeterminateProperty)
            UpdateIndicator();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        var percent = Math.Abs(Maximum - Minimum) < double.Epsilon ? 1.0 : (Value - Minimum) / (Maximum - Minimum);

        Percentage = percent * 100;

        var angle = Math.Abs(100 - Percentage) < double.Epsilon ? 360 : percent * 360 % 360;

        Angle = angle;
    }
}