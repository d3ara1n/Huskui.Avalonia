using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Dialogs;

public partial class DeleteConfirmationDialog : Dialog
{
    public DeleteConfirmationDialog() => InitializeComponent();

    protected override bool ValidateResult(object? result) => true;
}
