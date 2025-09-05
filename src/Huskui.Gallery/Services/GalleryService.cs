using System;
using System.Collections.ObjectModel;
using System.Linq;
using FluentIcons.Common;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views.Containers;
using Huskui.Gallery.Views.Controls;
using Huskui.Gallery.Views.Input;
using Huskui.Gallery.Views.Layout;
using Huskui.Gallery.Views.Overlays;

namespace Huskui.Gallery.Services
{
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
            CreateContainerCategory();
            CreateOverlaysCategory();
            CreateLayoutCategory();
            CreateInputCategory();
            CreateNavigationCategory();
            CreateDataCategory();
            CreateMediaCategory();
        }

        public ObservableCollection<GalleryItem> SearchItems(string searchQuery)
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
            {
                return AllItems;
            }

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
                        Category = "Controls",
                        Tags = ["button", "click", "action"]
                    },
                    new()
                    {
                        Title = "DropDownButtons",
                        Description = "Button controls with dropdown menus for additional actions",
                        Icon = Symbol.ChevronDown,
                        PageType = typeof(DropDownButtonsPage),
                        Category = "Controls",
                        Tags = ["dropdown", "button", "menu", "flyout", "actions"]
                    },
                    new()
                    {
                        Title = "HyperlinkButtons",
                        Description = "Link-style buttons for navigation and external references",
                        Icon = Symbol.Link,
                        PageType = typeof(HyperlinkButtonsPage),
                        Category = "Controls",
                        Tags = ["hyperlink", "link", "navigation", "url", "inline"]
                    },
                    new()
                    {
                        Title = "RadioButtons",
                        Description = "Single selection controls for mutually exclusive options",
                        Icon = Symbol.RadioButton,
                        PageType = typeof(RadioButtonsPage),
                        Category = "Input",
                        Tags = ["radio", "button", "selection", "exclusive", "group"]
                    },
                    new()
                    {
                        Title = "InfoBars",
                        Description = "Informational message components with different severity levels",
                        Icon = Symbol.Info,
                        PageType = typeof(InfoBarsPage),
                        Category = "Controls",
                        Tags = ["info", "message", "notification"]
                    },
                    new()
                    {
                        Title = "Tags",
                        Description = "Small labels for categorization and metadata",
                        Icon = Symbol.Tag,
                        PageType = typeof(TagsPage),
                        Category = "Controls",
                        Tags = ["tag", "label", "category"]
                    },
                    new()
                    {
                        Title = "IconLabels",
                        Description = "Combined icon and text labels for enhanced visual communication",
                        Icon = Symbol.Icons,
                        PageType = typeof(IconLabelsPage),
                        Category = "Controls",
                        Tags = ["icon", "label", "text", "fluent"]
                    },
                    new()
                    {
                        Title = "HighlightBlocks",
                        Description = "Emphasized content blocks for important information and callouts",
                        Icon = Symbol.Highlight,
                        PageType = typeof(HighlightBlocksPage),
                        Category = "Controls",
                        Tags = ["highlight", "text", "emphasis", "keyboard", "shortcut"]
                    },
                    new()
                    {
                        Title = "Dividers",
                        Description = "Visual separators for organizing and structuring content layout",
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
                        Description = "Container components for grouping and organizing content",
                        Icon = Symbol.RectangleLandscape,
                        PageType = typeof(CardsPage),
                        Category = "Container",
                        Tags = ["card", "container", "content"]
                    },
                    new()
                    {
                        Title = "BusyContainers",
                        Description = "Loading state containers with visual feedback for async operations",
                        Icon = Symbol.ArrowClockwise,
                        PageType = typeof(BusyContainersPage),
                        Category = "Container",
                        Tags = ["busy", "loading", "container", "blur", "progress"]
                    },
                    new()
                    {
                        Title = "SkeletonContainers",
                        Description = "Loading placeholders that mimic content structure during data fetching",
                        Icon = Symbol.RectangleLandscape,
                        PageType = typeof(SkeletonContainersPage),
                        Category = "Container",
                        Tags = ["skeleton", "loading", "placeholder", "shimmer"]
                    },
                    new()
                    {
                        Title = "ListBoxes",
                        Description = "Vertical list containers with different configurations",
                        Icon = Symbol.List,
                        PageType = typeof(ListBoxesPage),
                        Category = "Container",
                        Tags = ["list", "box", "vertical", "content"]
                    },
                    new()
                    {
                        Title = "TabStrips",
                        Description = "Horizontal tab containers for content organization",
                        Icon = Symbol.Tab,
                        PageType = typeof(TabStripsPage),
                        Category = "Container",
                        Tags = ["list", "tab", "vertical", "switcher"]
                    },
                    new()
                    {
                        Title = "TabControls",
                        Description = "Tab containers for content organization",
                        Icon = Symbol.Tab,
                        PageType = typeof(TabControlsPage),
                        Category = "Container",
                        Tags = ["list", "tab", "vertical", "switcher"]
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
                        Title = "Flyouts",
                        Description = "Pop-up containers for displaying content.",
                        Icon = Symbol.Layer,
                        PageType = typeof(FlyoutsPage),
                        Category = "Overlays",
                        Tags = ["flyout", "popup", "menu", "overlay"]
                    },
                    new()
                    {
                        Title = "ToolTips",
                        Description = "Display informational tooltips on hover.",
                        Icon = Symbol.TooltipQuote,
                        PageType = typeof(ToolTipsPage),
                        Category = "Overlays",
                        Tags = ["tooltip", "tip", "info", "hover"]
                    },
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
                        Description = "Modal containers for complex user interactions and extended workflows",
                        Icon = Symbol.RectangleLandscape,
                        PageType = typeof(ModalsPage),
                        Category = "Overlays",
                        Tags = ["modal", "long", "interaction", "settings", "profile"]
                    },
                    new()
                    {
                        Title = "Dialogs",
                        Description = "Modal dialogs for user input collection and binary decision making",
                        Icon = Symbol.Chat,
                        PageType = typeof(DialogsPage),
                        Category = "Overlays",
                        Tags = ["dialog", "confirmation", "input", "binary", "choice"]
                    },
                    new()
                    {
                        Title = "Notifications",
                        Description = "Status feedback notifications for user awareness and system updates",
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
                        Title = "FlexWrapPanels",
                        Description = "Flexible wrapping layout panels for responsive content arrangement",
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
                        Title = "ToggleSwitches",
                        Description = "Binary toggle controls for on/off settings and preferences",
                        Icon = Symbol.ToggleRight,
                        PageType = typeof(ToggleSwitchesPage),
                        Category = "Input",
                        Tags = ["toggle", "switch", "boolean", "settings"]
                    },
                    new()
                    {
                        Title = "TextBoxes",
                        Description = "Text input controls for single-line and multi-line text entry",
                        Icon = Symbol.TextBulletListSquare,
                        PageType = typeof(TextBoxesPage),
                        Category = "Input",
                        Tags = ["textbox", "input", "text", "validation", "form"]
                    },
                    new()
                    {
                        Title = "ComboBoxes",
                        Description = "Dropdown selection controls for choosing from predefined options",
                        Icon = Symbol.ChevronDown,
                        PageType = typeof(ComboBoxesPage),
                        Category = "Input",
                        Tags = ["combobox", "dropdown", "select", "picker"]
                    },
                    new()
                    {
                        Title = "CheckBoxes",
                        Description = "Binary selection controls for multiple choice options",
                        Icon = Symbol.CheckboxChecked,
                        PageType = typeof(CheckBoxesPage),
                        Category = "Input",
                        Tags = ["checkbox", "check", "selection", "boolean", "three-state"]
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
    }
}
