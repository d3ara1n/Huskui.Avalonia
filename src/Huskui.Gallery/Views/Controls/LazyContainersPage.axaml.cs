using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Huskui.Gallery.Views.Controls;

public partial class LazyContainersPage : UserControl
{
    public LazyContainersPage()
    {
        InitializeComponent();
    }

    private void OnRefreshClick(object? sender, RoutedEventArgs e)
    {
        // Simulate refresh action
        BadToggle.IsChecked = false;
    }

    private void OnReloadImageClick(object? sender, RoutedEventArgs e)
    {
        // Simulate image reload
        ImageErrorToggle.IsChecked = false;
    }
}
