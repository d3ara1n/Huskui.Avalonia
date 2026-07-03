using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using System.Linq;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_Frame, typeof(Frame))]
[TemplatePart(PART_BackButton, typeof(Button))]
[PseudoClasses(CLASS_PaneCollapsed)]
public class NavigationView : SelectingItemsControl
{
    public const string PART_Frame = nameof(PART_Frame);
    public const string PART_BackButton = nameof(PART_BackButton);
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

    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.Register<NavigationView, bool>(nameof(IsBackButtonVisible), defaultValue: true);

    static NavigationView() =>
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<NavigationView>(KeyboardNavigationMode.Once);

    public NavigationView()
    {
        AddHandler(Button.ClickEvent, OnItemClicked);
        ContainerPrepared += (_, _) => RefreshGroupStarts();
        ContainerIndexChanged += (_, _) => RefreshGroupStarts();
        ContainerClearing += (_, _) => RefreshGroupStarts();
    }

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

    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    /// <summary>
    /// Page factory forwarded to the internal <see cref="Frame"/>. Wire it through
    /// <c>FrameActivationMixin.Install</c> from the Huskui.Avalonia.Mvvm package.
    /// </summary>
    public Frame.PageActivatorDelegate? PageActivator
    {
        get => _frame?.PageActivator ?? _pendingActivator;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            _pendingActivator = value;
            if (_frame is not null)
                _frame.PageActivator = value;
        }
    }

    private Frame? _frame;
    private Button? _backButton;
    private Frame.PageActivatorDelegate? _pendingActivator;
    private object? _pendingSelection;
    private (Type PageType, object? Parameter, IPageTransition? Transition)? _pendingNavigate;
    private bool _syncingSelection;

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<NavigationItem>(item, out recycleKey);

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new NavigationItem();

    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is not NavigationItem nvi)
            return;

        // Wrapped non-NavigationItem data: present it as the item content.
        if (!ReferenceEquals(nvi, item))
            nvi.Content = item;

        nvi[!NavigationItem.IconTemplateProperty] = this[!IconTemplateProperty];
        nvi[!ContentControl.ContentTemplateProperty] = this[!ItemTemplateProperty];

        var category = nvi.Category;
        var isFirstOfCategory = !string.IsNullOrEmpty(category)
            && (index == 0 || Items[index - 1] is NavigationItem prev && prev.Category != category);
        nvi.MarkGroupStart(isFirstOfCategory);
        nvi.IsCollapsed = !IsPaneOpen;
    }

    private void RefreshGroupStarts()
    {
        foreach (var container in GetRealizedContainers().OfType<NavigationItem>())
        {
            var index = IndexFromContainer(container);
            if (index < 0)
                continue;
            var category = container.Category;
            var isFirst = !string.IsNullOrEmpty(category)
                && (index == 0 || Items[index - 1] is NavigationItem prev && prev.Category != category);
            container.MarkGroupStart(isFirst);
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _frame = e.NameScope.Find<Frame>(PART_Frame);
        _backButton = e.NameScope.Find<Button>(PART_BackButton);

        if (_frame is null)
            return;

        _backButton?.Command = _frame.GoBackCommand;

        if (_pendingActivator is not null)
            _frame.PageActivator = _pendingActivator;

        // NOTE: a deferred initial selection navigates with whatever PageActivator is current. Consumers
        //  who two-way bind SelectedItem before load should install their IViewActivator first, otherwise
        //  the first page is activated by Frame's default activator.
        if (_pendingSelection is { } deferred)
        {
            _pendingSelection = null;
            OnSelectedItemChanged(deferred);
        }

        if (_pendingNavigate is { } pendingNav)
        {
            _pendingNavigate = null;
            Navigate(pendingNav.PageType, pendingNav.Parameter, pendingNav.Transition);
        }
    }

    private void OnItemClicked(object? sender, RoutedEventArgs e)
    {
        if (e.Handled || e.Source is not Visual)
            return;

        var source = (Visual)e.Source;
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

        if (change.Property == SelectedItemProperty)
            OnSelectedItemChanged(change.NewValue);
        else if (change.Property == IsPaneOpenProperty)
        {
            var collapsed = !change.GetNewValue<bool>();
            PseudoClasses.Set(CLASS_PaneCollapsed, collapsed);
            foreach (var item in GetRealizedContainers().OfType<NavigationItem>())
                item.IsCollapsed = collapsed;
        }
    }

    /// <summary>
    /// Pushes a page onto the internal <see cref="Frame"/> — the single navigation primitive.
    /// Selection-driven navigation calls it internally; consumers call it for non-menu pages
    /// (Home, drill-down, deep links). It also keeps <see cref="SelectingItemsControl.SelectedItem"/>
    /// in sync: when the target page matches the current selection (navigation originated from the
    /// UI) it bails out; otherwise it selects the item bound to that page, or clears selection when
    /// the page is not in the menu.
    /// </summary>
    public void Navigate(Type pageType, object? parameter = null, IPageTransition? transition = null)
    {
        if (_frame is null)
        {
            _pendingNavigate = (pageType, parameter, transition);
            return;
        }

        _frame?.Navigate(pageType, parameter, transition);

        if (SelectedItem is NavigationItem { PageType: { } selectedPage } && selectedPage == pageType)
            return;

        var match = Items.OfType<NavigationItem>().FirstOrDefault(i => i.PageType == pageType);
        if (match == SelectedItem)
            return;

        _syncingSelection = true;
        try { SelectedItem = match; }
        finally { _syncingSelection = false; }
    }

    private void OnSelectedItemChanged(object? selected)
    {
        if (_syncingSelection)
            return;

        if (_frame is null)
        {
            _pendingSelection = selected;
            return;
        }

        if (selected is NavigationItem { PageType: { } pageType } item)
            Navigate(pageType, item.Parameter);
    }
}
