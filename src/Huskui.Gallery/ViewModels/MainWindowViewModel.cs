using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;
using Huskui.Gallery.ViewModels;

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

    [ObservableProperty]
    private ObservableCollection<CategoryGroupViewModel> _categoryGroups = new();

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
        var searchSubscription = _allItemsSource
            .Connect()
            .Filter(searchTextObservable.Select<string, Func<GalleryItem, bool>>(searchText =>
                item => string.IsNullOrWhiteSpace(searchText) || item.MatchesSearch(searchText)))
            .Bind(out _searchResults)
            .Subscribe();

        _disposables.Add(searchSubscription);

        // Initialize category groups
        InitializeCategoryGroups();

        // Update category groups when search text changes
        var categoryFilterSubscription = searchTextObservable
            .Subscribe(searchText =>
            {
                IsSearchActive = !string.IsNullOrWhiteSpace(searchText);
                UpdateCategoryFilters(searchText);
            });

        _disposables.Add(categoryFilterSubscription);
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

    [RelayCommand]
    private void SelectItem(GalleryItem item)
    {
        // Clear selection from all category groups first
        foreach (var categoryGroup in CategoryGroups)
        {
            categoryGroup.ClearSelection();
        }

        // Set the main selected item
        SelectedItem = item;
    }

    private void InitializeCategoryGroups()
    {
        CategoryGroups.Clear();

        foreach (var category in _galleryService.Categories)
        {
            // Create observable collection for this category's items
            var categoryItems = new ObservableCollection<GalleryItem>(category.Items);
            var readOnlyItems = new ReadOnlyObservableCollection<GalleryItem>(categoryItems);

            // Create category group view model
            var groupViewModel = new CategoryGroupViewModel(category, readOnlyItems);

            // Subscribe to selection changes
            groupViewModel.PropertyChanged += OnCategoryGroupPropertyChanged;

            CategoryGroups.Add(groupViewModel);
        }
    }

    private void UpdateCategoryFilters(string searchText)
    {
        foreach (var categoryGroup in CategoryGroups)
        {
            categoryGroup.UpdateFilter(searchText);
        }
    }

    private void OnCategoryGroupPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(CategoryGroupViewModel.SelectedItem) && sender is CategoryGroupViewModel categoryGroup)
        {
            if (categoryGroup.SelectedItem != null)
            {
                // Clear selection from other category groups
                foreach (var otherGroup in CategoryGroups)
                {
                    if (otherGroup != categoryGroup)
                    {
                        otherGroup.ClearSelection();
                    }
                }

                // Update main selected item
                SelectedItem = categoryGroup.SelectedItem;
            }
        }
    }

    public void Dispose()
    {
        _disposables.Dispose();
        _allItemsSource.Dispose();
        _navigationService.NavigationChanged -= OnNavigationChanged;

        // Unsubscribe from category group events
        foreach (var categoryGroup in CategoryGroups)
        {
            categoryGroup.PropertyChanged -= OnCategoryGroupPropertyChanged;
        }
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
