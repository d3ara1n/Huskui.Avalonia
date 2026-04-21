using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Huskui.Avalonia.Controls;

public class SwitchContainer : TemplatedControl
{
    public static readonly DirectProperty<SwitchContainer, SwitchCases> CasesProperty =
        AvaloniaProperty.RegisterDirect<SwitchContainer, SwitchCases>(
            nameof(Cases),
            o => o.Cases,
            (o, v) => o.Cases = v
        );

    public static readonly DirectProperty<SwitchContainer, SwitchCase?> CurrentCaseProperty =
        AvaloniaProperty.RegisterDirect<SwitchContainer, SwitchCase?>(
            nameof(CurrentCase),
            o => o.CurrentCase,
            (o, v) => o.CurrentCase = v
        );

    public static readonly DirectProperty<SwitchContainer, object?> ValueProperty =
        AvaloniaProperty.RegisterDirect<SwitchContainer, object?>(
            nameof(Value),
            o => o.Value,
            (o, v) => o.Value = v,
            defaultBindingMode: BindingMode.OneWay
        );

    public static readonly DirectProperty<SwitchContainer, Type?> TargetTypeProperty =
        AvaloniaProperty.RegisterDirect<SwitchContainer, Type?>(
            nameof(TargetType),
            o => o.TargetType,
            (o, v) => o.TargetType = v,
            defaultBindingMode: BindingMode.OneTime
        );

    public static readonly StyledProperty<object?> ContentProperty = AvaloniaProperty.Register<
        SwitchContainer,
        object?
    >(nameof(Content));

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    [Content]
    public SwitchCases Cases
    {
        get;
        set => SetAndRaise(CasesProperty, ref field, value);
    } = [];

    public SwitchCase? CurrentCase
    {
        get;
        set => SetAndRaise(CurrentCaseProperty, ref field, value);
    }

    public object? Value
    {
        get;
        set
        {
            if (SetAndRaise(ValueProperty, ref field, value))
            {
                EvaluateCases();
            }
        }
    }

    public Type? TargetType
    {
        get;
        set => SetAndRaise(TargetTypeProperty, ref field, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        EvaluateCases();
    }

    private void EvaluateCases()
    {
        if (CurrentCase?.Value != null && CurrentCase.Value.Equals(Value))
        // If the current case we're on already matches our current value,
        // then we don't have any work to do.
        {
            return;
        }

        var result = Cases.EvaluateCases(Value, TargetType);

        // Only bother changing things around if we actually have a new case. (this should handle prior null case as well)
        if (result != CurrentCase)
        {
            // If we don't have any cases or default, setting these to null is what we want to be blank again.
            Content = result?.Content;
            CurrentCase = result;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            if (change.OldValue is ILogical oldChild)
            {
                LogicalChildren.Remove(oldChild);
            }

            if (change.NewValue is ILogical newChild)
            {
                LogicalChildren.Add(newChild);
            }
        }
    }
}
