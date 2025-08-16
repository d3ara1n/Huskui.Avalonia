using System;
using System.Collections.Generic;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Services
{
    /// <summary>
    ///     Default implementation of INavigationService
    /// </summary>
    public class NavigationService : INavigationService
    {
        private readonly List<GalleryItem?> _history = [];
        private int _currentIndex = -1;

        #region INavigationService Members

        public GalleryItem? CurrentItem =>
            _currentIndex >= 0 && _currentIndex < _history.Count ? _history[_currentIndex] : null;

        public bool CanGoBack => _currentIndex > 0;
        public bool CanGoForward => _currentIndex < _history.Count - 1;

        public event EventHandler<GalleryItem?>? NavigationChanged;

        public void NavigateTo(GalleryItem item)
        {
            // Remove any forward history
            if (_currentIndex < _history.Count - 1)
            {
                _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
            }

            // Add new item to history
            _history.Add(item);
            _currentIndex = _history.Count - 1;

            NavigationChanged?.Invoke(this, item);
        }

        public void NavigateToHome()
        {
            // Remove any forward history
            if (_currentIndex < _history.Count - 1)
            {
                _history.RemoveRange(_currentIndex + 1, _history.Count - _currentIndex - 1);
            }

            // Add home to history
            _history.Add(null);
            _currentIndex = _history.Count - 1;

            NavigationChanged?.Invoke(this, null);
        }

        public void GoBack()
        {
            if (!CanGoBack)
            {
                return;
            }

            _currentIndex--;
            NavigationChanged?.Invoke(this, CurrentItem);
        }

        public void GoForward()
        {
            if (!CanGoForward)
            {
                return;
            }

            _currentIndex++;
            NavigationChanged?.Invoke(this, CurrentItem);
        }

        #endregion
    }
}