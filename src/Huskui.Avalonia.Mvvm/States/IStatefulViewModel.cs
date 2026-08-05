namespace Huskui.Avalonia.Mvvm.States;

public interface IStatefulViewModel<T>
    where T : class
{
    T? ViewState { get; set; }
}
