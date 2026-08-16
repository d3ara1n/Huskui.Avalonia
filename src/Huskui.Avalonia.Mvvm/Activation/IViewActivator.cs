using Huskui.Avalonia.Mvvm.States;
using System.Diagnostics.CodeAnalysis;

namespace Huskui.Avalonia.Mvvm.Activation;

public interface IViewActivator
{
    object? Activate(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        Type viewType,
        object? parameter = null
    );
}
