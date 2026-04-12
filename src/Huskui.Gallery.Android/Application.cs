using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery.Android;

[Application]
public class Application(nint javaReference, JniHandleOwnership transfer)
    : AvaloniaAndroidApplication<App>(javaReference, transfer)
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        App.ConfigureHostServices = services =>
            services.AddSingleton<
                Services.ISettingsViewFactory,
                Services.CompactSettingsViewFactory
            >();

        return App.ConfigureSharedBuilder(base.CustomizeAppBuilder(builder));
    }
}
