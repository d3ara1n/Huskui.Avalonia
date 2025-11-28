using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":dragover", ":drop")]
public class DropZone : ContentControl
{
    public static readonly RoutedEvent<DragOverEventArgs> DragOverEvent =
        RoutedEvent.Register<DropZone, DragOverEventArgs>(nameof(DragOver), RoutingStrategies.Direct);

    public static readonly RoutedEvent<DropEventArgs> DropEvent =
        RoutedEvent.Register<DropZone, DropEventArgs>(nameof(Drop), RoutingStrategies.Direct);

    public static readonly StyledProperty<object?> ModelProperty =
        AvaloniaProperty.Register<DropZone, object?>(nameof(Model));


    public DropZone() => DragDrop.SetAllowDrop(this, true);

    public object? Model
    {
        get => GetValue(ModelProperty);
        set => SetValue(ModelProperty, value);
    }


    public event EventHandler<DragOverEventArgs> DragOver
    {
        add => AddHandler(DragOverEvent, value);
        remove => RemoveHandler(DragOverEvent, value);
    }

    public event EventHandler<DropEventArgs> Drop
    {
        add => AddHandler(DragOverEvent, value);
        remove => RemoveHandler(DragOverEvent, value);
    }

    private void OnDragEnter(object? sender, DragEventArgs e)
    {
			var args = new DragOverEventArgs(e.DataTransfer) { RoutedEvent = DragOverEvent };
        RaiseEvent(args);
        PseudoClasses.Set(":drop", false);
        if (args.Accepted)
        {
            e.Handled = true;
            e.DragEffects = DragDropEffects.Copy;
            PseudoClasses.Set(":dragover", true);
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
            PseudoClasses.Set(":dragover", false);
        }
    }

    private void OnDragLeave(object? sender, DragEventArgs e)
    {
        e.Handled = true;
        PseudoClasses.Set(":dragover", false);
        PseudoClasses.Set(":drop", Model != null);
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        e.DragEffects = DragDropEffects.None;
        PseudoClasses.Set(":dragover", false);
			var validation = new DragOverEventArgs(e.DataTransfer) { RoutedEvent = DragOverEvent };
        RaiseEvent(validation);
        if (validation.Accepted)
        {
				var args = new DropEventArgs(e.DataTransfer) { RoutedEvent = DropEvent };
            RaiseEvent(args);
            if (args.Model != null)
            {
                e.Handled = true;
                Model = args.Model;
                return;
            }
        }

        Model = null;
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

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ModelProperty)
        {
            PseudoClasses.Set(":drop", change.NewValue != null);
        }
    }

    #region Nested type: DragOverEventArgs

		public class DragOverEventArgs(IDataTransfer data) : RoutedEventArgs
    {
			public IDataTransfer Data => data;
        public bool Accepted { get; set; }
    }

    #endregion

    #region Nested type: DropEventArgs

		public class DropEventArgs(IDataTransfer data) : RoutedEventArgs
    {
			public IDataTransfer Data => data;
        public object? Model { get; set; }
    }

    #endregion
}
