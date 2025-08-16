using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace Huskui.Gallery.Models;

/// <summary>
/// Represents a category of gallery items
/// </summary>
public partial class GalleryCategory : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private Symbol _icon = Symbol.Folder;

    [ObservableProperty]
    private ObservableCollection<GalleryItem> _items = [];

    [ObservableProperty]
    private bool _isExpanded = true;

    public int ItemCount => Items.Count;
}
