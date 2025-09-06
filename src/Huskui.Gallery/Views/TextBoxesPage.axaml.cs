using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class TextBoxesPage : ControlPage
{
    public TextBoxesPage() => InitializeComponent();

    private void OnClearFormClick(object? sender, RoutedEventArgs e)
    {
        FirstNameBox.Text = "";
        EmailBox.Text = "";
        PasswordBox.Text = "";
        BioBox.Text = "";
        ValidationToggle.IsChecked = false;
    }
}
