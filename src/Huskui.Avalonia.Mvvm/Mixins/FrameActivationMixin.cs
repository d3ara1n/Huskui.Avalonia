using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Activation;
using System.Diagnostics.CodeAnalysis;

namespace Huskui.Avalonia.Mvvm.Mixins;

public static class FrameActivationMixin
{
    [UnconditionalSuppressMessage(
        "TrimAnalysis",
        "IL2111",
        Justification = "Delegate creation over an annotated method is beyond ILLink's tracking; all "
            + "delegate invocations originate from Frame.Navigate/GoBack whose Type arguments carry "
            + "the constructor-rooting annotation."
    )]
    public static void Install(Frame frame, IViewActivator activator) =>
        frame.PageActivator = activator.Activate;
}
