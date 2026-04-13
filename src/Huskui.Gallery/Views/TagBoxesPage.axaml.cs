using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class TagBoxesPage : ControlPage
{
    private readonly ObservableCollection<string> _channelOptions =
    [
        "Email",
        "PagerDuty",
        "Slack",
        "SMS",
        "Status Page",
        "Teams",
        "Webhook",
    ];

    private readonly ObservableCollection<string> _formChannels = ["Slack", "PagerDuty"];
    private readonly ObservableCollection<string> _formLabels = ["sev-1", "customer-facing"];
    private readonly ObservableCollection<string> _formTeams = ["Platform", "SRE"];

    private readonly ObservableCollection<string> _labelOptions =
    [
        "accessibility",
        "backend",
        "blocking",
        "design",
        "frontend",
        "high-priority",
        "release-candidate",
        "ux-review",
    ];

    private readonly ObservableCollection<string> _releaseLabels = ["release-candidate", "blocking"];

    private readonly ObservableCollection<string> _teamOptions =
    [
        "Core UX",
        "Docs",
        "Platform",
        "QA",
        "Security",
        "SRE",
        "Support",
    ];

    private readonly ObservableCollection<string> _technologyOptions =
    [
        "Avalonia",
        "C#",
        "Cross-platform",
        "Design System",
        "Fluent Icons",
        "Huskui",
        "MVVM",
        "Reactive",
        "XAML",
    ];

    private readonly ObservableCollection<string> _technologySelections = ["Avalonia", "Huskui", "XAML"];

    public TagBoxesPage()
    {
        InitializeComponent();

        TechnologyTagBox.ItemsSource = _technologyOptions;
        TechnologyTagBox.SelectedItems = _technologySelections;

        ReleaseTagBox.ItemsSource = _labelOptions;
        ReleaseTagBox.SelectedItems = _releaseLabels;

        TeamTagBox.ItemsSource = _teamOptions;
        TeamTagBox.SelectedItems = _formTeams;

        ChannelTagBox.ItemsSource = _channelOptions;
        ChannelTagBox.SelectedItems = _formChannels;

        RuleLabelTagBox.ItemsSource = _labelOptions;
        RuleLabelTagBox.SelectedItems = _formLabels;

        SubscribeSelectionChanges(_technologySelections, UpdateTechnologySelectionText);
        SubscribeSelectionChanges(_releaseLabels, UpdateReleaseSelectionText);
        SubscribeSelectionChanges(_formTeams, UpdateFormSelectionText);
        SubscribeSelectionChanges(_formChannels, UpdateFormSelectionText);
        SubscribeSelectionChanges(_formLabels, UpdateFormSelectionText);

        UpdateTechnologySelectionText();
        UpdateReleaseSelectionText();
        UpdateFormSelectionText();
    }

    private void OnClearBasicClick(object? sender, RoutedEventArgs e) => _technologySelections.Clear();

    private void OnResetBasicClick(object? sender, RoutedEventArgs e)
    {
        ResetCollection(_technologySelections, ["Avalonia", "Huskui", "XAML"]);
        TechnologyTagBox.Text = string.Empty;
    }

    private void OnSeedCustomInputClick(object? sender, RoutedEventArgs e)
    {
        ReleaseTagBox.Text = "needs-copy-review";
    }

    private void OnResetCustomClick(object? sender, RoutedEventArgs e)
    {
        ResetCollection(_releaseLabels, ["release-candidate", "blocking"]);
        ReleaseTagBox.Text = string.Empty;
    }

    private void OnResetFormClick(object? sender, RoutedEventArgs e)
    {
        ResetCollection(_formTeams, ["Platform", "SRE"]);
        ResetCollection(_formChannels, ["Slack", "PagerDuty"]);
        ResetCollection(_formLabels, ["sev-1", "customer-facing"]);

        TeamTagBox.Text = string.Empty;
        ChannelTagBox.Text = string.Empty;
        RuleLabelTagBox.Text = string.Empty;
    }

    private void UpdateTechnologySelectionText()
    {
        TechnologySelectionText.Text = _technologySelections.Count == 0
            ? "Selected tags: none"
            : $"Selected tags: {string.Join(", ", _technologySelections)}";
    }

    private void UpdateReleaseSelectionText()
    {
        ReleaseSelectionText.Text = _releaseLabels.Count == 0
            ? "Release labels: none"
            : $"Release labels: {string.Join(", ", _releaseLabels)}";
    }

    private void UpdateFormSelectionText()
    {
        FormSelectionText.Text =
            $"Teams: {FormatSelection(_formTeams)} | Channels: {FormatSelection(_formChannels)} | Labels: {FormatSelection(_formLabels)}";
    }

    private static void ResetCollection(ObservableCollection<string> target, params string[] values)
    {
        target.Clear();

        foreach (var value in values)
        {
            target.Add(value);
        }
    }

    private static string FormatSelection(ObservableCollection<string> values) =>
        values.Count == 0 ? "none" : string.Join(", ", values);

    private static void SubscribeSelectionChanges(
        INotifyCollectionChanged source,
        Action onCollectionChanged
    )
    {
        source.CollectionChanged += (_, _) => onCollectionChanged();
    }
}
