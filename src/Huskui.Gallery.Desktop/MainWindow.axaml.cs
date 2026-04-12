using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Desktop;

public partial class MainWindow : AppWindow
{
    public MainWindow() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
