using System.Collections.ObjectModel;
using System.Linq;
using FluentIcons.Common;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views;

namespace Huskui.Gallery.Services;

/// <summary>
///     Default implementation of IGalleryService
/// </summary>
public class GalleryService : IGalleryService
{
    #region IGalleryService Members

    public ObservableCollection<GalleryCategory> Categories { get; } = [];
    public ObservableCollection<GalleryItem> AllItems { get; } = [];

    public void Initialize()
    {
        Categories.Clear();
        AllItems.Clear();

        // Create categories and items
        CreateControlsCategory();
        CreateContainersCategory();
        CreateOverlaysCategory();
        CreateLayoutCategory();
        CreateInputCategory();
        CreateNavigationCategory();
        CreateCollectionsCategory();
        CreateMediaCategory();

        foreach (var item in Categories.SelectMany(c => c.Items))
        {
            AllItems.Add(item);
        }
    }

    #endregion

    private void CreateControlsCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Controls",
            Description = "Interactive UI controls, buttons, and display components",
            Icon = Symbol.ControlButton,
            Items =
            [
                new()
                {
                    Title = "Buttons",
                    Description = "Interactive button controls with various styles and states",
                    Icon = Symbol.Button,
                    PageType = typeof(ButtonsPage),
                    Tags = ["button", "click", "action"]
                },
                new()
                {
                    Title = "DropDownButtons",
                    Description = "Button controls with dropdown menus for additional actions",
                    Icon = Symbol.ChevronDown,
                    PageType = typeof(DropDownButtonsPage),
                    Tags = ["dropdown", "button", "menu", "flyout", "actions"]
                },
                new()
                {
                    Title = "HyperlinkButtons",
                    Description = "Link-style buttons for navigation and external references",
                    Icon = Symbol.Link,
                    PageType = typeof(HyperlinkButtonsPage),
                    Tags = ["hyperlink", "link", "navigation", "url", "inline"]
                },
                new()
                {
                    Title = "RadioButtons",
                    Description = "Single selection controls for mutually exclusive options",
                    Icon = Symbol.RadioButton,
                    PageType = typeof(RadioButtonsPage),
                    Tags = ["radio", "button", "selection", "exclusive", "group"]
                },
                new()
                {
                    Title = "InfoBars",
                    Description = "Informational message components with different severity levels",
                    Icon = Symbol.Info,
                    PageType = typeof(InfoBarsPage),
                    Tags = ["info", "message", "notification"]
                },
                new()
                {
                    Title = "Tags",
                    Description = "Small labels for categorization and metadata",
                    Icon = Symbol.Tag,
                    PageType = typeof(TagsPage),
                    Tags = ["tag", "label", "category"]
                },
                new()
                {
                    Title = "IconLabels",
                    Description = "Combined icon and text labels for enhanced visual communication",
                    Icon = Symbol.Icons,
                    PageType = typeof(IconLabelsPage),
                    Tags = ["icon", "label", "text", "fluent"]
                },
                new()
                {
                    Title = "HighlightBlocks",
                    Description = "Emphasized content blocks for important information and callouts",
                    Icon = Symbol.Highlight,
                    PageType = typeof(HighlightBlocksPage),
                    Tags = ["highlight", "text", "emphasis", "keyboard", "shortcut"]
                },
                new()
                {
                    Title = "Dividers",
                    Description = "Visual separators for organizing and structuring content layout",
                    Icon = Symbol.Line,
                    PageType = typeof(DividersPage),
                    Tags = ["divider", "separator", "line", "section"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateContainersCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Containers",
            Description = "Container components for layout and content organization",
            Icon = Symbol.RectangleLandscape,
            Items =
            [
                new()
                {
                    Title = "Cards",
                    Description = "Container components for grouping and organizing content",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(CardsPage),
                    Tags = ["card", "container", "content"]
                },
                new()
                {
                    Title = "BusyContainers",
                    Description = "Loading state containers with visual feedback for async operations",
                    Icon = Symbol.ArrowClockwise,
                    PageType = typeof(BusyContainersPage),
                    Tags = ["busy", "loading", "container", "blur", "progress"]
                },
                new()
                {
                    Title = "SkeletonContainers",
                    Description = "Loading placeholders that mimic content structure during data fetching",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(SkeletonContainersPage),
                    Tags = ["skeleton", "loading", "placeholder", "shimmer"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateOverlaysCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Overlays",
            Description = "Modal overlays, dialogs, toasts, and notifications",
            Icon = Symbol.Layer,
            Items =
            [
                new()
                {
                    Title = "Flyouts",
                    Description = "Pop-up containers for displaying content.",
                    Icon = Symbol.Layer,
                    PageType = typeof(FlyoutsPage),
                    Tags = ["flyout", "popup", "menu", "overlay"]
                },
                new()
                {
                    Title = "ToolTips",
                    Description = "Display informational tooltips on hover.",
                    Icon = Symbol.TooltipQuote,
                    PageType = typeof(ToolTipsPage),
                    Tags = ["tooltip", "tip", "info", "hover"]
                },
                new()
                {
                    Title = "Toasts",
                    Description = "Heavy-weight content viewers that slide up from the bottom",
                    Icon = Symbol.SlideText,
                    PageType = typeof(ToastsPage),
                    Tags = ["toast", "content", "preview", "bottom", "heavy"]
                },
                new()
                {
                    Title = "Modals",
                    Description = "Modal containers for complex user interactions and extended workflows",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(ModalsPage),
                    Tags = ["modal", "long", "interaction", "settings", "profile"]
                },
                new()
                {
                    Title = "Dialogs",
                    Description = "Modal dialogs for user input collection and binary decision making",
                    Icon = Symbol.Chat,
                    PageType = typeof(DialogsPage),
                    Tags = ["dialog", "confirmation", "input", "binary", "choice"]
                },
                new()
                {
                    Title = "Growls",
                    Description = "Status feedback notifications for user awareness and system updates",
                    Icon = Symbol.Alert,
                    PageType = typeof(GrowlsPage),
                    Tags = ["notification", "alert", "message", "status", "feedback"]
                },
                new()
                {
                    Title = "Drawers (WIP)",
                    Description = "Floating drawer that can be dragged, resized, and collapsed",
                    Icon = Symbol.PanelBottom,
                    PageType = typeof(DrawerPage),
                    Tags = ["drawer", "panel", "bottom", "resizable", "collapsible"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateLayoutCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Layout",
            Description = "Layout containers and panels",
            Icon = Symbol.LayoutRowTwoSplitTop,
            Items =
            [
                new()
                {
                    Title = "FlexWrapPanels",
                    Description = "Flexible wrapping layout panels for responsive content arrangement",
                    Icon = Symbol.LayoutRowTwoSplitBottom,
                    PageType = typeof(FlexWrapPanelsPage),
                    Tags = ["flex", "wrap", "responsive", "dynamic", "panel"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateInputCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Input",
            Description = "Input controls and forms",
            Icon = Symbol.TextBoxSettings,
            Items =
            [
                new()
                {
                    Title = "ToggleSwitches",
                    Description = "Binary toggle controls for on/off settings and preferences",
                    Icon = Symbol.ToggleRight,
                    PageType = typeof(ToggleSwitchesPage),
                    Tags = ["toggle", "switch", "boolean", "settings"]
                },
                new()
                {
                    Title = "TextBoxes",
                    Description = "Text input controls for single-line and multi-line text entry",
                    Icon = Symbol.TextBulletListSquare,
                    PageType = typeof(TextBoxesPage),
                    Tags = ["textbox", "input", "text", "validation", "form"]
                },
                new()
                {
                    Title = "ComboBoxes",
                    Description = "Dropdown selection controls for choosing from predefined options",
                    Icon = Symbol.ChevronDown,
                    PageType = typeof(ComboBoxesPage),
                    Tags = ["combobox", "dropdown", "select", "picker"]
                },
                new()
                {
                    Title = "CheckBoxes",
                    Description = "Binary selection controls for multiple choice options",
                    Icon = Symbol.CheckboxChecked,
                    PageType = typeof(CheckBoxesPage),
                    Tags = ["checkbox", "check", "selection", "boolean", "three-state"]
                },
                new()
                {
                    Title = "RatingControls",
                    Description = "Star-based rating controls for user feedback and reviews",
                    Icon = Symbol.Star,
                    PageType = typeof(RatingControlsPage),
                    Tags = ["rating", "star", "review", "feedback", "score"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateNavigationCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Navigation",
            Description = "Navigation and menu components",
            Icon = Symbol.Navigation,
            Items =
            [
                new()
                {
                    Title = "TabControls",
                    Description = "Tab containers for content organization",
                    Icon = Symbol.Tab,
                    PageType = typeof(TabControlsPage),
                    Tags = ["list", "tab", "navigation", "vertical", "switcher"]
                },
                new()
                {
                    Title = "Frames",
                    Description = "Navigation containers for page transitions and routing",
                    Icon = Symbol.Navigation,
                    PageType = typeof(FramesPage),
                    Tags = ["frame", "navigation", "page", "transition"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateCollectionsCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Collections",
            Description = "Data display and collection controls",
            Icon = Symbol.DataHistogram,
            Items =
            [
                new()
                {
                    Title = "ListBoxes",
                    Description = "Vertical list containers with different configurations",
                    Icon = Symbol.List,
                    PageType = typeof(ListBoxesPage),
                    Tags = ["list", "box", "vertical", "content"]
                },
                new()
                {
                    Title = "TabStrips",
                    Description = "Horizontal tab containers for content organization",
                    Icon = Symbol.Tab,
                    PageType = typeof(TabStripsPage),
                    Tags = ["list", "tab", "vertical", "switcher"]
                },
                new()
                {
                    Title = "StepControls",
                    Description = "Stepped navigation controls for multi-step workflows",
                    Icon = Symbol.Navigation,
                    PageType = typeof(StepControlsPage),
                    Tags = ["step", "navigation", "workflow", "wizard"]
                }
            ]
        };

        Categories.Add(category);
    }

    private void CreateMediaCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Media", Description = "Media and graphics components", Icon = Symbol.Image, Items = []
        };

        Categories.Add(category);
    }
}
