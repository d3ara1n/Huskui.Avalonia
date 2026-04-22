using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Huskui.Avalonia.Mvvm.Models;

namespace Huskui.Avalonia.Mvvm.Mixins;

public static class ViewModelMixin
{
    private static readonly ConditionalWeakTable<Control, Store> Stores = new();

    public static void Attach<T>(T self)
        where T : Control
    {
        var store = Stores.GetValue(self, _ => new());
        if (store.IsAttached)
        {
            return;
        }

        self.Loaded += OnLoaded;
        self.Unloaded += OnUnloaded;
        self.DataContextChanged += OnDataContextChanged;
        store.IsAttached = true;
    }

    private static async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control control)
        {
            var store = Stores.GetValue(control, _ => new());
            store.IsLoaded = true;
            await ReconcileAsync(control, store);
        }
    }

    private static async void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control control)
        {
            var store = Stores.GetValue(control, _ => new());
            store.IsLoaded = false;
            store.CancelCurrent();
            await ReconcileAsync(control, store);
        }
    }

    private static async void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (sender is Control control)
        {
            var store = Stores.GetValue(control, _ => new());
            if (
                store is { IsLoaded: true, AttachedViewModel: not null }
                && !ReferenceEquals(store.AttachedViewModel, control.DataContext)
            )
            {
                store.CancelCurrent();
            }

            await ReconcileAsync(control, store);
        }
    }

    private static async Task ReconcileAsync(Control control, Store store)
    {
        if (Design.IsDesignMode)
        {
            return;
        }

        await store.Gate.WaitAsync();
        try
        {
            var desiredViewModel = store.IsLoaded ? control.DataContext as IViewModel : null;
            if (ReferenceEquals(store.AttachedViewModel, desiredViewModel))
            {
                return;
            }

            if (store.AttachedViewModel is { } currentViewModel)
            {
                store.AttachedViewModel = null;
                await DeinitializeAsync(currentViewModel);
                SetState(control);
            }

            if (desiredViewModel is null)
            {
                return;
            }

            var token = store.ReplaceTokenSource();
            store.AttachedViewModel = desiredViewModel;

            SetState(control, loading: true);
            try
            {
                await desiredViewModel.InitializeAsync(token);
                if (
                    !token.IsCancellationRequested
                    && ReferenceEquals(store.AttachedViewModel, desiredViewModel)
                )
                {
                    SetState(control, finished: true);
                }
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested)
            {
                if (ReferenceEquals(store.AttachedViewModel, desiredViewModel))
                {
                    SetState(control);
                }
            }
            catch
            {
                if (
                    !token.IsCancellationRequested
                    && ReferenceEquals(store.AttachedViewModel, desiredViewModel)
                )
                {
                    SetState(control, failed: true);
                }
            }
        }
        finally
        {
            store.Gate.Release();
        }
    }

    private static async Task DeinitializeAsync(IViewModel viewModel)
    {
        try
        {
            await viewModel.DeinitializeAsync();
        }
        catch
        {
            // nothing happened
        }
    }

    private static void SetState(
        object? sender,
        bool loading = false,
        bool finished = false,
        bool failed = false
    )
    {
        if (sender is Control control)
        {
            var pseudoClasses = (IPseudoClasses)control.Classes;
            pseudoClasses.Set(":loading", loading);
            pseudoClasses.Set(":finished", finished);
            pseudoClasses.Set(":failed", failed);
        }
    }

    private sealed class Store
    {
        public bool IsAttached { get; set; }
        public bool IsLoaded { get; set; }
        public SemaphoreSlim Gate { get; } = new(1, 1);
        public CancellationTokenSource TokenSource { get; private set; } = new();
        public IViewModel? AttachedViewModel { get; set; }

        public CancellationToken ReplaceTokenSource()
        {
            var oldTokenSource = TokenSource;
            TokenSource = new();
            oldTokenSource.Dispose();
            return TokenSource.Token;
        }

        public void CancelCurrent()
        {
            if (!TokenSource.IsCancellationRequested)
            {
                _ = TokenSource.CancelAsync();
            }
        }
    }
}
