using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Huskui.Gallery.Views;

public partial class CompactSettingsView : UserControl
{
    public CompactSettingsView() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
