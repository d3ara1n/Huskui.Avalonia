using Avalonia.Controls;
using Huskui.Gallery.Views;

namespace Huskui.Gallery.Services;

public class CompactSettingsViewFactory(IThemeService themeService) : ISettingsViewFactory
{
    public Control CreateSettingsView() => new CompactSettingsView { DataContext = themeService };
}
