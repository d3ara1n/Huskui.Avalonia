using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_Handle, typeof(Control))]
[TemplatePart(PART_ResizeLeft, typeof(Control))]
[TemplatePart(PART_ResizeRight, typeof(Control))]
[TemplatePart(PART_ResizeTop, typeof(Control))]
[TemplatePart(PART_CloseButton, typeof(Button))]
[TemplatePart(PART_ToggleStateButton, typeof(Button))]
[PseudoClasses(":open", ":dragging", ":resizing")]
public class Drawer : ContentControl
{
    public const string PART_Handle = nameof(PART_Handle);
    public const string PART_ResizeLeft = nameof(PART_ResizeLeft);
    public const string PART_ResizeRight = nameof(PART_ResizeRight);
    public const string PART_ResizeTop = nameof(PART_ResizeTop);
    public const string PART_CloseButton = nameof(PART_CloseButton);
    public const string PART_ToggleStateButton = nameof(PART_ToggleStateButton);

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(OffsetX));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Drawer, string?>(nameof(Title));

    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(HeaderHeight), 42d);

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

    static Drawer() => AffectsArrange<Drawer>(OffsetXProperty);

    public bool IsOpen
    {
        get => GetValue(IsOpenProperty);
        set => SetValue(IsOpenProperty, value);
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

    public double HeaderHeight
    {
        get => GetValue(HeaderHeightProperty);
        set => SetValue(HeaderHeightProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(Drawer);

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdatePseudoClasses();
        InvalidateMeasure();
    }


    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsOpenProperty)
        {
            OnIsOpenChanged(change);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

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
            _resizeLeft.PointerPressed += (s, ev) => OnResizePointerPressed(s, ev, ref _isResizingLeft);
            _resizeLeft.PointerMoved += OnResizePointerMoved;
            _resizeLeft.PointerReleased += OnResizePointerReleased;
        }

        if (_resizeRight != null)
        {
            _resizeRight.PointerPressed += (s, ev) => OnResizePointerPressed(s, ev, ref _isResizingRight);
            _resizeRight.PointerMoved += OnResizePointerMoved;
            _resizeRight.PointerReleased += OnResizePointerReleased;
        }

        if (_resizeTop != null)
        {
            _resizeTop.PointerPressed += (s, ev) => OnResizePointerPressed(s, ev, ref _isResizingTop);
            _resizeTop.PointerMoved += OnResizePointerMoved;
            _resizeTop.PointerReleased += OnResizePointerReleased;
        }

        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":dragging", _isDragging);
        PseudoClasses.Set(":resizing", _isResizingLeft || _isResizingRight || _isResizingTop);
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

    private void OnResizePointerPressed(object? sender, PointerPressedEventArgs e, ref bool resizingFlag)
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
        if ((_isResizingLeft || _isResizingRight || _isResizingTop) && Parent is Visual parent)
        {
            var currentPoint = e.GetPosition(parent);
            var delta = currentPoint - _lastPoint;

            if (_isResizingLeft)
            {
                var newWidth = Width - delta.X;
                if (newWidth > MinWidth)
                {
                    Width = newWidth;
                    OffsetX += delta.X;
                }
            }
            else if (_isResizingRight)
            {
                var newWidth = Width + delta.X;
                if (newWidth > MinWidth)
                {
                    Width = newWidth;
                }
            }
            else if (_isResizingTop && IsOpen) // Only allow height resize when open
            {
                var newHeight = Height - delta.Y;
                if (newHeight > MinHeight)
                {
                    Height = newHeight;
                }
            }

            _lastPoint = currentPoint;

            if (Parent is Control parentControl)
            {
                parentControl.InvalidateArrange();
            }

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

    protected override Size MeasureOverride(Size availableSize)
    {
        if (!IsOpen)
        {
            // When closed, height is restricted to HeaderHeight
            // We still measure content but don't give it space?
            // Or we just force Height to HeaderHeight?
            // Let's force the desired size to be Width x HeaderHeight

            // We need to call base.MeasureOverride to ensure children are measured
            base.MeasureOverride(availableSize);

            return new(Width, HeaderHeight);
        }

        return base.MeasureOverride(availableSize);
    }
}
