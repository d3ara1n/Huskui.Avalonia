using System.Collections;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Huskui.Avalonia.Controls;

/// <summary>
///     动作按钮行的布局容器。两件正交的事：<see cref="Layout" /> 决定按钮组在父容器里的水平对齐，
///     <see cref="PrimaryPlacement" /> 决定 <see cref="Button.IsDefault" /> 主按钮在组内的相对位置（首/尾）。
///     <see cref="PrimaryPlacementMode.Auto" /> = 按平台（macOS→Trailing，其余→Leading）。
///     子级自带主题，本面板不改写。
/// </summary>
public class ModalActionPanel : Panel
{
    /// <summary>
    ///     按钮组的水平对齐方式，语义对应 <see cref="HorizontalAlignment" />。
    ///     <see cref="Edge" /> 是快捷预设：整组贴到主按钮该在的那侧。
    /// </summary>
    public enum LayoutMode
    {
        Left,
        Right,
        Stretch,
        Edge
    }

    /// <summary>主按钮在组内的相对位置。</summary>
    public enum PrimaryPlacementMode
    {
        Auto,
        Leading,
        Trailing
    }

    static ModalActionPanel()
    {
        // Layout / Spacing 改变 desired size → AffectsMeasure（会级联触发 arrange）
        AffectsMeasure<ModalActionPanel>(LayoutProperty, SpacingProperty);
        // PrimaryPlacement 只改子级顺序/定位，不影响测量 → AffectsArrange
        AffectsArrange<ModalActionPanel>(PrimaryPlacementProperty);
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

    /// <summary>按钮组的水平对齐。默认 <see cref="LayoutMode.Edge" />。</summary>
    public LayoutMode Layout
    {
        get => GetValue(LayoutProperty);
        set => SetValue(LayoutProperty, value);
    }

    /// <summary>
    ///     主按钮（<see cref="Button.IsDefault" />）在组内的相对位置。默认
    ///     <see cref="PrimaryPlacementMode.Auto" />：macOS→Trailing，其余→Leading。找不到主按钮时此项无效。
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

        // 所有模式统一按内容宽报告期望尺寸；Stretch 在 ArrangeOverride 中处理
        var width = contentWidth;

        return new(width, maxHeight);
    }

    protected override Size ArrangeOverride(Size final)
    {
        var spacing = Spacing;
        var visible = GetVisibleChildren();
        if (visible.Count == 0)
            return final;

        // Edge：主按钮独享一边，其余按原序挤另一边
        if (Layout == LayoutMode.Edge)
        {
            ArrangeEdge(visible, final, spacing);
            return final;
        }

        // Left/Right/Stretch：先按 PrimaryPlacement 决定主在序列首/尾，再连续排列
        var ordered = OrderByPrimary(visible);
        switch (Layout)
        {
            case LayoutMode.Stretch:
                ArrangeStretch(ordered, final, spacing);
                break;
            case LayoutMode.Left:
                ArrangePacked(ordered, final, spacing, startX: 0);
                break;
            default: // Right
                ArrangePacked(
                    ordered,
                    final,
                    spacing,
                    startX: final.Width - ContentWidth(ordered, spacing)
                );
                break;
        }

        return final;
    }

    /// <summary>
    ///     Edge：主按钮独占平台侧，其余按文档顺序挤到对边。找不到主按钮 → 退化为 Right。
    /// </summary>
    private void ArrangeEdge(List<Control> visible, Size final, double spacing)
    {
        var primaryIdx = visible.FindIndex(c => c is Button { IsDefault: true });
        if (primaryIdx < 0)
        {
            ArrangePacked(
                visible,
                final,
                spacing,
                startX: final.Width - ContentWidth(visible, spacing)
            );
            return;
        }

        var primary = visible[primaryIdx];
        var others = new List<Control>(visible.Count - 1);
        for (var i = 0; i < visible.Count; i++)
            if (i != primaryIdx)
                others.Add(visible[i]);

        var primaryWidth = primary.DesiredSize.Width;

        if (IsTrailing())
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
            var x = final.Width - ContentWidth(others, spacing);
            foreach (var child in others)
            {
                child.Arrange(new(x, 0, child.DesiredSize.Width, final.Height));
                x += child.DesiredSize.Width + spacing;
            }
        }
    }

    /// <summary>按 PrimaryPlacement 重排可见子级：主按钮去首（Leading）或尾（Trailing）。</summary>
    private List<Control> OrderByPrimary(List<Control> visible)
    {
        var primaryIdx = visible.FindIndex(c => c is Button { IsDefault: true });
        if (primaryIdx < 0)
            return visible; // 无主按钮 → 保持文档顺序，PrimaryPlacement 无的放矢

        var primary = visible[primaryIdx];
        var ordered = new List<Control>(visible.Count);

        if (IsTrailing())
        {
            // 主按钮去尾，其余按文档顺序排前面
            for (var i = 0; i < visible.Count; i++)
                if (i != primaryIdx)
                    ordered.Add(visible[i]);
            ordered.Add(primary);
        }
        else
        {
            // 主按钮去首，其余按文档顺序排后面
            ordered.Add(primary);
            for (var i = 0; i < visible.Count; i++)
                if (i != primaryIdx)
                    ordered.Add(visible[i]);
        }

        return ordered;
    }

    private void ArrangeStretch(List<Control> ordered, Size final, double spacing)
    {
        var n = ordered.Count;
        var colWidth = double.Max((final.Width - spacing * (n - 1)) / n, 0d);
        var x = 0d;
        foreach (var child in ordered)
        {
            // slot 宽 = 列宽，依赖子级 HorizontalAlignment=Stretch 视觉填满
            child.Arrange(new(x, 0, colWidth, final.Height));
            x += colWidth + spacing;
        }
    }

    /// <summary>把序列从 startX 起依次排开，子级用各自 DesiredSize.Width。</summary>
    private void ArrangePacked(List<Control> ordered, Size final, double spacing, double startX)
    {
        var x = startX;
        foreach (var child in ordered)
        {
            child.Arrange(new(x, 0, child.DesiredSize.Width, final.Height));
            x += child.DesiredSize.Width + spacing;
        }
    }

    /// <summary>解析 PrimaryPlacement → 是否主按钮去尾。Auto = macOS→尾，其余→首。</summary>
    private bool IsTrailing() =>
        PrimaryPlacement switch
        {
            PrimaryPlacementMode.Leading => false,
            PrimaryPlacementMode.Trailing => true,
            _ => OperatingSystem.IsMacOS()
        };

    private double ContentWidth(List<Control> children, double spacing)
    {
        var sum = 0d;
        foreach (var child in children)
            sum += child.DesiredSize.Width;
        return sum + spacing * double.Max(children.Count - 1, 0);
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
