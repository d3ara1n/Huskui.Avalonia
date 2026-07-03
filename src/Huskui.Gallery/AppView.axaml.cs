using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;
using Huskui.Gallery.ViewModels;
using Huskui.Gallery.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery;

public partial class AppView : UserControl
{
    private Frame? _contentFrame;

    public AppView()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        _contentFrame ??= this.FindControl<Frame>("ContentFrame");

        if (DataContext is not MainWindowViewModel viewModel)
        {
            return;
        }

        var navigationService = App.ServiceProvider!.GetRequiredService<NavigationService>();
        navigationService.NavigationChanged -= OnNavigationChanged;
        navigationService.NavigationChanged += OnNavigationChanged;

        if (_contentFrame != null)
        {
            _contentFrame.PageActivator = viewModel.PageActivator;

            if (_contentFrame.Content == null)
            {
                ShowHomePage();
            }
        }
    }

    private void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        var navigationService = App.ServiceProvider!.GetRequiredService<NavigationService>();
        navigationService.NavigationChanged -= OnNavigationChanged;
    }

    private void OnNavigationChanged(MenuItemVo? item, bool canGoback, bool canGoForward)
    {
        if (item is { IsSeparator: false })
        {
            try
            {
                if (item.PageType == null)
                {
                    throw new NullReferenceException(item.Title);
                }

                _contentFrame?.Navigate(item.PageType, null, null);
            }
            catch (Exception ex)
            {
                _contentFrame?.Content = new TextBlock
                {
                    Text = $"Error loading page: {ex.Message}",
                    HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center
                };
            }
        }
        else
        {
            ShowHomePage();
        }
    }

    private void ShowHomePage() => _contentFrame?.Navigate(typeof(HomePage), null, null);
}
