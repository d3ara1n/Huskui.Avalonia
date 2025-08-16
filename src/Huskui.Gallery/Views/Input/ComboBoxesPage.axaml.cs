using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Huskui.Gallery.Views.Input;

public partial class ComboBoxesPage : UserControl
{
    public ComboBoxesPage()
    {
        InitializeComponent();
    }

    private void OnClearSelectionClick(object? sender, RoutedEventArgs e)
    {
        CountryCombo.SelectedIndex = -1;
        LanguageCombo.SelectedIndex = -1;
        WatermarkCombo.SelectedIndex = -1;
    }

    private void OnResetSelectionClick(object? sender, RoutedEventArgs e)
    {
        CountryCombo.SelectedIndex = 0;
        LanguageCombo.SelectedIndex = 2;
        WatermarkCombo.SelectedIndex = -1;
    }
}
