using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Huskui.Avalonia;

namespace Huskui.Gallery.Services;

/// <summary>
///     Default implementation of IThemeService
/// </summary>
public class ThemeService : IThemeService
{
    #region IThemeService Members

    public ThemeVariant CurrentTheme { get; private set; } = ThemeVariant.Default;

    public AccentColor CurrentAccent { get; private set; } = AccentColor.System;

    public BackgroundMaterial CurrentBackground { get; private set; } = BackgroundMaterial.Mica;

    public event EventHandler? ThemeChanged;

    public void SetTheme(ThemeVariant theme)
    {
        if (CurrentTheme == theme)
        {
            return;
        }

        CurrentTheme = theme;

        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = theme;
        }

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetAccent(AccentColor accent)
    {
        if (CurrentAccent == accent)
        {
            return;
        }

        CurrentAccent = accent;
        UpdateHuskuiTheme();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetBackground(BackgroundMaterial background)
    {
        if (CurrentBackground == background)
        {
            return;
        }

        CurrentBackground = background;
        UpdateWindowBackground();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleTheme()
    {
        var newTheme = CurrentTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
        SetTheme(newTheme);
    }

    #endregion

    private void UpdateHuskuiTheme()
    {
        if (Application.Current?.Styles is { } styles)
        {
            // Find and update the HuskuiTheme
            foreach (var t in styles)
            {
                if (t is HuskuiTheme huskuiTheme)
                {
                    huskuiTheme.Accent = CurrentAccent;
                    break;
                }
            }
        }
    }

    private void UpdateWindowBackground()
    {
        // Update main window TransparencyLevelHint based on selected material
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = desktop.MainWindow;
            if (mainWindow != null)
            {
                var transparencyLevel = CurrentBackground switch
                {
                    BackgroundMaterial.None => WindowTransparencyLevel.None,
                    BackgroundMaterial.Mica => WindowTransparencyLevel.Mica,
                    BackgroundMaterial.AcrylicBlur => WindowTransparencyLevel.AcrylicBlur,
                    BackgroundMaterial.Transparent => WindowTransparencyLevel.Transparent,
                    _ => WindowTransparencyLevel.None
                };

                mainWindow.TransparencyLevelHint = [transparencyLevel];
            }
        }
    }
}
