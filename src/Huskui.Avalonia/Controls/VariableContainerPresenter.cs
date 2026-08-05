using Avalonia.Controls.Presenters;
using Avalonia.Data;

namespace Huskui.Avalonia.Controls;

public class VariableContainerPresenter : ContentPresenter
{
    public VariableContainerPresenter()
    {
        Bind(
            ContentProperty,
            new Binding(nameof(Content))
            {
                RelativeSource = new(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(VariableContainer),
                },
            }
        );

        Bind(
            ContentTemplateProperty,
            new Binding(nameof(ContentTemplate))
            {
                RelativeSource = new(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(VariableContainer),
                },
            }
        );
    }
}
