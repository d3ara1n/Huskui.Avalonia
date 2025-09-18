using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace Huskui.Gallery.Models;

/// <summary>
///     Represents a category of gallery items
/// </summary>
public partial class GalleryCategory : ObservableObject
{
    [ObservableProperty]
    public partial string Description { get; set; } = string.Empty;

    [ObservableProperty]
    public partial Symbol Icon { get; set; } = Symbol.Folder;

    [ObservableProperty]
    public partial bool IsExpanded { get; set; } = true;

    [ObservableProperty]
    public partial ObservableCollection<GalleryItem> Items { get; set; } = [];

    [ObservableProperty]
    public partial string Name { get; set; } = string.Empty;

    public int ItemCount => Items.Count;
}
