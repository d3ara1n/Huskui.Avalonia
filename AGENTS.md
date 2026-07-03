# Project Conventions

## UI Design Rules

- When creating a `ControlTemplate`, internal template parts that are NOT exposed as `TemplatePart` attributes should **not** use the `PART_` prefix in their names. Reserve `PART_` exclusively for parts that are required/expected by the control's code-behind via `TemplatePart` attributes. This makes the distinction clear between contractually required parts and internal implementation details.

- Every control consists of a **paired `.cs` and `.axaml` file** in the same `Controls/` directory. The `.axaml` file is a `ResourceDictionary` containing `ControlTheme`(s), not a standalone control definition. New controls must follow this pairing.

- The default theme for a control uses `x:Key="{x:Type local:ControlName}"` for implicit styling. Alternative visual styles (e.g. Outline, Ghost) use named keys like `OutlineButtonTheme`.

- **Style classes (variant selectors) are `PascalCase`.** A class denotes a **variant** — a named look the consumer opts into — and the name is an adjective or noun describing that variant. Examples in use: `Primary`, `Danger`, `Small`. Color variants (Primary, Success, Warning, Danger) and size variants (Small, Large) are applied via **CSS classes**, not enum properties.

- **Pseudo-classes (state selectors) are `all-lowercase`.** A pseudo-class denotes a **runtime state** of the control. Examples: `:pointerover`, `:pressed`, `:checked`, `:disabled`, `:focus`, `:selected`, `:error`. Never capitalize a pseudo-class.

- The distinguishing question: **is the consumer choosing a look (`Primary`) or is the control reporting its own state (`:pressed`)?** Variant → PascalCase class; state → lowercase pseudo-class.

- Style variants (Default, Outline, Ghost) are applied by switching the `Theme` property to a different `ControlTheme` resource. Classes represent persistent visual variants — their styles target the control itself directly. Pseudo-classes represent transient states and their styles target internal template parts.

- **A variant class may only set the control's own exposed properties** (e.g. `Background`, `Foreground`, `BorderBrush`, `CornerRadius`, `Padding`). It must **not** reach into the control template and restyle named parts (`/template/ Border#PART_Xxx`). Restyling template internals is reserved for pseudo-class-driven states inside the same ControlTheme; variant classes stay at the public-property surface so they compose cleanly when consumers stack them (`Classes="Primary Small"`). For example, a `Primary` class on a Button changes `Background`/`Foreground`; it does **not** touch the inner `ContentPresenter` or `PART_Background` rectangle directly.

- The project uses a **Radix-inspired 12-step color scale** where step 9 is always the base/interactive color. When adding new semantic colors, follow this 12-step pattern and generate both Default (light) and Dark theme dictionary variants.

- Animation durations must reference the **named constants** from `Themes/Basics.axaml` (e.g. `ControlFasterAnimationDuration`, `ControlNormalAnimationDuration`) rather than hard-coded `TimeSpan` values.

- Corner radii must use the **indirection layer** — reference `StaticResource` keys like `SmallCornerRadius`, `FullCornerRadius` etc. from `CornerRadii.axaml`, not raw `CornerRadius` values. These keys already handle dynamic forwarding internally. When a converter is needed on a `StaticResource` corner radius, use `StaticResourceBinding` instead of `StaticResource`.

- The XAML namespace for the library is `https://github.com/d3ara1n/Huskui.Avalonia` (mapped to prefix `husk`). All new namespaces must be registered via `[XmlnsDefinition]` in `AssemblyInfo.cs`.

- **Do NOT run any formatting tools** (`csharpier`, `xstyler`, etc.). They can produce unintended changes across the entire repo. Only the user may invoke formatting.

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

- `TemplatePart` requires a **three-part declaration** on the control class:
  1. `[TemplatePart(PART_Name, typeof(ControlType))]` attribute
  2. `public const string PART_Name = nameof(PART_Name);` constant
  3. In XAML, reference via `Name="{x:Static local:ControlName.PART_Name}"` rather than a bare `Name="PART_Name"`
  ```csharp
  [TemplatePart(PART_ItemsControl, typeof(ItemsControl))]
  [TemplatePart(PART_QuickJumperPopup, typeof(Popup))]
  public class PaginationControl : TemplatedControl
  {
      public const string PART_ItemsControl = nameof(PART_ItemsControl);
      public const string PART_QuickJumperPopup = nameof(PART_QuickJumperPopup);
  }
  ```
  `nameof(PART_Xxx)` self-checks: if you rename the constant, the string updates automatically, keeping XAML and C# in sync. Never use bare string literals like `Find<ScrollViewer>("PART_ScrollViewer")`.

- **Template-internal elements that are NOT referenced from code-behind do NOT use the `PART_` prefix.** Give them descriptive, short names like `Background`, `Border`, `Indicator`, `ContentPresenter` — these names serve only styling selectors within the same ControlTheme and never appear in C#.

- **Pseudo-class names used in code-behind must be declared as `public const string` with a `CLASS_` prefix** — same principle as `PART_` for template parts. Never use bare pseudo-class string literals in `PseudoClasses.Set` / `PseudoClasses.Remove`:
  ```csharp
  public const string CLASS_Error = ":error";
  public const string CLASS_Selected = ":selected";
  ```
  Then use `PseudoClasses.Set(CLASS_Error, true)` instead of `PseudoClasses.Set(":error", true)`. Pseudo-class selectors in `.axaml` are still written as bare `:error`/`:selected` in style selectors — the constant is only for code-behind references.

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

## Code Organization

- **One type per `.cs` file.** Never declare more than one top-level type in a single file. A new type has only two valid homes: its own file, or nested inside the type it belongs to.
- **Choose by semantic ownership, not by visibility or who references it.** The question is whether the type is that other type's own concept — not whether it is public or used elsewhere.
  - **Nested type** when it is dedicated to an outer class, even if that class exposes it through its public API (e.g. as a parameter or return type). The fact that callers must supply/pass values of that type does **not** make it independent.
  - **Own file** when it is a shared model — a type with its own data/properties that View, ViewModel, and Services may all consume is a standalone entity and gets its own file (under `Models/` for models).

## Comments

**The default is no comment.** Names, types, and control flow are the documentation; a method that reads `Close(); Dispose();` needs no `// Close the dialog and release resources` above it. A comment earns its place only when the code is **counter-intuitive** — when a reader following the obvious reading would reach the wrong conclusion without a hint. Write to explain the *why*, never the *what*.

Two anti-patterns to avoid:

- **Restating the obvious.** Paraphrasing what the code already says in plain sight is noise. If the names and types already tell the story, the comment goes.
- **Repeating a project-wide mechanism at a single call site.** If a convention is already described in this AGENTS.md or is the default behavior shared by every sibling method, do **not** single out one site to re-explain it. Doing so implies the others differ when they don't, and misleads future readers. Comment the **exception**, never the rule.

When in doubt, leave the comment out. The legitimate reasons to write one are narrow and non-local: why a workaround exists, a non-obvious constraint or invariant, a link to an upstream issue, or a warning about something the next reader will get wrong without the hint.

## Code Style

- File-scoped namespaces, `var` everywhere, expression-bodied members preferred.
- Primary constructors and `field` keyword (C# 13+) are used freely.
- Nullable reference types are enabled globally.
- Private instance fields: `_camelCase`. Private static fields: `_camelCase`.
- Target frameworks: `net8.0;net10.0`. LangVersion is `preview`.
- Comments may be in either English or Chinese.

## Git Commit

- **Do not commit on your own initiative.** Make all the edits you need, then stop and wait for the user to explicitly tell you to commit. Never auto-commit after editing without being asked.
- First line follows Conventional Commits: `type(scope): description`.
- Blank line between first line and body; body lists key changes.
- **Scope chooses which package publishes.** The `publish.yml` workflow bumps a package only when a commit's scope resolves to it:
  - `Core` → the base package `Huskui.Avalonia`
  - a package suffix → `Huskui.Avalonia.<suffix>` (e.g. `Code`, `Markdown`, `Mvvm`)
  - any other scope (including the full package name) or no scope at all → no package is bumped. `feat`/`fix`/`perf`/`refactor` bump; `ci`/`chore`/`docs`/`style`/`test`/`build` never bump.
