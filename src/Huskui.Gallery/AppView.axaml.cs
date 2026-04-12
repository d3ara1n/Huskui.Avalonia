using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.ViewModels;
using Huskui.Gallery.Views;

namespace Huskui.Gallery;

public partial class AppView : UserControl
{
    private Frame? _contentFrame;
    private MainWindowViewModel? _viewModel;

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

        if (!ReferenceEquals(_viewModel, viewModel))
        {
            if (_viewModel != null)
            {
                _viewModel.NavigationService.NavigationChanged -= OnNavigationChanged;
            }

            _viewModel = viewModel;
            _viewModel.NavigationService.NavigationChanged += OnNavigationChanged;
        }

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
        if (_viewModel != null)
        {
            _viewModel.NavigationService.NavigationChanged -= OnNavigationChanged;
            _viewModel = null;
        }
    }

    private void OnNavigationChanged(object? sender, GalleryItem? item)
    {
        if (item?.PageType != null)
        {
            try
            {
                _contentFrame?.Navigate(item.PageType, null, null);
            }
            catch (Exception ex)
            {
                _contentFrame?.Content = new TextBlock
                {
                    Text = $"Error loading page: {ex.Message}",
                    HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = global::Avalonia.Layout.VerticalAlignment.Center,
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
