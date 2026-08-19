using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.States;

namespace Huskui.Gallery.Services;

public sealed class GalleryViewActivator(IServiceProvider provider, IViewStateManager stateManager)
    : ViewActivatorBase(provider, stateManager)
{
    protected override Type FindViewModelType(Type view) =>
        Type.GetType(view.FullName!.Replace("Page", "ViewModel", StringComparison.Ordinal))
     ?? Type.GetType(view.FullName!.Replace("View", "ViewModel", StringComparison.Ordinal))!;
}
