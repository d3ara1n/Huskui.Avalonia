namespace Huskui.Gallery.Models;

public sealed record MailMessage(
    string Sender,
    string SenderAddress,
    string Subject,
    string ReceivedAt,
    string Body)
{
    public string Preview => Body.ReplaceLineEndings(" ");
}
