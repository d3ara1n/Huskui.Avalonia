using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Huskui.Gallery.Services;
using Huskui.Gallery.ViewModels;
using Huskui.Gallery.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery;

public class App : Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }

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
            desktop.MainWindow = new MainWindow { DataContext = mainViewModel };
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

        // Register view models
        services.AddTransient<MainWindowViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // Initialize services
        var galleryService = ServiceProvider.GetRequiredService<IGalleryService>();
        galleryService.Initialize();
    }
}
