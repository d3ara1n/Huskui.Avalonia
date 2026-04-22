namespace Huskui.Avalonia.Mvvm.Activation;

internal sealed class ViewContext(IViewContextAccessor accessor) : IViewContext
{
    public object? Parameter => accessor.Parameter;
    public bool HasParameter => Parameter != null;

    public T? GetParameter<T>()
        where T : class => Parameter as T;

    public bool TryGetParameter<T>(out T? value)
        where T : class
    {
        value = Parameter as T;
        return value is not null;
    }

    public T GetRequiredParameter<T>()
        where T : class =>
        Parameter as T
        ?? throw new InvalidOperationException(
            $"Context parameter is not of type {typeof(T).FullName}."
        );
}

internal sealed class ViewContext<T>(IViewContext context) : IViewContext<T>
    where T : class
{
    public T? Parameter => context.GetParameter<T>();

    public T GetRequiredParameter() => context.GetRequiredParameter<T>();
}
