using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_AppSurface, typeof(AppSurface))]
public class AppWindow : Window
{
    public const string PART_AppSurface = nameof(PART_AppSurface);

    public static readonly DirectProperty<AppWindow, bool> IsMaximizedProperty =
        AvaloniaProperty.RegisterDirect<AppWindow, bool>(
            nameof(IsMaximized),
            o => o.IsMaximized,
            (o, v) => o.IsMaximized = v
        );

    private AppSurface? _appSurface;

    protected override Type StyleKeyOverride => typeof(AppWindow);

    public bool IsMaximized
    {
        get;
        set => SetAndRaise(IsMaximizedProperty, ref field, value);
    }

    public AppSurface? AppSurface => _appSurface;

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

        _appSurface = e.NameScope.Find<AppSurface>(PART_AppSurface);
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.MaskPointerPressed += OnMaskPointerPressed;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnregisterHandlers();
    }

    private void UnregisterHandlers()
    {
        if (_appSurface != null)
        {
            _appSurface.MaskPointerPressed -= OnMaskPointerPressed;
        }
    }

    private void OnMaskPointerPressed(object? sender, OverlayHost.MaskPointerPressedEventArgs e)
    {
        BeginMoveDrag(e.Inner);
        e.Handled = true;
    }

    public void PopToast(Toast toast)
    {
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.PopToast(toast);
    }

    public void PopSidebar(Sidebar sidebar)
    {
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.PopSidebar(sidebar);
    }

    public void PopDialog(Dialog dialog)
    {
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.PopDialog(dialog);
    }

    public void PopModal(Modal modal)
    {
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.PopModal(modal);
    }

    public void PopGrowl(GrowlItem growl)
    {
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.PopGrowl(growl);
    }

    public void PopDrawer(Drawer drawer)
    {
        ArgumentNullException.ThrowIfNull(_appSurface);
        _appSurface.PopDrawer(drawer);
    }
}
