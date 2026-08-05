using System.Windows.Input;

namespace Huskui.Avalonia.Models;

public class GrowlAction(string text, ICommand command, object? parameter = null)
{
    public GrowlAction() : this("_", DummyCommand.Instance) { }

    public string Text { get; set; } = text;
    public ICommand Command { get; set; } = command;
    public object? Parameter { get; set; } = parameter;
}
