using System;
using System.Collections.ObjectModel;
using System.Linq;
using FluentIcons.Common;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views.Controls;
using Huskui.Gallery.Views.Layout;
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
            Description = "Basic UI controls and components",
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
                    Title = "Cards",
                    Description = "Container components for grouping content",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(CardsPage),
                    Category = "Controls",
                    Tags = ["card", "container", "content"]
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
                    Title = "Busy Containers",
                    Description = "Loading state containers with blur effects",
                    Icon = Symbol.ArrowClockwise,
                    PageType = typeof(BusyContainersPage),
                    Category = "Controls",
                    Tags = ["busy", "loading", "container", "blur", "progress"]
                },
                new()
                {
                    Title = "Skeleton Containers",
                    Description = "Skeleton placeholders for loading content",
                    Icon = Symbol.RectangleLandscape,
                    PageType = typeof(SkeletonContainersPage),
                    Category = "Controls",
                    Tags = ["skeleton", "loading", "placeholder", "shimmer"]
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
                    Title = "Grids",
                    Description = "Grid layout examples and patterns",
                    Icon = Symbol.Grid,
                    PageType = typeof(GridsPage),
                    Category = "Layout",
                    Tags = ["grid", "layout", "responsive"]
                },
                new()
                {
                    Title = "Stack Panels",
                    Description = "Vertical and horizontal stacking",
                    Icon = Symbol.LayoutRowTwoSplitTop,
                    PageType = typeof(StackPanelsPage),
                    Category = "Layout",
                    Tags = ["stack", "panel", "vertical", "horizontal"]
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
                }
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