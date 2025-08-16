using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Huskui.Gallery.Views.Input;

public partial class TextBoxesPage : UserControl
{
    public TextBoxesPage()
    {
        InitializeComponent();
    }

    private void OnClearFormClick(object? sender, RoutedEventArgs e)
    {
        FirstNameBox.Text = "";
        EmailBox.Text = "";
        PasswordBox.Text = "";
        BioBox.Text = "";
        ValidationToggle.IsChecked = false;
    }
}
