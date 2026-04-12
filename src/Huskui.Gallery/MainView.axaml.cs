using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery;

public partial class MainView : AppSurface
{
    public MainView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
