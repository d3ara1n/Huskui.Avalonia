using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace Huskui.Gallery.Models
{
    /// <summary>
    ///     Represents a category of gallery items
    /// </summary>
    public partial class GalleryCategory : ObservableObject
    {
        [ObservableProperty]
        private string _description = string.Empty;

        [ObservableProperty]
        private Symbol _icon = Symbol.Folder;

        [ObservableProperty]
        private bool _isExpanded = true;

        [ObservableProperty]
        private ObservableCollection<GalleryItem> _items = [];

        [ObservableProperty]
        private string _name = string.Empty;

        public int ItemCount => Items.Count;
    }
}