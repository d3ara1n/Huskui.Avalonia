using System;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class SlidersPage : ControlPage
{
    public SlidersPage() => InitializeComponent();

    private void OnInteractiveSliderValueChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (InteractiveValueText != null)
            InteractiveValueText.Text = e.NewValue.ToString("0");
    }

    private void OnResetInteractiveClick(object? sender, RoutedEventArgs e) =>
        InteractiveSlider.Value = 50;

    private void OnVolumeChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (VolumeText != null)
            VolumeText.Text = $"{e.NewValue:0}%";
    }

    private void OnBrightnessChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (BrightnessText != null)
            BrightnessText.Text = $"{e.NewValue:0}%";
    }

    private void OnSpeedChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (SpeedText != null)
            SpeedText.Text = $"{e.NewValue / 10.0:0.0}x";
    }

    private void OnSeekChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (SeekText != null)
        {
            var current = TimeSpan.FromSeconds(e.NewValue);
            var total = TimeSpan.FromSeconds(300);
            SeekText.Text = $"{current:mm\\:ss} / {total:mm\\:ss}";
        }
    }

    private void OnResetMediaClick(object? sender, RoutedEventArgs e)
    {
        VolumeSlider.Value = 75;
        BrightnessSlider.Value = 50;
        SpeedSlider.Value = 10;
        SeekSlider.Value = 0;
    }
}
