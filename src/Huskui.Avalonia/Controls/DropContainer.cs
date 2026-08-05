using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":dragover")]
public class DropContainer : ContentControl
{
    public static readonly StyledProperty<object?> OverlayProperty =
        AvaloniaProperty.Register<DropContainer, object?>(nameof(Overlay));

    public static readonly RoutedEvent<DragOverEventArgs> DragOverEvent =
        RoutedEvent.Register<DropContainer, DragOverEventArgs>(nameof(DragOver), RoutingStrategies.Direct);

    public static readonly RoutedEvent<DropEventArgs> DropEvent =
        RoutedEvent.Register<DropContainer, DropEventArgs>(nameof(Drop), RoutingStrategies.Direct);

    public DropContainer() => DragDrop.SetAllowDrop(this, true);

    public object? Overlay
    {
        get => GetValue(OverlayProperty);
        set => SetValue(OverlayProperty, value);
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        AddHandler(DragDrop.DragEnterEvent, OnDragEnter);
        AddHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        AddHandler(DragDrop.DropEvent, OnDrop);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        RemoveHandler(DragDrop.DragEnterEvent, OnDragEnter);
        RemoveHandler(DragDrop.DragLeaveEvent, OnDragLeave);
        RemoveHandler(DragDrop.DropEvent, OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        PseudoClasses.Set(":dragover", false);
        var args = new DragOverEventArgs(this, e.DataTransfer);
        RaiseEvent(args);
        if (args.IsValid)
        {
            e.Handled = true;
            RaiseEvent(new DropEventArgs(this, e.DataTransfer));
        }
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        e.Handled = true;
        e.DragEffects = DragDropEffects.None;
        PseudoClasses.Set(":dragover", false);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
        var args = new DragOverEventArgs(this, e.DataTransfer);
        RaiseEvent(args);
        if (args.IsValid)
        {
            e.Handled = true;
            PseudoClasses.Set(":dragover", true);
            e.DragEffects = DragDropEffects.Copy;
        }
    }

    public event EventHandler<DragOverEventArgs> DragOver
    {
        add => AddHandler(DragOverEvent, value);
        remove => RemoveHandler(DragOverEvent, value);
    }

    public event EventHandler<DropEventArgs> Drop
    {
        add => AddHandler(DropEvent, value);
        remove => RemoveHandler(DropEvent, value);
    }

    #region Nested type: DragOverEventArgs

    public class DragOverEventArgs(object source, IDataTransfer data) : RoutedEventArgs(DragOverEvent, source)
    {
        public IDataTransfer Data => data;
        public bool IsValid { get; set; }
    }

    #endregion

    #region Nested type: DropEventArgs

    public class DropEventArgs(object source, IDataTransfer data) : RoutedEventArgs(DropEvent, source)
    {
        public IDataTransfer Data => data;
    }

    #endregion
}
