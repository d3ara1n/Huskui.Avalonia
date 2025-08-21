using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views.Input
{
    public partial class CheckBoxesPage : ControlPage
    {
        public CheckBoxesPage() => InitializeComponent();

        private void OnResetClick(object? sender, RoutedEventArgs e)
        {
            AnalyticsCheckBox.IsChecked = true;
            CrashReportsCheckBox.IsChecked = true;
            UsageDataCheckBox.IsChecked = false;
            MarketingCheckBox.IsChecked = false;
            UpdatesCheckBox.IsChecked = true;
            NewsletterCheckBox.IsChecked = false;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            // In a real application, you would save the settings here
            // For demo purposes, we'll just show the current state
        }

        private void OnResetAllClick(object? sender, RoutedEventArgs e) =>
            // Reset the three-state toggle
            ThreeStateToggle.IsChecked = false;
    }
}