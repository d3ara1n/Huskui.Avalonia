using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class DrawerPage : ControlPage
{
    public DrawerPage() => InitializeComponent();

    private AppSurface? GetAppSurface() => AppSurface.GetAppSurface(this);

    private void OpenDrawer_OnClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface != null)
        {
            var drawer = new Drawer
            {
                Title = "New Drawer",
                ShowToggleButton = ShowToggle.IsChecked ?? false,
                Content = new TextBlock
                {
                    Text = "This is the content of the drawer.",
                    VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                },
            };
            appSurface.PopDrawer(drawer);
        }
    }
}
