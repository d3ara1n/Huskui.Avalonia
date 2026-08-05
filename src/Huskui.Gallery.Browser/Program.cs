using System.Threading.Tasks;
using Avalonia;
using Avalonia.Browser;
using Huskui.Gallery;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery.Browser;

internal static class Program
{
    private static Task Main(string[] args)
    {
        App.ConfigureHostServices = services =>
            services.AddSingleton<
                Services.ISettingsViewFactory,
                Services.CompactSettingsViewFactory
            >();

        return BuildAvaloniaApp().StartBrowserAppAsync("out");
    }

    public static AppBuilder BuildAvaloniaApp() =>
        App.ConfigureSharedBuilder(AppBuilder.Configure<App>());
}
