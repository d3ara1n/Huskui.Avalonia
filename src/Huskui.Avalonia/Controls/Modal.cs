using Avalonia.Controls;

namespace Huskui.Avalonia.Controls;

public class Modal : ContentControl
{
    protected override Type StyleKeyOverride => typeof(Modal);

    public void Dismiss() => RaiseEvent(new OverlayItem.DismissRequestedEventArgs(this));
}