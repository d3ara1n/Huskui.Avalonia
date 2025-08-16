using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly IGalleryService _galleryService;
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private GalleryItem? _selectedItem;

    [ObservableProperty]
    private bool _isSearchActive;

    [ObservableProperty]
    private ObservableCollection<GalleryItem> _searchResults = new();

    [ObservableProperty]
    private bool _isSettingsOpen;

    public ObservableCollection<GalleryCategory> Categories => _galleryService.Categories;
    public IThemeService ThemeService => _themeService;
    public INavigationService NavigationService => _navigationService;

    public MainWindowViewModel(
        IGalleryService galleryService,
        INavigationService navigationService,
        IThemeService themeService)
    {
        _galleryService = galleryService;
        _navigationService = navigationService;
        _themeService = themeService;

        _navigationService.NavigationChanged += OnNavigationChanged;
    }

    partial void OnSearchTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            IsSearchActive = false;
            SearchResults.Clear();
        }
        else
        {
            IsSearchActive = true;
            var results = _galleryService.SearchItems(value);
            SearchResults.Clear();
            foreach (var item in results)
            {
                SearchResults.Add(item);
            }
        }
    }

    partial void OnSelectedItemChanged(GalleryItem? value)
    {
        if (value != null)
        {
            _navigationService.NavigateTo(value);
        }
    }

    [RelayCommand]
    private void NavigateHome()
    {
        _navigationService.NavigateToHome();
        SelectedItem = null;
        SearchText = string.Empty;
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoBack();
        UpdateSelectedItemFromNavigation();
    }

    [RelayCommand]
    private void GoForward()
    {
        _navigationService.GoForward();
        UpdateSelectedItemFromNavigation();
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        _themeService.ToggleTheme();
    }

    [RelayCommand]
    private void ToggleSettings()
    {
        IsSettingsOpen = !IsSettingsOpen;
    }

    [RelayCommand]
    private void SelectSearchResult(GalleryItem item)
    {
        SelectedItem = item;
        SearchText = string.Empty;
    }

    private void OnNavigationChanged(object? sender, GalleryItem? item)
    {
        UpdateSelectedItemFromNavigation();
    }

    private void UpdateSelectedItemFromNavigation()
    {
        var currentItem = _navigationService.CurrentItem;
        if (SelectedItem != currentItem)
        {
            SelectedItem = currentItem;
        }
    }
}
