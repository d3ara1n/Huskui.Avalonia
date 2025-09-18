using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class StepControlsPage : ControlPage
{
    public StepControlsPage()
    {
        InitializeComponent();
    }

    private void OnNextStepClick(object? sender, RoutedEventArgs e)
    {
        Control.SelectedIndex++;
    }

    private void OnPreviousStepClick(object? sender, RoutedEventArgs e)
    {
        if (Control.SelectedIndex == -1)
            Control.SelectedIndex = Control.Items.Count - 1;
        else
            Control.SelectedIndex--;
    }
}
