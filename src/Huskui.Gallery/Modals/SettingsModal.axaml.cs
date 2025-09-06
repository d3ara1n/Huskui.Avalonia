using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Modals;

public partial class SettingsModal : Modal
{
    public SettingsModal() => InitializeComponent();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Dismiss();

    private void OnSaveClick(object? sender, RoutedEventArgs e) =>
        // Save settings logic here
        Dismiss();

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        // Reset to defaults logic here
    }
}
