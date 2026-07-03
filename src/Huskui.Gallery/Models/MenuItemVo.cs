using FluentIcons.Common;

namespace Huskui.Gallery.Models;

public class MenuItemVo
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public Symbol Icon { get; init; }
    public bool IsSeparator { get; init; }
    public bool IsNew { get; init; }
    public bool IsUpdated { get; init; }
    public Type? PageType { get; init; }
    public List<string> Tags { get; init; } = [];
    private string SearchText => $"{Title} {Description} {string.Join(" ", Tags)}".ToLowerInvariant();

    public bool MatchesSearch(string searchTerm) =>
        string.IsNullOrWhiteSpace(searchTerm)
     || SearchText.Contains(searchTerm, StringComparison.InvariantCultureIgnoreCase);

}
