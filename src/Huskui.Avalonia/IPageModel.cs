namespace Huskui.Avalonia;

public interface IPageModel
{
    CancellationToken PageToken { get; set; }
    Task InitializeAsync();
    Task DeinitializeAsync();
}
