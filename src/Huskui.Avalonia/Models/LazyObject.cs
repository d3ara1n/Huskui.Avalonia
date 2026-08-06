using Avalonia;

namespace Huskui.Avalonia.Models;

public class LazyObject(
    Func<CancellationToken, Task<object?>> factory,
    Action<object?>? callback = null,
    CancellationToken token = default
) : AvaloniaObject
{
    public static readonly DirectProperty<LazyObject, object?> ValueProperty =
        AvaloniaProperty.RegisterDirect<LazyObject, object?>(
            nameof(Value),
            o => o.Value,
            (o, v) => o.Value = v
        );

    public static readonly DirectProperty<LazyObject, bool> IsInProgressProperty =
        AvaloniaProperty.RegisterDirect<LazyObject, bool>(
            nameof(IsInProgress),
            o => o.IsInProgress,
            (o, v) => o.IsInProgress = v
        );

    private readonly CancellationTokenSource _cts = CancellationTokenSource.CreateLinkedTokenSource(
        token
    );

    public object? Value
    {
        get;
        private set => SetAndRaise(ValueProperty, ref field, value);
    }

    public bool IsCancelled => _cts.IsCancellationRequested;

    public bool IsInProgress
    {
        get;
        private set => SetAndRaise(IsInProgressProperty, ref field, value);
    }

    public Action<object?>? Callback { get; set; } = callback;

    public void Cancel() => _cts.Cancel();

    public async Task FetchAsync()
    {
        IsInProgress = true;
        try
        {
            var value = await factory(_cts.Token);
            Value = value;
            Callback?.Invoke(value);
        }
        finally
        {
            // NOTE: factory 抛异常时也必须复位，否则 IsInProgress 永远卡 true，后续
            // Source 替换的 in-progress 判定与骨架显隐都会失真。
            IsInProgress = false;
        }
    }
}
