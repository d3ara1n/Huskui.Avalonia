using Huskui.Avalonia.Mvvm.States;
using System.Diagnostics.CodeAnalysis;

namespace Huskui.Avalonia.Mvvm.States;

public interface IViewStateStore
{
    object GetOrCreate(
        string key,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type stateType
    );
    void Release(string key);
    void Flush();
}
