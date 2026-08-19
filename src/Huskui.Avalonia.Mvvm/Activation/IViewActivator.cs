namespace Huskui.Avalonia.Mvvm.Activation;

public interface IViewActivator
{
    object? Activate(Type viewType, object? parameter = null);
}
