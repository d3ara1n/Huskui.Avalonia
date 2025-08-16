using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.ViewModels;
using Huskui.Gallery.Views.Pages;

namespace Huskui.Gallery.Views;

public partial class MainWindow : AppWindow
{
    private ContentControl? _contentFrame;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        _contentFrame = this.FindControl<ContentControl>("ContentFrame");

        if (DataContext is MainWindowViewModel viewModel)
        {
            viewModel.NavigationService.NavigationChanged += OnNavigationChanged;
            
            // Navigate to home initially
            ShowHomePage();
        }
    }

    private void OnNavigationChanged(object? sender, Models.GalleryItem? item)
    {
        if (item?.PageType != null)
        {
            try
            {
                var page = Activator.CreateInstance(item.PageType);
                if (_contentFrame != null)
                {
                    _contentFrame.Content = page;
                }
            }
            catch (Exception ex)
            {
                // Show error page or fallback content
                if (_contentFrame != null)
                {
                    _contentFrame.Content = new TextBlock
                    {
                        Text = $"Error loading page: {ex.Message}",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
        }
        else
        {
            ShowHomePage();
        }
    }

    private void ShowHomePage()
    {
        if (_contentFrame != null)
        {
            _contentFrame.Content = new HomePage();
        }
    }
}
