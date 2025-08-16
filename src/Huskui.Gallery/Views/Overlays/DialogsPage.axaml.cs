using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Dialogs;

namespace Huskui.Gallery.Views.Overlays;

public partial class DialogsPage : UserControl
{
    public DialogsPage()
    {
        InitializeComponent();
    }

    private AppWindow? GetAppWindow()
    {
        return TopLevel.GetTopLevel(this) as AppWindow;
    }

    private void OnShowDeleteConfirmClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new DeleteConfirmationDialog();
        appWindow.PopDialog(dialog);
    }

    private void OnShowSaveConfirmClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "Save Changes",
            Content = "Do you want to save your changes before closing?",
            PrimaryText = "Save",
            SecondaryText = "Don't Save",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }

    private void OnShowExitConfirmClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "Exit Application",
            Content = "Are you sure you want to exit? Any unsaved work will be lost.",
            PrimaryText = "Exit",
            SecondaryText = "Cancel",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }

    private void OnShowRenameDialogClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new RenameFileDialog();
        appWindow.PopDialog(dialog);
    }

    private void OnShowCreateFolderClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "Create New Folder",
            Content = new StackPanel
            {
                Spacing = 8,
                Children =
                {
                    new TextBlock { Text = "Folder name:" },
                    new TextBox { Watermark = "Enter folder name" }
                }
            },
            PrimaryText = "Create",
            SecondaryText = "Cancel",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }

    private void OnShowPasswordDialogClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "Set Password",
            Content = new StackPanel
            {
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = "Enter a new password for this document:" },
                    new TextBox { Watermark = "Password", PasswordChar = '•' },
                    new TextBox { Watermark = "Confirm password", PasswordChar = '•' }
                }
            },
            PrimaryText = "Set Password",
            SecondaryText = "Cancel",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }

    private void OnShowUnsavedChangesClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "Unsaved Changes",
            Content = "You have unsaved changes in this document. What would you like to do?",
            PrimaryText = "Save Changes",
            SecondaryText = "Discard Changes",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }

    private void OnShowOverwriteDialogClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "File Already Exists",
            Content = "A file named 'report.pdf' already exists in this location. Do you want to replace it?",
            PrimaryText = "Replace",
            SecondaryText = "Keep Both",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }

    private void OnShowPermanentActionClick(object? sender, RoutedEventArgs e)
    {
        var appWindow = GetAppWindow();
        if (appWindow == null) return;

        var dialog = new Dialog
        {
            Title = "Permanent Action",
            Content = "This will permanently delete all items in the Recycle Bin. This action cannot be undone.",
            PrimaryText = "Empty Recycle Bin",
            SecondaryText = "Cancel",
            IsPrimaryButtonVisible = true
        };

        appWindow.PopDialog(dialog);
    }
}
