using System.Diagnostics.CodeAnalysis;

namespace Huskui.Avalonia.Mvvm.States.Managers;

public sealed class ReflectionViewStateManager(
    IViewStateStore store,
    IViewStateKeyFactory keyFactory
) : IViewStateManager
{
    private readonly Dictionary<object, string> _attached = new(ReferenceEqualityComparer.Instance);

    [UnconditionalSuppressMessage(
        "TrimAnalysis",
        "IL2075",
        Justification = "Reflects over consumer view-model types discovered at runtime; consumers "
            + "trimming their own assemblies must keep view-model interfaces and the ViewState "
            + "property reachable, or supply a non-reflective IViewStateManager."
    )]
    [UnconditionalSuppressMessage(
        "TrimAnalysis",
        "IL2062",
        Justification = "stateType is derived from the runtime interface scan above; consumers "
            + "trimming their own assemblies must keep state types constructible, or supply a "
            + "non-reflective IViewStateManager."
    )]
    public bool TryAttach(object viewModel)
    {
        var type = viewModel.GetType();
        var stateInterface = type.GetInterfaces()
            .FirstOrDefault(x =>
                x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IStatefulViewModel<>)
            );
        if (stateInterface is null)
        {
            return false;
        }

        var stateType = stateInterface.GetGenericArguments()[0];
        var partitionKey = (viewModel as IViewStateKeyProvider)?.ViewStateKey;
        var stateKey = keyFactory.CreateKey(type, partitionKey);
        var state = store.GetOrCreate(stateKey, stateType);
        stateInterface
            .GetProperty(nameof(IStatefulViewModel<>.ViewState))!
            .SetValue(viewModel, state);
        _attached[viewModel] = stateKey;
        return true;
    }

    public void Detach(object viewModel)
    {
        if (_attached.TryGetValue(viewModel, out var key))
        {
            store.Release(key);
            _attached.Remove(viewModel);
        }
    }
}
