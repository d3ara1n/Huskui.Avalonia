using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(CLASS_Selected, CLASS_GroupStart, CLASS_Collapsed)]
public class NavigationItem : Button, ISelectable
{
    public const string CLASS_Selected = ":selected";
    public const string CLASS_GroupStart = ":group-start";
    public const string CLASS_Collapsed = ":collapsed";

    public static readonly StyledProperty<bool> IsSelectedProperty =
        SelectingItemsControl.IsSelectedProperty.AddOwner<NavigationItem>();

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<NavigationItem, object?>(nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavigationItem, IDataTemplate?>(nameof(IconTemplate));

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

    internal void MarkGroupStart(bool value) => PseudoClasses.Set(CLASS_GroupStart, value);

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsCollapsedProperty)
            PseudoClasses.Set(CLASS_Collapsed, change.GetNewValue<bool>());
    }
}
