using System.Windows.Input;

namespace Huskui.Avalonia.Models;

public class InternalAsyncCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    #region ICommand Members

    private bool _isRunning;
    public event EventHandler? CanExecuteChanged;

    bool ICommand.CanExecute(object? parameter) => !_isRunning && (canExecute?.Invoke() ?? true);

    // async void on purpose: execute()'s exceptions surface on the captured
    // SynchronizationContext (host's unhandled-exception handler) instead of
    // dying in a discarded Task.
    async void ICommand.Execute(object? parameter)
    {
        if (!((ICommand)this).CanExecute(null))
            return;
        _isRunning = true;
        OnCanExecuteChanged();
        try
        {
            await execute();
        }
        finally
        {
            _isRunning = false;
            OnCanExecuteChanged();
        }
    }

    #endregion

    public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class InternalAsyncCommand<T>(Func<T?, Task> execute, Func<T?, bool>? canExecute = null) : ICommand
{
    #region ICommand Members

    private bool _isRunning;
    public event EventHandler? CanExecuteChanged;

    bool ICommand.CanExecute(object? parameter) =>
        !_isRunning
     && parameter switch
        {
            null => canExecute?.Invoke(default) ?? true,
            T it => canExecute?.Invoke(it) ?? true,
            _ => false,
        };

    // async void on purpose: execute()'s exceptions surface on the captured
    // SynchronizationContext (host's unhandled-exception handler) instead of
    // dying in a discarded Task.
    async void ICommand.Execute(object? parameter)
    {
        if (parameter is not null and not T)
            throw new InvalidCastException(
                "Parameter must be null or of type " + typeof(T).FullName
            );

        if (!((ICommand)this).CanExecute(parameter))
            return;
        _isRunning = true;
        OnCanExecuteChanged();
        try
        {
            await execute(parameter is T it ? it : default);
        }
        finally
        {
            _isRunning = false;
            OnCanExecuteChanged();
        }
    }

    #endregion

    public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
