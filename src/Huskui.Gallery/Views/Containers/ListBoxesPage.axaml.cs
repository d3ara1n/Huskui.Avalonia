using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views.Containers;

public partial class ListBoxesPage : ControlPage
{
    public ListBoxesPage() => InitializeComponent();

    private void OnClearSelectionClick(object? sender, RoutedEventArgs e)
    {
        BasicListBox.SelectedIndex = -1;
        MultiSelectListBox.SelectedIndex = -1;
        CustomListBox.SelectedIndex = -1;
    }

    private void OnResetSelectionClick(object? sender, RoutedEventArgs e)
    {
        BasicListBox.SelectedIndex = 0;
        MultiSelectListBox.SelectedIndex = 0;
        CustomListBox.SelectedIndex = 0;
    }
}
