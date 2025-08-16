using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Styling;
using Huskui.Avalonia;

namespace Huskui.Gallery.Services;

/// <summary>
/// Default implementation of IThemeService
/// </summary>
public class ThemeService : IThemeService
{
    private ThemeVariant _currentTheme = ThemeVariant.Default;
    private AccentColor _currentAccent = AccentColor.Blue;
    private BackgroundMaterial _currentBackground = BackgroundMaterial.Mica;

    public ThemeVariant CurrentTheme => _currentTheme;
    public AccentColor CurrentAccent => _currentAccent;
    public BackgroundMaterial CurrentBackground => _currentBackground;

    public event EventHandler? ThemeChanged;

    public void SetTheme(ThemeVariant theme)
    {
        if (_currentTheme == theme) return;

        _currentTheme = theme;
        
        if (Application.Current is { } app)
        {
            app.RequestedThemeVariant = theme;
        }

        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetAccent(AccentColor accent)
    {
        if (_currentAccent == accent) return;

        _currentAccent = accent;
        UpdateHuskuiTheme();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetBackground(BackgroundMaterial background)
    {
        if (_currentBackground == background) return;

        _currentBackground = background;
        UpdateWindowBackground();
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public void ToggleTheme()
    {
        var newTheme = _currentTheme == ThemeVariant.Dark 
            ? ThemeVariant.Light 
            : ThemeVariant.Dark;
        SetTheme(newTheme);
    }

    private void UpdateHuskuiTheme()
    {
        if (Application.Current?.Styles is { } styles)
        {
            // Find and update the HuskuiTheme
            for (int i = 0; i < styles.Count; i++)
            {
                if (styles[i] is HuskuiTheme huskuiTheme)
                {
                    huskuiTheme.Accent = _currentAccent;
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
                var transparencyLevel = _currentBackground switch
                {
                    BackgroundMaterial.None => WindowTransparencyLevel.None,
                    BackgroundMaterial.Mica => WindowTransparencyLevel.Mica,
                    BackgroundMaterial.AcrylicBlur => WindowTransparencyLevel.AcrylicBlur,
                    BackgroundMaterial.Transparent => WindowTransparencyLevel.Transparent,
                    _ => WindowTransparencyLevel.None
                };

                mainWindow.TransparencyLevelHint = new[] { transparencyLevel };
            }
        }
    }
}
