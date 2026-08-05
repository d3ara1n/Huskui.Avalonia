# Huskui.Avalonia.Mvvm

MVVM integration helpers for [Huskui.Avalonia](https://github.com/d3ara1n/Huskui.Avalonia).

This package currently provides four mechanisms that work together:

- `IViewModel` lifecycle binding for any Avalonia `Control`
- view activation (`IViewActivator`) for `Frame`-based navigation
- activation parameter injection (`IViewContext`)
- view state attach/persist/restore (`IStatefulViewModel<T>`)

The goal is to keep ViewModels UI-agnostic while still giving Views a predictable lifecycle and a simple way to restore page-level state.

## Install

```xml
<PackageReference Include="Huskui.Avalonia.Mvvm" Version="*" />
```

## Overview

The package is split into independent layers:

1. `ViewModelMixin`
   Binds `Control.Loaded`, `Control.Unloaded`, and `DataContextChanged` to `IViewModel.InitializeAsync` / `DeinitializeAsync`.
2. `ViewActivatorBase`
   Creates the view, resolves the ViewModel from DI, injects navigation parameters through `IViewContext`, then attaches the lifecycle/state mixins.
3. `ViewStateMixin` + `IViewStateManager`
   Detects `IStatefulViewModel<T>`, assigns `ViewState`, and releases it when the view detaches.
4. `IViewStateStore` + `IViewStatePersistence`
   Caches state instances in memory and optionally persists them.

You can use only the parts you need, but most applications will register both activation and state support:

```csharp
services
    .AddViewModelActivation<MyViewActivator>()
    .AddViewState(builder => builder.WithStatePersistence<MyViewStatePersistence>());
```

## 1. ViewModel Lifecycle

### Core interface

```csharp
namespace Huskui.Avalonia.Mvvm.Models;

public interface IViewModel
{
    Task InitializeAsync(CancellationToken cancellationToken);
    Task DeinitializeAsync();
}
```

### Recommended base class

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using Huskui.Avalonia.Mvvm.Models;

public abstract class ViewModelBase : ObservableObject, IViewModel
{
    public virtual Task InitializeAsync(CancellationToken cancellationToken) =>
        OnInitializeAsync(cancellationToken);

    public virtual Task DeinitializeAsync() =>
        OnDeinitializeAsync();

    protected virtual Task OnInitializeAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    protected virtual Task OnDeinitializeAsync() =>
        Task.CompletedTask;
}
```

### Attaching lifecycle behavior

```csharp
using Huskui.Avalonia.Mvvm.Mixins;

var page = new MyUserControl
{
    DataContext = new MyViewModel()
};

ViewModelMixin.Attach(page);
```

When attached, the mixin does the following:

1. On `Loaded`, if `DataContext` implements `IViewModel`, calls `InitializeAsync`.
2. On `Unloaded`, cancels the active initialization token and calls `DeinitializeAsync`.
3. On `DataContextChanged`, deinitializes the old ViewModel and initializes the new one when necessary.
4. Applies pseudo-classes to the control:
   `:loading`, `:finished`, `:failed`

### Styling loading states

```xml
<Style Selector="local|MyPage:loading">
    <Setter Property="Opacity" Value="0.6" />
</Style>

<Style Selector="local|MyPage:failed">
    <Setter Property="Background" Value="IndianRed" />
</Style>
```

### Lifecycle notes

- `InitializeAsync` receives a token that is cancelled when the control unloads or when the `DataContext` switches away from the current ViewModel.
- `DeinitializeAsync` is called best-effort. Exceptions are swallowed by the mixin.
- The mixin serializes transitions with an internal gate, so overlapping load/unload/data-context events do not run lifecycle methods concurrently.
- `Design.IsDesignMode` short-circuits initialization.
- `Attach` is idempotent for the same control.

### Important usage notes

- Treat `InitializeAsync` as re-entrant across the lifetime of the same ViewModel instance. A control can be loaded, unloaded, then loaded again.
- Respect the cancellation token in long-running work. If initialization ignores cancellation, stale results can still complete after navigation.
- Do not assume `:finished` means the page is still current. It only means the latest initialization finished without cancellation or exception.

## 2. View Activation

If you use `Huskui.Avalonia.Controls.Frame`, the recommended setup is an `IViewActivator`.

### Registration

```csharp
services.AddViewModelActivation<MyViewActivator>();
```

This registers:

- `IViewActivator`
- `IViewContextAccessor`
- `IViewContext`
- `IViewContext<T>`

### Core interface

```csharp
namespace Huskui.Avalonia.Mvvm.Activation;

public interface IViewActivator
{
    object? Activate(Type viewType, object? parameter = null);
}
```

### Base activator

```csharp
using Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.States;

public sealed class MyViewActivator(IServiceProvider provider, IViewStateManager stateManager)
    : ViewActivatorBase(provider, stateManager)
{
    protected override Type FindViewModelType(Type view)
    {
        return Type.GetType(view.FullName!.Replace("View", "ViewModel"))!;
    }
}
```

`ViewActivatorBase` does all of the mechanical work:

1. Creates the view.
2. Creates a DI scope.
3. Stores the navigation parameter in `IViewContextAccessor`.
4. Resolves or creates the ViewModel from DI.
5. Assigns `view.DataContext`.
6. Attaches `ViewModelMixin` and `ViewStateMixin`.

That attach behavior is the default behavior of `ViewActivatorBase.Activate(...)`.

If you do not want the base activator to attach one or both mixins, override `Activate` yourself:

```csharp
public sealed class MyViewActivator(IServiceProvider provider, IViewStateManager stateManager)
    : ViewActivatorBase(provider, stateManager)
{
    public override object? Activate(Type viewType, object? parameter = null)
    {
        var view = (Control?)Activator.CreateInstance(viewType);
        if (view is null)
        {
            return null;
        }

        using var scope = provider.CreateScope();
        var accessor = scope.ServiceProvider.GetRequiredService<IViewContextAccessor>();
        accessor.Parameter = parameter;

        var viewModelType = FindViewModelType(viewType);
        var viewModel = ActivatorUtilities.GetServiceOrCreateInstance(scope.ServiceProvider, viewModelType);
        view.DataContext = viewModel;

        // Attach only what you want.
        ViewModelMixin.Attach(view);
        // ViewStateMixin.Attach(view, stateManager);

        return view;
    }

    protected override Type FindViewModelType(Type view) =>
        Type.GetType(view.FullName!.Replace("View", "ViewModel"))!;
}
```

### Installing the activator on a `Frame`

```csharp
using Huskui.Avalonia.Mvvm.Mixins;

FrameActivationMixin.Install(frame, serviceProvider.GetRequiredService<IViewActivator>());
```

This is equivalent to:

```csharp
frame.PageActivator = activator.Activate;
```

### Activation notes

- `ViewActivatorBase` only supports views derived from `Avalonia.Controls.Control`.
- `FindViewModelType` is application-defined. Convention-based mapping is common, but not required.
- The base `Activate` implementation attaches both `ViewModelMixin` and `ViewStateMixin` automatically.
- If you manually create views instead of using `IViewActivator`, you must manually attach `ViewModelMixin` and, if needed, `ViewStateMixin`.
- If you need different attach behavior, override `Activate` instead of using the base implementation as-is.
- `ViewActivatorBase` creates a temporary DI scope during activation. `IViewContext` is designed for this flow. Be careful with additional scoped services whose lifetime must outlive activation.

## 3. Passing Navigation Parameters

Navigation parameters are exposed through `IViewContext` and `IViewContext<T>`.

### Interfaces

```csharp
public interface IViewContext
{
    object? Parameter { get; }
    bool HasParameter { get; }
    T? GetParameter<T>() where T : class;
    bool TryGetParameter<T>(out T? parameter) where T : class;
    T GetRequiredParameter<T>() where T : class;
}

public interface IViewContext<out T> where T : class
{
    T? Parameter { get; }
}
```

### Example

```csharp
public record SearchArguments(string? Query, string? Label);

public sealed class SearchViewModel(
    IViewContext<SearchArguments> context,
    RepositoryService repositoryService)
    : ViewModelBase
{
    public string QueryText { get; } = context.Parameter?.Query ?? string.Empty;
}
```

Or use the untyped API when the parameter type is not fixed:

```csharp
public sealed class ErrorViewModel(IViewContext context) : ViewModelBase
{
    public Exception Exception { get; } = context.GetRequiredParameter<Exception>();
}
```

### Parameter notes

- `IViewContext<T>` is ideal when the page always expects one parameter type.
- `GetRequiredParameter<T>()` throws if the parameter is missing or of the wrong type.
- `Parameter == null` also means `HasParameter == false`.
- The parameter is only supplied through activation. If you instantiate the ViewModel manually, DI will not magically invent a context value.

## 4. View State

View state is for page-level UI state that should survive view recreation, such as:

- selected tab
- filter values
- search text
- scroll-related data you store explicitly
- temporary wizard progress

It is not a replacement for domain state, repositories, or long-lived application services.

### Minimal stateful ViewModel

```csharp
using Huskui.Avalonia.Mvvm.States;

public sealed partial class SearchViewModel : ViewModelBase, IStatefulViewModel<SearchViewModel.State>
{
    public sealed class State
    {
        public string Query { get; set; } = string.Empty;
        public int SelectedTabIndex { get; set; }
    }

    public State? ViewState { get; set; }
}
```

When the view loads and `ViewStateMixin` is attached:

1. `IViewStateManager.TryAttach` checks whether the ViewModel implements `IStatefulViewModel<T>`.
2. A state key is generated.
3. `IViewStateStore.GetOrCreate` loads persisted state or creates a new one.
4. The `ViewState` property is assigned.
5. When the view unloads or the `DataContext` changes away, the manager detaches and releases the state.

### Registration

```csharp
services.AddViewState();
```

By default this uses:

- `ReflectionViewStateManager`
- `DefaultViewStateFactory`
- `NullStatePersistence`
- `DefaultViewStateStore`

With custom persistence:

```csharp
services.AddViewState(builder =>
{
    builder.WithStatePersistence<MyViewStatePersistence>();
});
```

With full customization:

```csharp
services.AddViewState(builder =>
{
    builder.WithStateManager<MyViewStateManager>();
    builder.WithKeyFactory<MyViewStateKeyFactory>();
    builder.WithStatePersistence<MyViewStatePersistence>();
});
```

### Partitioning state with `IViewStateKeyProvider`

By default, the key factory uses the ViewModel type as the state identity.

That means all instances of the same ViewModel type resolve to the same state key unless you provide an additional partition key.

Use `IViewStateKeyProvider` when the same ViewModel type can represent multiple logical pages:

```csharp
public sealed partial class InstanceViewModel
    : ViewModelBase,
      IStatefulViewModel<InstanceViewModel.StateView>,
      IViewStateKeyProvider
{
    public StateView? ViewState { get; set; }

    public string ViewStateKey => instanceId;
}
```

This prevents different instances from accidentally sharing one state object.

### Persistence interface

```csharp
public interface IViewStatePersistence
{
    void Save(string key, Type stateType, object value);
    object? Load(string key, Type stateType);
}
```

Example:

```csharp
public sealed class MyViewStatePersistence(PersistenceService persistenceService)
    : IViewStatePersistence
{
    public void Save(string key, Type stateType, object value) =>
        persistenceService.SetViewState(key, value);

    public object? Load(string key, Type stateType) =>
        persistenceService.GetViewState(key, stateType);
}
```

### State persistence behavior

The default store is reference-counted per key.

- The first attach for a key loads or creates the state object.
- Additional attaches for the same key reuse the exact same in-memory object.
- `Save` is called when the last attached owner releases that key.
- `IViewStateStore.Flush()` forces `Save` for all still-cached states and clears the in-memory store.

This has several consequences:

- If two live views use the same state key, they share the same `ViewState` instance.
- If you do not want sharing, provide a more specific `ViewStateKey` or custom key factory.
- Changes made to `ViewState` are not automatically persisted on every property assignment.
- Persistence usually happens on detach or store flush, not immediately.

### Shutdown and manual flushing

Applications that use state persistence should flush before shutdown:

```csharp
serviceProvider.GetRequiredService<IViewStateStore>().Flush();
```

This is especially important when a view is still attached at application exit, because its state may never reach the final `Release` call.

### Very important note for custom persistence

`IViewStateStore.Flush()` exists to drain the default store's in-memory cache.

The default store keeps attached states in memory and normally calls `IViewStatePersistence.Save(...)` only when the last attached owner releases a state key.

If the application shuts down before some attached views unload, those states may never reach the final `Release(...)` call.

Calling `IViewStateStore.Flush()` forces the store to push every still-cached state into `IViewStatePersistence.Save(...)` and then clears the store cache.

So the main reason to call it is not "flush the persistence layer", but "make sure the store does not lose still-attached state during shutdown".

If your custom `IViewStatePersistence` also has its own buffering or delayed-write mechanism, that is a separate concern. In that case, you may still need to call your persistence layer's own flush/commit API, but that behavior is outside the contract of `IViewStateStore.Flush()`.

### State notes and pitfalls

- The default store creates a new state instance with `Activator.CreateInstance(stateType)` when `Load(...)` returns `null`. Your state type should therefore be instantiable, typically with a public parameterless constructor.
- `ViewState` is assigned by reflection through the `IStatefulViewModel<T>` interface. Keep the property writable.
- `NullStatePersistence` means restore/save is effectively disabled. You still get an in-memory state object during attachment, but nothing is persisted across releases.
- Put only serializable and restore-worthy UI data into `ViewState`. Do not store services, controls, disposable resources, or large graphs tied to live runtime state.

## 5. End-to-End Example

```csharp
// DI
services
    .AddViewModelActivation<AppViewActivator>()
    .AddViewState(builder => builder.WithStatePersistence<AppViewStatePersistence>());

// ViewModel
public sealed partial class SearchViewModel(
    IViewContext<SearchArgs> context,
    SearchService searchService)
    : ViewModelBase,
      IStatefulViewModel<SearchViewModel.State>
{
    public sealed class State
    {
        public string Query { get; set; } = string.Empty;
    }

    public State? ViewState { get; set; }

    protected override Task OnInitializeAsync(CancellationToken token)
    {
        if (context.Parameter is { Query: { } query } && ViewState is not null)
        {
            ViewState.Query = query;
        }

        return Task.CompletedTask;
    }
}

// Window / page host
FrameActivationMixin.Install(frame, serviceProvider.GetRequiredService<IViewActivator>());

// Shutdown
serviceProvider.GetRequiredService<IViewStateStore>().Flush();
```

## 6. Migration From The Old `IPageModel` Pattern

The package replaces the older pattern where lifecycle was owned by `Huskui.Avalonia.Controls.Page` and ViewModels implemented `IPageModel` directly.

| Old | New |
| --- | --- |
| `IPageModel` in UI package | `IViewModel` in MVVM package |
| mutable `PageToken` property | `CancellationToken` parameter |
| lifecycle hardcoded in `Page` | lifecycle attachable to any `Control` |
| parameter passing by ad-hoc manual wiring | `IViewContext` |
| no built-in page state restore model | `IStatefulViewModel<T>` + state store |

The new approach is more composable, testable, and reusable across different Huskui controls.
