using Avalonia;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace Huskui.Avalonia.Controls;

/// <summary>
///     An inline element that displays a HighlightBlock with proper baseline alignment.
///     Use this instead of HighlightBlock when embedding in TextBlock with other text.
/// </summary>
public class HighlightInline : InlineUIContainer
{
    public static readonly StyledProperty<string> TextProperty =
        HighlightBlock.TextProperty.AddOwner<HighlightInline>();

    public static readonly StyledProperty<bool> IsShortcutProperty =
        AvaloniaProperty.Register<HighlightInline, bool>(nameof(IsShortcut));

    public static readonly StyledProperty<bool> IsPrimaryProperty =
        AvaloniaProperty.Register<HighlightInline, bool>(nameof(IsPrimary));

    private readonly HighlightBlock _highlightBlock;

    public HighlightInline()
    {
        BaselineAlignment = BaselineAlignment.TextBottom;

        _highlightBlock = new();
        _highlightBlock.Bind(HighlightBlock.TextProperty, this.GetObservable(TextProperty));

        Child = _highlightBlock;
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public bool IsShortcut
    {
        get => GetValue(IsShortcutProperty);
        set => SetValue(IsShortcutProperty, value);
    }

    public bool IsPrimary
    {
        get => GetValue(IsPrimaryProperty);
        set => SetValue(IsPrimaryProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsShortcutProperty)
        {
            UpdateClasses();
        }
        else if (change.Property == IsPrimaryProperty)
        {
            UpdateClasses();
        }
    }

    private void UpdateClasses()
    {
        _highlightBlock.Classes.Clear();

        if (IsShortcut)
        {
            _highlightBlock.Classes.Add("Shortcut");
        }

        if (IsPrimary)
        {
            _highlightBlock.Classes.Add("Primary");
        }
    }
}
