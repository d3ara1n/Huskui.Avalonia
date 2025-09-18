using System;
using System.Collections.Generic;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace Huskui.Gallery.Models;

/// <summary>
///     Represents a gallery item that can be displayed in the navigation
/// </summary>
public partial class GalleryItem : ObservableObject
{
    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Symbol Icon { get; set; } = Symbol.Document;

    [ObservableProperty]
    public partial bool IsNew { get; set; }

    [ObservableProperty]
    public partial bool IsUpdated { get; set; }

    [ObservableProperty]
    public partial Type? PageType { get; set; }

    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial List<string> Tags { get; set; } = [];

    [ObservableProperty]
    public partial string Title { get; set; } = string.Empty;

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName is nameof(Title) or nameof(Description))
        {
            UpdateSearchText();
        }
    }

    private void UpdateSearchText() =>
        SearchText = $"{Title} {Description} {string.Join(" ", Tags)}".ToLowerInvariant();

    public bool MatchesSearch(string searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm)
     || SearchText.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase);
}
