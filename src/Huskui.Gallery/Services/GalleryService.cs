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
                    Title = "Text Inputs",
                    Description = "Text boxes and input fields",
                    Icon = Symbol.Textbox,
                    PageType = typeof(TextInputsPage),
                    Category = "Input",
                    Tags = ["textbox", "input", "text", "form"]
                },
                new()
                {
                    Title = "Toggles",
                    Description = "Switches, checkboxes, and radio buttons",
                    Icon = Symbol.ToggleLeft,
                    PageType = typeof(TogglesPage),
                    Category = "Input",
                    Tags = ["toggle", "switch", "checkbox", "radio"]
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
            Name = "Media",
            Description = "Media and graphics components",
            Icon = Symbol.Image,
            Items = []
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
        foreach (var item in AllItems.Where(item => item.Category.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
        {
            results.Add(item);
        }
        return results;
    }
}
