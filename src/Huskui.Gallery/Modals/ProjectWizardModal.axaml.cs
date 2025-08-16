using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Modals;

public partial class ProjectWizardModal : Modal
{
    public ProjectWizardModal()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e)
    {
        Dismiss();
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Dismiss();
    }

    private void OnNextClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to next step
        // For demo purposes, just close
        Dismiss();
    }
}
