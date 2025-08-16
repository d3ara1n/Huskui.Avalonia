using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace Huskui.Gallery.Models;

/// <summary>
/// Represents a gallery item that can be displayed in the navigation
/// </summary>
public partial class GalleryItem : ObservableObject
{
    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private Symbol _icon = Symbol.Document;

    [ObservableProperty]
    private Type? _pageType;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private List<string> _tags = [];

    [ObservableProperty]
    private bool _isNew;

    [ObservableProperty]
    private bool _isUpdated;

    [ObservableProperty]
    private string _searchText = string.Empty;

    public GalleryItem()
    {
        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(Title) or nameof(Description) or nameof(Category))
            {
                UpdateSearchText();
            }
        };
    }

    private void UpdateSearchText()
    {
        SearchText = $"{Title} {Description} {Category} {string.Join(" ", Tags)}".ToLowerInvariant();
    }

    public bool MatchesSearch(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return true;

        return SearchText.Contains(searchTerm.ToLowerInvariant());
    }
}
