using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class AutoCompleteBoxesPage : ControlPage
{
    private readonly IReadOnlyList<ProductSuggestion> _products =
    [
        new("Huskui UI Kit", "Design System", "$49 / seat"),
        new("Polymerium Cloud", "Infrastructure", "$129 / month"),
        new("Atlas Monitor", "Observability", "$89 / month"),
        new("Nova Docs", "Knowledge Base", "$19 / editor"),
        new("Signal Router", "Messaging", "$59 / month"),
    ];

    public AutoCompleteBoxesPage()
    {
        InitializeComponent();

        CitySearchBox.ItemsSource = new[]
        {
            "Amsterdam",
            "Bangkok",
            "Berlin",
            "Copenhagen",
            "Kyoto",
            "Melbourne",
            "San Francisco",
            "Singapore",
            "Tokyo",
            "Toronto",
            "Vancouver",
            "Zurich",
        };

        MemberSearchBox.ItemsSource = new[]
        {
            "Avery Carter - Design",
            "Daniel Kim - Platform",
            "Lena Wang - Growth",
            "Marcus Reed - Finance",
            "Noah Patel - Support",
            "Olivia Turner - Operations",
            "Sarah Chen - Product",
            "Sofia Ivanova - Research",
        };

        LanguageSearchBox.ItemsSource = new[]
        {
            "C#",
            "F#",
            "Go",
            "Java",
            "JavaScript",
            "Kotlin",
            "Python",
            "Rust",
            "Swift",
            "TypeScript",
        };

        ProductSearchBox.ItemsSource = _products;
        WorkspaceSearchBox.ItemsSource = new[]
        {
            "Customer Operations",
            "Design Systems",
            "Executive Reporting",
            "Growth Experiments",
            "Platform Reliability",
            "Release Management",
        };

        RoleSearchBox.ItemsSource = new[]
        {
            "Billing Reviewer",
            "Content Publisher",
            "Incident Commander",
            "Project Maintainer",
            "Read-only Auditor",
            "Workspace Admin",
        };

        InviteeSearchBox.ItemsSource = new[]
        {
            "amelia.ross@polymerium.dev",
            "david.nguyen@polymerium.dev",
            "lena.wang@polymerium.dev",
            "mia.johnson@polymerium.dev",
            "sarah.chen@polymerium.dev",
            "victor.park@polymerium.dev",
        };

        ProductSearchBox
            .GetObservable(AutoCompleteBox.SelectedItemProperty)
            .Subscribe(selected => UpdateProductSelection(selected as ProductSuggestion));

        ResetExamples();
    }

    private void OnClearInputsClick(object? sender, RoutedEventArgs e)
    {
        CitySearchBox.Text = string.Empty;
        CitySearchBox.SelectedItem = null;
        MemberSearchBox.Text = string.Empty;
        MemberSearchBox.SelectedItem = null;
        LanguageSearchBox.Text = string.Empty;
        LanguageSearchBox.SelectedItem = null;
    }

    private void OnResetExamplesClick(object? sender, RoutedEventArgs e) => ResetExamples();

    private void OnClearMemberSearchClick(object? sender, RoutedEventArgs e)
    {
        MemberSearchBox.Text = string.Empty;
        MemberSearchBox.SelectedItem = null;
    }

    private void OnClearFormClick(object? sender, RoutedEventArgs e)
    {
        WorkspaceSearchBox.Text = string.Empty;
        WorkspaceSearchBox.SelectedItem = null;
        RoleSearchBox.Text = string.Empty;
        RoleSearchBox.SelectedItem = null;
        InviteeSearchBox.Text = string.Empty;
        InviteeSearchBox.SelectedItem = null;
    }

    private void ResetExamples()
    {
        CitySearchBox.Text = "Singapore";
        CitySearchBox.SelectedItem = "Singapore";

        MemberSearchBox.Text = "Sarah";
        MemberSearchBox.SelectedItem = null;

        LanguageSearchBox.Text = "Type";
        LanguageSearchBox.SelectedItem = null;

        ProductSearchBox.SelectedItem = _products.First();
        ProductSearchBox.Text = _products.First().Name;
    }

    private void UpdateProductSelection(ProductSuggestion? selected)
    {
        ProductSelectionText.Text = selected is null
            ? "Selected product: none"
            : $"Selected product: {selected.Name} · {selected.Category} · {selected.PriceLabel}";
    }
}

public sealed record ProductSuggestion(string Name, string Category, string PriceLabel);
