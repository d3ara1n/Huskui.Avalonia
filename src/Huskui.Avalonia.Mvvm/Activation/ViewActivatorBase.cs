using Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Mixins;
using Huskui.Avalonia.Mvvm.States;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Avalonia.Mvvm.Activation;

public abstract class ViewActivatorBase(IServiceProvider provider, IViewStateManager stateManager)
    : IViewActivator
{
    public virtual object? Activate(Type viewType, object? parameter = null)
    {
        if (!viewType.IsAssignableTo(typeof(Control)))
        {
            throw new ArgumentOutOfRangeException(
                nameof(viewType),
                viewType,
                "Parameter view must be derived from Control"
            );
        }

        var modelType = FindViewModelType(viewType);

        var view = (Control?)Activator.CreateInstance(viewType);

        if (view is not null)
        {
            var scope = provider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IViewContextAccessor>();
            factory.Parameter = parameter;

            var viewModel = ActivatorUtilities.GetServiceOrCreateInstance(
                scope.ServiceProvider,
                modelType
            );
            view.DataContext = viewModel;
            ViewModelMixin.Attach(view, scope);
            ViewStateMixin.Attach(view, stateManager);
        }

        return view;
    }

    protected abstract Type FindViewModelType(Type view);
}
