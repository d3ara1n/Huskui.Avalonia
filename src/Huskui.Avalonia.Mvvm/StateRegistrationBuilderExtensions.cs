using Huskui.Avalonia.Mvvm.States;
using Huskui.Avalonia.Mvvm.States.Persistences;

namespace Huskui.Avalonia.Mvvm;

public static class StateRegistrationBuilderExtensions
{
    extension(StateRegistrationBuilder builder)
    {
        public StateRegistrationBuilder WithInMemoryPersistence() =>
            builder.WithStatePersistence<NullStatePersistence>();

        public StateRegistrationBuilder WithStatePersistence<T>()
            where T : IViewStatePersistence => builder.WithStatePersistence(typeof(T));

        public StateRegistrationBuilder WithKeyFactory<T>()
            where T : IViewStateKeyFactory => builder.WithKeyFactory(typeof(T));

        public StateRegistrationBuilder WithStateManager<T>()
            where T : IViewStateManager => builder.WithManager(typeof(T));
    }
}
