using System;
using System.Collections.ObjectModel;
using System.Linq;
using FluentIcons.Common;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views.Controls;
using Huskui.Gallery.Views.Layout;
using Huskui.Gallery.Views.Overlays;
using Huskui.Gallery.Views.Input;

namespace Huskui.Gallery.Services;

/// <summary>
/// Default implementation of IGalleryService
/// </summary>
public class GalleryService : IGalleryService
{
    public ObservableCollection<GalleryCategory> Categories { get; } = [];
    public ObservableCollection<GalleryItem> AllItems { get; } = [];

    public void Initialize()
    {
        Categories.Clear();
        AllItems.Clear();

        // Create categories and items
        CreateControlsCategory();
        CreateContainerCategory();
        CreateOverlaysCategory();
        CreateLayoutCategory();
        CreateInputCategory();
        CreateNavigationCategory();
        CreateDataCategory();
        CreateMediaCategory();
    }

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
                    Description = "Various button styles and states",
                    Icon = Symbol.Button,
                    PageType = typeof(ButtonsPage),
                    Category = "Controls",
                    Tags = ["button", "click", "action"]
                },
                new()
                {
                    Title = "DropDownButtons",
                    Description = "Dropdown buttons with flyout menus",
                    Icon = Symbol.ChevronDown,
                    PageType = typeof(DropDownButtonsPage),
                    Category = "Controls",
                    Tags = ["dropdown", "button", "menu", "flyout", "actions"]
                },
                new()
                {
                    Title = "HyperlinkButtons",
                    Description = "Navigation links with different themes for inline and standalone use",
                    Icon = Symbol.Link,
                    PageType = typeof(HyperlinkButtonsPage),
                    Category = "Controls",
                    Tags = ["hyperlink", "link", "navigation", "url", "inline"]
                },

                new()
                {
                    Title = "Info Bars",
                    Description = "Informational message components",
                    Icon = Symbol.Info,
                    PageType = typeof(InfoBarsPage),
                    Category = "Controls",
                    Tags = ["info", "message", "notification"]
                },
                new()
                {
                    Title = "Tags",
                    Description = "Small labels for categorization",
                    Icon = Symbol.Tag,
                    PageType = typeof(TagsPage),
                    Category = "Controls",
                    Tags = ["tag", "label", "category"]
                },
                new()
                {
                    Title = "Icon Labels",
                    Description = "Icon and text combinations",
                    Icon = Symbol.Icons,
                    PageType = typeof(IconLabelsPage),
                    Category = "Controls",
                    Tags = ["icon", "label", "text", "fluent"]
                },
                new()
                {
                    Title = "Highlight Blocks",
                    Description = "Inline text highlighting for emphasis",
                    Icon = Symbol.Highlight,
                    PageType = typeof(HighlightBlocksPage),
                    Category = "Controls",
                    Tags = ["highlight", "text", "emphasis", "keyboard", "shortcut"]
                },

                new()
                {
                    Title = "Dividers",
                    Description = "Visual separators for content sections",
                    Icon = Symbol.Line,
                    PageType = typeof(DividersPage),
                    Category = "Controls",
                    Tags = ["divider", "separator", "line", "section"]
                }
            ]
        };

        Categories.Add(category);
        foreach (var item in category.Items)
        {
            AllItems.Add(item);
        }
    }

    private void CreateContainerCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Container",
            Description = "Container components for layout and content organization",
            Icon = Symbol.RectangleLandscape,
            Items =
            [
                new()
                {
                    Title = "Cards",
                    Description = "Container components for grouping content",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(CardsPage),
                    Category = "Container",
                    Tags = ["card", "container", "content"]
                },
                new()
                {
                    Title = "Busy Containers",
                    Description = "Loading state containers with blur effects",
                    Icon = Symbol.ArrowClockwise,
                    PageType = typeof(BusyContainersPage),
                    Category = "Container",
                    Tags = ["busy", "loading", "container", "blur", "progress"]
                },
                new()
                {
                    Title = "Skeleton Containers",
                    Description = "Skeleton placeholders for loading content",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(SkeletonContainersPage),
                    Category = "Container",
                    Tags = ["skeleton", "loading", "placeholder", "shimmer"]
                }
            ]
        };

        Categories.Add(category);
        foreach (var item in category.Items)
        {
            AllItems.Add(item);
        }
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
                    Title = "Toasts",
                    Description = "Heavy-weight content viewers that slide up from the bottom",
                    Icon = Symbol.SlideText,
                    PageType = typeof(ToastsPage),
                    Category = "Overlays",
                    Tags = ["toast", "content", "preview", "bottom", "heavy"]
                },
                new()
                {
                    Title = "Modals",
                    Description = "Long interaction containers that cannot be dismissed externally",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(ModalsPage),
                    Category = "Overlays",
                    Tags = ["modal", "long", "interaction", "settings", "profile"]
                },
                new()
                {
                    Title = "Dialogs",
                    Description = "Binary choice dialogs for user input and confirmation",
                    Icon = Symbol.Chat,
                    PageType = typeof(DialogsPage),
                    Category = "Overlays",
                    Tags = ["dialog", "confirmation", "input", "binary", "choice"]
                },
                new()
                {
                    Title = "Notifications",
                    Description = "Notification items with different severity levels and actions",
                    Icon = Symbol.Alert,
                    PageType = typeof(NotificationsPage),
                    Category = "Overlays",
                    Tags = ["notification", "alert", "message", "status", "feedback"]
                }
            ]
        };

        Categories.Add(category);
        foreach (var item in category.Items)
        {
            AllItems.Add(item);
        }
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
                    Title = "FlexWrap Panels",
                    Description = "Flexible wrapping layout with dynamic width distribution",
                    Icon = Symbol.LayoutRowTwoSplitBottom,
                    PageType = typeof(FlexWrapPanelsPage),
                    Category = "Layout",
                    Tags = ["flex", "wrap", "responsive", "dynamic", "panel"]
                }
            ]
        };

        Categories.Add(category);
        foreach (var item in category.Items)
        {
            AllItems.Add(item);
        }
    }

    private void CreateInputCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Input",
            Description = "Input controls and forms",
            Icon = Symbol.TextboxSettings,
            Items =
            [
                new()
                {
                    Title = "Toggle Switches",
                    Description = "Modern toggle switches with styling",
                    Icon = Symbol.ToggleRight,
                    PageType = typeof(ToggleSwitchesPage),
                    Category = "Input",
                    Tags = ["toggle", "switch", "boolean", "settings"]
                },
                new()
                {
                    Title = "TextBoxes",
                    Description = "Text input controls with validation",
                    Icon = Symbol.TextBulletListSquare,
                    PageType = typeof(TextBoxesPage),
                    Category = "Input",
                    Tags = ["textbox", "input", "text", "validation", "form"]
                },
                new()
                {
                    Title = "ComboBoxes",
                    Description = "Dropdown selection controls",
                    Icon = Symbol.ChevronDown,
                    PageType = typeof(ComboBoxesPage),
                    Category = "Input",
                    Tags = ["combobox", "dropdown", "select", "picker"]
                },
                new()
                {
                    Title = "CheckBoxes",
                    Description = "Checkbox controls with three-state support",
                    Icon = Symbol.CheckboxChecked,
                    PageType = typeof(CheckBoxesPage),
                    Category = "Input",
                    Tags = ["checkbox", "check", "selection", "boolean", "three-state"]
                },
                new()
                {
                    Title = "RadioButtons",
                    Description = "Radio button controls for exclusive selection",
                    Icon = Symbol.RadioButton,
                    PageType = typeof(RadioButtonsPage),
                    Category = "Input",
                    Tags = ["radio", "button", "selection", "exclusive", "group"]
                },

            ]
        };

        Categories.Add(category);
        foreach (var item in category.Items)
        {
            AllItems.Add(item);
        }
    }

    private void CreateNavigationCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Navigation",
            Description = "Navigation and menu components",
            Icon = Symbol.Navigation,
            Items = []
        };

        Categories.Add(category);
        foreach (var item in category.Items)
        {
            AllItems.Add(item);
        }
    }

    private void CreateDataCategory()
    {
        var category = new GalleryCategory
        {
            Name = "Data",
            Description = "Data display and collection controls",
            Icon = Symbol.DataHistogram,
            Items = []
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

    public ObservableCollection<GalleryItem> SearchItems(string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return AllItems;

        var results = new ObservableCollection<GalleryItem>();
        foreach (var item in AllItems.Where(item => item.MatchesSearch(searchQuery)))
        {
            results.Add(item);
        }

        return results;
    }

    public ObservableCollection<GalleryItem> GetItemsByCategory(string categoryName)
    {
        var results = new ObservableCollection<GalleryItem>();
        foreach (var item in AllItems.Where(item => item.Category.Equals(categoryName,
                                                                         StringComparison.OrdinalIgnoreCase)))
        {
            results.Add(item);
        }

        return results;
    }
}