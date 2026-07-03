using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.Mixins;
using Huskui.Gallery.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery;

public partial class AppView : UserControl
{
    public AppView()
    {
        InitializeComponent();
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        FrameActivationMixin.Install(NavView, App.ServiceProvider!.GetRequiredService<IViewActivator>());

        NavView.Navigate(typeof(HomePage));
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        NavView.Navigate(typeof(HomePage));
    }
}
