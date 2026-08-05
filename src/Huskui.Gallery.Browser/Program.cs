using Avalonia;
using Avalonia.Browser;
using Huskui.Gallery.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery.Browser;

internal static class Program
{
    private static Task Main(string[] args)
    {
        App.ConfigureHostServices = services =>
            services.AddSingleton<ISettingsViewFactory, CompactSettingsViewFactory>();

        return BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp() => App.ConfigureSharedBuilder(AppBuilder.Configure<App>());
}
