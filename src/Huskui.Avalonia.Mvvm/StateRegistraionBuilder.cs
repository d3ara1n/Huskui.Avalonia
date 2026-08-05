using Huskui.Avalonia.Mvvm.States;
using Huskui.Avalonia.Mvvm.States.Managers;
using Huskui.Avalonia.Mvvm.States.Persistences;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Avalonia.Mvvm;

public class StateRegistrationBuilder
{
    private Type? _managerType;
    private Type? _persistenceType;
    private Type? _factoryType;

    public StateRegistrationBuilder WithManager(Type managerType)
    {
        _managerType = managerType;
        return this;
    }

    public StateRegistrationBuilder WithStatePersistence(Type persistenceType)
    {
        _persistenceType = persistenceType;
        return this;
    }

    public StateRegistrationBuilder WithKeyFactory(Type factoryType)
    {
        _factoryType = factoryType;
        return this;
    }

    public IServiceCollection Build(IServiceCollection services)
    {
        if (_managerType != null)
        {
            if (!_managerType.IsAssignableTo(typeof(IViewStateManager)))
            {
                throw new InvalidOperationException(
                    $"ManagerType should be assignable to {nameof(IViewStateManager)}"
                );
            }

            services.AddSingleton(typeof(IViewStateManager), _managerType);
        }
        else
        {
            services.AddSingleton<IViewStateManager, ReflectionViewStateManager>();
        }

        if (_persistenceType != null)
        {
            if (!_persistenceType.IsAssignableTo(typeof(IViewStatePersistence)))
            {
                throw new InvalidOperationException(
                    $"PersistenceType should be assignable to {nameof(IViewStatePersistence)}"
                );
            }

            services.AddSingleton(typeof(IViewStatePersistence), _persistenceType);
        }
        else
        {
            services.AddSingleton<IViewStatePersistence, NullStatePersistence>();
        }

        if (_factoryType != null)
        {
            if (!_factoryType.IsAssignableTo(typeof(IViewStateKeyFactory)))
            {
                throw new InvalidOperationException(
                    $"FactoryType should be assignable to {nameof(IViewStateKeyFactory)}"
                );
            }

            services.AddSingleton(typeof(IViewStateKeyFactory), _factoryType);
        }
        else
        {
            services.AddSingleton<IViewStateKeyFactory, DefaultViewStateFactory>();
        }

        return services;
    }
}
