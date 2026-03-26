using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Siderbars;

public partial class SmartFilterSidebar : Sidebar
{
    public SmartFilterSidebar() => InitializeComponent();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

    private void OnResetClick(object? sender, RoutedEventArgs e)
    {
        // Reset bound filter VM values in a real project
    }

    private void OnApplyClick(object? sender, RoutedEventArgs e) => Dismiss();
}
