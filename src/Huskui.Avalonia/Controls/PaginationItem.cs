using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":current")]
public class PaginationItem : Button
{
    public static readonly StyledProperty<bool> IsCurrentProperty =
        AvaloniaProperty.Register<PaginationItem, bool>(nameof(IsCurrent));

    public static readonly StyledProperty<int> PageIndexProperty =
        AvaloniaProperty.Register<PaginationItem, int>(nameof(PageIndex));

    public bool IsCurrent
    {
        get => GetValue(IsCurrentProperty);
        set => SetValue(IsCurrentProperty, value);
    }

    public int PageIndex
    {
        get => GetValue(PageIndexProperty);
        set => SetValue(PageIndexProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsCurrentProperty)
            PseudoClasses.Set(":current", change.GetNewValue<bool>());
    }
}
