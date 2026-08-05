using Avalonia.Interactivity;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Models;

namespace Huskui.Gallery.Views;

public partial class TableViewPage : ControlPage
{
    public IReadOnlyList<Country> Countries { get; } =
    [
        new("Japan", "Asia", "Tokyo", 125_800_000, 377_975, 4_212),
        new("Germany", "Europe", "Berlin", 83_200_000, 357_114, 4_076),
        new("Brazil", "Americas", "Brasília", 215_300_000, 8_510_295, 1_920),
        new("Canada", "Americas", "Ottawa", 38_900_000, 9_984_670, 2_140),
        new("Australia", "Oceania", "Canberra", 25_700_000, 7_692_024, 1_693),
        new("Egypt", "Africa", "Cairo", 109_300_000, 1_001_449, 477),
        new("Norway", "Europe", "Oslo", 5_400_000, 385_207, 546),
        new("Argentina", "Americas", "Buenos Aires", 45_800_000, 2_780_400, 632),
        new("Vietnam", "Asia", "Hanoi", 97_500_000, 331_212, 409),
        new("Kenya", "Africa", "Nairobi", 53_000_000, 580_367, 113),
    ];

    public TableViewPage() => InitializeComponent();

    private void OnClearSelectionClick(object? sender, RoutedEventArgs e) => BasicTable.SelectedIndex = -1;

    private void OnResizingToggleClick(object? sender, RoutedEventArgs e) =>
        ResizableTable.CanUserResizeColumns = ResizingToggle.IsChecked ?? true;
}
