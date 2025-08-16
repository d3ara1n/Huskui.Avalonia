using System;
using Avalonia.Styling;
using Huskui.Avalonia;

namespace Huskui.Gallery.Services;

/// <summary>
/// Service for managing application themes
/// </summary>
public interface IThemeService
{
    /// <summary>
    /// Gets the current theme variant
    /// </summary>
    ThemeVariant CurrentTheme { get; }

    /// <summary>
    /// Gets the current accent color
    /// </summary>
    AccentColor CurrentAccent { get; }

    /// <summary>
    /// Gets the current corner style
    /// </summary>
    CornerStyle CurrentCorner { get; }

    /// <summary>
    /// Event raised when theme changes
    /// </summary>
    event EventHandler? ThemeChanged;

    /// <summary>
    /// Sets the theme variant
    /// </summary>
    /// <param name="theme">The theme variant to set</param>
    void SetTheme(ThemeVariant theme);

    /// <summary>
    /// Sets the accent color
    /// </summary>
    /// <param name="accent">The accent color to set</param>
    void SetAccent(AccentColor accent);

    /// <summary>
    /// Sets the corner style
    /// </summary>
    /// <param name="corner">The corner style to set</param>
    void SetCorner(CornerStyle corner);

    /// <summary>
    /// Toggles between light and dark theme
    /// </summary>
    void ToggleTheme();
}
