using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_SelectingItemsControl, typeof(ListBox))]
[TemplatePart(PART_TextBox, typeof(TextBox))]
[PseudoClasses(":dropdownopen")]
public class TagBox : TemplatedControl
{
    public const string PART_Border = nameof(PART_Border);
    public const string PART_SelectingItemsControl = nameof(PART_SelectingItemsControl);
    public const string PART_TextBox = nameof(PART_TextBox);

    public static readonly StyledProperty<IEnumerable<string>?> ItemsSourceProperty =
        AvaloniaProperty.Register<TagBox, IEnumerable<string>?>(nameof(ItemsSource));

    public static readonly DirectProperty<TagBox, IList<string>> SelectedItemsProperty =
        AvaloniaProperty.RegisterDirect<TagBox, IList<string>>(
            nameof(SelectedItems),
            o => o.SelectedItems,
            (o, v) => o.SelectedItems = v
        );

    public static readonly StyledProperty<string?> TextProperty = AvaloniaProperty.Register<
        TagBox,
        string?
    >(nameof(Text), string.Empty);

    public static readonly StyledProperty<bool> IsDropDownOpenProperty = AvaloniaProperty.Register<
        TagBox,
        bool
    >(nameof(IsDropDownOpen));

    public static readonly StyledProperty<bool> AllowCustomTagsProperty = AvaloniaProperty.Register<
        TagBox,
        bool
    >(nameof(AllowCustomTags));

    public static readonly StyledProperty<int> MinimumPrefixLengthProperty =
        AvaloniaProperty.Register<TagBox, int>(nameof(MinimumPrefixLength));

    public static readonly StyledProperty<string?> PlaceholderTextProperty =
        AvaloniaProperty.Register<TagBox, string?>(nameof(PlaceholderText));

    public static readonly StyledProperty<IBrush?> PlaceholderForegroundProperty =
        AvaloniaProperty.Register<TagBox, IBrush?>(nameof(PlaceholderForeground));

    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        AvaloniaProperty.Register<TagBox, double>(nameof(MaxDropDownHeight), 336d);

    private static readonly StringComparer TagComparer = StringComparer.OrdinalIgnoreCase;

    private readonly ObservableCollection<string> _displayedSelectedItems = [];
    private readonly ObservableCollection<TagBoxSuggestion> _suggestions = [];
    private readonly ICommand _removeTagCommand;
    private INotifyCollectionChanged? _itemsSourceNotifier;
    private ListBox? _suggestionList;
    private TextBox? _textBox;

    private INotifyCollectionChanged? _selectedItemsNotifier;
    private IList<string> _selectedItems = new AvaloniaList<string>();

    static TagBox()
    {
        FocusableProperty.OverrideDefaultValue<TagBox>(true);
    }

    public TagBox()
    {
        _removeTagCommand = new DelegatingCommand(
            parameter => RemoveSelectedTag(parameter as string),
            parameter => parameter is string
        );

        AttachToSelectedItems(_selectedItems);
        SyncSelectedItemsView();
    }

    public IEnumerable<string>? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IList<string> SelectedItems
    {
        get => _selectedItems;
        set
        {
            var newValue = value ?? new AvaloniaList<string>();
            if (ReferenceEquals(_selectedItems, newValue))
            {
                return;
            }

            var oldValue = _selectedItems;
            DetachFromSelectedItems(oldValue);
            _selectedItems = newValue;
            AttachToSelectedItems(newValue);
            SyncSelectedItemsView();
            RefreshSuggestions();
            RaisePropertyChanged(SelectedItemsProperty, oldValue, newValue);
        }
    }

    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsDropDownOpen
    {
        get => GetValue(IsDropDownOpenProperty);
        set => SetValue(IsDropDownOpenProperty, value);
    }

    public bool AllowCustomTags
    {
        get => GetValue(AllowCustomTagsProperty);
        set => SetValue(AllowCustomTagsProperty, value);
    }

    public int MinimumPrefixLength
    {
        get => GetValue(MinimumPrefixLengthProperty);
        set => SetValue(MinimumPrefixLengthProperty, value);
    }

    public string? PlaceholderText
    {
        get => GetValue(PlaceholderTextProperty);
        set => SetValue(PlaceholderTextProperty, value);
    }

    public IBrush? PlaceholderForeground
    {
        get => GetValue(PlaceholderForegroundProperty);
        set => SetValue(PlaceholderForegroundProperty, value);
    }

    public double MaxDropDownHeight
    {
        get => GetValue(MaxDropDownHeightProperty);
        set => SetValue(MaxDropDownHeightProperty, value);
    }

    public ICommand RemoveTagCommand => _removeTagCommand;

    public IReadOnlyList<string> DisplayedSelectedItems => _displayedSelectedItems;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnregisterTemplateHandlers();

        _suggestionList = e.NameScope.Find<ListBox>(PART_SelectingItemsControl);
        _textBox = e.NameScope.Find<TextBox>(PART_TextBox);

        if (_suggestionList != null)
        {
            _suggestionList.ItemsSource = _suggestions;
            _suggestionList.AddHandler(
                PointerReleasedEvent,
                OnSuggestionPointerReleased,
                RoutingStrategies.Tunnel
            );
        }

        if (_textBox != null)
        {
            _textBox.KeyDown += OnTextBoxKeyDown;
            _textBox.GotFocus += OnTextBoxGotFocus;
        }

        RefreshSuggestions();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        UnregisterTemplateHandlers();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty)
        {
            DetachFromItemsSource(change.OldValue as IEnumerable<string>);
            AttachToItemsSource(change.NewValue as IEnumerable<string>);
            RefreshSuggestions();
            return;
        }

        if (change.Property == TextProperty)
        {
            RefreshSuggestions();
            return;
        }

        if (
            change.Property == AllowCustomTagsProperty
            || change.Property == MinimumPrefixLengthProperty
        )
        {
            RefreshSuggestions();
            return;
        }

        if (change.Property == IsDropDownOpenProperty)
        {
            PseudoClasses.Set(":dropdownopen", change.GetNewValue<bool>());
        }
    }

    private void OnTextBoxGotFocus(object? sender, RoutedEventArgs e) => RefreshSuggestions();

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Down)
        {
            MoveSelection(1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Up)
        {
            MoveSelection(-1);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && IsDropDownOpen)
        {
            IsDropDownOpen = false;
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Back && string.IsNullOrEmpty(Text))
        {
            RemoveLastSelectedTag();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Enter || e.Key == Key.Tab)
        {
            if (TryCommitActiveSuggestion() || TryCommitSelection(Text))
            {
                e.Handled = true;
            }
        }
    }

    private void OnSuggestionPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.Source is not Visual visual)
        {
            return;
        }

        var listBoxItem = visual.FindAncestorOfType<ListBoxItem>();
        if (listBoxItem?.DataContext is not TagBoxSuggestion suggestion)
        {
            return;
        }

        if (TryCommitSelection(suggestion.Text))
        {
            e.Handled = true;
        }
    }

    private void MoveSelection(int offset)
    {
        if (_suggestions.Count == 0)
        {
            IsDropDownOpen = false;
            return;
        }

        if (!IsDropDownOpen)
        {
            IsDropDownOpen = true;
        }

        if (_suggestionList == null)
        {
            return;
        }

        var nextIndex = _suggestionList.SelectedIndex;
        nextIndex = nextIndex < 0 ? (offset > 0 ? 0 : _suggestions.Count - 1) : nextIndex + offset;
        nextIndex = Math.Clamp(nextIndex, 0, _suggestions.Count - 1);

        _suggestionList.SelectedIndex = nextIndex;
        if (_suggestionList.SelectedItem != null)
        {
            _suggestionList.ScrollIntoView(_suggestionList.SelectedItem);
        }
    }

    private bool TryCommitActiveSuggestion()
    {
        if (_suggestions.Count == 0)
        {
            return false;
        }

        var suggestion = _suggestionList?.SelectedItem as TagBoxSuggestion ?? _suggestions[0];
        return TryCommitSelection(suggestion.Text);
    }

    private bool TryCommitSelection(string? value)
    {
        var normalized = NormalizeTag(value);
        if (string.IsNullOrEmpty(normalized))
        {
            return false;
        }

        if (ContainsTag(SelectedItems, normalized))
        {
            Text = string.Empty;
            RefreshSuggestions();
            return true;
        }

        if (!TryAddTag(normalized))
        {
            return false;
        }

        Text = string.Empty;
        SyncSelectedItemsView();
        RefreshSuggestions();
        _textBox?.Focus();
        return true;
    }

    private void RemoveLastSelectedTag()
    {
        if (_displayedSelectedItems.Count == 0)
        {
            return;
        }

        RemoveSelectedTag(_displayedSelectedItems[^1]);
    }

    private void RemoveSelectedTag(string? value)
    {
        var normalized = NormalizeTag(value);
        if (string.IsNullOrEmpty(normalized))
        {
            return;
        }

        if (!TryRemoveTag(normalized))
        {
            return;
        }

        SyncSelectedItemsView();
        RefreshSuggestions();
        _textBox?.Focus();
    }

    private bool TryAddTag(string value)
    {
        try
        {
            SelectedItems.Add(value);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool TryRemoveTag(string value)
    {
        var index = IndexOfTag(SelectedItems, value);
        if (index < 0)
        {
            return false;
        }

        try
        {
            SelectedItems.RemoveAt(index);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private void RefreshSuggestions()
    {
        var input = NormalizeInput(Text);
        var allowSuggestions = input.Length >= Math.Max(MinimumPrefixLength, 0);

        _suggestions.Clear();

        if (!allowSuggestions)
        {
            IsDropDownOpen = false;
            UpdateSuggestionSelection();
            return;
        }

        var selected = ToTagSet(SelectedItems);
        var seen = new HashSet<string>(TagComparer);

        if (AllowCustomTags && !string.IsNullOrWhiteSpace(input) && !selected.Contains(input))
        {
            _suggestions.Add(new(input, true));
            seen.Add(input);
        }

        foreach (var item in EnumerateItems(ItemsSource))
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                continue;
            }

            if (selected.Contains(item) || seen.Contains(item))
            {
                continue;
            }

            if (!MatchesFilter(item, input))
            {
                continue;
            }

            _suggestions.Add(new(item, false));
            seen.Add(item);
        }

        IsDropDownOpen = _textBox?.IsFocused == true && _suggestions.Count > 0;
        UpdateSuggestionSelection();
    }

    private void UpdateSuggestionSelection()
    {
        if (_suggestionList == null)
        {
            return;
        }

        _suggestionList.SelectedIndex = _suggestions.Count > 0 ? 0 : -1;
    }

    private static bool MatchesFilter(string item, string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        return item.Contains(input, StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeInput(string? value) => value?.Trim() ?? string.Empty;

    private static string? NormalizeTag(string? value)
    {
        var normalized = NormalizeInput(value);
        return normalized.Length == 0 ? null : normalized;
    }

    private static int IndexOfTag(IEnumerable<string> source, string value)
    {
        var index = 0;
        foreach (var item in source)
        {
            if (TagComparer.Equals(item, value))
            {
                return index;
            }

            index++;
        }

        return -1;
    }

    private static bool ContainsTag(IEnumerable<string> source, string value) =>
        IndexOfTag(source, value) >= 0;

    private static HashSet<string> ToTagSet(IEnumerable<string> source)
    {
        var set = new HashSet<string>(TagComparer);
        foreach (var item in source)
        {
            if (!string.IsNullOrWhiteSpace(item))
            {
                set.Add(item);
            }
        }

        return set;
    }

    private static IEnumerable<string> EnumerateItems(IEnumerable<string>? source)
    {
        if (source == null)
        {
            yield break;
        }

        foreach (var item in source)
        {
            yield return item;
        }
    }

    private void SyncSelectedItemsView()
    {
        _displayedSelectedItems.Clear();

        var seen = new HashSet<string>(TagComparer);
        foreach (var item in SelectedItems)
        {
            if (string.IsNullOrWhiteSpace(item) || !seen.Add(item))
            {
                continue;
            }

            _displayedSelectedItems.Add(item);
        }
    }

    private void AttachToItemsSource(IEnumerable<string>? itemsSource)
    {
        _itemsSourceNotifier = itemsSource as INotifyCollectionChanged;
        if (_itemsSourceNotifier != null)
        {
            _itemsSourceNotifier.CollectionChanged += OnItemsSourceCollectionChanged;
        }
    }

    private void DetachFromItemsSource(IEnumerable<string>? itemsSource)
    {
        var notifier = itemsSource as INotifyCollectionChanged ?? _itemsSourceNotifier;
        if (notifier != null)
        {
            notifier.CollectionChanged -= OnItemsSourceCollectionChanged;
        }

        _itemsSourceNotifier = null;
    }

    private void AttachToSelectedItems(IList<string> selectedItems)
    {
        _selectedItemsNotifier = selectedItems as INotifyCollectionChanged;
        if (_selectedItemsNotifier != null)
        {
            _selectedItemsNotifier.CollectionChanged += OnSelectedItemsCollectionChanged;
        }
    }

    private void DetachFromSelectedItems(IList<string> selectedItems)
    {
        var notifier = selectedItems as INotifyCollectionChanged ?? _selectedItemsNotifier;
        if (notifier != null)
        {
            notifier.CollectionChanged -= OnSelectedItemsCollectionChanged;
        }

        _selectedItemsNotifier = null;
    }

    private void OnItemsSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RefreshSuggestions();
    }

    private void OnSelectedItemsCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e
    )
    {
        SyncSelectedItemsView();
        RefreshSuggestions();
    }

    private void UnregisterTemplateHandlers()
    {
        if (_suggestionList != null)
        {
            _suggestionList.RemoveHandler(PointerReleasedEvent, OnSuggestionPointerReleased);
            _suggestionList.ItemsSource = null;
        }

        if (_textBox != null)
        {
            _textBox.KeyDown -= OnTextBoxKeyDown;
            _textBox.GotFocus -= OnTextBoxGotFocus;
        }

        _suggestionList = null;
        _textBox = null;
    }
}

public sealed class TagBoxSuggestion(string text, bool isCustom)
{
    public string Text { get; } = text;

    public bool IsCustom { get; } = isCustom;
}

internal sealed class DelegatingCommand(
    Action<object?> execute,
    Predicate<object?>? canExecute = null
) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

    public void Execute(object? parameter) => execute(parameter);

    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
