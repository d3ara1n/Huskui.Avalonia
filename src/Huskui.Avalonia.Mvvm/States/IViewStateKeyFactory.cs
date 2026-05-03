namespace Huskui.Avalonia.Mvvm.States;

public interface IViewStateKeyFactory
{
    string CreateKey(Type owner, string? partitionKey);
}
