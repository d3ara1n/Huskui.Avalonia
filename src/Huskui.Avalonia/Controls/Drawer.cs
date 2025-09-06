using Avalonia.Controls;

namespace Huskui.Avalonia.Controls;

public class Drawer : ContentControl
{
    protected override Type StyleKeyOverride => typeof(Drawer);

    public void Dismiss() => RaiseEvent(new OverlayItem.DismissRequestedEventArgs(this));
}
