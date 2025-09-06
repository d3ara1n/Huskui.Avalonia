using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Toasts;

public partial class ProductDetailsToast : Toast
{
    public ProductDetailsToast() => InitializeComponent();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

    private void OnAddToCartClick(object? sender, RoutedEventArgs e)
    {
        // Add to cart logic
    }

    private void OnBuyNowClick(object? sender, RoutedEventArgs e) =>
        // Buy now logic
        Dismiss();

    private void OnViewFullClick(object? sender, RoutedEventArgs e) =>
        // Navigate to full product page
        Dismiss();
}
