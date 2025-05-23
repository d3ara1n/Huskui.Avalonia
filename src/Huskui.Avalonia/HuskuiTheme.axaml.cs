using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace Huskui.Avalonia;

public class HuskuiTheme : Styles
{
    public HuskuiTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }
}