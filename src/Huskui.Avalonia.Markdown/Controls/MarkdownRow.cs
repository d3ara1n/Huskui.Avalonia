using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Markdown.Controls;

/// <summary>
///     A single table row: renders its <see cref="ItemsControl.Items" /> as the cells of a
///     uniformly-distributed <see cref="UniformGrid" />. The column count is kept in sync with the
///     number of cells.
/// </summary>
public class MarkdownRow : ItemsControl
{
    protected override Size MeasureOverride(Size availableSize)
    {
        // base materializes the ItemsPanel (the UniformGrid) on the first pass, so the panel only
        // exists after this returns — not before.
        var desired = base.MeasureOverride(availableSize);

        if (ItemsPanelRoot is UniformGrid grid)
        {
            var columns = Math.Max(1, Items.Count);
            if (grid.Columns != columns)
                grid.Columns = columns;
        }

        return desired;
    }
}
