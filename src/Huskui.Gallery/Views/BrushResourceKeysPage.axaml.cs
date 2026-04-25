using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using Avalonia.Media;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class BrushResourceKeysPage : ControlPage
{
    public BrushResourceKeysPage()
    {
        InitializeComponent();
        Sections = CreateSections();
        DataContext = this;
    }

    public IReadOnlyList<BrushSection> Sections { get; }

    private async void CopyKeyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string key } button)
            return;

        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel?.Clipboard is null)
            return;

        await topLevel.Clipboard.SetTextAsync(key);

        var originalContent = button.Content;
        button.Content = "Copied";

        await Task.Delay(1200);

        if (Equals(button.Content, "Copied"))
            button.Content = originalContent;
    }

    private static IReadOnlyList<BrushSection> CreateSections() =>
        [
            new(
                "Shell Surfaces",
                "Top-level surfaces used by the app shell before component-specific control and overlay families come into play.",
                "These keys are the basic canvas layers for windows, flyouts, app layers, and cards. They are the right starting point when you need structure rather than semantic state.",
                [
                    CreateEntry(
                        "TransparentBrush",
                        "Transparent placeholder",
                        "Use when a template requires a brush but the layer should stay visually empty.",
                        "Source: TransparentColor"
                    ),
                    CreateEntry(
                        "FlyoutBackgroundBrush",
                        "Floating shell background",
                        "Used by flyouts, dialogs, sidebars, and other detached floating containers.",
                        "Source: FlyoutColor"
                    ),
                    CreateEntry(
                        "WindowBackgroundBrush",
                        "Window canvas background",
                        "The root app-window surface behind the main layer and card system.",
                        "Source: Gray3Color"
                    ),
                    CreateEntry(
                        "LayerBackgroundBrush",
                        "Primary app surface",
                        "The main layer that content pages and layouts typically sit on.",
                        "Source: LayerColor"
                    ),
                    CreateEntry(
                        "CardBackgroundBrush",
                        "Raised content background",
                        "For cards and inset surfaces that need separation from the main layer.",
                        "Source: CardColor"
                    ),
                ]
            ),
            new(
                "Overlay Surfaces",
                "Brushes for floating layers, scrims, and overlay-hosted interaction patterns.",
                "Overlay names are the ones that look the most unusual: Reversed flips to the Layover palette, Solid removes translucency for a fully opaque panel, Smoke is the dim curtain, and Mask is the heavier modal blocker.",
                [
                    CreateEntry(
                        "OverlayBackgroundBrush",
                        "Baseline overlay surface",
                        "Default overlay-host background when you want the standard translucent overlay look.",
                        "Source: Overlay1Color"
                    ),
                    CreateEntry(
                        "OverlayReversedBackgroundBrush",
                        "Polarity-flipped overlay surface",
                        "Use when an overlay needs to read against an already-overlay-like surface and the default polarity is not clear enough.",
                        "Source: Layover1Color",
                        "Reversed uses the Layover palette instead of Overlay to intentionally invert the overlay feel."
                    ),
                    CreateEntry(
                        "OverlayInteractiveBackgroundBrush",
                        "Interactive overlay surface",
                        "For clickable or hoverable rows and affordances living inside overlay content.",
                        "Source: InteractiveColor"
                    ),
                    CreateEntry(
                        "OverlayHalfBackgroundBrush",
                        "Lighter overlay step",
                        "Use for the softer of the two standard overlay intensities.",
                        "Source: Overlay1Color",
                        "Half is the lighter overlay elevation step."
                    ),
                    CreateEntry(
                        "OverlayFullBackgroundBrush",
                        "Stronger overlay step",
                        "Use when the overlay needs a more present surface than the Half step.",
                        "Source: Overlay2Color",
                        "Full is the stronger overlay elevation step."
                    ),
                    CreateEntry(
                        "OverlaySolidBackgroundBrush",
                        "Opaque overlay panel",
                        "For overlay content that should feel fully solid instead of visually blending with the backdrop.",
                        "Source: Gray1Color",
                        "Solid is the escape hatch when a floating surface must stop looking translucent."
                    ),
                    CreateEntry(
                        "OverlaySmokeBackgroundBrush",
                        "Backdrop dimmer",
                        "Use behind content to dim the scene without becoming the content surface itself.",
                        "Source: SmokeColor",
                        "Smoke is the curtain behind the overlay, not the overlay panel."
                    ),
                    CreateEntry(
                        "OverlayMaskBackgroundBrush",
                        "Heavy modal scrim",
                        "For stronger modal blocking layers that should read as a mask over the underlying app.",
                        "Source: Layover7Color",
                        "Mask is heavier than Smoke and is intended for blocking, not subtle dimming."
                    ),
                ]
            ),
            new(
                "Neutral Control Brushes",
                "The default control family for buttons, inputs, borders, and readable content on neutral surfaces.",
                "This family is intentionally pragmatic rather than perfectly step-based. Colors.axaml even calls out that the original palette mapping is not a strict numeric ladder, so treat these as role tokens first and numeric color steps second.",
                CreateControlFamilyEntries(
                    familyPrefix: "Control",
                    familyLabel: "neutral",
                    sourcePrefix: "Gray",
                    backgroundStep: "7",
                    interactiveStep: "5",
                    borderStep: "7",
                    interactiveBorderStep: "7",
                    translucentStep: "9",
                    foregroundStep: "12",
                    translucentForegroundStep: "11"
                )
            ),
            CreateVariantSection(
                title: "Accent Control Brushes",
                description: "Semantic control brushes for primary emphasis and brand-colored interaction.",
                namingNote: "Accent keeps the exact same naming grid as the neutral Control family. Swap to this family when the control needs to communicate primary emphasis without changing how the control is structured.",
                familyPrefix: "ControlAccent",
                familyLabel: "accent",
                sourcePrefix: "Accent"
            ),
            CreateVariantSection(
                title: "Success Control Brushes",
                description: "Semantic control brushes for positive outcomes, confirmations, and healthy states.",
                namingNote: "Success is the semantic green family. Use it when the control itself needs to communicate confirmation, completion, or healthy state rather than merely decorating surrounding content.",
                familyPrefix: "ControlSuccess",
                familyLabel: "success",
                sourcePrefix: "Success"
            ),
            CreateVariantSection(
                title: "Warning Control Brushes",
                description: "Semantic control brushes for cautionary but non-destructive interaction states.",
                namingNote: "Warning is for caution, attention, and reversible risk. It is the right fit when the user should slow down, but not necessarily stop.",
                familyPrefix: "ControlWarning",
                familyLabel: "warning",
                sourcePrefix: "Warning"
            ),
            CreateVariantSection(
                title: "Danger Control Brushes",
                description: "Semantic control brushes for destructive, irreversible, or critical interaction states.",
                namingNote: "Danger is the strongest semantic family. Reserve it for destructive actions or critical error states where visual urgency is part of the interaction contract.",
                familyPrefix: "ControlDanger",
                familyLabel: "danger",
                sourcePrefix: "Danger"
            ),
            new(
                "Foreground Helpers",
                "Foreground-only helpers for secondary emphasis, reversed contrast, and theme-aware extremes.",
                "Secondary lowers emphasis, Reversed flips contrast against stronger surfaces, and White/Black are semantic extremes that still map through theme tokens instead of promising literal raw white or black.",
                [
                    CreateEntry(
                        "ControlSecondaryForegroundBrush",
                        "Lower-emphasis foreground",
                        "Use for supporting text, helper copy, metadata, and less prominent icons.",
                        "Source: Gray10Color"
                    ),
                    CreateEntry(
                        "ControlReversedForegroundBrush",
                        "Inverted contrast foreground",
                        "Use on strong or dark surfaces where the default control foreground would not provide the intended contrast role.",
                        "Source: Gray1Color"
                    ),
                    CreateEntry(
                        "ControlWhiteForegroundBrush",
                        "Theme-aware white extreme",
                        "Use when you need the white-side semantic extreme for contrast-sensitive content.",
                        "Source: WhiteColor",
                        "This is theme-aware and may invert under dark theme setup instead of staying literal white."
                    ),
                    CreateEntry(
                        "ControlBlackForegroundBrush",
                        "Theme-aware black extreme",
                        "Use when you need the black-side semantic extreme for contrast-sensitive content.",
                        "Source: BlackColor",
                        "This is theme-aware and may invert under dark theme setup instead of staying literal black."
                    ),
                ]
            ),
        ];

    private static BrushSection CreateVariantSection(
        string title,
        string description,
        string namingNote,
        string familyPrefix,
        string familyLabel,
        string sourcePrefix
    ) =>
        new(
            title,
            description,
            namingNote,
            CreateControlFamilyEntries(
                familyPrefix,
                familyLabel,
                sourcePrefix,
                backgroundStep: "8",
                interactiveStep: "9",
                borderStep: "8",
                interactiveBorderStep: "9",
                translucentStep: "9",
                foregroundStep: "9",
                translucentForegroundStep: "11"
            )
        );

    private static IReadOnlyList<BrushEntry> CreateControlFamilyEntries(
        string familyPrefix,
        string familyLabel,
        string sourcePrefix,
        string backgroundStep,
        string interactiveStep,
        string borderStep,
        string interactiveBorderStep,
        string translucentStep,
        string foregroundStep,
        string translucentForegroundStep
    ) =>
        [
            CreateEntry(
                $"{familyPrefix}BackgroundBrush",
                "Resting background",
                $"Default {familyLabel} surface for controls in their resting state.",
                $"Source: {sourcePrefix}{backgroundStep}Color"
            ),
            CreateEntry(
                $"{familyPrefix}InteractiveBackgroundBrush",
                "Interactive background",
                $"Use when the {familyLabel} control surface is hoverable, pressable, or actively engaged.",
                $"Source: {sourcePrefix}{interactiveStep}Color"
            ),
            CreateEntry(
                $"{familyPrefix}BorderBrush",
                "Resting border",
                $"Default stroke for non-interactive or resting {familyLabel} control chrome.",
                $"Source: {sourcePrefix}{borderStep}Color"
            ),
            CreateEntry(
                $"{familyPrefix}InteractiveBorderBrush",
                "Interactive border",
                $"Use when the border itself needs to reflect hover, focus, selection, or active {familyLabel} state.",
                $"Source: {sourcePrefix}{interactiveBorderStep}Color"
            ),
            CreateEntry(
                $"{familyPrefix}TranslucentHalfBackgroundBrush",
                "Lighter translucent background",
                $"A softer {familyLabel} tint wash for static emphasis and subtle fills.",
                $"Source: {sourcePrefix}{translucentStep}Color at 10% opacity",
                "Translucent Half is intended as a static tint layer rather than a general-purpose animated interaction brush."
            ),
            CreateEntry(
                $"{familyPrefix}TranslucentFullBackgroundBrush",
                "Stronger translucent background",
                $"A stronger {familyLabel} tint wash when Half is too subtle.",
                $"Source: {sourcePrefix}{translucentStep}Color at 20% opacity",
                "Translucent Full is the denser companion to Half and is best kept for static washes and section highlighting."
            ),
            CreateEntry(
                $"{familyPrefix}ForegroundBrush",
                "Primary semantic foreground",
                $"Readable {familyLabel} text and icon color for the strongest semantic emphasis.",
                $"Source: {sourcePrefix}{foregroundStep}Color"
            ),
            CreateEntry(
                $"{familyPrefix}TranslucentForegroundBrush",
                "Softer semantic foreground",
                $"Supporting text and icon color when the main {familyLabel} foreground is visually too strong.",
                $"Source: {sourcePrefix}{translucentForegroundStep}Color"
            ),
        ];

    private static BrushEntry CreateEntry(
        string key,
        string role,
        string usage,
        string source,
        string? note = null
    ) => new(key, role, usage, source, note, ResolveBrush(key));

    private static IBrush ResolveBrush(string key)
    {
        if (
            Application.Current?.TryFindResource(
                key,
                Application.Current.ActualThemeVariant,
                out var resource
            ) == true
            && resource is IBrush brush
        )
            return brush;

        return Brushes.Transparent;
    }
}

public sealed record BrushSection(
    string Title,
    string Description,
    string NamingNote,
    IReadOnlyList<BrushEntry> Items
)
{
    public string CountLabel => $"{Items.Count} keys";
}

public sealed record BrushEntry(
    string Key,
    string Role,
    string Usage,
    string Source,
    string? Note,
    IBrush PreviewBrush
);
