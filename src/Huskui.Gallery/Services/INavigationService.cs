using System;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Services
{
    /// <summary>
    ///     Service for handling navigation within the gallery
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        ///     Gets the current gallery item
        /// </summary>
        GalleryItem? CurrentItem { get; }

        /// <summary>
        ///     Checks if navigation can go back
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        ///     Checks if navigation can go forward
        /// </summary>
        bool CanGoForward { get; }

        /// <summary>
        ///     Event raised when navigation occurs
        /// </summary>
        event EventHandler<GalleryItem?>? NavigationChanged;

        /// <summary>
        ///     Navigates to a gallery item
        /// </summary>
        /// <param name="item">The item to navigate to</param>
        void NavigateTo(GalleryItem item);

        /// <summary>
        ///     Navigates to the home page
        /// </summary>
        void NavigateToHome();

        /// <summary>
        ///     Goes back in navigation history
        /// </summary>
        void GoBack();

        /// <summary>
        ///     Goes forward in navigation history
        /// </summary>
        void GoForward();
    }
}