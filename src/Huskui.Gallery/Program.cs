using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Fonts;

namespace Huskui.Gallery;

internal class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
           .Configure<App>()
           .ConfigureFonts(fontManager =>
            {
                fontManager.AddFontCollection(new EmbeddedFontCollection(new("fonts:Manrope"),
                                                                         new("avares://Huskui.Gallery/Assets/Fonts/Manrope")));
            })
           .With(new FontManagerOptions { DefaultFamilyName = "fonts:Manrope#Manrope" })
           .UsePlatformDetect()
           .LogToTextWriter(Console.Out);
}
