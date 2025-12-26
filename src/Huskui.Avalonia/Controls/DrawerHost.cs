using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ItemsPresenter, typeof(ItemsPresenter))]
[PseudoClasses(":present")]
public class DrawerHost : TemplatedControl
{
    public const string PART_ItemsPresenter = nameof(PART_ItemsPresenter);

    public static readonly StyledProperty<int> ItemCountProperty =
        AvaloniaProperty.Register<OverlayHost, int>(nameof(ItemCount));

    public int ItemCount
    {
        get => GetValue(ItemCountProperty);
        set => SetValue(ItemCountProperty, value);
    }


    [Content]
    public AvaloniaList<Drawer> Items { get; } = [];

    public void Pop(Drawer drawer)
    {
        var height = Bounds.Height * 2 / 3;
        var width = Bounds.Width * 1 / 2;
        if (height > drawer.MaxHeight)
        {
            height = drawer.MaxHeight;
        }
        else if (height < drawer.MinHeight)
        {
            height = drawer.MinHeight;
        }

        if (width > drawer.MaxWidth)
        {
            width = drawer.MaxWidth;
        }
        else if (width < drawer.MinWidth)
        {
            width = drawer.MinWidth;
        }

        drawer.Height = height;
        drawer.Width = width;
        drawer.OffsetX = (Bounds.Width - width) / 2;
        Items.Add(drawer);
        ItemCount = Items.Count;
        LogicalChildren.Add(drawer);
    }
}
