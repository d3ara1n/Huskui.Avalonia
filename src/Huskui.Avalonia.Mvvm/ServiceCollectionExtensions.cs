using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.Models;
using Huskui.Avalonia.Mvvm.States;
using Microsoft.Extensions.DependencyInjection;

namespace Huskui.Avalonia.Mvvm;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddViewModelActivation(Type activatorType)
        {
            if (!activatorType.IsAssignableTo(typeof(IViewActivator)))
            {
                throw new ArgumentException($"{activatorType} must implement {nameof(IViewActivator)}");
            }

            services.AddSingleton(typeof(IViewActivator), activatorType);
            services.AddScoped<IViewContextAccessor, ViewContextAccessor>();
            services.AddScoped<IViewContext, ViewContext>();
            services.AddScoped(typeof(IViewContext<>), typeof(ViewContext<>));

            return services;
        }

        public IServiceCollection AddViewModelActivation<T>() where T : IViewActivator =>
            services.AddViewModelActivation(typeof(T));

        public IServiceCollection AddViewState(Action<StateRegistrationBuilder>? configure = null)
        {
            var builder = new StateRegistrationBuilder();
            configure?.Invoke(builder);
            builder.Build(services);
            services.AddSingleton<IViewStateStore, DefaultViewStateStore>();

            return services;
        }
    }
}
