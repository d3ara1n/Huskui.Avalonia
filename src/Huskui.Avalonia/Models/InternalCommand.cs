using System.Windows.Input;

namespace Huskui.Avalonia.Models;

public class InternalCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    #region ICommand Members

    bool ICommand.CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    void ICommand.Execute(object? parameter) => execute();

    public event EventHandler? CanExecuteChanged;

    #endregion

    public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}

public class InternalCommand<T>(Action<T?> execute, Func<T?, bool>? canExecute = null) : ICommand
{
    #region ICommand Members

    bool ICommand.CanExecute(object? parameter) =>
        parameter switch
        {
            null => canExecute?.Invoke(default) ?? true,
            T it => canExecute?.Invoke(it) ?? true,
            _ => false,
        };

    void ICommand.Execute(object? parameter)
    {
        switch (parameter)
        {
            case null:
                execute(default);
                break;
            case T it:
                execute(it);
                break;
            default:
                throw new InvalidCastException(
                    "Parameter must be null or of type " + typeof(T).FullName
                );
        }
    }

    public event EventHandler? CanExecuteChanged;

    #endregion

    public void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
