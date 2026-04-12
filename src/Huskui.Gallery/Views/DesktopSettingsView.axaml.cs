using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Huskui.Gallery.Views;

public partial class DesktopSettingsView : UserControl
{
    public DesktopSettingsView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
