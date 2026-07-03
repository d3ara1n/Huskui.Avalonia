using System.Collections.ObjectModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly SourceList<NavigationItem> _allItemsSource = new();
    private readonly CompositeDisposable _disposables = new();
    private readonly IScheduler _uiScheduler = Scheduler.CurrentThread;

    public MainWindowViewModel(
        MenuItemService menuItemService,
        IThemeService themeService,
        ISettingsViewFactory settingsViewFactory
    )
    {
        ThemeService = themeService;
        SettingsView = settingsViewFactory.CreateSettingsView();

        var filter = this.WhenPropertyChanged(x => x.SearchText)
                         .Select(x => BuildFilter(x.Value));

        _allItemsSource.AddRange(menuItemService.AllMenus);

        _disposables.Add(
            _allItemsSource
               .Connect()
               .Filter(filter)
               .Bind(out var results)
               .Subscribe());

        SearchResults = results;
    }

    public ReadOnlyObservableCollection<NavigationItem> SearchResults { get; private set; } = null!;

    [ObservableProperty]
    public partial bool IsPaneOpen { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial NavigationItem? SelectedItem { get; set; }

    [ObservableProperty]
    public partial MenuItemVo? SelectedEntry { get; set; }

    partial void OnSelectedItemChanged(NavigationItem? value) => SelectedEntry = value?.Content as MenuItemVo;

    private IThemeService ThemeService { get; }

    public Control SettingsView { get; }

    public void Dispose()
    {
        _disposables.Dispose();
        _allItemsSource.Dispose();
    }

    private static Func<NavigationItem, bool> BuildFilter(string? search) =>
        string.IsNullOrWhiteSpace(search)
            ? _ => true
            : item => item.Content is MenuItemVo vo && vo.MatchesSearch(search);

    [RelayCommand]
    private void ToggleTheme() => ThemeService.ToggleTheme();

    [RelayCommand]
    private void TogglePane() => IsPaneOpen = !IsPaneOpen;

    [RelayCommand]
    private void ToggleSettings() => IsSettingsOpen = !IsSettingsOpen;
}
