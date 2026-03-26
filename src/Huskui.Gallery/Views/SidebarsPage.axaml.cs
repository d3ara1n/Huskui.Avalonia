using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Siderbars;

namespace Huskui.Gallery.Views;

public partial class SidebarsPage : ControlPage
{
    public SidebarsPage() => InitializeComponent();

    private AppWindow? GetAppWindow() => TopLevel.GetTopLevel(this) as AppWindow;

    private void OnShowProjectDetailsClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        appWindow.PopSidebar(new ProjectDetailsSidebar());
    }

    private void OnShowSmartFilterClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        appWindow.PopSidebar(new SmartFilterSidebar());
    }

    private void OnShowTeamMemberEditorClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null)
        {
            return;
        }

        appWindow.PopSidebar(new TeamMemberEditorSidebar());
    }
}
