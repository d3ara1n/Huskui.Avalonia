using Avalonia.Controls;

namespace Huskui.Avalonia.Markdown.Models;

/// <summary>
///     A pre-rendered Markdown table row. Each index maps to a column; the cell's content is the
///     fully-rendered control, surfaced to <see cref="TableView" /> via an int indexer so columns
///     can bind <c>[index]</c>.
/// </summary>
internal sealed class MarkdownTableRow
{
    private readonly Control?[] _cells;

    public MarkdownTableRow(Control?[] cells) => _cells = cells;

    public Control? this[int index] =>
        index >= 0 && index < _cells.Length ? _cells[index] : null;
}
