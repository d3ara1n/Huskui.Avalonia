using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Modals
{
    public partial class UserProfileModal : Modal
    {
        public UserProfileModal() => InitializeComponent();

        private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

        private void OnCancelClick(object? sender, RoutedEventArgs e) => Dismiss();

        private void OnSaveClick(object? sender, RoutedEventArgs e) =>
            // Save profile logic here
            Dismiss();
    }
}
