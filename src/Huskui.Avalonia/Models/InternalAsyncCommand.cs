using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace Huskui.Avalonia.Models;

public class InternalAsyncCommand(Func<Task> execute, Func<bool>? canExecute = null) : ICommand
{
    #region ICommand Members

    private bool _isRunning;
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute() => !_isRunning && (canExecute?.Invoke() ?? true);

    bool ICommand.CanExecute(object? parameter) => CanExecute();

    public async Task ExecuteAsync()
    {
        if (!CanExecute())
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

    void ICommand.Execute(object? parameter) => _ = ExecuteAsync();

    #endregion

    internal void OnCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
