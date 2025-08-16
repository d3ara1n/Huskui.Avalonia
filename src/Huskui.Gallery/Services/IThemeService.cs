using System;
using Avalonia.Styling;
using Huskui.Avalonia;

namespace Huskui.Gallery.Services;

/// <summary>
/// Background material types for the application (maps to Avalonia TransparencyLevelHint)
/// </summary>
public enum BackgroundMaterial
{
    /// <summary>
    /// No transparency - solid background
    /// </summary>
    None,

    /// <summary>
    /// Mica effect - modern translucent material (Windows 11+)
    /// </summary>
    Mica,

    /// <summary>
    /// Acrylic effect - blurred translucent background
    /// </summary>
    AcrylicBlur,

    /// <summary>
    /// Transparent background
    /// </summary>
    Transparent
}

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
    /// Gets the current background material
    /// </summary>
    BackgroundMaterial CurrentBackground { get; }

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
    /// Sets the background material
    /// </summary>
    /// <param name="background">The background material to set</param>
    void SetBackground(BackgroundMaterial background);

    /// <summary>
    /// Toggles between light and dark theme
    /// </summary>
    void ToggleTheme();
}
