using Avalonia.Animation;

namespace Huskui.Gallery.Models;

public record TransitionInfo(IPageTransition Transition)
{
    public string Name => Transition.GetType().Name.Replace("Transition", string.Empty);
}
