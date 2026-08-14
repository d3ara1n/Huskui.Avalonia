using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ItemsControl, typeof(ItemsControl), IsRequired = true)]
[TemplatePart(PART_QuickJumperPopup, typeof(Popup))]
[TemplatePart(PART_QuickJumperInput, typeof(NumericUpDown))]
[TemplatePart(PART_QuickJumperButton, typeof(Button))]
public class PaginationControl : TemplatedControl
{
    public const string PART_ItemsControl = nameof(PART_ItemsControl);
    public const string PART_QuickJumperPopup = nameof(PART_QuickJumperPopup);
    public const string PART_QuickJumperInput = nameof(PART_QuickJumperInput);
    public const string PART_QuickJumperButton = nameof(PART_QuickJumperButton);

    private const int ELLIPSIS_PAGE_INDEX = -1;
    private const int MAX_VISIBLE_PAGE_COUNT = 7;

    private ItemsControl? _itemsControl;
    private Popup? _quickJumperPopup;
    private NumericUpDown? _quickJumperInput;
    private Button? _quickJumperButton;
    private bool _updating;

    public PaginationControl() => AddHandler(Button.ClickEvent, OnButtonClick);

    static PaginationControl()
    {
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<PaginationControl>(KeyboardNavigationMode.Once);
    }

    public static readonly StyledProperty<int> TotalCountProperty =
        AvaloniaProperty.Register<PaginationControl, int>(nameof(TotalCount));

    public static readonly StyledProperty<int> PageSizeProperty =
        AvaloniaProperty.Register<PaginationControl, int>(nameof(PageSize), defaultValue: 10);

    public static readonly StyledProperty<int> PageIndexProperty =
        AvaloniaProperty.Register<PaginationControl, int>(nameof(PageIndex), defaultValue: 0);

    public static readonly DirectProperty<PaginationControl, int> PageCountProperty =
        AvaloniaProperty.RegisterDirect<PaginationControl, int>(nameof(PageCount), o => o.PageCount);

    public static readonly DirectProperty<PaginationControl, int> DisplayIndexProperty =
        AvaloniaProperty.RegisterDirect<PaginationControl, int>(nameof(DisplayIndex), o => o.DisplayIndex);

    public static readonly RoutedEvent<PropertyChangedRoutedEventArgs<int>> IndexChangedEvent =
        RoutedEvent.Register<PaginationControl, PropertyChangedRoutedEventArgs<int>>(
            nameof(IndexChanged),
            RoutingStrategies.Bubble
        );

    public int TotalCount
    {
        get => GetValue(TotalCountProperty);
        set => SetValue(TotalCountProperty, value);
    }

    public int PageSize
    {
        get => GetValue(PageSizeProperty);
        set => SetValue(PageSizeProperty, value);
    }

    /// <summary>
    /// Gets or sets the current page index (0-based). This is the authoritative page state — bind or
    /// observe it to drive data-loading logic (e.g. <c>items.Skip(PageIndex * PageSize).Take(PageSize)</c>).
    /// Changing <see cref="TotalCount"/> or <see cref="PageSize"/> may coerce it into range, which also
    /// raises <see cref="IndexChanged"/>. For the 1-based page number shown in the UI, use <see cref="DisplayIndex"/>.
    /// </summary>
    public int PageIndex
    {
        get => GetValue(PageIndexProperty);
        set => SetValue(PageIndexProperty, value);
    }

    /// <summary>Gets the 1-based page number as shown in the UI, equivalent to <see cref="PageIndex"/> + 1.</summary>
    public int DisplayIndex
    {
        get;
        private set => SetAndRaise(DisplayIndexProperty, ref field, value);
    } = 1;

    public int PageCount
    {
        get;
        private set => SetAndRaise(PageCountProperty, ref field, value);
    }

    /// <summary>
    /// Raised after the current page has changed.
    /// <see cref="PropertyChangedRoutedEventArgs{T}.OldValue"/> and <see cref="PropertyChangedRoutedEventArgs{T}.NewValue"/>
    /// are 0-based; use <see cref="DisplayIndex"/> for the 1-based page number.
    /// </summary>
    public event EventHandler<PropertyChangedRoutedEventArgs<int>>? IndexChanged
    {
        add => AddHandler(IndexChangedEvent, value);
        remove => RemoveHandler(IndexChangedEvent, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (_updating)
            return;

        if (change.Property == TotalCountProperty || change.Property == PageSizeProperty)
        {
            Refresh(updatePageCount: true);
        }
        else if (change.Property == PageIndexProperty)
        {
            Refresh(updatePageCount: false, oldIndex: change.GetOldValue<int>());
        }
    }

    private void RaiseIndexChanged(int oldIndex, int newIndex) =>
        RaiseEvent(
            new PropertyChangedRoutedEventArgs<int>(IndexChangedEvent, this, oldIndex, newIndex)
        );

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        ClearPageItems();

        if (_quickJumperInput is not null)
            _quickJumperInput.KeyDown -= OnQuickJumperInputKeyDown;

        if (_quickJumperButton is not null)
            _quickJumperButton.Click -= OnQuickJumperButtonClick;

        _itemsControl = e.NameScope.Find<ItemsControl>(PART_ItemsControl);
        _quickJumperPopup = e.NameScope.Find<Popup>(PART_QuickJumperPopup);
        _quickJumperInput = e.NameScope.Find<NumericUpDown>(PART_QuickJumperInput);
        _quickJumperButton = e.NameScope.Find<Button>(PART_QuickJumperButton);

        if (_quickJumperInput is not null)
            _quickJumperInput.KeyDown += OnQuickJumperInputKeyDown;

        if (_quickJumperButton is not null)
            _quickJumperButton.Click += OnQuickJumperButtonClick;

        Refresh(updatePageCount: true);
    }

    public void GoToFirst() => GoToPage(0);

    public void GoToLast() => GoToPage(PageCount - 1);

    public void GoToNext() => GoToPage(PageIndex + 1);

    public void GoToPrevious() => GoToPage(PageIndex - 1);

    public void GoToPage(int page) => PageIndex = CoercePageIndex(page);

    private void OnButtonClick(object? sender, RoutedEventArgs e)
    {
        if (e.Source is not PaginationItem item)
            return;

        e.Handled = true;

        if (item.PageIndex == ELLIPSIS_PAGE_INDEX)
        {
            OpenQuickJumper(item);
            return;
        }

        GoToPage(item.PageIndex);
    }

    private void OnQuickJumperButtonClick(object? sender, RoutedEventArgs e) => CommitQuickJump();

    private void OnQuickJumperInputKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            CommitQuickJump();
            e.Handled = true;
        }
        else if (e.Key == Key.Escape)
        {
            _quickJumperPopup?.IsOpen = false;
            e.Handled = true;
        }
    }

    private void OpenQuickJumper(PaginationItem placementTarget)
    {
        if (_quickJumperPopup is null || _quickJumperInput is null)
            return;

        _quickJumperInput.Maximum = Math.Max(1, PageCount);

        _quickJumperInput.Value = DisplayIndex;
        _quickJumperInput.PlaceholderText = DisplayIndex.ToString();
        _quickJumperPopup.PlacementTarget = placementTarget;
        _quickJumperPopup.IsOpen = true;
        _quickJumperInput.Focus();
    }

    private void CommitQuickJump()
    {
        if (_quickJumperInput?.Value is { } value)
        {
            var maxPage = Math.Max(1, PageCount);
            var displayPage = decimal.ToInt32(Math.Clamp(value, 1, maxPage));
            GoToPage(displayPage - 1);
        }

        _quickJumperPopup?.IsOpen = false;
    }

    private int ComputePageCount() => TotalCount > 0 && PageSize > 0 ? (TotalCount - 1) / PageSize + 1 : 0;

    private int CoercePageIndex(int page) => Math.Clamp(page, 0, Math.Max(0, PageCount - 1));

    private void Refresh(bool updatePageCount, int? oldIndex = null)
    {
        // When a PageIndex change triggers the refresh, PageIndex already holds the new value, so
        // that caller must supply the pre-change index; every other path reads the still-unchanged
        // PageIndex as the origin for IndexChanged.
        var from = oldIndex ?? PageIndex;
        _updating = true;

        try
        {
            if (updatePageCount)
                PageCount = ComputePageCount();

            PageIndex = CoercePageIndex(PageIndex);
            DisplayIndex = PageIndex + 1;
            UpdatePageItems();
            _quickJumperInput?.Maximum = Math.Max(1, PageCount);
        }
        finally
        {
            _updating = false;
        }

        if (from != PageIndex)
            RaiseIndexChanged(from, PageIndex);
    }

    private void UpdatePageItems()
    {
        if (_itemsControl is null)
            return;

        var pages = GetPageSlots();
        EnsurePageItems(pages.Length);

        var items = PageItems;
        for (var i = 0; i < pages.Length; i++)
            SetPageItem(items[i], pages[i]);
    }

    private int[] GetPageSlots()
    {
        var total = PageCount;
        if (total <= 0)
            return [];

        var maxVisibleCount = Math.Max(5, MAX_VISIBLE_PAGE_COUNT);
        if (total <= maxVisibleCount)
            return Enumerable.Range(0, total).ToArray();

        var current = CoercePageIndex(PageIndex);
        var middlePageCount = maxVisibleCount - 4;
        var firstMiddlePage = current - middlePageCount / 2;
        var lastMiddlePage = firstMiddlePage + middlePageCount - 1;

        if (firstMiddlePage <= 2)
        {
            firstMiddlePage = 1;
            lastMiddlePage = maxVisibleCount - 3;
        }
        else if (lastMiddlePage >= total - 3)
        {
            firstMiddlePage = total - maxVisibleCount + 2;
            lastMiddlePage = total - 2;
        }

        var slots = new List<int>(maxVisibleCount) { 0 };

        if (firstMiddlePage > 1)
            slots.Add(ELLIPSIS_PAGE_INDEX);

        for (var page = firstMiddlePage; page <= lastMiddlePage; page++)
            slots.Add(page);

        if (lastMiddlePage < total - 2)
            slots.Add(ELLIPSIS_PAGE_INDEX);

        slots.Add(total - 1);
        return slots.ToArray();
    }

    private IReadOnlyList<PaginationItem> PageItems =>
        _itemsControl?.ItemsSource as IReadOnlyList<PaginationItem> ?? [];

    private void EnsurePageItems(int count)
    {
        if (_itemsControl is null || PageItems.Count == count)
            return;

        ClearPageItems();

        _itemsControl.ItemsSource = Enumerable.Range(0, count).Select(_ => new PaginationItem()).ToArray();
    }

    private void ClearPageItems()
    {
        _itemsControl?.ItemsSource = null;
    }

    private void SetPageItem(PaginationItem item, int pageIndex)
    {
        item.PageIndex = pageIndex;
        item.IsCurrent = pageIndex == PageIndex;
        item.IsEnabled = true;
    }
}
