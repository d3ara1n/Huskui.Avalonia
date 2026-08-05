using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Huskui.Avalonia.Mvvm.Models;

namespace Huskui.Gallery.ViewModels;

public class ViewModelBase : ObservableObject, IViewModel
{
    public Task InitializeAsync(CancellationToken cancellationToken) =>
        OnInitializedAsync(cancellationToken);

    public Task DeinitializeAsync() => OnDeinitializedAsync();

    protected virtual Task OnInitializedAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;

    protected virtual Task OnDeinitializedAsync() => Task.CompletedTask;
}
