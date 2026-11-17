using CommunityToolkit.Mvvm.ComponentModel;
using FluentIcons.Common;

namespace Huskui.Gallery.Models;

public partial class ProjectFile(string name, long bytes) : ObservableObject
{
    public string Name { get; } = name;

    public long Bytes { get; } = bytes;

    public string SizeLabel =>
        Bytes >= 1_000_000 ? $"{Bytes / 1_000_000d:0.0} MB" : $"{Bytes / 1_000d:0} KB";

    public Symbol Icon =>
        Path.GetExtension(Name).ToLowerInvariant() switch
        {
            ".pdf" => Symbol.DocumentPdf,
            ".png" or ".jpg" => Symbol.Image,
            _ => Symbol.Document,
        };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsCompleted), nameof(StatusText))]
    public partial double Progress { get; set; }

    public bool IsCompleted => Progress >= 100;

    public string StatusText =>
        Progress switch
        {
            <= 0 => "Queued",
            >= 100 => "Done",
            _ => $"{Progress:0}%",
        };
}
