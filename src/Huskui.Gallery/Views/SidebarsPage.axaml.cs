using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Siderbars;

namespace Huskui.Gallery.Views;

public partial class SidebarsPage : ControlPage
{
    public SidebarsPage() => InitializeComponent();

    private AppSurface? GetAppSurface() =>
        TopLevel.GetTopLevel(this) is IAppSurfaceAccessor accessor
            ? accessor.GetAppSurface()
            : null;

    private void OnShowProjectDetailsClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        appSurface.PopSidebar(new ProjectDetailsSidebar());
    }

    private void OnShowSmartFilterClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        appSurface.PopSidebar(new SmartFilterSidebar());
    }

    private void OnShowTeamMemberEditorClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        appSurface.PopSidebar(new TeamMemberEditorSidebar());
    }
}
