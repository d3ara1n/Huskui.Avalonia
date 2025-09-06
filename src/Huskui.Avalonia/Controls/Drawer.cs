using Avalonia.Controls;

namespace Huskui.Avalonia.Controls;

public class Drawer : ContentControl
{
    public void Dismiss()
    {
        RaiseEvent(new OverlayItem.DismissRequestedEventArgs(this));
    }
}
