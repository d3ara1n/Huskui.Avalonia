using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Models;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Drawers;

public partial class ComposeMailDrawer : Drawer
{
    private const string UntitledTitle = "New message";

    public ComposeMailDrawer() => InitializeComponent();

    public ComposeMailDrawer(MailMessage replyTo)
        : this()
    {
        var quoted = replyTo
            .Body.ReplaceLineEndings("\n")
            .Split('\n')
            .Select(line => line.Length == 0 ? ">" : $"> {line}");

        ToBox.Text = replyTo.SenderAddress;
        SubjectBox.Text = $"Re: {replyTo.Subject}";
        BodyBox.Text = $"\n\n{replyTo.Sender} wrote:\n{string.Join('\n', quoted)}";
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (string.IsNullOrEmpty(ToBox.Text))
        {
            ToBox.Focus();
        }
        else
        {
            BodyBox.CaretIndex = 0;
            BodyBox.Focus();
        }
    }

    private void OnSubjectChanged(object? sender, TextChangedEventArgs e) =>
        Title = string.IsNullOrWhiteSpace(SubjectBox.Text) ? UntitledTitle : SubjectBox.Text;

    private void OnSendClick(object? sender, RoutedEventArgs e)
    {
        AppSurface
            .GetAppSurface(this)
            ?.PopGrowl(
                new()
                {
                    Level = GrowlLevel.Success,
                    Title = "Message sent",
                    Content = $"\"{Title}\" was sent to {ToBox.Text}.",
                    IsCloseButtonVisible = true,
                }
            );
        Dismiss();
    }

    private async void OnDiscardClick(object? sender, RoutedEventArgs e)
    {
        var surface = AppSurface.GetAppSurface(this);
        if (
            surface is null
            || (string.IsNullOrWhiteSpace(SubjectBox.Text) && string.IsNullOrWhiteSpace(BodyBox.Text))
        )
        {
            Dismiss();
            return;
        }

        var dialog = new Dialog
        {
            Title = "Discard this draft?",
            Message = "The subject and message you have written will be lost.",
            PrimaryText = "Discard",
            SecondaryText = "Keep editing",
            IsPrimaryButtonVisible = true,
        };
        surface.PopDialog(dialog);

        if (await dialog.CompletionSource.Task)
        {
            Dismiss();
        }
    }
}
