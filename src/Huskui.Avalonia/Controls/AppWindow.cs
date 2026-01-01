using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ToastHost, typeof(OverlayHost))]
[TemplatePart(PART_SidebarHost, typeof(OverlayHost))]
[TemplatePart(PART_ModalHost, typeof(OverlayHost))]
[TemplatePart(PART_DialogHost, typeof(OverlayHost))]
[TemplatePart(PART_GrowlHost, typeof(GrowlHost))]
[TemplatePart(PART_DrawerHost, typeof(DrawerHost))]
[PseudoClasses(":obstructed")]
public class AppWindow : Window
{
    public const string PART_ToastHost = nameof(PART_ToastHost);
    public const string PART_SidebarHost = nameof(PART_SidebarHost);
    public const string PART_ModalHost = nameof(PART_ModalHost);
    public const string PART_DialogHost = nameof(PART_DialogHost);
    public const string PART_GrowlHost = nameof(PART_GrowlHost);
    public const string PART_DrawerHost = nameof(PART_DrawerHost);

    public static readonly DirectProperty<AppWindow, bool> IsMaximizedProperty =
        AvaloniaProperty.RegisterDirect<AppWindow, bool>(nameof(IsMaximized),
                                                         o => o.IsMaximized,
                                                         (o, v) => o.IsMaximized = v);

    private OverlayHost? _dialogHost;

    private DrawerHost? _drawerHost;
    private GrowlHost? _growlHost;
    private OverlayHost? _modalHost;
    private OverlayHost? _sidebarHost;
    private OverlayHost? _toastHost;

    protected override Type StyleKeyOverride => typeof(AppWindow);

    public bool IsMaximized
    {
        get;
        set => SetAndRaise(IsMaximizedProperty, ref field, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == WindowStateProperty)
        {
            IsMaximized = WindowState == WindowState.Maximized;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnregisterHandlers();

        _growlHost = e.NameScope.Find<GrowlHost>(PART_GrowlHost);
        _toastHost = e.NameScope.Find<OverlayHost>(PART_ToastHost);
        _sidebarHost = e.NameScope.Find<OverlayHost>(PART_SidebarHost);
        _modalHost = e.NameScope.Find<OverlayHost>(PART_ModalHost);
        _dialogHost = e.NameScope.Find<OverlayHost>(PART_DialogHost);
        _drawerHost = e.NameScope.Find<DrawerHost>(PART_DrawerHost);

        ArgumentNullException.ThrowIfNull(_toastHost);
        ArgumentNullException.ThrowIfNull(_sidebarHost);
        ArgumentNullException.ThrowIfNull(_modalHost);
        ArgumentNullException.ThrowIfNull(_dialogHost);
        ArgumentNullException.ThrowIfNull(_growlHost);
        ArgumentNullException.ThrowIfNull(_drawerHost);

        _toastHost.IsPresentChanged += UpdateObstructed;
        _sidebarHost.IsPresentChanged += UpdateObstructed;
        _modalHost.IsPresentChanged += UpdateObstructed;
        _dialogHost.IsPresentChanged += UpdateObstructed;
        _toastHost.MaskPointerPressed += OnMaskPointerPressed;
        _sidebarHost.MaskPointerPressed += OnMaskPointerPressed;
        _modalHost.MaskPointerPressed += OnMaskPointerPressed;
        _dialogHost.MaskPointerPressed += OnMaskPointerPressed;

        LogicalChildren.Add(_drawerHost);
        LogicalChildren.Add(_toastHost);
        LogicalChildren.Add(_sidebarHost);
        LogicalChildren.Add(_modalHost);
        LogicalChildren.Add(_dialogHost);
        LogicalChildren.Add(_growlHost);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnregisterHandlers();
    }

    private void UnregisterHandlers()
    {
        if (_toastHost != null)
        {
            _toastHost.IsPresentChanged -= UpdateObstructed;
            _toastHost.MaskPointerPressed -= OnMaskPointerPressed;
        }

        if (_sidebarHost != null)
        {
            _sidebarHost.IsPresentChanged -= UpdateObstructed;
            _sidebarHost.MaskPointerPressed -= OnMaskPointerPressed;
        }

        if (_modalHost != null)
        {
            _modalHost.IsPresentChanged -= UpdateObstructed;
            _modalHost.MaskPointerPressed -= OnMaskPointerPressed;
        }

        if (_dialogHost != null)
        {
            _dialogHost.IsPresentChanged -= UpdateObstructed;
            _dialogHost.MaskPointerPressed -= OnMaskPointerPressed;
        }
    }

    private void OnMaskPointerPressed(object? sender, OverlayHost.MaskPointerPressedEventArgs e)
    {
        BeginMoveDrag(e.Inner);
        e.Handled = true;
    }

    private void UpdateObstructed(object? _, PropertyChangedRoutedEventArgs<bool> __)
    {
        ArgumentNullException.ThrowIfNull(_toastHost);
        ArgumentNullException.ThrowIfNull(_sidebarHost);
        ArgumentNullException.ThrowIfNull(_modalHost);
        ArgumentNullException.ThrowIfNull(_dialogHost);

        PseudoClasses.Set(":obstructed",
                          _toastHost.IsPresent
                       || _sidebarHost.IsPresent
                       || _modalHost.IsPresent
                       || _dialogHost.IsPresent);
    }

    public void PopToast(Toast toast)
    {
        ArgumentNullException.ThrowIfNull(_toastHost);
        _toastHost.Pop(toast);
    }

    public void PopSidebar(Sidebar sidebar)
    {
        ArgumentNullException.ThrowIfNull(_sidebarHost);
        _sidebarHost.Pop(sidebar);
    }

    public void PopDialog(Dialog dialog)
    {
        ArgumentNullException.ThrowIfNull(_dialogHost);
        _dialogHost.Pop(dialog);
    }

    public void PopModal(Modal modal)
    {
        ArgumentNullException.ThrowIfNull(_modalHost);
        _modalHost.Pop(modal);
    }

    public void PopGrowl(GrowlItem growl)
    {
        ArgumentNullException.ThrowIfNull(_growlHost);
        _growlHost.Pop(growl);
    }

    public void PopDrawer(Drawer drawer)
    {
        ArgumentNullException.ThrowIfNull(_drawerHost);
        _drawerHost.Pop(drawer);
    }
}
