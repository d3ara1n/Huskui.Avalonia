using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Huskui.Avalonia;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery.ViewModels;

public partial class MainWindowViewModel : ObservableObject, IDisposable
{
    private readonly IServiceProvider _serviceProvider;

    // DynamicData collections
    private readonly SourceList<GalleryItem> _allItemsSource = new();
    private readonly CompositeDisposable _disposables = new();
    private readonly IGalleryService _galleryService;
    private readonly ReadOnlyObservableCollection<GalleryItem> _searchResults;

    public MainWindowViewModel(
        IGalleryService galleryService,
        INavigationService navigationService,
        IThemeService themeService,
        IServiceProvider serviceProvider)
    {
        _galleryService = galleryService;
        NavigationService = navigationService;
        ThemeService = themeService;
        _serviceProvider = serviceProvider;

        NavigationService.NavigationChanged += OnNavigationChanged;

        // Initialize all items source
        _allItemsSource.AddRange(_galleryService.AllItems);

        // Setup reactive search
        var searchTextObservable = this
                                  .WhenPropertyChanged(x => x.SearchText)
                                  .Select(x => x.Value ?? string.Empty)
                                  .Throttle(TimeSpan.FromMilliseconds(300))
                                  .DistinctUntilChanged();

        // Create filtered collection based on search text
        var searchSubscription = _allItemsSource
                                .Connect()
                                .Filter(searchTextObservable.Select<string, Func<GalleryItem, bool>>(searchText =>
                                            item => string.IsNullOrWhiteSpace(searchText)
                                                 || item.MatchesSearch(searchText)))
                                .Bind(out _searchResults)
                                .Subscribe();

        _disposables.Add(searchSubscription);

        // Initialize category groups
        InitializeCategoryGroups();

        // Update category groups when search text changes
        var categoryFilterSubscription = searchTextObservable.Subscribe(searchText =>
        {
            IsSearchActive = !string.IsNullOrWhiteSpace(searchText);
            UpdateCategoryFilters(searchText);
        });

        _disposables.Add(categoryFilterSubscription);
    }

    [ObservableProperty]
    public partial ObservableCollection<CategoryGroupViewModel> CategoryGroups { get; set; } = new();

    [ObservableProperty]
    public partial bool IsSearchActive { get; set; }

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial GalleryItem? SelectedItem { get; set; }

    public IThemeService ThemeService { get; }

    public INavigationService NavigationService { get; }

    public Frame.PageActivatorDelegate PageActivator => ActivatePage;

    public ReadOnlyObservableCollection<GalleryItem> SearchResults => _searchResults;

    #region IDisposable Members

    public void Dispose()
    {
        _disposables.Dispose();
        _allItemsSource.Dispose();
        NavigationService.NavigationChanged -= OnNavigationChanged;

        // Unsubscribe from category group events
        foreach (var categoryGroup in CategoryGroups)
        {
            categoryGroup.PropertyChanged -= OnCategoryGroupPropertyChanged;
        }
    }

    #endregion

    private object? ActivatePage(Type view, object? parameter)
    {
        if (!view.IsAssignableTo(typeof(Page)))
        {
            throw new ArgumentOutOfRangeException(nameof(view), view, "Parameter view must be derived from Page");
        }

        var name = view.FullName!.Replace("View", "ViewModel", StringComparison.Ordinal);
        var type = Type.GetType(name);

        var page = Activator.CreateInstance(view) as Page;

        if (page is not null && type is not null)
        {
            if (!type.IsAssignableTo(typeof(ObservableObject)))
            {
                throw new ArgumentOutOfRangeException(nameof(view),
                                                      type,
                                                      $"{view.Name} was bound to a view model which is not derived from ObservableObject");
            }

            var viewModel = ActivatorUtilities.CreateInstance(_serviceProvider, type);

            page.DataContext = viewModel;

            if (viewModel is IPageModel pageModel)
            {
                pageModel.PageToken = page.LifetimeToken;
                page.Model = pageModel;
            }
        }

        return page;
    }

    // SearchText changes are now handled reactively in the constructor

    partial void OnSelectedItemChanged(GalleryItem? value)
    {
        if (value != null)
        {
            NavigationService.NavigateTo(value);
        }
    }

    [RelayCommand]
    private void NavigateHome()
    {
        NavigationService.NavigateToHome();
        SelectedItem = null;
        SearchText = string.Empty;
    }

    [RelayCommand]
    private void GoBack()
    {
        NavigationService.GoBack();
        UpdateSelectedItemFromNavigation();
    }

    [RelayCommand]
    private void GoForward()
    {
        NavigationService.GoForward();
        UpdateSelectedItemFromNavigation();
    }

    [RelayCommand]
    private void ToggleTheme() => ThemeService.ToggleTheme();

    [RelayCommand]
    private void ToggleSettings() => IsSettingsOpen = !IsSettingsOpen;

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
        if (e.PropertyName == nameof(CategoryGroupViewModel.SelectedItem)
         && sender is CategoryGroupViewModel categoryGroup)
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

    private void OnNavigationChanged(object? sender, GalleryItem? item) => UpdateSelectedItemFromNavigation();

    private void UpdateSelectedItemFromNavigation()
    {
        var currentItem = NavigationService.CurrentItem;
        if (SelectedItem != currentItem)
        {
            SelectedItem = currentItem;
        }
    }
}
