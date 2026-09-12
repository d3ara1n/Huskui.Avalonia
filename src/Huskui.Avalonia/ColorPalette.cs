namespace Huskui.Avalonia;

/// <summary>
///     A curated color palette: a tinted gray scale paired with a harmonized accent.
///     Apply one via <see cref="HuskuiTheme.ApplyPalette" /> — it presets Gray and Accent,
///     either of which can still be overridden individually afterwards.
/// </summary>
public record ColorPalette(GrayColor Gray, AccentColor Accent)
{
    /// <summary>
    ///     Pure Neutral gray with a Neutral accent — colorless baseline for content-first apps.
    /// </summary>
    public static ColorPalette Neutral { get; } = new(GrayColor.Neutral, AccentColor.Neutral);

    /// <summary>
    ///     Slate gray with a Blue accent — cool and focused, for tools and dashboards.
    /// </summary>
    public static ColorPalette Ocean { get; } = new(GrayColor.Slate, AccentColor.Blue);

    /// <summary>
    ///     Sage gray with a Green accent — calm and natural.
    /// </summary>
    public static ColorPalette Forest { get; } = new(GrayColor.Sage, AccentColor.Green);

    /// <summary>
    ///     Mauve gray with a Plum accent — soft and mellow.
    /// </summary>
    public static ColorPalette Sakura { get; } = new(GrayColor.Mauve, AccentColor.Plum);

    /// <summary>
    ///     Warm gray with an Ember accent — deep charcoal warmed by amber, the signature dark look.
    /// </summary>
    public static ColorPalette Ember { get; } = new(GrayColor.Warm, AccentColor.Ember);

    /// <summary>
    ///     Sand gray with an Amber accent — bright and sunny.
    /// </summary>
    public static ColorPalette Honey { get; } = new(GrayColor.Sand, AccentColor.Amber);

    /// <summary>
    ///     Olive gray with a Lime accent — fresh chartreuse on mossy gray.
    /// </summary>
    public static ColorPalette Matcha { get; } = new(GrayColor.Olive, AccentColor.Lime);
}
