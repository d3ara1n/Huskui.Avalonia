namespace Huskui.Avalonia.Mvvm.States.Persistences;

public class NullStatePersistence : IViewStatePersistence
{
    public void Save(string key, Type stateType, object value) { }

    public object? Load(string key, Type stateType) => null;
}
