using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Huskui.Avalonia.Mvvm.Models;

namespace Huskui.Avalonia.Mvvm.Mixins;

public static class ViewModelAttachableMixin
{
    private static readonly ConditionalWeakTable<Control, Store> Stores = new();

    public static void Attach<T>(T self) where T : Control
    {
        var store = Stores.GetValue(self, _ => new());
        if (store.IsAttached)
        {
            return;
        }

        self.Loaded += OnLoaded;
        self.Unloaded += OnUnloaded;
        store.IsAttached = true;
    }

    private static async void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Control control)
        {
            if (!Design.IsDesignMode)
            {
                if (control.DataContext is IViewModel viewModel)
                {
                    var store = Stores.GetValue(control, _ => new());
                    store.InitialViewModel = viewModel;
                    store.ResetTokenSource();

                    SetState(sender, loading: true);
                    try
                    {
                        await viewModel.InitializeAsync(store.TokenSource.Token);
                        SetState(sender, finished: true);
                    }
                    catch
                    {
                        SetState(sender, failed: true);
                    }
                }
            }
        }
    }

    private static async void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (!Design.IsDesignMode)
        {
            if (sender is Control control)
            {
                var store = Stores.GetValue(control, _ => new());
                if (!store.TokenSource.IsCancellationRequested)
                {
                    await store.TokenSource.CancelAsync();
                }

                if (store.InitialViewModel is { } viewModel)
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
            }
        }
    }

    private static void SetState(object? sender, bool loading = false, bool finished = false, bool failed = false)
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
        public CancellationTokenSource TokenSource { get; private set; } = new();
        public IViewModel? InitialViewModel { get; set; }

        public void ResetTokenSource()
        {
            if (!TokenSource.IsCancellationRequested)
            {
                return;
            }

            TokenSource.Dispose();
            TokenSource = new();
        }
    }
}
