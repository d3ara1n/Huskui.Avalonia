using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Siderbars;

public partial class SettingsSidebar : Sidebar
{
    public SettingsSidebar()
    {
        InitializeComponent();
    }

    public SettingsSidebar(object? content) : this()
    {
        this.FindControl<ContentControl>("ContentHost")!.Content = content;
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();
}
