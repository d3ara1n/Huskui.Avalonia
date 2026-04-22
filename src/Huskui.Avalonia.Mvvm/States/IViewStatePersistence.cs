namespace Huskui.Avalonia.Mvvm.States;

public interface IViewStatePersistence
{
    void Save(string key, Type stateType, object value);
    object? Load(string key, Type stateType);
}
