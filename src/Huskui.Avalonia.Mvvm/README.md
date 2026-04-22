# Huskui.Avalonia.Mvvm

MVVM integration helpers for [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia). Provides a lightweight `IViewModel` lifecycle interface and a `ViewModelAttachableMixin` that wires View loading/unloading to ViewModel initialization/deinitialization — without coupling your ViewModel layer to Huskui controls.

## Install

```xml
<PackageReference Include="Huskui.Avalonia.Mvvm" Version="*" />
```

## Core API

### `IViewModel`

```csharp
namespace Huskui.Avalonia.Mvvm.Models;

public interface IViewModel
{
    Task InitializeAsync(CancellationToken cancellationToken);
    Task DeinitializeAsync();
}
```

### `ViewModelAttachableMixin`

```csharp
namespace Huskui.Avalonia.Mvvm.Mixins;

public static class ViewModelAttachableMixin
{
    public static void Attach<T>(T self) where T : Control;
}
```

Call `Attach` on any `Control` (typically a `Page`). When the control loads, if its `DataContext` implements `IViewModel`, the mixin will:

1. Call `InitializeAsync` with a `CancellationToken` that is cancelled on unload.
2. Set pseudo-classes `:loading` → `:finished` (or `:failed`) on the control during the process.
3. Call `DeinitializeAsync` when the control unloads.

---

## Usage

### 1.0.0 and later (recommended)

Your ViewModel implements `IViewModel` directly and is completely decoupled from Huskui.Avalonia controls:

```csharp
using Huskui.Avalonia.Mvvm.Models;

// ViewModelBase is YOUR base class — use whatever MVVM framework you like.
// Just implement IViewModel.
public class MyViewModel : ObservableObject, IViewModel
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        // Load data, call APIs, etc.
        // cancellationToken is cancelled when the View unloads.
    }

    public async Task DeinitializeAsync()
    {
        // Clean up resources
    }
}
```

A common pattern is to define a project-specific base class:

```csharp
public abstract class ViewModelBase : ObservableObject, IViewModel
{
    public Task InitializeAsync(CancellationToken ct) => OnInitializedAsync(ct);
    public Task DeinitializeAsync() => OnDeinitializedAsync();

    protected virtual Task OnInitializedAsync(CancellationToken ct) => Task.CompletedTask;
    protected virtual Task OnDeinitializedAsync() => Task.CompletedTask;
}
```

Then in your navigation/page activation logic, attach the mixin:

```csharp
using Huskui.Avalonia.Mvvm.Mixins;

var page = new MyPage();
page.DataContext = new MyViewModel();

// This attaches the Loaded/Unloaded lifecycle hooks
ViewModelAttachableMixin.Attach(page);
```

**Key points:**

- The ViewModel has **no reference** to any Huskui.Avalonia control type.
- `InitializeAsync` receives a `CancellationToken` — no need to pass a token from the View.
- The mixin uses a `ConditionalWeakTable` internally, so it won't leak the control.

### Before 1.0.0 (legacy)

Previously, the lifecycle was managed directly by `Huskui.Avalonia.Controls.Page` via the `IPageModel` interface:

```csharp
// Old interface (from Huskui.Avalonia, now removed)
public interface IPageModel
{
    CancellationToken PageToken { get; set; }
    Task InitializeAsync();
    Task DeinitializeAsync();
}
```

And the ViewModel was wired up manually:

```csharp
var page = new MyPage();
var viewModel = new MyViewModel();

if (viewModel is IPageModel pageModel)
{
    pageModel.PageToken = page.LifetimeToken;
    page.Model = pageModel;
}

page.DataContext = viewModel;
```

**Problems with this approach:**

- `IPageModel` was defined in `Huskui.Avalonia`, so ViewModels depended on the UI library.
- `PageToken` was a mutable property injected from the View — the ViewModel had to be aware of it.
- Lifecycle management was hardcoded inside `Page.OnLoaded`/`Page.OnUnloaded`, not reusable for other controls.

## Migration Guide

| Before (1.0.0)                                 | After (1.0.0)                                          |
|-------------------------------------------------|--------------------------------------------------------|
| `IPageModel` (in `Huskui.Avalonia`)            | `IViewModel` (in `Huskui.Avalonia.Mvvm`)              |
| `PageToken` property on ViewModel               | `CancellationToken` parameter on `InitializeAsync`    |
| `page.Model = pageModel`                        | `ViewModelAttachableMixin.Attach(page)`               |
| Lifecycle coupled to `Page` control             | Lifecycle works with **any** `Control`                |
| ViewModel references `Huskui.Avalonia`          | ViewModel references only `Huskui.Avalonia.Mvvm`     |
