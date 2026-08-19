using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System.Windows.Input;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(CLASS_PaneCollapsed)]
public class NavigationView : SelectingItemsControl
{
    public const string CLASS_PaneCollapsed = ":pane-collapsed";

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavigationView, IDataTemplate?>(nameof(IconTemplate));

    public static readonly StyledProperty<bool> IsPaneOpenProperty =
        AvaloniaProperty.Register<NavigationView, bool>(
            nameof(IsPaneOpen),
            defaultValue: true,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<double> PaneOpenWidthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(PaneOpenWidth), defaultValue: 320);

    public static readonly StyledProperty<double> PaneClosedWidthProperty =
        AvaloniaProperty.Register<NavigationView, double>(nameof(PaneClosedWidth), defaultValue: 82);

    public static readonly StyledProperty<object?> PaneHeaderProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(PaneHeader));

    public static readonly StyledProperty<object?> PaneFooterProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(PaneFooter));

    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<NavigationView, object?>(nameof(Content));

    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsBackButtonVisible), defaultValue: true);

    public static readonly StyledProperty<ICommand?> BackCommandProperty =
        AvaloniaProperty.Register<NavigationView, ICommand?>(nameof(BackCommand));

    static NavigationView() =>
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<NavigationView>(KeyboardNavigationMode.Once);

    public NavigationView() => AddHandler(Button.ClickEvent, OnItemClicked);

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public double PaneOpenWidth
    {
        get => GetValue(PaneOpenWidthProperty);
        set => SetValue(PaneOpenWidthProperty, value);
    }

    public double PaneClosedWidth
    {
        get => GetValue(PaneClosedWidthProperty);
        set => SetValue(PaneClosedWidthProperty, value);
    }

    public object? PaneHeader
    {
        get => GetValue(PaneHeaderProperty);
        set => SetValue(PaneHeaderProperty, value);
    }

    public object? PaneFooter
    {
        get => GetValue(PaneFooterProperty);
        set => SetValue(PaneFooterProperty, value);
    }

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    public ICommand? BackCommand
    {
        get => GetValue(BackCommandProperty);
        set => SetValue(BackCommandProperty, value);
    }

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<NavigationItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new NavigationItem();

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is not NavigationItem nvi)
            return;

        // Pure-model items forward their display fields by property-name binding; the control never
        // assumes the item type. NavigationItem-as-data still works via the self-equal path.
        if (!ReferenceEquals(nvi, item))
        {
            nvi.Content = item;
            nvi[!NavigationItem.IconProperty] = new Binding("Icon") { Source = item };
            nvi[!NavigationItem.CategoryProperty] = new Binding("Category") { Source = item };
        }

        nvi[!NavigationItem.IconTemplateProperty] = this[!IconTemplateProperty];
        nvi[!ContentControl.ContentTemplateProperty] = this[!ItemTemplateProperty];

        nvi.IsCollapsed = !IsPaneOpen;
    }

    private void RefreshGroupStarts()
    {
        foreach (var container in GetRealizedContainers().OfType<NavigationItem>())
        {
            var index = IndexFromContainer(container);
            if (index < 0)
                continue;
            container.MarkGroupStart(IsFirstOfCategory(index));
        }
    }

    protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
    {
        base.ContainerForItemPreparedOverride(container, item, index);
        RefreshGroupStarts();
    }

    protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
    {
        base.ContainerIndexChangedOverride(container, oldIndex, newIndex);
        RefreshGroupStarts();
    }

    protected override void ClearContainerForItemOverride(Control container)
    {
        base.ClearContainerForItemOverride(container);
        RefreshGroupStarts();
    }

    private bool IsFirstOfCategory(int index)
    {
        if (ContainerFromIndex(index) is not NavigationItem current)
            return false;
        var category = current.Category;
        if (string.IsNullOrEmpty(category))
            return false;
        if (index == 0)
            return true;
        return (ContainerFromIndex(index - 1) as NavigationItem)?.Category != category;
    }

    private void OnItemClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Handled || e.Source is not Visual source)
            return;

        var item = source.GetSelfAndVisualAncestors().OfType<NavigationItem>().FirstOrDefault();
        if (item is null)
            return;

        var index = IndexFromContainer(item);
        if (index >= 0)
        {
            UpdateSelection(index);
            e.Handled = true;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsPaneOpenProperty)
        {
            var collapsed = !change.GetNewValue<bool>();
            PseudoClasses.Set(CLASS_PaneCollapsed, collapsed);
            foreach (var item in GetRealizedContainers().OfType<NavigationItem>())
                item.IsCollapsed = collapsed;
        }
    }
}
