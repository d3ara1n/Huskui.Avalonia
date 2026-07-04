using FluentIcons.Common;

namespace Huskui.Gallery.Models;

public class MenuItemVo
{
    public Symbol Icon { get; init; }
    public Type? PageType { get; init; }
    public string? Category { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Tags { get; init; } = [];
    public bool IsNew { get; init; }
    public bool IsUpdated { get; init; }

    private string SearchText => $"{Title} {Description} {string.Join(" ", Tags)}".ToLowerInvariant();

    public bool MatchesSearch(string searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm)
     || SearchText.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase);
}
