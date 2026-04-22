using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Mvvm.States;

namespace Huskui.Avalonia.Mvvm.Mixins;

public static class ViewStateMixin
{
    private static readonly ConditionalWeakTable<Control, Store> Stores = new();

    public static void Attach<T>(T control, IViewStateManager manager) where T : Control
    {
        var store = Stores.GetValue(control, _ => new());
        if (store.IsAttached)
        {
            return;
        }

        store.Manager = manager;
        control.Loaded += OnLoaded;
        control.Unloaded += OnUnloaded;
        control.DataContextChanged += OnDataContextChanged;
        store.IsAttached = true;
    }

    private static void OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        var store = Stores.GetValue(control, _ => new());
        store.IsLoaded = true;
        Reconcile(control, store);
    }

    private static void OnUnloaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        var store = Stores.GetValue(control, _ => new());
        store.IsLoaded = false;
        Reconcile(control, store);
    }

    private static void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (sender is not Control control)
        {
            return;
        }

        var store = Stores.GetValue(control, _ => new());
        Reconcile(control, store);
    }

    private static void Reconcile(Control control, Store store)
    {
        var desiredViewModel = store.IsLoaded ? control.DataContext : null;
        if (ReferenceEquals(store.AttachedViewModel, desiredViewModel))
        {
            return;
        }

        if (store.AttachedViewModel is not null)
        {
            store.Manager!.Detach(store.AttachedViewModel);
            store.AttachedViewModel = null;
        }

        if (desiredViewModel is not null && store.Manager?.TryAttach(desiredViewModel) == true)
        {
            store.AttachedViewModel = desiredViewModel;
        }
    }

    private sealed class Store
    {
        public bool IsAttached { get; set; }
        public bool IsLoaded { get; set; }
        public IViewStateManager? Manager { get; set; }
        public object? AttachedViewModel { get; set; }
    }
}
