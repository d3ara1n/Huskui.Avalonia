using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Siderbars;

public partial class TeamMemberEditorSidebar : Sidebar
{
    public TeamMemberEditorSidebar() => InitializeComponent();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();

    private void OnSaveClick(object? sender, RoutedEventArgs e) => Dismiss();
}
