using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Huskui.Avalonia.Controls;

/// <summary>
///     动作按钮行的布局容器。提供四种摆放形态，其中 <see cref="LayoutMode.Edge" /> 会按平台把
///     <see cref="Button.IsDefault" /> 主按钮推到正确的边（macOS 右、其余左）。子级自带主题，本面板不改写。
/// </summary>
public class ModalActionPanel : Panel
{
    public enum LayoutMode
    {
        Start,
        End,
        Fill,
        Edge
    }

    public enum PrimaryPlacementMode
    {
        Auto,
        Leading,
        Trailing
    }

    public static readonly StyledProperty<LayoutMode> LayoutProperty = AvaloniaProperty.Register<
        ModalActionPanel,
        LayoutMode
    >(nameof(Layout), defaultValue: LayoutMode.Edge);

    public static readonly StyledProperty<PrimaryPlacementMode> PrimaryPlacementProperty =
        AvaloniaProperty.Register<
            ModalActionPanel,
            PrimaryPlacementMode
        >(nameof(PrimaryPlacement), defaultValue: PrimaryPlacementMode.Auto);

    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<
        ModalActionPanel,
        double
    >(nameof(Spacing), defaultValue: 8d);

    /// <summary>
    ///     整体摆放形态。默认 <see cref="LayoutMode.Edge" />（用户自定义 Modal 常用）；
    ///     Dialog 主题里显式覆盖为 <see cref="LayoutMode.Fill" />。
    /// </summary>
    public LayoutMode Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    /// <summary>
    ///     仅 <see cref="LayoutMode.Edge" /> 生效。主按钮（<see cref="Button.IsDefault" />）落在哪一边。
    ///     默认 <see cref="PrimaryPlacementMode.Auto" />：macOS→Trailing，其余→Leading。
    /// </summary>
    public PrimaryPlacementMode PrimaryPlacement
    {
        get => GetValue(PrimaryPlacementProperty);
        set => SetValue(PrimaryPlacementProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var spacing = Spacing;
        var visible = GetVisibleChildren();
        if (visible.Count == 0)
            return default;

        // 用无限宽测量，得到每个子级的「内容宽」，不受 Stretch 干扰
        var measureSlot = new Size(double.PositiveInfinity, availableSize.Height);
        var contentWidth = 0d;
        var maxHeight = 0d;
        foreach (var child in visible)
        {
            child.Measure(measureSlot);
            contentWidth += child.DesiredSize.Width;
            if (child.DesiredSize.Height > maxHeight)
                maxHeight = child.DesiredSize.Height;
        }

        contentWidth += spacing * (visible.Count - 1);

        // Fill 模式：父级给有限宽时声明占满，父级给无限宽时退化为内容宽
        var width =
            Layout == LayoutMode.Fill && !double.IsInfinity(availableSize.Width)
                ? availableSize.Width
                : contentWidth;

        return new(width, maxHeight);
    }

    protected override Size ArrangeOverride(Size final)
    {
        var spacing = Spacing;
        var visible = GetVisibleChildren();
        if (visible.Count == 0)
            return final;

        switch (Layout)
        {
            case LayoutMode.Fill:
                ArrangeFill(visible, final, spacing);
                break;
            case LayoutMode.Edge:
                ArrangeEdge(visible, final, spacing);
                break;
            case LayoutMode.Start:
                ArrangeStart(visible, final, spacing);
                break;
            default: // End
                ArrangeEnd(visible, final, spacing);
                break;
        }

        return final;
    }

    private void ArrangeFill(IList visible, Size final, double spacing)
    {
        var n = visible.Count;
        var colWidth = double.Max((final.Width - spacing * (n - 1)) / n, 0d);
        var x = 0d;
        foreach (Control child in visible)
        {
            // slot 宽 = 列宽，依赖子级 HorizontalAlignment=Stretch 视觉填满
            child.Arrange(new(x, 0, colWidth, final.Height));
            x += colWidth + spacing;
        }
    }

    private void ArrangeEdge(IList visible, Size final, double spacing)
    {
        // 找主按钮：第一个可见且 IsDefault 的 Button。找不到 → 退化为 End
        Control? primary = null;
        var others = new List<Control>();
        foreach (Control child in visible)
        {
            if (primary == null && child is Button { IsDefault: true })
                primary = child;
            else
                others.Add(child);
        }

        if (primary == null)
        {
            ArrangeEnd(visible, final, spacing);
            return;
        }

        var primaryWidth = primary.DesiredSize.Width;
        var othersWidth = ContentWidth(others, spacing);

        var trailing =
            ResolvePrimarySide() is PrimaryPlacementMode.Trailing;

        if (trailing)
        {
            // 主按钮在右，其余在左保持文档顺序
            var x = 0d;
            foreach (var child in others)
            {
                child.Arrange(new(x, 0, child.DesiredSize.Width, final.Height));
                x += child.DesiredSize.Width + spacing;
            }

            primary.Arrange(new(final.Width - primaryWidth, 0, primaryWidth, final.Height));
        }
        else
        {
            // 主按钮在左，其余在右
            primary.Arrange(new(0, 0, primaryWidth, final.Height));
            var x = final.Width - othersWidth;
            foreach (var child in others)
            {
                child.Arrange(new(x, 0, child.DesiredSize.Width, final.Height));
                x += child.DesiredSize.Width + spacing;
            }
        }
    }

    private void ArrangeStart(IList visible, Size final, double spacing)
    {
        var x = 0d;
        foreach (Control child in visible)
        {
            child.Arrange(new(x, 0, child.DesiredSize.Width, final.Height));
            x += child.DesiredSize.Width + spacing;
        }
    }

    private void ArrangeEnd(IList visible, Size final, double spacing)
    {
        var totalWidth = ContentWidth(visible, spacing);
        var x = final.Width - totalWidth;
        foreach (Control child in visible)
        {
            child.Arrange(new(x, 0, child.DesiredSize.Width, final.Height));
            x += child.DesiredSize.Width + spacing;
        }
    }

    private PrimaryPlacementMode ResolvePrimarySide() =>
        PrimaryPlacement switch
        {
            PrimaryPlacementMode.Leading => PrimaryPlacementMode.Leading,
            PrimaryPlacementMode.Trailing => PrimaryPlacementMode.Trailing,
            _ => OperatingSystem.IsMacOS()
                ? PrimaryPlacementMode.Trailing
                : PrimaryPlacementMode.Leading
        };

    private double ContentWidth(IList children, double spacing)
    {
        var sum = 0d;
        var count = 0;
        foreach (Control child in children)
        {
            sum += child.DesiredSize.Width;
            count++;
        }

        return sum + spacing * double.Max(count - 1, 0);
    }

    private List<Control> GetVisibleChildren()
    {
        var list = new List<Control>();
        foreach (Control child in Children)
            if (child.IsVisible)
                list.Add(child);
        return list;
    }
}
