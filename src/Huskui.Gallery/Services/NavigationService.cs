using Huskui.Gallery.Models;

namespace Huskui.Gallery.Services;

/// <summary>
///
/// </summary>
public class NavigationService
{
    private readonly List<MenuItemVo> _history = [];
    private int _currentIndex = -1;

    #region INavigationService Members

    private MenuItemVo? CurrentItem =>
        _currentIndex >= 0 && _currentIndex < _history.Count ? _history[_currentIndex] : null;

    private bool CanGoBack => _currentIndex > 0;
    private bool CanGoForward => _currentIndex < _history.Count - 1;

    /// <summary>
    /// MenuItemVo,CanGoback,CanGoForward
    /// </summary>
    public Action<MenuItemVo?,bool,bool>? NavigationChanged;

    public void NavigateTo(MenuItemVo item)
    {
        if (item.IsSeparator) return;
        if (item == CurrentItem) return;
        // Remove any forward history
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex + 1, _history.Count - 1);
        }

        // Add new item to history
        _history.Add(item);
        _currentIndex = _history.Count - 1;

        NavigationChanged?.Invoke(item,CanGoBack,CanGoForward);
    }

    public void NavigateToHome()
    {
        // Remove any forward history
        if (_currentIndex < _history.Count - 1)
        {
            _history.RemoveRange(_currentIndex, _history.Count - _currentIndex);
        }

        NavigationChanged?.Invoke(null,CanGoBack,CanGoForward);
    }

    public void GoBack()
    {
        if (!CanGoBack)
        {
            return;
        }

        _currentIndex--;
        NavigationChanged?.Invoke(CurrentItem,CanGoBack,CanGoForward);
    }

    public void GoForward()
    {
        if (!CanGoForward)
        {
            return;
        }

        _currentIndex++;
        NavigationChanged?.Invoke(CurrentItem,CanGoBack,CanGoForward);
    }

    #endregion
}
