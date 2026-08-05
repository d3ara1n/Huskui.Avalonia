using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_Handle, typeof(Control))]
[TemplatePart(PART_ResizeLeft, typeof(Control))]
[TemplatePart(PART_ResizeRight, typeof(Control))]
[TemplatePart(PART_ResizeTop, typeof(Control))]
[TemplatePart(PART_ToggleStateButton, typeof(Button))]
[PseudoClasses(":expanded", ":dragging", ":resizing")]
public class Drawer : ContentControl
{
    public const string PART_Handle = nameof(PART_Handle);
    public const string PART_ResizeLeft = nameof(PART_ResizeLeft);
    public const string PART_ResizeRight = nameof(PART_ResizeRight);
    public const string PART_ResizeTop = nameof(PART_ResizeTop);
    public const string PART_CloseButton = nameof(PART_CloseButton);
    public const string PART_ToggleStateButton = nameof(PART_ToggleStateButton);

    public static readonly StyledProperty<bool> IsExpandedProperty = AvaloniaProperty.Register<
        Drawer,
        bool
    >(nameof(IsExpanded), true);

    public static readonly RoutedEvent<RoutedEventArgs> ExpandedEvent = RoutedEvent.Register<
        Drawer,
        RoutedEventArgs
    >(nameof(Expanded), RoutingStrategies.Bubble);

    public static readonly RoutedEvent<RoutedEventArgs> CollapsedEvent = RoutedEvent.Register<
        Drawer,
        RoutedEventArgs
    >(nameof(Collapsed), RoutingStrategies.Bubble);

    public static readonly StyledProperty<double> OffsetXProperty = AvaloniaProperty.Register<
        Drawer,
        double
    >(nameof(OffsetX));

    public static readonly StyledProperty<string?> TitleProperty = AvaloniaProperty.Register<
        Drawer,
        string?
    >(nameof(Title));

    public static readonly StyledProperty<bool> IsDismissableProperty = AvaloniaProperty.Register<
        Drawer,
        bool
    >(nameof(IsDismissable), true);

    public static readonly StyledProperty<bool> IsToggleButtonVisibleProperty =
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsToggleButtonVisible), true);

    public static readonly StyledProperty<double> HeaderHeightProperty = AvaloniaProperty.Register<
        Drawer,
        double
    >(nameof(HeaderHeight), 42d);

    private DrawerPanel? _drawerPanel;

    private Control? _header;
    private bool _isDragging;
    private bool _isResizingLeft;
    private bool _isResizingRight;
    private bool _isResizingTop;

    private Point _lastPoint;
    private Control? _resizeLeft;
    private Control? _resizeRight;
    private Control? _resizeTop;
    private double? _expandedHeight;

    static Drawer() => AffectsArrange<Drawer>(OffsetXProperty);

    public Drawer() => DismissCommand = new InternalCommand(Dismiss);

    public bool IsDismissed { get; internal set; }

    public ICommand DismissCommand { get; }

    public bool IsExpanded
    {
        get => GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public event EventHandler<RoutedEventArgs>? Expanded
    {
        add => AddHandler(ExpandedEvent, value);
        remove => RemoveHandler(ExpandedEvent, value);
    }

    public event EventHandler<RoutedEventArgs>? Collapsed
    {
        add => AddHandler(CollapsedEvent, value);
        remove => RemoveHandler(CollapsedEvent, value);
    }

    public double OffsetX
    {
        get => GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsDismissable
    {
        get => GetValue(IsDismissableProperty);
        set => SetValue(IsDismissableProperty, value);
    }

    public bool IsToggleButtonVisible
    {
        get => GetValue(IsToggleButtonVisibleProperty);
        set => SetValue(IsToggleButtonVisibleProperty, value);
    }

    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Drawer);

    private void OnIsExpandedChanged(AvaloniaPropertyChangedEventArgs e)
    {
        var wasExpanded = e.GetOldValue<bool>();
        var isExpanded = e.GetNewValue<bool>();

        if (wasExpanded == isExpanded)
        {
            return;
        }

        if (isExpanded)
        {
            if (_expandedHeight.HasValue)
            {
                Height = _expandedHeight.Value;
            }
        }
        else
        {
            if (!double.IsNaN(Height))
            {
                _expandedHeight = Height;
            }
            Height = HeaderHeight;
        }
        UpdatePseudoClasses();
        InvalidateMeasure();
        InvalidateArrange();
        _drawerPanel?.InvalidateArrange();

        if (isExpanded)
        {
            OnExpanded(new(ExpandedEvent, this));
        }
        else
        {
            OnCollapsed(new(CollapsedEvent, this));
        }
    }

    protected virtual void OnExpanded(RoutedEventArgs e) => RaiseEvent(e);

    protected virtual void OnCollapsed(RoutedEventArgs e) => RaiseEvent(e);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsExpandedProperty)
        {
            OnIsExpandedChanged(change);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnregisterHandlers();

        _header = e.NameScope.Find<Control>(PART_Handle);
        _resizeLeft = e.NameScope.Find<Control>(PART_ResizeLeft);
        _resizeRight = e.NameScope.Find<Control>(PART_ResizeRight);
        _resizeTop = e.NameScope.Find<Control>(PART_ResizeTop);
        _drawerPanel = this.FindAncestorOfType<DrawerPanel>();

        if (_header != null)
        {
            _header.PointerPressed += OnHeaderPointerPressed;
            _header.PointerMoved += OnHandlePointerMoved;
            _header.PointerReleased += OnHandlePointerReleased;
        }

        if (_resizeLeft != null)
        {
            _resizeLeft.PointerPressed += (s, ev) =>
                OnResizePointerPressed(s, ev, ref _isResizingLeft);
            _resizeLeft.PointerMoved += OnResizePointerMoved;
            _resizeLeft.PointerReleased += OnResizePointerReleased;
        }

        if (_resizeRight != null)
        {
            _resizeRight.PointerPressed += (s, ev) =>
                OnResizePointerPressed(s, ev, ref _isResizingRight);
            _resizeRight.PointerMoved += OnResizePointerMoved;
            _resizeRight.PointerReleased += OnResizePointerReleased;
        }

        if (_resizeTop != null)
        {
            _resizeTop.PointerPressed += (s, ev) =>
                OnResizePointerPressed(s, ev, ref _isResizingTop);
            _resizeTop.PointerMoved += OnResizePointerMoved;
            _resizeTop.PointerReleased += OnResizePointerReleased;
        }

        UpdatePseudoClasses();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnregisterHandlers();
    }

    private void UnregisterHandlers()
    {
        if (_header != null)
        {
            _header.PointerPressed -= OnHeaderPointerPressed;
            _header.PointerMoved -= OnHandlePointerMoved;
            _header.PointerReleased -= OnHandlePointerReleased;
        }

        if (_resizeLeft != null)
        {
            _resizeLeft.PointerMoved -= OnResizePointerMoved;
            _resizeLeft.PointerReleased -= OnResizePointerReleased;
        }

        if (_resizeRight != null)
        {
            _resizeRight.PointerMoved -= OnResizePointerMoved;
            _resizeRight.PointerReleased -= OnResizePointerReleased;
        }

        if (_resizeTop != null)
        {
            _resizeTop.PointerMoved -= OnResizePointerMoved;
            _resizeTop.PointerReleased -= OnResizePointerReleased;
        }
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":expanded", IsExpanded);
        PseudoClasses.Set(":dragging", _isDragging);
        PseudoClasses.Set(":resizing", _isResizingLeft || _isResizingRight || _isResizingTop);
    }

    protected override void OnGotFocus(FocusChangedEventArgs e)
    {
        base.OnGotFocus(e);
        BringToFront();
    }

    private void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _lastPoint = e.GetPosition(Parent as Visual);
            UpdatePseudoClasses();
            e.Pointer.Capture(_header);
            e.Handled = true;
        }
    }

    private void OnHandlePointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && Parent is Visual parent)
        {
            var currentPoint = e.GetPosition(parent);
            var delta = currentPoint - _lastPoint;
            OffsetX += delta.X;
            _lastPoint = currentPoint;

            // Request layout update on parent to handle clamping
            _drawerPanel?.InvalidateArrange();

            e.Handled = true;
        }
    }

    private void OnHandlePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            UpdatePseudoClasses();
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }

    private void OnResizePointerPressed(
        object? sender,
        PointerPressedEventArgs e,
        ref bool resizingFlag
    )
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            resizingFlag = true;
            _lastPoint = e.GetPosition(Parent as Visual);
            UpdatePseudoClasses();
            e.Pointer.Capture(sender as Control);
            e.Handled = true;
        }
    }

    private void OnResizePointerMoved(object? sender, PointerEventArgs e)
    {
        if (
            (_isResizingLeft || _isResizingRight || _isResizingTop)
            && Parent is Control parentControl
        )
        {
            var currentPoint = e.GetPosition(parentControl);
            var delta = currentPoint - _lastPoint;
            var parentWidth = parentControl.Bounds.Width;
            var parentHeight = parentControl.Bounds.Height;
            if (_isResizingLeft)
            {
                var maxWidth = Math.Max(MinWidth, Width + OffsetX);
                var newWidth = Width - delta.X;
                newWidth = Math.Clamp(newWidth, MinWidth, maxWidth);
                var actualDelta = Width - newWidth;
                Width = newWidth;
                OffsetX += actualDelta;
            }
            else if (_isResizingRight)
            {
                var maxWidth = Math.Max(MinWidth, parentWidth - OffsetX);
                var newWidth = Width + delta.X;
                newWidth = Math.Clamp(newWidth, MinWidth, maxWidth);
                Width = newWidth;
            }
            else if (_isResizingTop && IsExpanded)
            {
                var minOpenHeight = Math.Max(HeaderHeight, 100);
                var maxHeight = Math.Max(minOpenHeight, parentHeight);
                var newHeight = Height - delta.Y;
                newHeight = Math.Clamp(newHeight, minOpenHeight, maxHeight);
                Height = newHeight;
                _expandedHeight = newHeight;
            }
            _lastPoint = currentPoint;
            parentControl.InvalidateArrange();
            e.Handled = true;
        }
    }

    private void OnResizePointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isResizingLeft || _isResizingRight || _isResizingTop)
        {
            _isResizingLeft = false;
            _isResizingRight = false;
            _isResizingTop = false;
            UpdatePseudoClasses();
            e.Pointer.Capture(null);
            e.Handled = true;
        }
    }

    public void BringToFront()
    {
        var args = new DrawerHost.BringToFrontRequestedEventArgs(this) { Drawer = this };
        RaiseEvent(args);
    }

    public void Dismiss()
    {
        var args = new DrawerHost.DismissRequestedEventArgs(this) { Drawer = this };
        RaiseEvent(args);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (!IsExpanded)
        {
            // When collapsed, height is restricted to HeaderHeight
            // We still measure content but don't give it space?
            // Or we just force Height to HeaderHeight?
            // Let's force the desired size to be Width x HeaderHeight

            // We need to call base.MeasureOverride to ensure children are measured

            // No, we don't mesaure its children
            // base.MeasureOverride(availableSize);

            return new(Width, HeaderHeight);
        }

        return base.MeasureOverride(availableSize);
    }
}
