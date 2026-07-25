using System.Reflection;
using Avalonia;
using Avalonia.Interactivity;
using Huskui.Avalonia;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Modals;

public partial class AboutModal : Modal
{
    public AboutModal()
    {
        InitializeComponent();
        LibraryVersionTag.Content = $"Huskui.Avalonia {GetVersion(typeof(HuskuiTheme).Assembly)}";
        AvaloniaVersionTag.Content = $"Avalonia {GetVersion(typeof(Application).Assembly)}";
    }

    private static string GetVersion(Assembly assembly)
    {
        var informational = assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
           ?.InformationalVersion;
        return informational is { Length: > 0 }
            ? informational.Split('+')[0]
            : assembly.GetName().Version?.ToString(3) ?? "Unknown";
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Dismiss();
}
