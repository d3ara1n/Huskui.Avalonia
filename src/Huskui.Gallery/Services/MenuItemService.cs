using FluentIcons.Common;
using Huskui.Gallery.Models;
using Huskui.Gallery.Views;

namespace Huskui.Gallery.Services;

public class MenuItemService
{
    private readonly List<MenuItemVo> _allMenus = [];

    public List<MenuItemVo> AllMenus => _allMenus;

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
            new MenuItemVo
            {
                Icon = Symbol.Button,
                PageType = typeof(ButtonsPage),
                Category = category,
                Title = "Buttons",
                Description = "Interactive button controls with various styles and states",
                Tags = ["button", "click", "action"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ChevronDown,
                PageType = typeof(DropDownButtonsPage),
                Category = category,
                Title = "DropDownButtons",
                Description = "Button controls with dropdown menus for additional actions",
                Tags = ["dropdown", "button", "menu", "flyout", "actions"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Link,
                PageType = typeof(HyperlinkButtonsPage),
                Category = category,
                Title = "HyperlinkButtons",
                Description = "Link-style buttons for navigation and external references",
                Tags = ["hyperlink", "link", "navigation", "url", "inline"],
            },
            new MenuItemVo
            {
                Icon = Symbol.RadioButton,
                PageType = typeof(RadioButtonsPage),
                Category = category,
                Title = "RadioButtons",
                Description = "Single selection controls for mutually exclusive options",
                Tags = ["radio", "button", "selection", "exclusive", "group"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Info,
                PageType = typeof(InfoBarsPage),
                Category = category,
                Title = "InfoBars",
                Description = "Informational message components with different severity levels",
                Tags = ["info", "message", "notification"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Tag,
                PageType = typeof(TagsPage),
                Category = category,
                Title = "Tags",
                Description = "Small labels for categorization and metadata",
                Tags = ["tag", "label", "category"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Icons,
                PageType = typeof(IconLabelsPage),
                Category = category,
                Title = "IconLabels",
                Description =
                    "Combined icon and text labels for enhanced visual communication",
                Tags = ["icon", "label", "text", "fluent"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Highlight,
                PageType = typeof(HighlightBlocksPage),
                Category = category,
                Title = "HighlightBlocks",
                Description =
                    "Emphasized content blocks for important information and callouts",
                Tags = ["highlight", "text", "emphasis", "keyboard", "shortcut"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Line,
                PageType = typeof(DividersPage),
                Category = category,
                Title = "Dividers",
                Description =
                    "Visual separators for organizing and structuring content layout",
                Tags = ["divider", "separator", "line", "section"],
            },
        ]);
    }

    private void CreateContainersCategory()
    {
        const string category = "Containers";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.RectangleLandscape,
                PageType = typeof(CardsPage),
                Category = category,
                Title = "Cards",
                Description = "Container components for grouping and organizing content",
                Tags = ["card", "container", "content"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Group,
                PageType = typeof(GroupBoxesPage),
                Category = category,
                Title = "GroupBoxes",
                Description =
                    "Labeled containers with header bar for grouping related content and forms",
                Tags = ["groupbox", "group", "container", "header", "form", "section"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ArrowClockwise,
                PageType = typeof(BusyContainersPage),
                Category = category,
                Title = "BusyContainers",
                Description =
                    "Loading state containers with visual feedback for async operations",
                Tags = ["busy", "loading", "container", "blur", "progress"],
            },
            new MenuItemVo
            {
                Icon = Symbol.RectangleLandscape,
                PageType = typeof(SkeletonContainersPage),
                Category = category,
                Title = "SkeletonContainers",
                Description =
                    "Loading placeholders that mimic content structure during data fetching",
                Tags = ["skeleton", "loading", "placeholder", "shimmer"],
            },
            new MenuItemVo
            {
                Icon = Symbol.BranchFork,
                PageType = typeof(SwitchContainerPage),
                Category = category,
                Title = "SwitchContainer",
                Description =
                    "Conditional content container that swaps rendered branches based on the current value",
                Tags = ["switchcontainer", "switch", "case", "content", "state", "container"],
            },
            new MenuItemVo
            {
                Icon = Symbol.LayerDiagonal,
                PageType = typeof(VariableContainerPage),
                Category = category,
                Title = "VariableContainer",
                Description =
                    "Reusable content container whose wrapper can be fully customized through Template",
                Tags = ["template", "contentcontrol", "switchcontainer", "container", "wrapper"],
            },
        ]);
    }

    private void CreateOverlaysCategory()
    {
        const string category = "Overlays";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.Layer,
                PageType = typeof(FlyoutsPage),
                Category = category,
                Title = "Flyouts",
                Description = "Pop-up containers for displaying content.",
                Tags = ["flyout", "popup", "menu", "overlay"],
            },
            new MenuItemVo
            {
                Icon = Symbol.TooltipQuote,
                PageType = typeof(ToolTipsPage),
                Category = category,
                Title = "ToolTips",
                Description = "Display informational tooltips on hover.",
                Tags = ["tooltip", "tip", "info", "hover"],
            },
            new MenuItemVo
            {
                Icon = Symbol.SlideText,
                PageType = typeof(ToastsPage),
                Category = category,
                Title = "Toasts",
                Description = "Heavy-weight content viewers that slide up from the bottom",
                Tags = ["toast", "content", "preview", "bottom", "heavy"],
            },
            new MenuItemVo
            {
                Icon = Symbol.RectangleLandscape,
                PageType = typeof(ModalsPage),
                Category = category,
                Title = "Modals",
                Description =
                    "Modal containers for complex user interactions and extended workflows",
                Tags = ["modal", "long", "interaction", "settings", "profile"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Chat,
                PageType = typeof(DialogsPage),
                Category = category,
                Title = "Dialogs",
                Description =
                    "Modal dialogs for user input collection and binary decision making",
                Tags = ["dialog", "confirmation", "input", "binary", "choice"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Alert,
                PageType = typeof(GrowlsPage),
                Category = category,
                Title = "Growls",
                Description =
                    "Status feedback notifications for user awareness and system updates",
                Tags = ["notification", "alert", "message", "status", "feedback"],
            },
            new MenuItemVo
            {
                Icon = Symbol.PanelBottom,
                PageType = typeof(DrawerPage),
                Category = category,
                Title = "Drawers",
                Description =
                    "Floating drawer that can be dragged, resized, and collapsed",
                Tags = ["drawer", "panel", "bottom", "resizable", "collapsible"],
            },
            new MenuItemVo
            {
                Icon = Symbol.PanelRight,
                PageType = typeof(SidebarsPage),
                Category = category,
                Title = "Sidebars",
                Description = "Sliding side panels for contextual content and quick actions",
                Tags = ["sidebar", "panel", "overlay", "slide", "contextual"],
            },
        ]);
    }

    private void CreateLayoutCategory()
    {
        const string category = "Layout";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.LayoutRowTwoSplitBottom,
                PageType = typeof(FlexWrapPanelsPage),
                Category = category,
                Title = "FlexWrapPanels",
                Description =
                    "Flexible wrapping layout panels for responsive content arrangement",
                Tags = ["flex", "wrap", "responsive", "dynamic", "panel"],
            },
            new MenuItemVo
            {
                Icon = Symbol.LayoutRowTwoSplitBottom,
                PageType = typeof(ModalActionPanelsPage),
                Category = category,
                Title = "ModalActionPanels",
                Description =
                    "Platform-aware action button layout that pushes the default (IsDefault) button to the correct edge per platform",
                Tags = ["modal", "action", "panel", "platform", "dialog", "footer", "layout"],
            },
        ]);
    }

    private void CreateInputCategory()
    {
        const string category = "Input";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.Search,
                PageType = typeof(AutoCompleteBoxesPage),
                Category = category,
                Title = "AutoCompleteBoxes",
                Description =
                    "Search input controls with inline suggestions and text completion",
                Tags = ["autocomplete", "search", "suggestion", "input", "lookup"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ToggleRight,
                PageType = typeof(ToggleSwitchesPage),
                Category = category,
                Title = "ToggleSwitches",
                Description = "Binary toggle controls for on/off settings and preferences",
                Tags = ["toggle", "switch", "boolean", "settings"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ChevronUpDown,
                PageType = typeof(NumericUpDownsPage),
                Category = category,
                Title = "NumericUpDowns",
                Description =
                    "Numeric input controls with built-in step actions and Huskui field styling",
                Tags = ["numericupdown", "number", "stepper", "input", "numeric"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Calendar,
                PageType = typeof(DatePickersPage),
                Category = category,
                Title = "DatePickers",
                Description =
                    "Date selection controls using Avalonia's built-in picker with Huskui field styling",
                Tags = ["datepicker", "date", "calendar", "picker", "input", "schedule"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Clock,
                PageType = typeof(TimePickersPage),
                Category = category,
                Title = "TimePickers",
                Description =
                    "Time selection controls using Avalonia's built-in picker with Huskui field styling",
                Tags = ["timepicker", "time", "picker", "input", "schedule"],
            },
            new MenuItemVo
            {
                Icon = Symbol.CalendarDate,
                PageType = typeof(CalendarDatePickerPage),
                Category = category,
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
            new MenuItemVo
            {
                Icon = Symbol.TextBulletListSquare,
                PageType = typeof(TextBoxesPage),
                Category = category,
                Title = "TextBoxes",
                Description = "Text input controls for single-line and multi-line text entry",
                Tags = ["textbox", "input", "text", "validation", "form"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Tag,
                PageType = typeof(TagBoxesPage),
                Category = category,
                Title = "TagBoxes",
                Description =
                    "Multi-select text inputs that combine inline tags with searchable suggestions",
                Tags = ["tagbox", "tag", "input", "multi-select", "autocomplete"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ChevronDown,
                PageType = typeof(ComboBoxesPage),
                Category = category,
                Title = "ComboBoxes",
                Description = "Dropdown selection controls for choosing from predefined options",
                Tags = ["combobox", "dropdown", "select", "picker"],
            },
            new MenuItemVo
            {
                Icon = Symbol.CheckboxChecked,
                PageType = typeof(CheckBoxesPage),
                Category = category,
                Title = "CheckBoxes",
                Description = "Binary selection controls for multiple choice options",
                Tags = ["checkbox", "check", "selection", "boolean", "three-state"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Star,
                PageType = typeof(RatingControlsPage),
                Category = category,
                Title = "RatingControls",
                Description = "Star-based rating controls for user feedback and reviews",
                Tags = ["rating", "star", "review", "feedback", "score"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ArrowSync,
                PageType = typeof(SlidersPage),
                Category = category,
                Title = "Sliders",
                Description = "Range selection controls for continuous value adjustment",
                Tags = ["slider", "range", "track", "value", "thumb"],
            },
        ]);
    }

    private void CreateNavigationCategory()
    {
        const string category = "Navigation";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.Tab,
                PageType = typeof(TabControlsPage),
                Category = category,
                Title = "TabControls",
                Description = "Tab containers for content organization",
                Tags = ["list", "tab", "navigation", "vertical", "switcher"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Navigation,
                PageType = typeof(FramesPage),
                Category = category,
                Title = "Frames",
                Description = "Navigation containers for page transitions and routing",
                Tags = ["frame", "navigation", "page", "transition"],
            },
        ]);
    }

    private void CreateCollectionsCategory()
    {
        const string category = "Collections";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.List,
                PageType = typeof(ListBoxesPage),
                Category = category,
                Title = "ListBoxes",
                Description =
                    "Vertical list containers with different configurations",
                Tags = ["list", "box", "vertical", "content"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Tab,
                PageType = typeof(TabStripsPage),
                Category = category,
                Title = "TabStrips",
                Description = "Horizontal tab containers for content organization",
                Tags = ["list", "tab", "vertical", "switcher"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ArrowSort,
                PageType = typeof(PaginationControlsPage),
                Category = category,
                Title = "PaginationControls",
                Description = "Page navigation controls for browsing through paged data",
                Tags = ["pagination", "page", "navigation", "data", "paging"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Navigation,
                PageType = typeof(StepControlsPage),
                Category = category,
                Title = "StepControls",
                Description = "Stepped navigation controls for multi-step workflows",
                Tags = ["step", "navigation", "workflow", "wizard"],
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
            new MenuItemVo
            {
                Icon = Symbol.BranchFork,
                PageType = typeof(MvvmPage),
                Category = category,
                Title = "Mvvm",
                Description =
                    "Overview of the Huskui.Avalonia.Mvvm extension for activation, lifecycle binding, and view state",
                Tags = ["mvvm", "activation", "lifecycle", "state", "extension", "viewmodel"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Code,
                PageType = typeof(CodeViewerPage),
                Category = category,
                Title = "CodeViewer",
                Description =
                    "Syntax-highlighted code presentation control from Huskui.Avalonia.Code extension library",
                Tags = ["code", "viewer", "syntax", "highlight", "extension", "colorcode"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ArrowSwap,
                PageType = typeof(DiffViewsPage),
                Category = category,
                Title = "DiffView",
                Description =
                    "Side-by-side difference viewer from Huskui.Avalonia.Code with DiffPlex-based computation and overview bar",
                Tags = ["diff", "viewer", "difference", "side-by-side", "extension", "compare"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ZoomIn,
                PageType = typeof(ZoomViewsPage),
                Category = category,
                Title = "ZoomView",
                Description =
                    "Bounded zoom &amp; pan container with minimap, scroll bars, and fit-to-content from Huskui.Avalonia.Code",
                Tags = ["zoom", "pan", "minimap", "scroll", "viewport", "extension", "fit"],
            },
            new MenuItemVo
            {
                Icon = Symbol.Document,
                PageType = typeof(MarkdownViewerPage),
                Category = category,
                Title = "MarkdownViewer",
                Description =
                    "Markdown rendering control from Huskui.Avalonia.Markdown extension library (not built-in)",
                Tags = ["markdown", "viewer", "render", "document", "extension", "markdig"],
            },
        ]);
    }

    private void CreateDocumentsCategory()
    {
        const string category = "Documents";

        _allMenus.AddRange(
        [
            new MenuItemVo
            {
                Icon = Symbol.DataHistogram,
                PageType = typeof(ConvertersPage),
                Category = category,
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
            new MenuItemVo
            {
                Icon = Symbol.Layer,
                PageType = typeof(OverlayAppSurfacePage),
                Category = category,
                Title = "Overlay AppSurface Usage",
                Description =
                    "How AppSurface should be hosted on desktop, browser, and mobile so Huskui overlays can render",
                Tags = ["overlay", "appsurface", "appwindow", "browser", "mobile", "dialog", "modal"],
            },
            new MenuItemVo
            {
                Icon = Symbol.ColorBackground,
                PageType = typeof(ResourcesPage),
                Category = category,
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
            new MenuItemVo
            {
                Icon = Symbol.Color,
                PageType = typeof(BrushResourceKeysPage),
                Category = category,
                Title = "Brush Resource Keys",
                Description =
                    "Categorized reference for theme brush keys with live previews, copy buttons, and naming notes",
                Tags =
                ["brush", "resource", "theme", "color", "overlay", "foreground", "background"],
            },
        ]);
    }
}
