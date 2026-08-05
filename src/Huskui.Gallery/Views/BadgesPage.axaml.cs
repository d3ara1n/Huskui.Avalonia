using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class BadgesPage : ControlPage
{
    public BadgesPage() => InitializeComponent();

    private AppSurface? GetAppSurface() => AppSurface.GetAppSurface(this);

    private void OnOpenDialogClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface is null)
        {
            return;
        }

        appSurface.PopDialog(
            new Dialog
            {
                Title = "Badge layering",
                Content = CreateOverlayBadgeDemo(
                    "This badge belongs to the dialog layer. Badges on the page behind stay covered by the smoke mask."
                ),
                PrimaryText = "OK",
                IsPrimaryButtonVisible = true,
            }
        );
    }

    private void OnOpenModalClick(object? sender, RoutedEventArgs e)
    {
        var appSurface = GetAppSurface();
        if (appSurface is null)
        {
            return;
        }

        var modal = new Modal();
        var content = CreateOverlayBadgeDemo(
            "This badge belongs to the modal layer. Badges on the page behind stay covered by the smoke mask."
        );
        var close = new Button { Content = "Close", HorizontalAlignment = HorizontalAlignment.Center };
        close.Click += (_, _) => modal.Dismiss();
        content.Children.Add(close);

        modal.Content = content;

        appSurface.PopModal(modal);
    }

    private static StackPanel CreateOverlayBadgeDemo(string message)
    {
        var button = new Button
        {
            Content = "Overlay action",
            Classes = { "Primary" },
        };
        BadgeService.SetContent(button, "9");
        BadgeService.SetClasses(button, "Danger");

        return new StackPanel
        {
            MaxWidth = 320,
            Spacing = 12,
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    TextWrapping = TextWrapping.Wrap,
                },
                button,
            },
        };
    }
}
