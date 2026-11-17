using System.Collections.ObjectModel;
using Avalonia.Interactivity;
using Huskui.Avalonia.Controls;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Drawers;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Views;

public partial class DrawerPage : ControlPage
{
    private static readonly MailMessage[] _inbox =
    [
        new(
            "Mira Chen",
            "mira.chen@northwind.dev",
            "Q3 roadmap review: feedback needed by Friday",
            "09:42",
            """
            Hi team,

            I've attached the draft roadmap for Q3. Before we lock it in, I'd like a reply from each area owner covering three things:

            1. Which items you can commit to, and which are stretch goals.
            2. Any dependency on another team that isn't listed yet.
            3. Headcount or budget assumptions you're unsure about.

            The biggest open question is the sync engine rewrite. Platform estimates eight weeks, but that assumes the storage migration lands in July. If it slips, we either move the rewrite to Q4 or ship a smaller incremental fix first.

            Please keep replies short. Bullet points are fine. I'll collect everything into one summary for Monday's planning session.

            Thanks,
            Mira
            """
        ),
        new(
            "Daniel Osei",
            "daniel.osei@northwind.dev",
            "Staging outage postmortem",
            "Yesterday",
            """
            Summary: staging was unavailable for 47 minutes on Tuesday after a certificate rotation job replaced the wildcard certificate with one missing the intermediate chain.

            Timeline:
            - 14:02 rotation job runs
            - 14:05 first alerts from synthetic checks
            - 14:31 root cause identified
            - 14:49 previous certificate restored

            Action items:
            - Validate the full chain before swapping certificates.
            - Page the on-call engineer from synthetic checks instead of posting to the channel only.

            Let me know if I missed anything before I publish this.
            """
        ),
        new(
            "Priya Natarajan",
            "priya@contoso.io",
            "Contract renewal and updated pricing",
            "Mon",
            """
            Hello,

            Our current agreement ends on the 30th. The renewal quote reflects the new per-seat pricing we discussed, with the volume discount applied from 200 seats.

            If you'd like to switch to annual billing, I can add a further 8% discount. I just need confirmation of the billing contact and the purchase order number.

            Best regards,
            Priya
            """
        ),
    ];

    private readonly ObservableCollection<ProjectFile> _projectFiles =
    [
        new("brand-guidelines.pdf", 2_400_000),
        new("homepage-hero.png", 860_000),
    ];

    public DrawerPage()
    {
        InitializeComponent();

        InboxList.ItemsSource = _inbox;
        InboxList.SelectedIndex = 0;
        ProjectFileList.ItemsSource = _projectFiles;
    }

    private AppSurface? GetAppSurface() => AppSurface.GetAppSurface(this);

    private void OnComposeClick(object? sender, RoutedEventArgs e) =>
        GetAppSurface()?.PopDrawer(new ComposeMailDrawer());

    private void OnReplyClick(object? sender, RoutedEventArgs e)
    {
        if (InboxList.SelectedItem is MailMessage message)
        {
            GetAppSurface()?.PopDrawer(new ComposeMailDrawer(message));
        }
    }

    private void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        ProjectFile[] queue =
        [
            new("design-review-notes.pdf", 1_800_000),
            new("onboarding-flow.png", 3_200_000),
            new("pricing-table.png", 1_100_000),
            new("release-checklist.md", 420_000),
        ];

        var drawer = new UploadQueueDrawer(queue);
        drawer.FileUploaded += (_, file) => _projectFiles.Add(file);
        GetAppSurface()?.PopDrawer(drawer);
    }
}
