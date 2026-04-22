using Avalonia.Markup.Xaml;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class MvvmPage : ControlPage
{
    public MvvmPage() => InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
