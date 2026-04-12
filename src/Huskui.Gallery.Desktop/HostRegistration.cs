using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery.Desktop;

internal static class HostRegistration
{
    private static bool configured;

    public static void Configure()
    {
        if (configured)
        {
            return;
        }

        App.ConfigureHostServices = services =>
            services.AddSingleton<
                Services.ISettingsViewFactory,
                Services.DesktopSettingsViewFactory
            >();
        App.DesktopWindowFactory = viewModel => new MainWindow { DataContext = viewModel };
        configured = true;
    }
}
