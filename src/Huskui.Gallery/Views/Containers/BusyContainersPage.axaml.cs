using System.Threading.Tasks;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views.Containers;

public partial class BusyContainersPage : ControlPage
{
    public BusyContainersPage() => InitializeComponent();

    private async void OnSubmitClick(object? sender, RoutedEventArgs e)
    {
        FormContainer.IsBusy = true;

        // Simulate form submission delay
        await Task.Delay(3000);

        FormContainer.IsBusy = false;
    }

    // Interactive demo now uses ToggleButton binding - no event handlers needed
}
