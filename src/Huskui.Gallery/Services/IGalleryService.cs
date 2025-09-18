using System.Collections.ObjectModel;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Services;

/// <summary>
///     Service for managing gallery items and categories
/// </summary>
public interface IGalleryService
{
    /// <summary>
    ///     Gets all available categories
    /// </summary>
    ObservableCollection<GalleryCategory> Categories { get; }

    /// <summary>
    ///     Gets all gallery items
    /// </summary>
    ObservableCollection<GalleryItem> AllItems { get; }

    /// <summary>
    ///     Initializes the gallery data
    /// </summary>
    void Initialize();
}
