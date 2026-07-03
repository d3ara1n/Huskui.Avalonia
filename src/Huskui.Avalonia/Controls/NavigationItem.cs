using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(PC_SELECTED, PC_GROUP_START, PC_PANE_COLLAPSED)]
public class NavigationItem : Button, ISelectable
{
    public const string PC_SELECTED = ":selected";
    public const string PC_GROUP_START = ":group-start";
    public const string PC_PANE_COLLAPSED = ":collapsed";

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<NavigationItem>();

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<NavigationItem, object?>(nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavigationItem, IDataTemplate?>(nameof(IconTemplate));

    public static readonly StyledProperty<Type?> PageTypeProperty =
        AvaloniaProperty.Register<NavigationItem, Type?>(nameof(PageType));

    public static readonly StyledProperty<object?> ParameterProperty =
        AvaloniaProperty.Register<NavigationItem, object?>(nameof(Parameter));

    public static readonly StyledProperty<string?> CategoryProperty =
        AvaloniaProperty.Register<NavigationItem, string?>(nameof(Category));

    public static readonly StyledProperty<bool> IsCollapsedProperty =
        AvaloniaProperty.Register<NavigationItem, bool>(nameof(IsCollapsed));

    static NavigationItem() => SelectableMixin.Attach<NavigationItem>(IsSelectedProperty);

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public Type? PageType
    {
        get => GetValue(PageTypeProperty);
        set => SetValue(PageTypeProperty, value);
    }

    public object? Parameter
    {
        get => GetValue(ParameterProperty);
        set => SetValue(ParameterProperty, value);
    }

    public string? Category
    {
        get => GetValue(CategoryProperty);
        set => SetValue(CategoryProperty, value);
    }

    public bool IsSelected
    {
        get => GetValue(IsSelectedProperty);
        set => SetValue(IsSelectedProperty, value);
    }

    public bool IsCollapsed
    {
        get => GetValue(IsCollapsedProperty);
        set => SetValue(IsCollapsedProperty, value);
    }

    internal void MarkGroupStart(bool value) => PseudoClasses.Set(PC_GROUP_START, value);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsCollapsedProperty)
            PseudoClasses.Set(PC_PANE_COLLAPSED, change.GetNewValue<bool>());
    }
}
