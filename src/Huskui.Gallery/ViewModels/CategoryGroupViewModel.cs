using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.ViewModels
{
    /// <summary>
    ///     ViewModel for a category group that supports filtering
    /// </summary>
    public partial class CategoryGroupViewModel : ObservableObject
    {
        private readonly ReadOnlyObservableCollection<GalleryItem> _allItems;
        private readonly GalleryCategory _category;

        [ObservableProperty]
        private ReadOnlyObservableCollection<GalleryItem> _filteredItems;

        [ObservableProperty]
        private bool _isVisible = true;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private GalleryItem? _selectedItem;

        public CategoryGroupViewModel(GalleryCategory category, ReadOnlyObservableCollection<GalleryItem> allItems)
        {
            _category = category;
            _allItems = allItems;
            FilteredItems = allItems;

            // Initially show all items
            UpdateFilteredItems();
        }

        public string Name => _category.Name;
        public string Description => _category.Description;

        /// <summary>
        ///     Updates the filtered items based on search text
        /// </summary>
        /// <param name="searchText">The search text to filter by</param>
        public void UpdateFilter(string searchText)
        {
            SearchText = searchText;
            UpdateFilteredItems();
        }

        /// <summary>
        ///     Clears the selection in this category group
        /// </summary>
        public void ClearSelection() => SelectedItem = null;

        private void UpdateFilteredItems()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                // Show all items when no search
                FilteredItems = _allItems;
                IsVisible = _allItems.Count > 0;
            }
            else
            {
                // Filter items based on search text
                var filtered = _allItems.Where(item => item.MatchesSearch(SearchText)).ToList();

                // Create a new observable collection with filtered items
                var filteredCollection = new ObservableCollection<GalleryItem>(filtered);
                FilteredItems = new(filteredCollection);

                // Hide category if no items match
                IsVisible = filtered.Count > 0;
            }
        }
    }
}
