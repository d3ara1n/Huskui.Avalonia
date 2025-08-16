using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Huskui.Gallery.Views.Controls
{
    public partial class BusyContainersPage : UserControl
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
}