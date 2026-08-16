using Huskui.Avalonia.Mvvm.States;
using Huskui.Avalonia.Mvvm.States.Persistences;
using System.Diagnostics.CodeAnalysis;

namespace Huskui.Avalonia.Mvvm;

public static class StateRegistrationBuilderExtensions
{
    extension(StateRegistrationBuilder builder)
    {
        public StateRegistrationBuilder WithInMemoryPersistence() =>
            builder.WithStatePersistence<NullStatePersistence>();

        public StateRegistrationBuilder WithStatePersistence<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T
        >()
            where T : IViewStatePersistence => builder.WithStatePersistence(typeof(T));

        public StateRegistrationBuilder WithKeyFactory<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T
        >()
            where T : IViewStateKeyFactory => builder.WithKeyFactory(typeof(T));

        public StateRegistrationBuilder WithStateManager<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T
        >()
            where T : IViewStateManager => builder.WithManager(typeof(T));
    }
}
