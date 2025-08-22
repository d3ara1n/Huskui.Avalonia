namespace Huskui.Avalonia
{
    public interface IPageModel
    {
        Task InitializeAsync(CancellationToken token = default);
        Task DeinitializeAsync(CancellationToken token = default);
    }
}
