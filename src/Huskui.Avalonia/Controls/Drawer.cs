using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_Header, typeof(Control))]
[TemplatePart(PART_ResizeLeft, typeof(Control))]
[TemplatePart(PART_ResizeRight, typeof(Control))]
[TemplatePart(PART_ResizeTop, typeof(Control))]
[TemplatePart(PART_CloseButton, typeof(Button))]
[TemplatePart(PART_ToggleStateButton, typeof(Button))]
[PseudoClasses(":open", ":closed", ":dragging", ":resizing")]
public class Drawer : ContentControl
{
    public const string PART_Header = "PART_Header";
    public const string PART_ResizeLeft = "PART_ResizeLeft";
    public const string PART_ResizeRight = "PART_ResizeRight";
    public const string PART_ResizeTop = "PART_ResizeTop";
    public const string PART_CloseButton = "PART_CloseButton";
    public const string PART_ToggleStateButton = "PART_ToggleStateButton";

    public static readonly StyledProperty<bool> IsOpenProperty =
        AvaloniaProperty.Register<Drawer, bool>(nameof(IsOpen), true);

    public static readonly StyledProperty<double> OffsetXProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(OffsetX));

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<Drawer, string?>(nameof(Title));

    public static readonly StyledProperty<double> HeaderHeightProperty =
        AvaloniaProperty.Register<Drawer, double>(nameof(HeaderHeight), 32.0);

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

    private Control? _header;
    private Control? _resizeLeft;
    private Control? _resizeRight;
    private Control? _resizeTop;
    private Button? _closeButton;
    private Button? _toggleStateButton;

    private Point _lastPoint;
    private bool _isDragging;
    private bool _isResizingLeft;
    private bool _isResizingRight;
    private bool _isResizingTop;

    protected override Type StyleKeyOverride => typeof(Drawer);

    static Drawer()
    {
        IsOpenProperty.Changed.AddClassHandler<Drawer>((x, e) => x.OnIsOpenChanged(e));
    }

    private void OnIsOpenChanged(AvaloniaPropertyChangedEventArgs e)
    {
        UpdatePseudoClasses();
        InvalidateMeasure();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _header = e.NameScope.Find<Control>(PART_Header);
        _resizeLeft = e.NameScope.Find<Control>(PART_ResizeLeft);
        _resizeRight = e.NameScope.Find<Control>(PART_ResizeRight);
        _resizeTop = e.NameScope.Find<Control>(PART_ResizeTop);
        _closeButton = e.NameScope.Find<Button>(PART_CloseButton);
        _toggleStateButton = e.NameScope.Find<Button>(PART_ToggleStateButton);

        if (_header != null)
        {
            _header.PointerPressed += OnHeaderPointerPressed;
            _header.PointerMoved += OnHeaderPointerMoved;
            _header.PointerReleased += OnHeaderPointerReleased;
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

        if (_closeButton != null)
        {
            _closeButton.Click += (s, ev) =>
            {
                // Logic to close/remove the drawer. 
                // For now, we can just hide it or raise an event.
                // Let's assume the parent might want to remove it.
                IsVisible = false; 
            };
        }

        if (_toggleStateButton != null)
        {
            _toggleStateButton.Click += (s, ev) => IsOpen = !IsOpen;
        }

        UpdatePseudoClasses();
    }

    private void UpdatePseudoClasses()
    {
        PseudoClasses.Set(":open", IsOpen);
        PseudoClasses.Set(":closed", !IsOpen);
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

    private void OnHeaderPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && Parent is Visual parent)
        {
            var currentPoint = e.GetPosition(parent);
            var delta = currentPoint - _lastPoint;
            OffsetX += delta.X;
            _lastPoint = currentPoint;
            
            // Request layout update on parent to handle clamping
            if (Parent is Control parentControl)
            {
                parentControl.InvalidateArrange();
            }
            
            e.Handled = true;
        }
    }

    private void OnHeaderPointerReleased(object? sender, PointerReleasedEventArgs e)
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
            
            return new Size(Width, HeaderHeight);
        }
        
        return base.MeasureOverride(availableSize);
    }
}