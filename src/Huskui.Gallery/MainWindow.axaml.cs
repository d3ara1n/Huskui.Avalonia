using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.ViewModels;
using Huskui.Gallery.Views;

namespace Huskui.Gallery;

public partial class MainWindow : AppWindow
{
    private Frame? _contentFrame;

    public MainWindow() => InitializeComponent();

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        _contentFrame = this.FindControl<Frame>("ContentFrame");

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.NavigationService.NavigationChanged += OnNavigationChanged;
            _contentFrame?.PageActivator = viewModel.PageActivator;

            // Navigate to home initially
            ShowHomePage();
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
                // Show error page or fallback content
                // _contentFrame?.Content = new TextBlock
                // {
                //     Text = $"Error loading page: {ex.Message}",
                //     HorizontalAlignment = HorizontalAlignment.Center,
                //     VerticalAlignment = VerticalAlignment.Center
                // };
            }
        }
        else
        {
            ShowHomePage();
        }
    }

    private void ShowHomePage() => _contentFrame?.Navigate(typeof(HomePage), null, null);
}
