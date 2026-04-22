namespace Huskui.Avalonia.Mvvm.Models;

public interface IViewModel
{
    Task InitializeAsync(CancellationToken cancellationToken);
    Task DeinitializeAsync();
}
