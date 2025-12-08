using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class StepControlsPage : ControlPage
{
    public StepControlsPage() => InitializeComponent();

    private void OnNextStepClick(object? sender, RoutedEventArgs e) => Control.SelectedIndex++;

    private void OnPreviousStepClick(object? sender, RoutedEventArgs e)
    {
        if (Control.SelectedIndex == -1)
        {
            Control.SelectedIndex = Control.Items.Count - 1;
        }
        else
        {
            Control.SelectedIndex--;
        }
    }

    private void OnMinimalNextStepClick(object? sender, RoutedEventArgs e) => MinimalControl.SelectedIndex++;

    private void OnMinimalPreviousStepClick(object? sender, RoutedEventArgs e)
    {
        if (MinimalControl.SelectedIndex == -1)
        {
            MinimalControl.SelectedIndex = MinimalControl.Items.Count - 1;
        }
        else
        {
            MinimalControl.SelectedIndex--;
        }
    }
}
