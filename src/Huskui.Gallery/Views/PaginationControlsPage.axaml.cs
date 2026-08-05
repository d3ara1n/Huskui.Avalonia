using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class PaginationControlsPage : ControlPage
{
    public PaginationControlsPage() => InitializeComponent();

    private void OnGoToFirst(object? sender, RoutedEventArgs e) => BasicPagination.GoToFirst();

    private void OnGoToLast(object? sender, RoutedEventArgs e) => BasicPagination.GoToLast();

    private void OnGoToPage20(object? sender, RoutedEventArgs e) =>
        LargePagination.PageIndex = 19;

    private void OnGoToPage50(object? sender, RoutedEventArgs e) =>
        LargePagination.PageIndex = 49;

    private void OnEnabledToggled(object? sender, RoutedEventArgs e) =>
        DisabledPagination.IsEnabled = !DisabledPagination.IsEnabled;
}
