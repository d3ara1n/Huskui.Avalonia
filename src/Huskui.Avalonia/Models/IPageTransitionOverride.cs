using Avalonia.Animation;

namespace Huskui.Avalonia.Models;

public interface IPageTransitionOverride
{
    IPageTransition? TransitionOverride { get; }
}
