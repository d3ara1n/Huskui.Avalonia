namespace Huskui.Avalonia.Mvvm.Activation;

public interface IViewContext
{
    object? Parameter { get; }
    bool HasParameter { get; }
    T? GetParameter<T>() where T : class;
    bool TryGetParameter<T>(out T? parameter) where T : class;
    T GetRequiredParameter<T>() where T : class;
}

public interface IViewContext<out T> where T : class
{
    T? Parameter { get; }
}
