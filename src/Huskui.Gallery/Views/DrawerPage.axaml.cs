using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class DrawerPage : ControlPage
{
    public DrawerPage() => InitializeComponent();

    private void OpenDrawer_OnClick(object? sender, RoutedEventArgs e)
    {
        var window = this.FindAncestorOfType<AppWindow>();
        if (window != null)
        {
            var drawer = new Drawer
            {
                Title = "New Drawer",
                Width = 400,
                Height = 300,
                Content = new TextBlock
                {
                    Text = "This is the content of the drawer.",
                    VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
                    HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center
                }
            };
            window.PopDrawer(drawer);
        }
    }
}