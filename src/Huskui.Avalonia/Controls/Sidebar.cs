using Avalonia.Controls;

namespace Huskui.Avalonia.Controls;

public class Sidebar : ContentControl
{
    protected override Type StyleKeyOverride => typeof(Sidebar);

    public void Dismiss() => RaiseEvent(new OverlayItem.DismissRequestedEventArgs(this));
}
