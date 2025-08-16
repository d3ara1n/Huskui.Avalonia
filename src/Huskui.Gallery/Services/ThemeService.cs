using System;
using Avalonia;
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
    private CornerStyle _currentCorner = CornerStyle.Normal;

    public ThemeVariant CurrentTheme => _currentTheme;
    public AccentColor CurrentAccent => _currentAccent;
    public CornerStyle CurrentCorner => _currentCorner;

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

    public void SetCorner(CornerStyle corner)
    {
        if (_currentCorner == corner) return;

        _currentCorner = corner;
        UpdateHuskuiTheme();
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
                    huskuiTheme.Corner = _currentCorner;
                    break;
                }
            }
        }
    }
}
