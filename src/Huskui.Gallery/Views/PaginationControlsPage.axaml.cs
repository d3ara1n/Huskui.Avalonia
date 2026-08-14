using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class PaginationControlsPage : ControlPage
{
    private const int MaxLogEntries = 8;

    private readonly ObservableCollection<string> _indexChangedEntries = [];

    public PaginationControlsPage()
    {
        InitializeComponent();

        IndexChangedLog.ItemsSource = _indexChangedEntries;
        BasicPagination.IndexChanged += (_, e) =>
        {
            _indexChangedEntries.Insert(0, $"PageIndex {e.OldValue} → {e.NewValue} (page {e.OldValue + 1} → {e.NewValue + 1})");

            if (_indexChangedEntries.Count > MaxLogEntries)
                _indexChangedEntries.RemoveAt(_indexChangedEntries.Count - 1);
        };
    }

    private void OnGoToFirst(object? sender, RoutedEventArgs e) => BasicPagination.GoToFirst();

    private void OnGoToLast(object? sender, RoutedEventArgs e) => BasicPagination.GoToLast();

    private void OnGoToPage20(object? sender, RoutedEventArgs e) =>
        LargePagination.PageIndex = 19;

    private void OnGoToPage50(object? sender, RoutedEventArgs e) =>
        LargePagination.PageIndex = 49;

    private void OnEnabledToggled(object? sender, RoutedEventArgs e) =>
        DisabledPagination.IsEnabled = !DisabledPagination.IsEnabled;
}
