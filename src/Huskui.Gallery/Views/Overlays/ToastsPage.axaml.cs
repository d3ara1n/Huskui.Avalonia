using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Toasts;

namespace Huskui.Gallery.Views.Overlays
{
    public partial class ToastsPage : UserControl
    {
        public ToastsPage() => InitializeComponent();

        private AppWindow? GetAppWindow() => TopLevel.GetTopLevel(this) as AppWindow;

        private void OnShowBlogPreviewClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var toast = new BlogPreviewToast();
            appWindow.PopToast(toast);
        }


        private void OnShowProductDetailsClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var toast = new ProductDetailsToast();
            appWindow.PopToast(toast);
        }
    }
}