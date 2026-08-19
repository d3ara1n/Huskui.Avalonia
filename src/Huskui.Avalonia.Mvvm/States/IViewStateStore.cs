namespace Huskui.Avalonia.Mvvm.States;

public interface IViewStateStore
{
    object GetOrCreate(string key, Type stateType);
    void Release(string key);
    void Flush();
}
