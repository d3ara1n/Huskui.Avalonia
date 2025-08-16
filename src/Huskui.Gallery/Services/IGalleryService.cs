using System.Collections.ObjectModel;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Services;

/// <summary>
/// Service for managing gallery items and categories
/// </summary>
public interface IGalleryService
{
    /// <summary>
    /// Gets all available categories
    /// </summary>
    ObservableCollection<GalleryCategory> Categories { get; }

    /// <summary>
    /// Gets all gallery items
    /// </summary>
    ObservableCollection<GalleryItem> AllItems { get; }

    /// <summary>
    /// Searches for items matching the given query
    /// </summary>
    /// <param name="searchQuery">The search query</param>
    /// <returns>Collection of matching items</returns>
    ObservableCollection<GalleryItem> SearchItems(string searchQuery);

    /// <summary>
    /// Gets items by category
    /// </summary>
    /// <param name="categoryName">The category name</param>
    /// <returns>Collection of items in the category</returns>
    ObservableCollection<GalleryItem> GetItemsByCategory(string categoryName);

    /// <summary>
    /// Initializes the gallery data
    /// </summary>
    void Initialize();
}
