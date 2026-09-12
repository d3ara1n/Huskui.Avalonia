using System;
using System.Linq;
using Avalonia.Styling;
using Huskui.Avalonia;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.Models;

/// <summary>
///     User-friendly theme variant item
/// </summary>
public record ThemeVariantItem(string DisplayName, string Description, ThemeVariant Variant)
{
    public static readonly ThemeVariantItem[] All =
    [
        new(
            "Follow System",
            "Automatically switch between light and dark based on system settings",
            ThemeVariant.Default
        ),
        new("Light Theme", "Light color scheme with bright backgrounds", ThemeVariant.Light),
        new("Dark Theme", "Dark color scheme with dark backgrounds", ThemeVariant.Dark),
    ];
}

/// <summary>
///     User-friendly accent color item
/// </summary>
public record AccentColorItem(string DisplayName, string Description, AccentColor Color)
{
    public static readonly AccentColorItem[] All =
    [
        new("System", "Use the accent color from the system settings", AccentColor.System),
        new("Ember", "Muted amber glow, signature of the Ember palette", AccentColor.Ember),
        .. Enum.GetValues<AccentColor>()
               .Where(c => c is not (AccentColor.System or AccentColor.Ember))
               .Select(c => new AccentColorItem(c.ToString(), $"Pairs well with {RecommendedGray(c)} gray", c)),
    ];

    private static GrayColor RecommendedGray(AccentColor color) =>
        color switch
        {
            AccentColor.Neutral => GrayColor.Neutral,
            AccentColor.Lime => GrayColor.Olive,
            AccentColor.Sky or AccentColor.Cyan or AccentColor.Iris or AccentColor.Indigo
                or AccentColor.Blue => GrayColor.Slate,
            AccentColor.Teal or AccentColor.Jade or AccentColor.Mint or AccentColor.Green
                or AccentColor.Grass => GrayColor.Sage,
            AccentColor.Yellow or AccentColor.Gold or AccentColor.Amber or AccentColor.Orange
                or AccentColor.Bronze or AccentColor.Brown => GrayColor.Sand,
            _ => GrayColor.Mauve,
        };
}

/// <summary>
///     User-friendly gray scale item
/// </summary>
public record GrayColorItem(string DisplayName, string Description, GrayColor Color)
{
    public static readonly GrayColorItem[] All =
    [
        new("Neutral", "Pure gray without any color tint", GrayColor.Neutral),
        new("Slate", "Cool blue-tinted gray, pairs with blue/cyan accents", GrayColor.Slate),
        new("Mauve", "Purple-tinted gray, pairs with pink/plum accents", GrayColor.Mauve),
        new("Sage", "Green-tinted gray, pairs with green/teal accents", GrayColor.Sage),
        new("Olive", "Yellow-green tinted gray, pairs with lime/grass accents", GrayColor.Olive),
        new("Sand", "Warm sandy gray, pairs with amber/orange accents", GrayColor.Sand),
        new("Warm", "Deep warm charcoal, cozy and amber-friendly", GrayColor.Warm),
    ];
}

/// <summary>
///     User-friendly curated palette item (gray + accent pair)
/// </summary>
public record PaletteItem(string DisplayName, string Description, ColorPalette Palette)
{
    public static readonly PaletteItem[] All =
    [
        new("Neutral", "Colorless baseline, content first", ColorPalette.Neutral),
        new("Ocean", "Cool slate gray with classic blue", ColorPalette.Ocean),
        new("Forest", "Calm sage gray with natural green", ColorPalette.Forest),
        new("Sakura", "Soft mauve gray with mellow plum", ColorPalette.Sakura),
        new("Ember", "Deep warm charcoal with muted amber glow", ColorPalette.Ember),
        new("Honey", "Bright sand gray with sunny amber", ColorPalette.Honey),
        new("Matcha", "Mossy olive gray with fresh lime", ColorPalette.Matcha),
    ];
}

/// <summary>
///     User-friendly corner style item
/// </summary>
public record CornerStyleItem(string DisplayName, string Description, CornerStyle Style)
{
    public static readonly CornerStyleItem[] All =
    [
        new("Large", "Extra radius for enthusiast", CornerStyle.Large),
        new("Normal", "Rounded corners with smooth curves", CornerStyle.Normal),
        new("None", "Sharp corners with no rounding", CornerStyle.None),
    ];
}

/// <summary>
///     User-friendly background material item
/// </summary>
public record BackgroundMaterialItem(
    string DisplayName,
    string Description,
    BackgroundMaterial Material
)
{
    public static readonly BackgroundMaterialItem[] All =
    [
        new(
            "Solid",
            "Standard opaque background with no transparency effects",
            BackgroundMaterial.None
        ),
        new(
            "Mica",
            "Modern translucent material with subtle texture (Windows 11+)",
            BackgroundMaterial.Mica
        ),
        new(
            "Acrylic Blur",
            "Blurred translucent background with depth effect",
            BackgroundMaterial.AcrylicBlur
        ),
        new(
            "Transparent",
            "Fully transparent background showing desktop behind",
            BackgroundMaterial.Transparent
        ),
    ];
}
