using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.Mixins;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery;

public partial class AppView : UserControl
{
    public AppView()
    {
        InitializeComponent();
        NavView.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        // SelectionChanged is a bubbling routed event; nested SelectingItemsControls
        // inside the page (e.g. AutoCompleteBox's suggestion ListBox) bubble up here.
        // Only react to selection changes originating from the NavView itself.
        if (!ReferenceEquals(e.Source, NavView))
            return;

        if (NavView.SelectedItem is MenuItemVo { PageType: { } page })
            Frame.Navigate(page);
    }

    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        FrameActivationMixin.Install(Frame, App.ServiceProvider!.GetRequiredService<IViewActivator>());

        Frame.Navigate(typeof(HomePage));
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Frame.Navigate(typeof(HomePage));
    }
}
