using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":windows", ":macos", ":linux")]
[TemplatePart(PART_AppSurface, typeof(AppSurface))]
public class AppWindow : Window
{
    public const string PART_AppSurface = nameof(PART_AppSurface);

    public static readonly DirectProperty<AppWindow, bool> IsMaximizedProperty =
        AvaloniaProperty.RegisterDirect<AppWindow, bool>(nameof(IsMaximized),
                                                         o => o.IsMaximized,
                                                         (o, v) => o.IsMaximized = v);

    public AppWindow()
    {
        if (OperatingSystem.IsWindows())
        {
            PseudoClasses.Set(":windows", true);
        }
        else if (OperatingSystem.IsMacOS())
        {
            PseudoClasses.Set(":macos", true);
        }
        else if (OperatingSystem.IsLinux())
        {
            PseudoClasses.Set(":linux", true);
        }
    }

    protected override Type StyleKeyOverride => typeof(AppWindow);

    public bool IsMaximized
    {
        get;
        set => SetAndRaise(IsMaximizedProperty, ref field, value);
    }

    public AppSurface? AppSurface { get; private set; }

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

        AppSurface = e.NameScope.Find<AppSurface>(PART_AppSurface);
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.MaskPointerPressed += OnMaskPointerPressed;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnregisterHandlers();
    }

    private void UnregisterHandlers()
    {
        if (AppSurface != null)
        {
            AppSurface.MaskPointerPressed -= OnMaskPointerPressed;
        }
    }

    private void OnMaskPointerPressed(object? sender, OverlayHost.MaskPointerPressedEventArgs e)
    {
        BeginMoveDrag(e.Inner);
        e.Handled = true;
    }

    public void PopToast(Toast toast)
    {
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.PopToast(toast);
    }

    public void PopSidebar(Sidebar sidebar)
    {
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.PopSidebar(sidebar);
    }

    public void PopDialog(Dialog dialog)
    {
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.PopDialog(dialog);
    }

    public void PopModal(Modal modal)
    {
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.PopModal(modal);
    }

    public void PopGrowl(GrowlItem growl)
    {
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.PopGrowl(growl);
    }

    public void PopDrawer(Drawer drawer)
    {
        ArgumentNullException.ThrowIfNull(AppSurface);
        AppSurface.PopDrawer(drawer);
    }
}
