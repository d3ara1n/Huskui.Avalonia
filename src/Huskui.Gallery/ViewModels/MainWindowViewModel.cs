using System.Collections.ObjectModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Styling;
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
    private readonly ISettingsViewFactory _settingsViewFactory;

    public MainWindowViewModel(
        MenuItemService menuItemService,
        IThemeService themeService,
        ISettingsViewFactory settingsViewFactory
    )
    {
        ThemeService = themeService;
        _settingsViewFactory = settingsViewFactory;
        IsDarkTheme = themeService.CurrentTheme == ThemeVariant.Dark;
        themeService.ThemeChanged += (_, _) => IsDarkTheme = themeService.CurrentTheme == ThemeVariant.Dark;

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
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial MenuItemVo? SelectedEntry { get; set; }

    [ObservableProperty]
    public partial bool IsDarkTheme { get; set; }

    private IThemeService ThemeService { get; }

    public Control CreateSettingsView() => _settingsViewFactory.CreateSettingsView();

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
}
