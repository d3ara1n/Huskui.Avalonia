namespace Huskui.Avalonia.Mvvm.States;

public interface IViewStateManager
{
    bool TryAttach(object viewModel);
    void Detach(object viewModel);
}
