using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Modals;

namespace Huskui.Gallery.Views;

public partial class ModalsPage : ControlPage
{
    public ModalsPage() => InitializeComponent();

    private AppSurface? GetAppSurface() => AppSurface.GetAppSurface(this);

    private void OnShowSettingsModalClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        var modal = new SettingsModal();
        appSurface.PopModal(modal);
    }

    private void OnShowProfileModalClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        var modal = new UserProfileModal();
        appSurface.PopModal(modal);
    }

    private void OnShowProjectWizardClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface == null)
        {
            return;
        }

        var modal = new ProjectWizardModal();
        appSurface.PopModal(modal);
    }
}
