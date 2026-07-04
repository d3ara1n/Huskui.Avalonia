using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.ViewModels;

public partial class MainWindowViewModel : ViewModelBase, IDisposable
{
    private readonly SourceList<MenuItemVo> _allItemsSource = new();
    private readonly CompositeDisposable _disposables = new();

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

    public ReadOnlyObservableCollection<MenuItemVo> SearchResults { get; private set; } = null!;

    [ObservableProperty]
    public partial bool IsPaneOpen { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSettingsOpen { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MenuItemVo? SelectedEntry { get; set; }

    private IThemeService ThemeService { get; }

    public Control SettingsView { get; }

    public void Dispose()
    {
        _disposables.Dispose();
        _allItemsSource.Dispose();
    }

    private static Func<MenuItemVo, bool> BuildFilter(string? search) =>
        string.IsNullOrWhiteSpace(search)
            ? _ => true
            : vo => vo.MatchesSearch(search);

    [RelayCommand]
    private void ToggleTheme() => ThemeService.ToggleTheme();

    [RelayCommand]
    private void TogglePane() => IsPaneOpen = !IsPaneOpen;

    [RelayCommand]
    private void ToggleSettings() => IsSettingsOpen = !IsSettingsOpen;
}
