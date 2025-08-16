using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Toasts
{
    public partial class BlogPreviewToast : Toast
    {
        public BlogPreviewToast() => InitializeComponent();

        private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

        private void OnReadFullClick(object? sender, RoutedEventArgs e) =>
            // Navigate to full article
            Dismiss();
    }
}