using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Input;

namespace Huskui.Avalonia;

[TemplatePart(PART_ITEMS_PRESENTER, typeof(ItemsPresenter))]
public class NavMenu : ItemsControl, ICustomKeyboardNavigation
{
    public const string PART_ITEMS_PRESENTER = "PartItemsPresenter";

    public static readonly StyledProperty<object?> SelectedItemProperty = AvaloniaProperty.Register<NavMenu, object?>(
     nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }




    public (bool handled, IInputElement? next) GetNext(IInputElement element, NavigationDirection direction) =>
        throw new NotImplementedException();
}
