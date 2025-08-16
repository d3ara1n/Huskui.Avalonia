using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IGalleryService _galleryService;
    private readonly INavigationService _navigationService;
    private readonly IThemeService _themeService;
    private readonly CompositeDisposable _disposables = new();

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private GalleryItem? _selectedItem;

    [ObservableProperty]
    private bool _isSearchActive;

    [ObservableProperty]
    private bool _isSettingsOpen;

    // DynamicData collections
    private readonly SourceList<GalleryItem> _allItemsSource = new();
    private readonly ReadOnlyObservableCollection<GalleryItem> _searchResults = null!;

    public ObservableCollection<GalleryCategory> Categories => _galleryService.Categories;
    public IThemeService ThemeService => _themeService;
    public INavigationService NavigationService => _navigationService;
    public ReadOnlyObservableCollection<GalleryItem> SearchResults => _searchResults;

    // Unified display items - shows either search results or all items based on search state
    public ReadOnlyObservableCollection<GalleryItem> DisplayItems => _searchResults;

    public MainWindowViewModel(
        IGalleryService galleryService,
        INavigationService navigationService,
        IThemeService themeService)
    {
        _galleryService = galleryService;
        _navigationService = navigationService;
        _themeService = themeService;

        _navigationService.NavigationChanged += OnNavigationChanged;

        // Initialize all items source
        _allItemsSource.AddRange(_galleryService.AllItems);

        // Setup reactive search
        var searchTextObservable = this.WhenPropertyChanged(x => x.SearchText)
            .Select(x => x.Value ?? string.Empty)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged();

        // Create filtered collection based on search text
        // When search is empty, show all items; when search has text, show filtered results
        var searchSubscription = _allItemsSource
            .Connect()
            .Filter(searchTextObservable.Select<string, Func<GalleryItem, bool>>(searchText =>
                item => string.IsNullOrWhiteSpace(searchText) || item.MatchesSearch(searchText)))
            .Bind(out _searchResults)
            .Subscribe();

        _disposables.Add(searchSubscription);

        // Update IsSearchActive based on search text
        var searchActiveSubscription = searchTextObservable
            .Select(text => !string.IsNullOrWhiteSpace(text))
            .Subscribe(isActive => IsSearchActive = isActive);

        _disposables.Add(searchActiveSubscription);
    }

    // SearchText changes are now handled reactively in the constructor

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

    // SelectSearchResult command removed - now using unified selection through SelectedItem binding

    public void Dispose()
    {
        _disposables.Dispose();
        _allItemsSource.Dispose();
        _navigationService.NavigationChanged -= OnNavigationChanged;
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
