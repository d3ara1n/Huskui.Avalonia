# Building applications with Huskui.Avalonia

This is a design and usage guide for agents building applications with Huskui.Avalonia. Read it before choosing controls, composing pages, or customizing a Huskui interface. It describes the library's public usage patterns; the consuming application owns its business logic, architecture, and coding conventions.

Huskui provides a Radix-inspired visual system, themes for standard Avalonia controls, and additional controls for application navigation, content, feedback, and overlays. Build from those components and their public properties. Use this guide to choose a pattern, then consult the linked example or API for that pattern's details.

The guide follows the source revision shipped alongside it. The current source targets `net8.0` and `net10.0` and references Avalonia `12.1.0`. For a NuGet application, check its resolved package versions before adapting examples. Links below point to the repository's `main` branch; use the corresponding release revision when working with an older package. The resource dictionaries and public APIs at that revision define exact values and behavior.

## 1. Start with the application shell

Add `Huskui.Avalonia` to an Avalonia application. Install optional packages only for the features needed; see [Extensions](#8-optional-extensions-and-mvvm).

```sh
dotnet add package Huskui.Avalonia
```

Load `HuskuiTheme` in `App.axaml`. It supplies the library's resources and standard control themes; the Gallery uses it as the base theme without an additional `FluentTheme`.

```xml
<Application
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
    x:Class="MyApp.App"
    RequestedThemeVariant="Default">
    <Application.Styles>
        <husk:HuskuiTheme Accent="Ember" Gray="Warm" Corner="Normal" />
    </Application.Styles>
</Application>
```

`Accent`, `Gray`, and `Corner` are independent settings. `Ember` and `Warm` are an example palette, not a required brand identity. `RequestedThemeVariant="Default"` follows the system; `Light` and `Dark` explicitly select a theme. These are Avalonia theme variants, separate from Huskui's accent and gray palettes.

Use `husk:AppWindow` for a desktop window. It already contains an `AppSurface` with the overlay hosts. Its code-behind must derive from `Huskui.Avalonia.Controls.AppWindow`; create that window through the application's normal Avalonia desktop lifetime setup.

```xml
<husk:AppWindow
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:husk="https://github.com/d3ara1n/Huskui.Avalonia"
    x:Class="MyApp.MainWindow"
    Title="Workspace"
    Width="960"
    Height="640">
    <Grid Margin="24" RowDefinitions="Auto,*" RowSpacing="16">
        <TextBlock Text="Workspace" FontSize="{StaticResource ExtraLargeFontSize}" />
        <husk:Card Grid.Row="1">
            <TextBlock Text="Choose a project to get started." />
        </husk:Card>
    </Grid>
</husk:AppWindow>
```

For a single-view application, or an existing plain `Window`, wrap the application content in `husk:AppSurface` to provide the same overlay infrastructure. Add a surface at the intended application boundary; an ordinary page inside `AppWindow` already has access to one. Nested surfaces create separate host scopes.

Standard Avalonia controls retain their usual names: `<Button>`, `<TextBox>`, `<ListBox>`, `<TableView>`. Additional controls use the library namespace: `<husk:Card>`, `<husk:Page>`, `<husk:Dialog>`. The C# namespace for core custom controls is `Huskui.Avalonia.Controls`.

Reference: [Gallery application styles](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/App.axaml), [desktop shell](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery.Desktop/MainWindow.axaml), [single-view shell](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/MainView.axaml).

## 2. Visual foundations

### Color and surface roles

Use a coherent neutral foundation and an accent for emphasis, selection, and primary actions. Success, warning, and danger communicate meaning. Preserve that distinction when applying an application's branding.

Huskui uses 12-step color scales with light and dark variants. Step 9 is the base accent color, but application UI normally consumes semantic brushes rather than selecting scale steps individually.

| Resource | Role |
| --- | --- |
| `WindowBackgroundBrush` | Application window background |
| `LayerBackgroundBrush` | A secondary content layer |
| `CardBackgroundBrush` | Grouped content surfaces; already used by `Card` |
| `FlyoutBackgroundBrush` | Popup and flyout surfaces |
| `ControlForegroundBrush` | Main readable text |
| `ControlSecondaryForegroundBrush` | Secondary metadata and hints; check legibility at the chosen size |
| `ControlBorderBrush` | Neutral boundaries |
| `ControlAccentForegroundBrush` | Accent emphasis in text or icons |
| `ControlAccentTranslucentHalfBackgroundBrush` | Subtle accent-tinted surface |
| `ControlSuccessForegroundBrush`, `ControlWarningForegroundBrush`, `ControlDangerForegroundBrush` | Semantic feedback |

Use the built-in color variants for controls that expose them. Their themes choose the foreground/background pairing and interaction states. When drawing an application-specific surface, reuse the semantic resources:

```xml
<Border
    Padding="16"
    Background="{DynamicResource CardBackgroundBrush}"
    BorderBrush="{DynamicResource ControlBorderBrush}"
    BorderThickness="1"
    CornerRadius="{StaticResource LargeCornerRadius}">
    <TextBlock
        Text="Changes are saved locally."
        Foreground="{DynamicResource ControlForegroundBrush}" />
</Border>
```

The built-in brush objects use dynamic color references internally. `StaticResource` can reuse those brushes, as the library templates do; `DynamicResource` also follows replacement of the brush resource itself. Use `DynamicResource` for application resources that will be replaced at runtime.

For new semantic application colors, define matching light and dark resources. A fixed white background or a copied accent hex value will not follow the library theme. Full definitions: [Colors.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Themes/Colors.axaml).

### Typography and icons

Use a small hierarchy appropriate to application UI: page heading, section heading, body, and supporting text. The application chooses its font family and language coverage; the Gallery's bundled fonts are application assets, not an automatic dependency of Huskui controls.

| Role | Starting resource |
| --- | --- |
| Body and ordinary control text | `MediumFontSize` |
| Section heading | `LargeFontSize` |
| Page heading | `ExtraLargeFontSize` |
| Compact metadata | `SmallFontSize`, only when readable for the target audience |
| Emphasis | `ControlStrongFontWeight` |

These are starting roles, not a claim that every size is suitable for every platform. Adjust the application's typography for text scaling, localization, and touch use. Use wrapping for explanatory text and deliberate trimming for compact labels.

`husk:IconLabel` combines a Fluent icon and text. Its `Icon` property is a `FluentIcons.Common.Symbol`, not an arbitrary string or image URI. Verify the symbol name in the installed FluentIcons version. For example:

```xml
<Button Classes="Primary">
    <husk:IconLabel Icon="Home" Text="Home" />
</Button>
```

Give icon-only actions an accessible name and a discoverable explanation. State and validation messages need text in addition to color or an icon.

Reference: [font resources](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Themes/FontSize.axaml), [IconLabel examples](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/IconLabelsPage.axaml).

### Shape, depth, and motion

Use `SmallCornerRadius`, `MediumCornerRadius`, `LargeCornerRadius`, or `FullCornerRadius` through **`StaticResource`**. These keys already forward dynamically to the active corner preset. `HuskuiTheme.Corner` selects `None`, `Normal`, or `Large`. A converter over a corner resource uses Huskui's `StaticResourceBinding` extension; consult an existing example before adding one.

Use `Card` to group related content and the built-in overlay controls for temporary surfaces. Let spacing, borders, and surface colors establish hierarchy; add shadows when they clarify elevation. The controls already provide hover, pressed, focus, and transition styling.

Additional animations should use the named duration resources from [Basics.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Themes/Basics.axaml), such as `ControlFasterAnimationDuration` and `ControlNormalAnimationDuration`. Keep motion tied to state changes and task progress. The application owns any reduced-motion preference and its integration; do not assume custom animations inherit that policy automatically.

## 3. Choose the right customization layer

| Need | Mechanism | Example |
| --- | --- | --- |
| Change the overall palette or geometry | `HuskuiTheme` settings | `Accent="Ember" Gray="Warm" Corner="Large"` |
| Select an alternative control appearance | `Theme` with a named `ControlTheme` | `Theme="{StaticResource OutlineButtonTheme}"` |
| Select a supported color or size variant | PascalCase `Classes` | `Classes="Primary Small"` |
| Set content, layout, or an application-specific adjustment | Public properties, bindings, and scoped styles | `Padding`, `IsEnabled`, `Command` |
| Reflect a runtime state | Control properties and framework behavior | `IsChecked`, `IsBusy`, focus, pointer state |

For buttons, the default theme, `OutlineButtonTheme`, and `GhostButtonTheme` are different appearances. `Primary`, `Success`, `Warning`, and `Danger` are color variants; `Small` and `Large` are size variants.

```xml
<StackPanel Orientation="Horizontal" Spacing="8">
    <Button Classes="Primary" Content="Save" />
    <Button Theme="{StaticResource OutlineButtonTheme}" Content="Cancel" />
    <Button Theme="{StaticResource GhostButtonTheme}" Classes="Danger Small" Content="Delete" />
</StackPanel>
```

Use primary emphasis for the main action in a decision group. Color and size classes compose; two conflicting color classes have no useful semantic meaning. Class support is specific to each control and theme. A `Primary` button does not imply that every control accepts `Primary`.

`TextBox` offers `UnderlineTextBoxTheme`, `FieldTextBoxTheme`, and `EmbeddedTextBoxTheme` in addition to its default appearance. The default theme supports `Clearable` and `Multiline`. In this Avalonia version, hint text uses `PlaceholderText`.

```xml
<TextBox Classes="Clearable" PlaceholderText="Search projects" />
```

Pseudo-classes such as `:pressed`, `:disabled`, and `:checked` are runtime state selectors. Application code sets the corresponding public state; it does not add strings such as `:pressed` to `Classes`.

For repeated application adjustments, add a scoped style targeting public properties. Reserve template replacement for requirements that need a different structure; that work must preserve the control's required parts and interaction behavior. Internal named template elements are not application styling contracts.

Reference: [button themes](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Controls/Button.axaml), [text box themes](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Controls/TextBox.axaml).

## 4. Layout and page composition

Let the container own the rhythm: `StackPanel.Spacing`, `Grid.RowSpacing` / `ColumnSpacing`, and `DockPanel.VerticalSpacing` / `HorizontalSpacing`. Use `Padding` for inner space and `Margin` for genuine outer insets. For repeated items, choose an items panel with the appropriate spacing capability.

An application can start with 8 units within a small group, 16 between related sections, and 24 around a page, then tune for its density. These are composition suggestions, not library spacing tokens. Avalonia layout values are device-independent units.

Use `Grid` with `Auto` and `*` tracks for toolbars plus a remaining content area. Keep lists and tables in a bounded viewport so their scrolling and virtualization can work. `husk:Page` already contains a `ScrollViewer`; choose a plain `UserControl` with an explicit layout when a screen needs its own fixed toolbar and independently scrolling data region.

Adapt navigation width and column count to available space and content. Test a narrow window and long localized labels. Navigation pane collapsing is controlled through `NavigationView.IsPaneOpen`; the application decides when to change it.

| Screen | Starting composition | Application responsibility |
| --- | --- | --- |
| Settings | `Page` → sections in a `StackPanel` → labeled inputs; `Card` where grouping helps | Validation, save/cancel behavior, unsaved changes |
| Data browser | `UserControl` → `Grid` with toolbar, `TableView` or `ListBox`, optional `PaginationControl` | Querying, filtering, ordering, selection actions |
| Detail screen | `Page` → heading and content groups; `Frame` for history | Loading by identifier and restoring state |
| Multi-step flow | `StepControl` for progress, content for the active step, `Modal` when the task is an overlay | Step validation, transitions, submission |

### A settings section

This fragment expects a view model with `ProjectName`, `Description`, and `SaveCommand`. In a compiled-binding view, declare the actual model with `x:DataType` on the view root. The model owns validation and command availability.

```xml
<husk:Card MaxWidth="640" HorizontalAlignment="Stretch">
    <StackPanel Spacing="16">
        <TextBlock Text="Project settings" FontSize="{StaticResource LargeFontSize}" />
        <StackPanel Spacing="8">
            <TextBlock x:Name="ProjectNameLabel" Text="Project name" />
            <TextBox
                AutomationProperties.LabeledBy="{Binding #ProjectNameLabel}"
                Text="{Binding ProjectName, Mode=TwoWay}" />
        </StackPanel>
        <StackPanel Spacing="8">
            <TextBlock x:Name="DescriptionLabel" Text="Description" />
            <TextBox
                AutomationProperties.LabeledBy="{Binding #DescriptionLabel}"
                Classes="Multiline"
                MinHeight="96"
                Text="{Binding Description, Mode=TwoWay}" />
        </StackPanel>
        <Button
            HorizontalAlignment="Left"
            Classes="Primary"
            Command="{Binding SaveCommand}"
            Content="Save changes" />
    </StackPanel>
</husk:Card>
```

Keep field labels visible. Use Avalonia's validation mechanisms for field errors and persistent explanatory text when recovery needs more context. A notification alone is insufficient for a validation problem the user still needs to fix.

## 5. Navigation and view lifetime

`NavigationView` presents navigation items and selection. `Frame` activates views, records navigation history, and runs transitions. The application connects selection or a command to `Frame.Navigate`; assigning navigation items does not create routes automatically.

A shell can place a `Frame` in `NavigationView.Content`. Explicit content syntax avoids confusing page content with the navigation item collection:

```xml
<husk:NavigationView
    ItemsSource="{Binding NavigationItems}"
    SelectedItem="{Binding SelectedNavigationItem, Mode=TwoWay}"
    BackCommand="{Binding #MainFrame.GoBackCommand}">
    <husk:NavigationView.Content>
        <husk:Frame x:Name="MainFrame" />
    </husk:NavigationView.Content>
</husk:NavigationView>
```

Provide an `ItemTemplate` for the application's navigation model and an `IconTemplate` when using icons. For model items, the control reads `Icon` and `Category` by property name. The [Gallery shell](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/AppView.axaml) shows the complete templates and selection wiring in its [code-behind](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/AppView.axaml.cs).

If routing from `SelectionChanged`, first check that `e.Source` is the `NavigationView` itself. Selection events from controls inside the page can bubble through the shell and must not trigger a new navigation.

From the view or the application's navigation adapter, where `frame` is the `Huskui.Avalonia.Controls.Frame` and `detailsViewModel` is application-owned:

```csharp
frame.Navigate(typeof(ProjectDetailsView), detailsViewModel);

if (frame.CanGoBack)
    frame.GoBack();
```

The default `PageActivator` constructs the supplied type with a parameterless constructor and assigns the navigation parameter as its `DataContext`. The view can be a `UserControl` or `husk:Page`. `Page` adds a header and back-button presentation associated with its ancestor `Frame`.

History records the view type and parameter. Going back invokes the activator again; it does not preserve the previous control instance. Put durable state in the application's models or use the optional Mvvm state facilities. For dependency injection or a different activation policy, assign `Frame.PageActivator`; see [MVVM](#8-optional-extensions-and-mvvm).

## 6. Feedback and overlays

### Choose by interaction, not just the name

| Need | Control | Presentation and ownership |
| --- | --- | --- |
| Message within the current page | `InfoBar` | `Header` + `Content`; color classes include `Primary`, `Success`, `Warning`, `Danger` |
| Non-modal notification or progress feedback | `GrowlItem` | `AppSurface.PopGrowl`; `GrowlHost` manages the collection |
| Confirmation or small input with a result | `Dialog` | `AppSurface.PopDialog`; confirm/cancel completion |
| Larger focused task | `Modal` | `AppSurface.PopModal`; application-composed content and actions |
| Modal side panel | `Sidebar` | `AppSurface.PopSidebar`; background interaction is obstructed |
| Modal content panel entering from below | `Toast` | `AppSurface.PopToast`; this is part of the modal overlay system |
| Movable, resizable non-modal panel | `Drawer` | `AppSurface.PopDrawer`; separate `DrawerHost` |

**Huskui's `Toast` is a modal content surface. Use `GrowlItem` for the familiar non-modal notification use case.** `Sidebar` and `Drawer` also have different interaction models; choose based on whether the user must finish or dismiss the panel before returning to the page.

### Find the surface and request presentation

On the UI thread, after the calling control is attached and the shell is templated, find its surface with `AppSurface.GetAppSurface(control)`. It finds an enclosing surface or the containing `AppWindow`'s surface. It can return `null` for detached controls or a shell without a surface; resolve this at the view/application UI-service boundary.

The following helper can live in a view or application-owned UI service. Its `owner` is an attached control in the intended window:

```csharp
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Huskui.Avalonia.Controls;

private static async Task<bool> ConfirmDiscardAsync(Control owner)
{
    var surface = AppSurface.GetAppSurface(owner)
        ?? throw new InvalidOperationException("An attached AppSurface is required.");

    var dialog = new Dialog
    {
        Title = "Discard changes?",
        Message = "Your unsaved edits will be lost.",
        PrimaryText = "Discard",
        SecondaryText = "Keep editing",
        IsPrimaryButtonVisible = true,
    };

    surface.PopDialog(dialog);
    return await dialog.CompletionSource.Task;
}
```

`PopDialog` returns `void`. Await the dialog's `CompletionSource.Task`: `true` means confirmation; cancellation or detachment completes it with `false`. A custom input dialog stores its value in `Result`; consume that value only after successful confirmation. Create a new dialog for each presentation because its completion source is not reset.

For synchronous confirmation validation, a dialog can override `ValidateResult` or reject `ConfirmRequested` via `args.Rejected`. Perform asynchronous application work in the application's command/service flow; the confirm event itself is not an awaitable submission pipeline.

A non-modal notification uses the same surface. Here `surface` has already been resolved as above:

```csharp
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Models;

var notification = new GrowlItem
{
    Level = GrowlLevel.Success,
    Title = "Project saved",
    Content = "Your changes are available locally.",
    IsCloseButtonVisible = true,
};
surface.PopGrowl(notification);
```

`GrowlItem.Level` communicates runtime severity; it is an enum, unlike the visual variant classes on `InfoBar`. Growls have no automatic timeout in the core host. Let the user close them, or have the application's notification service manage timed dismissal. Progress notifications expose `Progress`, `ProgressMaximum`, `IsProgressIndeterminate`, and `IsProgressBarVisible`.

Use the presented control's `Dismiss()` method when closing it programmatically. It requests dismissal through the appropriate host, preserving removal and transitions. Each presentation should own a fresh control instance; do not place the same control in two hosts. `Pop*` methods do not by themselves promise backdrop-click or Escape dismissal—wire any extra dismissal policy deliberately.

For bound custom overlay content, assign its application `DataContext` explicitly. Keep host lookup in the view or an application-owned UI service so business view models can request confirmation or notification without depending on a window instance.

Reference: [dialog behavior and result example](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/DialogsPage.axaml.cs), [custom input dialog](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Dialogs/EmailInputDialog.axaml), [AppSurface API](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Controls/AppSurface.cs).

### Loading, empty, and error states

| State | Pattern | What the application supplies |
| --- | --- | --- |
| Content is loading and its shape is known | `SkeletonContainer.IsLoading` | Placeholder dimensions matching the eventual content; `IsAnimated` controls animation |
| An existing region is busy | `BusyContainer.IsBusy` | `PendingContent` with a progress indicator or explanation |
| Progress is unknown / measurable | `husk:ProgressRing` or standard `ProgressBar` | `IsIndeterminate`, or range and value |
| An object has not been selected or loaded | `PlaceholderContainer` | `Source`, `SourceTemplate`, and `Placeholder` |
| A collection has no items | Application empty-state composition | Count-based state and an action to populate or change the filter |
| An operation failed | Field validation or an inline `InfoBar` | A useful error message, retained input, and recovery action |

`PlaceholderContainer` switches on whether `Source` is `null`; an empty collection is still a non-null source. `BusyContainer` overlays and blurs its content. It does not enforce command cancellation or prevent every keyboard/programmatic action; command availability and duplicate-operation protection remain application logic.

## 7. Lists, tables, and paging

Use `ListBox` for selectable items, `ItemsControl` for repeated non-selectable content, and Avalonia's **`TableView`** for read-only tabular presentation with configurable columns. Huskui supplies the `TableView` theme; there is no `husk:TableView` type in this library. Consult the installed Avalonia API before assuming DataGrid-style editing or sorting members.

The [TableView example](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/TableViewPage.axaml) demonstrates `TableViewColumn.Binding`, `CellTemplate`, column widths, and selection. Define the row type for compiled cell bindings, separately from the page view model. Keep the table in a finite-height region so it owns its scrolling.

`husk:PaginationControl` exposes `TotalCount`, `PageSize`, and **zero-based** `PageIndex`. `DisplayIndex` is the one-based number shown to the user. Bind `PageIndex` with `Mode=TwoWay` or handle `IndexChanged` to drive fetching or slicing. Changes to the total or page size can coerce the index and raise that event too; the application owns query execution and stale-result handling.

`husk:InfiniteScrollView` loads through an `ItemsSource` implementing `IInfiniteCollection` (`HasNext`, `IsFetching`, `FetchAsync`). `InfiniteCollection<T>` supplies a batch factory and stops after an empty batch. Replacing the source disposes the old source when it implements `IDisposable`, so give the view ownership of that collection. The view does not present fetch failures; provide explicit error and recovery state in the application's loading implementation. Read the linked source contract before using it for a remote feed.

Additional components include `Tag` / `TagBox` for labels and tag selection, `StepControl` for staged progress, `TimelineControl` for event sequences, and `DropZone` / `DropContainer` for drag-and-drop interactions. Choose a component from its example and public API; similar names in other UI libraries are not evidence of matching properties.

## 8. Optional extensions and MVVM

| Package | Use it for | Starting point |
| --- | --- | --- |
| `Huskui.Avalonia.Code` | Syntax-highlighted source display, `DiffView`, and `ZoomView` | [Code README](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia.Code/README.md), [diff example](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/DiffViewsPage.axaml), [zoom example](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/ZoomViewsPage.axaml) |
| `Huskui.Avalonia.Markdown` | Markdown rendered as native Avalonia controls | [Markdown README](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia.Markdown/README.md) |
| `Huskui.Avalonia.Mvvm` | View-model lifecycle, DI activation, and optional view state | [Mvvm README](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia.Mvvm/README.md) |

Code and Markdown controls use the same `husk:` XAML namespace. With `HuskuiTheme` present, their extension styles are discovered when their assemblies load; manual duplicate style includes are unnecessary.

```xml
<husk:CodeViewer Code="{Binding SourceCode}" Language="csharp" />
<husk:MarkdownViewer Markdown="{Binding MarkdownText}" />
```

These are separate fragments to place in the application's layout. `CodeViewer` displays code; it is not an editable IDE control. For Markdown-specific appearance, use the extension's documented semantic classes and theme resources.

The core library does not require the Mvvm extension or a particular command framework. Existing application bindings and commands can be used directly. When adding the Mvvm extension:

- `IViewModel` supplies `InitializeAsync(CancellationToken)` and `DeinitializeAsync()`.
- `ViewModelMixin.Attach(view)` connects that lifecycle to loading, unloading, and data-context changes. Initialization can happen again; honor cancellation and avoid assuming a single lifetime call.
- `IViewActivator` / `ViewActivatorBase` support activation and DI integration. Use `FrameActivationMixin.Install(frame, activator)` to connect an activator to `Frame.PageActivator`.
- `IStatefulViewModel<T>`, view-state services, and persistence are optional; read the state section before choosing identity and storage behavior.

## 9. Find the exact example and verify the application

Read the relevant example's `.axaml` for composition and its `.axaml.cs` for behavior. Gallery types such as `ControlPage`, `ExampleContainer`, and Gallery services are demonstration infrastructure; application code should use the controls shown inside them.

| Task | Example or source |
| --- | --- |
| Buttons, variants, and actions | [ButtonsPage.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/ButtonsPage.axaml) |
| Form entry and field appearances | [TextBoxesPage.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/TextBoxesPage.axaml) |
| Browse the semantic brushes | [BrushResourceKeysPage.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/BrushResourceKeysPage.axaml) |
| Navigation and transitions | [FramesPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/FramesPage.axaml.cs) |
| Modal content and actions | [ModalsPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/ModalsPage.axaml.cs) |
| Non-modal feedback | [GrowlsPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/GrowlsPage.axaml.cs) |
| Modal side panels | [SidebarsPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/SidebarsPage.axaml.cs) |
| Floating drawers | [DrawerPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/DrawerPage.axaml.cs) |
| Busy and skeleton presentation | [BusyContainersPage.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/BusyContainersPage.axaml), [SkeletonContainersPage.axaml](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/SkeletonContainersPage.axaml) |
| Paging | [PaginationControlsPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/PaginationControlsPage.axaml.cs) |
| Incremental loading contract | [InfiniteScrollView.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Controls/InfiniteScrollView.cs), [IInfiniteCollection.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Models/IInfiniteCollection.cs), [InfiniteCollection.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Avalonia/Models/InfiniteCollection.cs) |
| MVVM activation and lifecycle | [MvvmPage.axaml.cs](https://github.com/d3ara1n/Huskui.Avalonia/blob/main/src/Huskui.Gallery/Views/MvvmPage.axaml.cs) |

For core custom controls, inspect the class and theme in [Controls](https://github.com/d3ara1n/Huskui.Avalonia/tree/main/src/Huskui.Avalonia/Controls). A theme file without a sibling class can style an Avalonia-owned control; verify ownership before choosing the namespace. Standard control API contracts come from the matching Avalonia version.

To inspect the real rendered controls from a source checkout, run the desktop executable project:

```sh
dotnet run --project src/Huskui.Gallery.Desktop
```

Before considering an application screen complete:

1. Build the consuming project with its actual package versions and resolve XAML, resource, and binding errors.
2. Exercise the relevant interaction: navigation/back, confirmation/cancellation, save availability, loading/error recovery, or paging. Check that overlays use the intended window's surface.
3. Inspect the real screen in light and dark themes, at its narrow supported size, and with long text. Check keyboard focus, labels, and access to actions while content scrolls.

Report which behaviors were actually exercised. A successful build checks API and XAML compatibility; it does not establish visual quality or correct runtime bindings.
