using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.Mixins;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery;

public partial class AppView : UserControl
{
    public AppView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (this.FindControl<NavigationView>("NavView") is { } navView)
            FrameActivationMixin.Install(
                navView,
                App.ServiceProvider!.GetRequiredService<IViewActivator>());
    }
}
