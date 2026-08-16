using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.LogicalTree;

namespace Huskui.Avalonia.Controls;

public class VariableContainerPresenter : ContentPresenter
{
    private IDisposable? _contentSubscription;
    private IDisposable? _contentTemplateSubscription;

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        DetachSubscriptions();

        if (this.GetLogicalAncestors().OfType<VariableContainer>().FirstOrDefault() is not { } container)
            return;

        _contentSubscription = Bind(
            ContentProperty,
            container.GetObservable(ContentControl.ContentProperty)
        );
        _contentTemplateSubscription = Bind(
            ContentTemplateProperty,
            container.GetObservable(ContentControl.ContentTemplateProperty)
        );
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        DetachSubscriptions();
    }

    private void DetachSubscriptions()
    {
        _contentSubscription?.Dispose();
        _contentSubscription = null;
        _contentTemplateSubscription?.Dispose();
        _contentTemplateSubscription = null;
    }
}
