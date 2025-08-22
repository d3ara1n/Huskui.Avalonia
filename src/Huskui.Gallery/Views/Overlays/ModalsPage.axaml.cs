using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Modals;

namespace Huskui.Gallery.Views.Overlays
{
    public partial class ModalsPage : ControlPage
    {
        public ModalsPage() => InitializeComponent();

        private AppWindow? GetAppWindow() => TopLevel.GetTopLevel(this) as AppWindow;

        private void OnShowSettingsModalClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var modal = new Modal { Content = new SettingsModal() };
            appWindow.PopModal(modal);
        }

        private void OnShowProfileModalClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var modal = new UserProfileModal();
            appWindow.PopModal(modal);
        }


        private void OnShowProjectWizardClick(object? sender, RoutedEventArgs e)
        {
            var appWindow = GetAppWindow();
            if (appWindow == null)
            {
                return;
            }

            var modal = new ProjectWizardModal();
            appWindow.PopModal(modal);
        }
    }
}
