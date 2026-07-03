using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Avalonia.VisualTree;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ITEMS_PRESENTER, typeof(ItemsPresenter))]
[PseudoClasses(HORIZONTAL_COLLAPSED)]
public class NavMenu : ItemsControl
{
    public const string PART_ITEMS_PRESENTER = "PART_ItemsPresenter";
    public const string HORIZONTAL_COLLAPSED = ":horizontal-collapsed";

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<NavMenu, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<object?> HeaderTemplateProperty =
        HeaderedContentControl.HeaderProperty.AddOwner<NavMenu>();


    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<NavMenu, object?>(nameof(Footer));

    public static readonly StyledProperty<bool> IsHorizontalCollapsedProperty =
        AvaloniaProperty.Register<NavMenu, bool>(nameof(IsHorizontalCollapsed));

    public static readonly StyledProperty<double> ExpandWidthProperty = AvaloniaProperty.Register<NavMenu, double>(
     nameof(ExpandWidth), double.NaN);

    public static readonly StyledProperty<double> CollapseWidthProperty = AvaloniaProperty.Register<NavMenu, double>(
     nameof(CollapseWidth), double.NaN);

    public static readonly RoutedEvent<SelectionChangedEventArgs> SelectionChangedEvent =
        RoutedEvent.Register<NavMenu, SelectionChangedEventArgs>(nameof(SelectionChanged), RoutingStrategies.Bubble);

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavMenu, IDataTemplate?>(
                                                           nameof(IconTemplate));

    public static readonly StyledProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<NavMenu, IDataTemplate?>(
                                                           nameof(ContentTemplate));

    public static readonly StyledProperty<BindingBase?> IconBindingProperty =
        AvaloniaProperty.Register<NavMenu, BindingBase?>(
                                                         nameof(IconBinding));

    public static readonly StyledProperty<BindingBase?> TitleBindingProperty =
        AvaloniaProperty.Register<NavMenu, BindingBase?>(
                                                         nameof(TitleBinding));

    public static readonly StyledProperty<BindingBase?> DescriptionBindingProperty =
        AvaloniaProperty.Register<NavMenu, BindingBase?>(
                                                         nameof(DescriptionBinding));

    public static readonly StyledProperty<BindingBase?> CommandBindingProperty =
        AvaloniaProperty.Register<NavMenu, BindingBase?>(
                                                         nameof(CommandBinding));

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public object? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public IDataTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public bool IsHorizontalCollapsed
    {
        get => GetValue(IsHorizontalCollapsedProperty);
        set => SetValue(IsHorizontalCollapsedProperty, value);
    }

    public double ExpandWidth
    {
        get => GetValue(ExpandWidthProperty);
        set => SetValue(ExpandWidthProperty, value);
    }

    public double CollapseWidth
    {
        get => GetValue(CollapseWidthProperty);
        set => SetValue(CollapseWidthProperty, value);
    }

    public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
    {
        add => AddHandler(SelectionChangedEvent, value);
        remove => RemoveHandler(SelectionChangedEvent, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? IconBinding
    {
        get => GetValue(IconBindingProperty);
        set => SetValue(IconBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? TitleBinding
    {
        get => GetValue(TitleBindingProperty);
        set => SetValue(TitleBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? DescriptionBinding
    {
        get => GetValue(DescriptionBindingProperty);
        set => SetValue(DescriptionBindingProperty, value);
    }

    [AssignBinding]
    [InheritDataTypeFromItems(nameof(ItemsSource))]
    public BindingBase? CommandBinding
    {
        get => GetValue(CommandBindingProperty);
        set => SetValue(CommandBindingProperty, value);
    }

    private ItemsPresenter? _itemsPresenter;
    private NavMenuItem? _selectedContainer;

    static NavMenu()
    {
        SelectedItemProperty.Changed.AddClassHandler<NavMenu, object?>((o, e) => o.OnSelectedItemChanged(e));
        IsHorizontalCollapsedProperty.Changed.AddClassHandler<NavMenu, bool>((o, e) =>
                                                                                 o.PseudoClasses.Set(HORIZONTAL_COLLAPSED, e.NewValue.Value));
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
    {
        return NeedsContainer<NavMenuItem>(item, out recycleKey);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new NavMenuItem();
    }

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);
        if (container is NavMenuItem menuItem)
        {
            if (IconBinding is not null)
            {
                menuItem.Bind(NavMenuItem.IconProperty, IconBinding);
            }

            if (TitleBinding is not null)
            {
                menuItem.Bind(NavMenuItem.TitleProperty, TitleBinding);
            }

            if (DescriptionBinding is not null)
            {
                menuItem.Bind(NavMenuItem.DescriptionProperty, DescriptionBinding);
            }
            if (ContentTemplate is not null)
            {
                menuItem.Content = item;
            }

            else
            {
                if (TitleBinding is not null)
                {
                    menuItem.Bind(ContentControl.ContentProperty, TitleBinding);
                }
            }
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _itemsPresenter = e.NameScope.Find<ItemsPresenter>(PART_ITEMS_PRESENTER);
        if (_itemsPresenter is not null)
            KeyboardNavigation.SetTabNavigation(_itemsPresenter, KeyboardNavigationMode.Once);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        TryToSelectItem(SelectedItem);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Handled) return;

        if (e.Key is Key.Up or Key.Down or Key.Left or Key.Right or Key.Home or Key.End)
        {
            e.Handled = true;
            var source = GetContainerFromEventSource(e.Source);
            if (GetNextItem(source, e.Key) is { } target)
            {
                target.Focus(NavigationMethod.Directional);
            }
        }
    }

    private void TryToSelectItem(object? item)
    {
        if (item is null) return;

        if (item is NavMenuItem menuItem)
        {
            menuItem.SelectItem(menuItem);
            return;
        }

        if (ContainerFromItem(item) is NavMenuItem container)
        {
            container.SelectItem(container);
        }
    }

    private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs<object?> args)
    {
        _selectedContainer?.SetSelected(false);

        var newValue = args.NewValue.Value;
        if (newValue is not null)
        {
            _selectedContainer = ContainerFromItem(newValue) as NavMenuItem;
            _selectedContainer?.SetSelected(true);
        }

        TryToSelectItem(newValue);
        var a = new SelectionChangedEventArgs(
            SelectionChangedEvent,
            new[] { args.OldValue.Value },
            new[] { newValue });
        RaiseEvent(a);
    }

    private NavMenuItem? GetNextItem(NavMenuItem? current, Key key)
    {
        if (current is null) return FindNextValidSibling(0, 1);
        var index = IndexFromContainer(current);

        return key switch
        {
            Key.Up or Key.Left => FindNextValidSibling(index - 1, -1),
            Key.Down or Key.Right => FindNextValidSibling(index + 1, 1),
            Key.Home => FindNextValidSibling(0, 1),
            Key.End => FindNextValidSibling(ItemCount - 1, -1),
            _ => null
        };

        NavMenuItem? FindNextValidSibling(int start, int step)
        {
            for (var i = start; i >= 0 && i < ItemCount; i += step)
            {
                if (ContainerFromIndex(i) is NavMenuItem item)
                {
                    if (item is { IsEnabled: true, IsSeparator: false })
                    {
                        return item;
                    }
                }
            }
            return null;
        }
    }

    private NavMenuItem? GetContainerFromEventSource(object? eventSource)
    {
        if (eventSource is not Visual visual) return null;
        return visual.GetSelfAndVisualAncestors()
                     .OfType<NavMenuItem>()
                     .FirstOrDefault();
    }
}
