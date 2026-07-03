using FluentIcons.Common;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views;

namespace Huskui.Gallery.Services;

public class MenuItemService
{
    private readonly List<NavigationItem> _allMenus = [];

    public List<NavigationItem> AllMenus => _allMenus;

    public void Initialize()
    {
        _allMenus.Clear();

        CreateControlsCategory();
        CreateContainersCategory();
        CreateOverlaysCategory();
        CreateLayoutCategory();
        CreateInputCategory();
        CreateNavigationCategory();
        CreateCollectionsCategory();
        CreateMediaCategory();
        CreateExtensionCategory();
        CreateDocumentsCategory();
    }

    private void CreateControlsCategory()
    {
        const string category = "Controls";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.Button,
                Content = new MenuItemVo
                {
                    Title = "Buttons",
                    Description = "Interactive button controls with various styles and states",
                    Tags = ["button", "click", "action"],
                },
                PageType = typeof(ButtonsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ChevronDown,
                Content = new MenuItemVo
                {
                    Title = "DropDownButtons",
                    Description = "Button controls with dropdown menus for additional actions",
                    Tags = ["dropdown", "button", "menu", "flyout", "actions"],
                },
                PageType = typeof(DropDownButtonsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Link,
                Content = new MenuItemVo
                {
                    Title = "HyperlinkButtons",
                    Description = "Link-style buttons for navigation and external references",
                    Tags = ["hyperlink", "link", "navigation", "url", "inline"],
                },
                PageType = typeof(HyperlinkButtonsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.RadioButton,
                Content = new MenuItemVo
                {
                    Title = "RadioButtons",
                    Description = "Single selection controls for mutually exclusive options",
                    Tags = ["radio", "button", "selection", "exclusive", "group"],
                },
                PageType = typeof(RadioButtonsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Info,
                Content = new MenuItemVo
                {
                    Title = "InfoBars",
                    Description = "Informational message components with different severity levels",
                    Tags = ["info", "message", "notification"],
                },
                PageType = typeof(InfoBarsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Tag,
                Content = new MenuItemVo
                {
                    Title = "Tags",
                    Description = "Small labels for categorization and metadata",
                    Tags = ["tag", "label", "category"],
                },
                PageType = typeof(TagsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Icons,
                Content = new MenuItemVo
                {
                    Title = "IconLabels",
                    Description =
                        "Combined icon and text labels for enhanced visual communication",
                    Tags = ["icon", "label", "text", "fluent"],
                },
                PageType = typeof(IconLabelsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Highlight,
                Content = new MenuItemVo
                {
                    Title = "HighlightBlocks",
                    Description =
                        "Emphasized content blocks for important information and callouts",
                    Tags = ["highlight", "text", "emphasis", "keyboard", "shortcut"],
                },
                PageType = typeof(HighlightBlocksPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Line,
                Content = new MenuItemVo
                {
                    Title = "Dividers",
                    Description =
                        "Visual separators for organizing and structuring content layout",
                    Tags = ["divider", "separator", "line", "section"],
                },
                PageType = typeof(DividersPage),
                Category = category,
            },
        ]);
    }

    private void CreateContainersCategory()
    {
        const string category = "Containers";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.RectangleLandscape,
                Content = new MenuItemVo
                {
                    Title = "Cards",
                    Description = "Container components for grouping and organizing content",
                    Tags = ["card", "container", "content"],
                },
                PageType = typeof(CardsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Group,
                Content = new MenuItemVo
                {
                    Title = "GroupBoxes",
                    Description =
                        "Labeled containers with header bar for grouping related content and forms",
                    Tags = ["groupbox", "group", "container", "header", "form", "section"],
                },
                PageType = typeof(GroupBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ArrowClockwise,
                Content = new MenuItemVo
                {
                    Title = "BusyContainers",
                    Description =
                        "Loading state containers with visual feedback for async operations",
                    Tags = ["busy", "loading", "container", "blur", "progress"],
                },
                PageType = typeof(BusyContainersPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.RectangleLandscape,
                Content = new MenuItemVo
                {
                    Title = "SkeletonContainers",
                    Description =
                        "Loading placeholders that mimic content structure during data fetching",
                    Tags = ["skeleton", "loading", "placeholder", "shimmer"],
                },
                PageType = typeof(SkeletonContainersPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.BranchFork,
                Content = new MenuItemVo
                {
                    Title = "SwitchContainer",
                    Description =
                        "Conditional content container that swaps rendered branches based on the current value",
                    Tags = ["switchcontainer", "switch", "case", "content", "state", "container"],
                },
                PageType = typeof(SwitchContainerPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.LayerDiagonal,
                Content = new MenuItemVo
                {
                    Title = "VariableContainer",
                    Description =
                        "Reusable content container whose wrapper can be fully customized through Template",
                    Tags = ["template", "contentcontrol", "switchcontainer", "container", "wrapper"],
                },
                PageType = typeof(VariableContainerPage),
                Category = category,
            },
        ]);
    }

    private void CreateOverlaysCategory()
    {
        const string category = "Overlays";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.Layer,
                Content = new MenuItemVo
                {
                    Title = "Flyouts",
                    Description = "Pop-up containers for displaying content.",
                    Tags = ["flyout", "popup", "menu", "overlay"],
                },
                PageType = typeof(FlyoutsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.TooltipQuote,
                Content = new MenuItemVo
                {
                    Title = "ToolTips",
                    Description = "Display informational tooltips on hover.",
                    Tags = ["tooltip", "tip", "info", "hover"],
                },
                PageType = typeof(ToolTipsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.SlideText,
                Content = new MenuItemVo
                {
                    Title = "Toasts",
                    Description = "Heavy-weight content viewers that slide up from the bottom",
                    Tags = ["toast", "content", "preview", "bottom", "heavy"],
                },
                PageType = typeof(ToastsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.RectangleLandscape,
                Content = new MenuItemVo
                {
                    Title = "Modals",
                    Description =
                        "Modal containers for complex user interactions and extended workflows",
                    Tags = ["modal", "long", "interaction", "settings", "profile"],
                },
                PageType = typeof(ModalsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Chat,
                Content = new MenuItemVo
                {
                    Title = "Dialogs",
                    Description =
                        "Modal dialogs for user input collection and binary decision making",
                    Tags = ["dialog", "confirmation", "input", "binary", "choice"],
                },
                PageType = typeof(DialogsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Alert,
                Content = new MenuItemVo
                {
                    Title = "Growls",
                    Description =
                        "Status feedback notifications for user awareness and system updates",
                    Tags = ["notification", "alert", "message", "status", "feedback"],
                },
                PageType = typeof(GrowlsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.PanelBottom,
                Content = new MenuItemVo
                {
                    Title = "Drawers",
                    Description =
                        "Floating drawer that can be dragged, resized, and collapsed",
                    Tags = ["drawer", "panel", "bottom", "resizable", "collapsible"],
                },
                PageType = typeof(DrawerPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.PanelRight,
                Content = new MenuItemVo
                {
                    Title = "Sidebars",
                    Description = "Sliding side panels for contextual content and quick actions",
                    Tags = ["sidebar", "panel", "overlay", "slide", "contextual"],
                },
                PageType = typeof(SidebarsPage),
                Category = category,
            },
        ]);
    }

    private void CreateLayoutCategory()
    {
        const string category = "Layout";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.LayoutRowTwoSplitBottom,
                Content = new MenuItemVo
                {
                    Title = "FlexWrapPanels",
                    Description =
                        "Flexible wrapping layout panels for responsive content arrangement",
                    Tags = ["flex", "wrap", "responsive", "dynamic", "panel"],
                },
                PageType = typeof(FlexWrapPanelsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.LayoutRowTwoSplitBottom,
                Content = new MenuItemVo
                {
                    Title = "ModalActionPanels",
                    Description =
                        "Platform-aware action button layout that pushes the default (IsDefault) button to the correct edge per platform",
                    Tags = ["modal", "action", "panel", "platform", "dialog", "footer", "layout"],
                },
                PageType = typeof(ModalActionPanelsPage),
                Category = category,
            },
        ]);
    }

    private void CreateInputCategory()
    {
        const string category = "Input";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.Search,
                Content = new MenuItemVo
                {
                    Title = "AutoCompleteBoxes",
                    Description =
                        "Search input controls with inline suggestions and text completion",
                    Tags = ["autocomplete", "search", "suggestion", "input", "lookup"],
                },
                PageType = typeof(AutoCompleteBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ToggleRight,
                Content = new MenuItemVo
                {
                    Title = "ToggleSwitches",
                    Description = "Binary toggle controls for on/off settings and preferences",
                    Tags = ["toggle", "switch", "boolean", "settings"],
                },
                PageType = typeof(ToggleSwitchesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ChevronUpDown,
                Content = new MenuItemVo
                {
                    Title = "NumericUpDowns",
                    Description =
                        "Numeric input controls with built-in step actions and Huskui field styling",
                    Tags = ["numericupdown", "number", "stepper", "input", "numeric"],
                },
                PageType = typeof(NumericUpDownsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Calendar,
                Content = new MenuItemVo
                {
                    Title = "DatePickers",
                    Description =
                        "Date selection controls using Avalonia's built-in picker with Huskui field styling",
                    Tags = ["datepicker", "date", "calendar", "picker", "input", "schedule"],
                },
                PageType = typeof(DatePickersPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Clock,
                Content = new MenuItemVo
                {
                    Title = "TimePickers",
                    Description =
                        "Time selection controls using Avalonia's built-in picker with Huskui field styling",
                    Tags = ["timepicker", "time", "picker", "input", "schedule"],
                },
                PageType = typeof(TimePickersPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.CalendarDate,
                Content = new MenuItemVo
                {
                    Title = "CalendarDatePicker",
                    Description =
                        "Date selection controls with inline calendar dropdown and Huskui field styling",
                    Tags =
                    [
                        "calendardatepicker",
                        "calendar",
                        "date",
                        "picker",
                        "input",
                        "dropdown",
                        "schedule",
                    ],
                },
                PageType = typeof(CalendarDatePickerPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.TextBulletListSquare,
                Content = new MenuItemVo
                {
                    Title = "TextBoxes",
                    Description = "Text input controls for single-line and multi-line text entry",
                    Tags = ["textbox", "input", "text", "validation", "form"],
                },
                PageType = typeof(TextBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Tag,
                Content = new MenuItemVo
                {
                    Title = "TagBoxes",
                    Description =
                        "Multi-select text inputs that combine inline tags with searchable suggestions",
                    Tags = ["tagbox", "tag", "input", "multi-select", "autocomplete"],
                },
                PageType = typeof(TagBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ChevronDown,
                Content = new MenuItemVo
                {
                    Title = "ComboBoxes",
                    Description = "Dropdown selection controls for choosing from predefined options",
                    Tags = ["combobox", "dropdown", "select", "picker"],
                },
                PageType = typeof(ComboBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.CheckboxChecked,
                Content = new MenuItemVo
                {
                    Title = "CheckBoxes",
                    Description = "Binary selection controls for multiple choice options",
                    Tags = ["checkbox", "check", "selection", "boolean", "three-state"],
                },
                PageType = typeof(CheckBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Star,
                Content = new MenuItemVo
                {
                    Title = "RatingControls",
                    Description = "Star-based rating controls for user feedback and reviews",
                    Tags = ["rating", "star", "review", "feedback", "score"],
                },
                PageType = typeof(RatingControlsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ArrowSync,
                Content = new MenuItemVo
                {
                    Title = "Sliders",
                    Description = "Range selection controls for continuous value adjustment",
                    Tags = ["slider", "range", "track", "value", "thumb"],
                },
                PageType = typeof(SlidersPage),
                Category = category,
            },
        ]);
    }

    private void CreateNavigationCategory()
    {
        const string category = "Navigation";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.Tab,
                Content = new MenuItemVo
                {
                    Title = "TabControls",
                    Description = "Tab containers for content organization",
                    Tags = ["list", "tab", "navigation", "vertical", "switcher"],
                },
                PageType = typeof(TabControlsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Navigation,
                Content = new MenuItemVo
                {
                    Title = "Frames",
                    Description = "Navigation containers for page transitions and routing",
                    Tags = ["frame", "navigation", "page", "transition"],
                },
                PageType = typeof(FramesPage),
                Category = category,
            },
        ]);
    }

    private void CreateCollectionsCategory()
    {
        const string category = "Collections";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.List,
                Content = new MenuItemVo
                {
                    Title = "ListBoxes",
                    Description =
                        "Vertical list containers with different configurations",
                    Tags = ["list", "box", "vertical", "content"],
                },
                PageType = typeof(ListBoxesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Tab,
                Content = new MenuItemVo
                {
                    Title = "TabStrips",
                    Description = "Horizontal tab containers for content organization",
                    Tags = ["list", "tab", "vertical", "switcher"],
                },
                PageType = typeof(TabStripsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ArrowSort,
                Content = new MenuItemVo
                {
                    Title = "PaginationControls",
                    Description = "Page navigation controls for browsing through paged data",
                    Tags = ["pagination", "page", "navigation", "data", "paging"],
                },
                PageType = typeof(PaginationControlsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Navigation,
                Content = new MenuItemVo
                {
                    Title = "StepControls",
                    Description = "Stepped navigation controls for multi-step workflows",
                    Tags = ["step", "navigation", "workflow", "wizard"],
                },
                PageType = typeof(StepControlsPage),
                Category = category,
            },
        ]);
    }

    private void CreateMediaCategory()
    {
    }

    private void CreateExtensionCategory()
    {
        const string category = "Extensions";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.BranchFork,
                Content = new MenuItemVo
                {
                    Title = "Mvvm",
                    Description =
                        "Overview of the Huskui.Avalonia.Mvvm extension for activation, lifecycle binding, and view state",
                    Tags = ["mvvm", "activation", "lifecycle", "state", "extension", "viewmodel"],
                },
                PageType = typeof(MvvmPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Code,
                Content = new MenuItemVo
                {
                    Title = "CodeViewer",
                    Description =
                        "Syntax-highlighted code presentation control from Huskui.Avalonia.Code extension library",
                    Tags = ["code", "viewer", "syntax", "highlight", "extension", "colorcode"],
                },
                PageType = typeof(CodeViewerPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ArrowSwap,
                Content = new MenuItemVo
                {
                    Title = "DiffView",
                    Description =
                        "Side-by-side difference viewer from Huskui.Avalonia.Code with DiffPlex-based computation and overview bar",
                    Tags = ["diff", "viewer", "difference", "side-by-side", "extension", "compare"],
                },
                PageType = typeof(DiffViewsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ZoomIn,
                Content = new MenuItemVo
                {
                    Title = "ZoomView",
                    Description =
                        "Bounded zoom &amp; pan container with minimap, scroll bars, and fit-to-content from Huskui.Avalonia.Code",
                    Tags = ["zoom", "pan", "minimap", "scroll", "viewport", "extension", "fit"],
                },
                PageType = typeof(ZoomViewsPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Document,
                Content = new MenuItemVo
                {
                    Title = "MarkdownViewer",
                    Description =
                        "Markdown rendering control from Huskui.Avalonia.Markdown extension library (not built-in)",
                    Tags = ["markdown", "viewer", "render", "document", "extension", "markdig"],
                },
                PageType = typeof(MarkdownViewerPage),
                Category = category,
            },
        ]);
    }

    private void CreateDocumentsCategory()
    {
        const string category = "Documents";

        _allMenus.AddRange(
        [
            new()
            {
                Icon = Symbol.DataHistogram,
                Content = new MenuItemVo
                {
                    Title = "Builtin Converters",
                    Description =
                        "Overview of Huskui converter patterns, families, and practical XAML usage",
                    Tags =
                    [
                        "converter",
                        "multibinding",
                        "relay",
                        "string",
                        "number",
                        "cornerradius",
                        "thickness",
                    ],
                },
                PageType = typeof(ConvertersPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Layer,
                Content = new MenuItemVo
                {
                    Title = "Overlay AppSurface Usage",
                    Description =
                        "How AppSurface should be hosted on desktop, browser, and mobile so Huskui overlays can render",
                    Tags = ["overlay", "appsurface", "appwindow", "browser", "mobile", "dialog", "modal"],
                },
                PageType = typeof(OverlayAppSurfacePage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.ColorBackground,
                Content = new MenuItemVo
                {
                    Title = "Resource Usage Notes",
                    Description =
                        "How Brush/CornerRadius resources and resource binding extensions work",
                    Tags =
                    [
                        "resource",
                        "brush",
                        "cornerradius",
                        "staticresource",
                        "dynamicresource",
                        "binding",
                    ],
                },
                PageType = typeof(ResourcesPage),
                Category = category,
            },
            new()
            {
                Icon = Symbol.Color,
                Content = new MenuItemVo
                {
                    Title = "Brush Resource Keys",
                    Description =
                        "Categorized reference for theme brush keys with live previews, copy buttons, and naming notes",
                    Tags =
                    ["brush", "resource", "theme", "color", "overlay", "foreground", "background"],
                },
                PageType = typeof(BrushResourceKeysPage),
                Category = category,
            },
        ]);
    }
}
