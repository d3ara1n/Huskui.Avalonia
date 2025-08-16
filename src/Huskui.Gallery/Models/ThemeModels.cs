using Avalonia.Styling;
using Huskui.Avalonia;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.Models;

/// <summary>
/// User-friendly theme variant item
/// </summary>
public record ThemeVariantItem(string DisplayName, string Description, ThemeVariant Variant)
{
    public static readonly ThemeVariantItem[] All = 
    [
        new("Follow System", "Automatically switch between light and dark based on system settings", ThemeVariant.Default),
        new("Light Theme", "Light color scheme with bright backgrounds", ThemeVariant.Light),
        new("Dark Theme", "Dark color scheme with dark backgrounds", ThemeVariant.Dark)
    ];
}

/// <summary>
/// User-friendly accent color item
/// </summary>
public record AccentColorItem(string DisplayName, string Description, AccentColor Color)
{
    public static readonly AccentColorItem[] All = 
    [
        new("Blue", "Classic blue accent color", AccentColor.Blue),
        new("Green", "Fresh green accent color", AccentColor.Green),
        new("Orange", "Vibrant orange accent color", AccentColor.Orange),
        new("Pink", "Playful pink accent color", AccentColor.Pink),
        new("Purple", "Rich purple accent color", AccentColor.Purple),
        new("Red", "Bold red accent color", AccentColor.Red),
        new("Teal", "Calm teal accent color", AccentColor.Teal),
        new("Yellow", "Bright yellow accent color", AccentColor.Yellow)
    ];
}

/// <summary>
/// User-friendly background material item
/// </summary>
public record BackgroundMaterialItem(string DisplayName, string Description, BackgroundMaterial Material)
{
    public static readonly BackgroundMaterialItem[] All =
    [
        new("Solid", "Standard opaque background with no transparency effects", BackgroundMaterial.None),
        new("Mica", "Modern translucent material with subtle texture (Windows 11+)", BackgroundMaterial.Mica),
        new("Acrylic Blur", "Blurred translucent background with depth effect", BackgroundMaterial.AcrylicBlur),
        new("Transparent", "Fully transparent background showing desktop behind", BackgroundMaterial.Transparent)
    ];
}
