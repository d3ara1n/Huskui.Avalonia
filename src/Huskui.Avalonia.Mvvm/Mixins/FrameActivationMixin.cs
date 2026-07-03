using Huskui.Avalonia.Controls;
using Huskui.Avalonia.Mvvm.Activation;
using Huskui.Avalonia.Mvvm.Models;

namespace Huskui.Avalonia.Mvvm.Mixins;

public static class FrameActivationMixin
{
    public static void Install(Frame frame, IViewActivator activator) =>
        frame.PageActivator = activator.Activate;

    public static void Install(NavigationView view, IViewActivator activator) =>
        view.PageActivator = activator.Activate;
}
