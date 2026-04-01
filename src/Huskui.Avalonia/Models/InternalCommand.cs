using System.Windows.Input;

namespace Huskui.Avalonia.Models;

public class InternalCommand(Action execute, Func<bool>? canExecute = null) : ICommand
{
    #region ICommand Members

    public bool CanExecute(object? parameter) => canExecute?.Invoke() ?? true;

    public void Execute(object? parameter) => execute();

    public event EventHandler? CanExecuteChanged;

    #endregion

    internal void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
