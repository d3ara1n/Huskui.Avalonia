using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Fonts;
using Huskui.Gallery.Services;
using Huskui.Gallery.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery;

public class App : Application
{
    public static Action<IServiceCollection>? ConfigureHostServices { get; set; }

    public static Func<MainWindowViewModel, Window>? DesktopWindowFactory { get; set; }

    public static ServiceProvider? ServiceProvider { get; private set; }

    public static AppBuilder ConfigureSharedBuilder(AppBuilder builder) =>
        builder
            .ConfigureFonts(fontManager =>
            {
                fontManager.AddFontCollection(
                    new EmbeddedFontCollection(
                        new("fonts:Manrope"),
                        new("avares://Huskui.Gallery/Assets/Fonts/Manrope")
                    )
                );
            })
            .With(
                new global::Avalonia.Media.FontManagerOptions
                {
                    DefaultFamilyName = "fonts:Manrope#Manrope",
                }
            );

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        ConfigureServices();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = ServiceProvider?.GetRequiredService<MainWindowViewModel>();
            ArgumentNullException.ThrowIfNull(mainViewModel);
            ArgumentNullException.ThrowIfNull(DesktopWindowFactory);
            desktop.MainWindow = DesktopWindowFactory(mainViewModel);
        }
        else if (ApplicationLifetime is IActivityApplicationLifetime activityLifetime)
        {
            activityLifetime.MainViewFactory = CreateMainView;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = CreateMainView();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static void ConfigureServices()
    {
        var services = new ServiceCollection();

        // Register services
        services.AddSingleton<IGalleryService, GalleryService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISettingsViewFactory, DesktopSettingsViewFactory>();

        ConfigureHostServices?.Invoke(services);

        // Register view models
        services.AddTransient<MainWindowViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Initialize services
        var galleryService = ServiceProvider.GetRequiredService<IGalleryService>();
        galleryService.Initialize();
    }

    private static Control CreateMainView()
    {
        var mainViewModel = ServiceProvider?.GetRequiredService<MainWindowViewModel>();
        ArgumentNullException.ThrowIfNull(mainViewModel);

        return new MainView { DataContext = mainViewModel };
    }
}
