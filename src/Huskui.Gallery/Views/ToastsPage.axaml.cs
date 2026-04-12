using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Toasts;

namespace Huskui.Gallery.Views;

public partial class ToastsPage : ControlPage
{
    public ToastsPage() => InitializeComponent();

    private AppSurface? GetAppSurface() =>
        TopLevel.GetTopLevel(this) is IAppSurfaceAccessor accessor
            ? accessor.GetAppSurface()
            : null;

    private void OnShowBlogPreviewClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        var toast = new BlogPreviewToast();
        appSurface.PopToast(toast);
    }

    private void OnShowProductDetailsClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        var toast = new ProductDetailsToast();
        appSurface.PopToast(toast);
    }
}
