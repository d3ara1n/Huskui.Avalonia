using Avalonia.Controls;
using Huskui.Gallery.Views;

namespace Huskui.Gallery.Services;

public class DesktopSettingsViewFactory(IThemeService themeService) : ISettingsViewFactory
{
    public Control CreateSettingsView() => new DesktopSettingsView { DataContext = themeService };
}
