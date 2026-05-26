# Project Conventions

## UI Design Rules

- When creating a `ControlTemplate`, internal template parts that are NOT exposed as `TemplatePart` attributes should **not** use the `PART_` prefix in their names. Reserve `PART_` exclusively for parts that are required/expected by the control's code-behind via `TemplatePart` attributes. This makes the distinction clear between contractually required parts and internal implementation details.

- Every control consists of a **paired `.cs` and `.axaml` file** in the same `Controls/` directory. The `.axaml` file is a `ResourceDictionary` containing `ControlTheme`(s), not a standalone control definition. New controls must follow this pairing.

- The default theme for a control uses `x:Key="{x:Type local:ControlName}"` for implicit styling. Alternative visual styles (e.g. Outline, Ghost) use named keys like `OutlineButtonTheme`.

- Color variants (Primary, Success, Warning, Danger) and size variants (Small, Large) are applied via **CSS classes**, not enum properties. Style variants (Default, Outline, Ghost) are applied by switching the `Theme` property to a different `ControlTheme` resource. Classes represent persistent visual variants — their styles target the control itself directly. Pseudo-classes (`:pointerover`, `:pressed`, `:disabled` etc.) represent transient states and their styles target internal template parts.

- The project uses a **Radix-inspired 12-step color scale** where step 9 is always the base/interactive color. When adding new semantic colors, follow this 12-step pattern and generate both Default (light) and Dark theme dictionary variants.

- Animation durations must reference the **named constants** from `Themes/Basics.axaml` (e.g. `ControlFasterAnimationDuration`, `ControlNormalAnimationDuration`) rather than hard-coded `TimeSpan` values. Transitions should use `SineEaseOut` easing.

- Corner radii must use the **indirection layer** — reference `StaticResource` keys like `SmallCornerRadius`, `FullCornerRadius` etc. from `CornerRadii.axaml`, not raw `CornerRadius` values. These keys already handle dynamic forwarding internally. When a converter is needed on a `StaticResource` corner radius, use `StaticResourceBinding` instead of `StaticResource`.

- The XAML namespace for the library is `https://github.com/d3ara1n/Huskui.Avalonia` (mapped to prefix `husk`). All new namespaces must be registered via `[XmlnsDefinition]` in `AssemblyInfo.cs`.

## Control Implementation Patterns

- `StyledProperty` registration uses multi-line generic syntax with type parameters on separate lines:
  ```csharp
  public static readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<
      ControlName,
      bool
  >(nameof(IsReadOnly));
  ```

- Use `OnPropertyChanged` override to react to property changes (including setting `PseudoClasses`), not property change callbacks.

- The `field` keyword is used with `SetAndRaise` for `DirectProperty` accessors:
  ```csharp
  public int PageCount
  {
      get;
      private set => SetAndRaise(PageCountProperty, ref field, value);
  }
  ```

- In `OnApplyTemplate`, event handlers for template parts must be **unregistered then re-registered within the same method** — keep the `-=` and `+=` pairs together so subscription and unsubscription are visible at a glance. Do not split them into separate lifecycle methods.

- Use the project's own `InternalCommand` / `InternalAsyncCommand` (in `Models/`) for ICommand implementations within controls, not CommunityToolkit's `RelayCommand`.

- Overlay-type controls (Toast, Dialog, Drawer, etc.) expose a `Dismiss()` method that raises `DismissRequestedEvent` to bubble up to their host container. Follow this pattern for new overlay controls.

- `TemplatePart` constants must be declared as `public const string` using `nameof()`:
  ```csharp
  public const string PART_ItemsControl = nameof(PART_ItemsControl);
  ```

## Resource Key Naming

Resource keys follow the pattern `{Owner}{Variant}{State}{Property}Type`. Examples:
- `ControlBackgroundBrush`, `ControlInteractiveBorderBrush`
- `ControlAccentForegroundBrush`, `ControlDangerTranslucentHalfBackgroundBrush`
- `Accent9Color`, `Gray3Color`
- `SmallCornerRadius`, `FullCornerRadius`
- `ControlFasterAnimationDuration`

## Converter Pattern

Converters are organized as **static properties on static classes** (e.g. `CornerRadiusConverters`, `BoolConverters`), using `RelayConverter` / `RelayMultiConverter` for lambda-based implementations. They are referenced in XAML via `{x:Static}`:
```xml
Converter="{x:Static husk:CornerRadiusConverters.Top}"
```
Do not create standalone `IValueConverter` classes unless the converter is complex enough to warrant it.

## Code Style

- File-scoped namespaces, `var` everywhere, expression-bodied members preferred.
- Primary constructors and `field` keyword (C# 13+) are used freely.
- Nullable reference types are enabled globally.
- Private instance fields: `_camelCase`. Private static fields: `camelCase` (no underscore prefix).
- Target frameworks: `net8.0;net10.0`. LangVersion is `preview`.
- Comments may be in either English or Chinese.
