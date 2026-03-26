using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Siderbars;

public partial class ProjectDetailsSidebar : Sidebar
{
    public ProjectDetailsSidebar() => InitializeComponent();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

    private void OnOpenProjectClick(object? sender, RoutedEventArgs e) => Dismiss();
}
