using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Huskui.Gallery.Views.Input;

public partial class RadioButtonsPage : UserControl
{
    public RadioButtonsPage()
    {
        InitializeComponent();
    }

    private void OnClearSelectionClick(object? sender, RoutedEventArgs e)
    {
        // Clear all radio button selections
        SmallRadio.IsChecked = false;
        MediumRadio.IsChecked = false;
        LargeRadio.IsChecked = false;
        
        RedRadio.IsChecked = false;
        BlueRadio.IsChecked = false;
        GreenRadio.IsChecked = false;
    }

    private void OnResetPreferencesClick(object? sender, RoutedEventArgs e)
    {
        // Reset to default preferences
        LightThemeRadio.IsChecked = true;
        EnglishRadio.IsChecked = true;
        NormalRadio.IsChecked = true;
    }

    private void OnApplyPreferencesClick(object? sender, RoutedEventArgs e)
    {
        // In a real application, you would apply the settings here
        // For demo purposes, we'll just show the current state
    }

    private void OnResetGroupsClick(object? sender, RoutedEventArgs e)
    {
        // Reset to default selections
        SmallRadio.IsChecked = true;
        BlueRadio.IsChecked = true;
    }
}
