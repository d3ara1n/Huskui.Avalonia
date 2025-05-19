using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Views;

namespace Huskui.Gallery;

public partial class MainWindow : AppWindow
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is MainWindowContext context)
        {
            context.Delegate = Root;
            Root.PageActivator = MainWindowContext.Activate;
            Root.Navigate(typeof(PortalView), null, null);
        }
    }
}