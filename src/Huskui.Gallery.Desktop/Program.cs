using System;
using Avalonia;
using Huskui.Gallery;

namespace Huskui.Gallery.Desktop;

internal sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        HostRegistration.Configure();
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp() =>
        App.ConfigureSharedBuilder(AppBuilder.Configure<App>())
            .UsePlatformDetect()
            .LogToTextWriter(Console.Out)
            .WithDeveloperTools();
}
