using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ToastHost, typeof(OverlayHost))]
[TemplatePart(PART_DrawerHost, typeof(OverlayHost))]
[TemplatePart(PART_ModalHost, typeof(OverlayHost))]
[TemplatePart(PART_DialogHost, typeof(OverlayHost))]
[TemplatePart(PART_NotificationHost, typeof(NotificationHost))]
[PseudoClasses(":obstructed")]
public class AppWindow : Window
{
    public const string PART_ToastHost = nameof(PART_ToastHost);
    public const string PART_DrawerHost = nameof(PART_DrawerHost);
    public const string PART_ModalHost = nameof(PART_ModalHost);
    public const string PART_DialogHost = nameof(PART_DialogHost);
    public const string PART_NotificationHost = nameof(PART_NotificationHost);

    public static readonly DirectProperty<AppWindow, bool> IsMaximizedProperty =
        AvaloniaProperty.RegisterDirect<AppWindow, bool>(nameof(IsMaximized),
                                                         o => o.IsMaximized,
                                                         (o, v) => o.IsMaximized = v);

    private OverlayHost? _toastHost;
    private OverlayHost? _drawerHost;
    private OverlayHost? _modalHost;
    private OverlayHost? _dialogHost;
    private NotificationHost? _notificationHost;

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

        if (_toastHost != null)
        {
            _toastHost.IsPresentChanged -= UpdateObstructed;
            _toastHost.MaskPointerPressed -= OnMaskPointerPressed;
        }

        if (_drawerHost != null)
        {
            _drawerHost.IsPresentChanged -= UpdateObstructed;
            _drawerHost.MaskPointerPressed -= OnMaskPointerPressed;
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

        _notificationHost = e.NameScope.Find<NotificationHost>(PART_NotificationHost);
        _toastHost = e.NameScope.Find<OverlayHost>(PART_ToastHost);
        _drawerHost = e.NameScope.Find<OverlayHost>(PART_DrawerHost);
        _modalHost = e.NameScope.Find<OverlayHost>(PART_ModalHost);
        _dialogHost = e.NameScope.Find<OverlayHost>(PART_DialogHost);

        ArgumentNullException.ThrowIfNull(_toastHost);
        ArgumentNullException.ThrowIfNull(_drawerHost);
        ArgumentNullException.ThrowIfNull(_modalHost);
        ArgumentNullException.ThrowIfNull(_dialogHost);
        ArgumentNullException.ThrowIfNull(_notificationHost);

        _toastHost.IsPresentChanged += UpdateObstructed;
        _drawerHost.IsPresentChanged += UpdateObstructed;
        _modalHost.IsPresentChanged += UpdateObstructed;
        _dialogHost.IsPresentChanged += UpdateObstructed;
        _toastHost.MaskPointerPressed += OnMaskPointerPressed;
        _drawerHost.MaskPointerPressed += OnMaskPointerPressed;
        _modalHost.MaskPointerPressed += OnMaskPointerPressed;
        _dialogHost.MaskPointerPressed += OnMaskPointerPressed;

        LogicalChildren.Add(_toastHost);
        LogicalChildren.Add(_drawerHost);
        LogicalChildren.Add(_modalHost);
        LogicalChildren.Add(_dialogHost);
        LogicalChildren.Add(_notificationHost);
    }

    private void OnMaskPointerPressed(object? sender, OverlayHost.MaskPointerPressedEventArgs e)
    {
        BeginMoveDrag(e.Inner);
        e.Handled = true;
    }

    private void UpdateObstructed(object? _, PropertyChangedRoutedEventArgs<bool> __)
    {
        ArgumentNullException.ThrowIfNull(_toastHost);
        ArgumentNullException.ThrowIfNull(_drawerHost);
        ArgumentNullException.ThrowIfNull(_modalHost);
        ArgumentNullException.ThrowIfNull(_dialogHost);

        PseudoClasses.Set(":obstructed", _toastHost.IsPresent || _drawerHost.IsPresent || _modalHost.IsPresent || _dialogHost.IsPresent);
    }

    public void PopToast(Toast toast)
    {
        ArgumentNullException.ThrowIfNull(_toastHost);
        _toastHost.Pop(toast);
    }

    public void PopDrawer(Drawer drawer)
    {
        ArgumentNullException.ThrowIfNull(_drawerHost);
        _drawerHost.Pop(drawer);
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

    public void PopNotification(NotificationItem notification)
    {
        ArgumentNullException.ThrowIfNull(_notificationHost);
        _notificationHost.Pop(notification);
    }
}
