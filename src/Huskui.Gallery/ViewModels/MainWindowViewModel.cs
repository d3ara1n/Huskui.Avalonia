using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Mixins;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Gallery.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    // DynamicData collections
    private readonly SourceList<MenuItemVo> _allItemsSource = new();
    private readonly CompositeDisposable _disposables = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly NavigationService _navigationService;
    private readonly MenuItemService _menuItemService;

    public MainWindowViewModel(
        NavigationService navigationService,
        MenuItemService menuItemService,
        IThemeService themeService,
        ISettingsViewFactory settingsViewFactory,
        IServiceProvider serviceProvider
    )
    {
        _navigationService = navigationService;
        _menuItemService = menuItemService;
        ThemeService = themeService;
        _serviceProvider = serviceProvider;
        SettingsView = settingsViewFactory.CreateSettingsView();
        _navigationService.NavigationChanged += OnNavigationChanged;

        // Setup reactive search
        var searchTextObservable = this.WhenPropertyChanged(x => x.SearchText)
            .Select(x => x.Value ?? string.Empty)
            .Throttle(TimeSpan.FromMilliseconds(300))
            .DistinctUntilChanged();

        // Create filtered collection based on search text
        var searchSubscription = searchTextObservable
           .Subscribe(searchText =>
            {
                SearchResults.Clear();
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    SearchResults.AddRange(_allItemsSource.Items);
                }
                else
                {
                    SearchResults.AddRange(_allItemsSource.Items.Where(item => item.MatchesSearch(searchText) && !item.IsSeparator));
                }
            });

        _disposables.Add(searchSubscription);

        // Initialize category groups
        InitializeMenuItem();

    }

    [ObservableProperty]
    public partial AvaloniaList<MenuItemVo> SearchResults { get; set; } = [];

    [ObservableProperty]
    public partial bool CanGoback { get; set; } = false;

    [ObservableProperty]
    public partial bool CanGoForward { get; set; } = false;


    [ObservableProperty]
    public partial bool IsSearchActive { get; set; }

    [ObservableProperty]
    public partial bool HorizonCollapsed { get; set; }

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MenuItemVo? SelectedItem { get; set; }

    private IThemeService ThemeService { get; }

    public Control SettingsView { get; }

    public Frame.PageActivatorDelegate PageActivator => ActivatePage;

    #region IDisposable Members

    public void Dispose()
    {
        _disposables.Dispose();
        _allItemsSource.Dispose();
        _navigationService.NavigationChanged -= OnNavigationChanged;

    }

    #endregion

    private object? ActivatePage(Type view, object? parameter)
    {
        if (!view.IsAssignableTo(typeof(Avalonia.Controls.Page)))
        {
            throw new ArgumentOutOfRangeException(
                nameof(view),
                view,
                "Parameter view must be derived from Page"
            );
        }

        var name = view.FullName!.Replace("View", "ViewModel", StringComparison.Ordinal);
        var type = Type.GetType(name);

        var page = Activator.CreateInstance(view) as Avalonia.Controls.Page;

        if (page is not null && type is not null)
        {
            if (!type.IsAssignableTo(typeof(ObservableObject)))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(view),
                    type,
                    $"{view.Name} was bound to a view model which is not derived from ObservableObject"
                );
            }

            var viewModel = ActivatorUtilities.CreateInstance(_serviceProvider, type);

            page.DataContext = viewModel;
            ViewModelMixin.Attach(page);
        }

        return page;
    }

    // SearchText changes are now handled reactively in the constructor

    partial void OnSelectedItemChanged(MenuItemVo? value)
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
    }

    [RelayCommand]
    private void GoForward()
    {
        _navigationService.GoForward();
    }

    private void OnNavigationChanged(MenuItemVo? item,bool canGoBack,bool canGoForward)
    {
        Dispatcher.UIThread.Post(() =>
        {
            //if (item == SelectedItem) return;
            SelectedItem = item;
            CanGoback = canGoBack;
            CanGoForward = canGoForward;
        });
    }

    [RelayCommand]
    private void ToggleTheme() => ThemeService.ToggleTheme();

    [RelayCommand]
    private void Collapse() => HorizonCollapsed = !HorizonCollapsed;

    [RelayCommand]
    private void ToggleSettings() => IsSettingsOpen = !IsSettingsOpen;


    private void InitializeMenuItem()
    {
        _allItemsSource.AddRange(_menuItemService.AllMenus);
    }
}
