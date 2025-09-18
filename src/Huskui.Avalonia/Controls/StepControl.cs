using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;

namespace Huskui.Avalonia.Controls;

public class StepControl : SelectingItemsControl
{
    private static readonly FuncTemplate<Panel?> DefaultPanel =
        new(() => new StackPanel { Orientation = Orientation.Horizontal });

    static StepControl()
    {
        ItemsPanelProperty.OverrideDefaultValue<StepControl>(DefaultPanel);
        SelectionModeProperty.OverrideDefaultValue<StepControl>(SelectionMode.Single);
        KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<StepControl>(KeyboardNavigationMode.Once);
    }

    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
        new StepItem();

    protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
        NeedsContainer<StepItem>(item, out recycleKey);


    public void NextStep() => SelectedIndex++;

    public void PreviousStep()
    {
        if (SelectedIndex == -1)
        {
            SelectedIndex = Items.Count - 1;
        }
        else
        {
            SelectedIndex--;
        }
    }


    // 似乎能被 ContainerIndexChangedOverride 替代掉
    // protected override void ClearContainerForItemOverride(Control element)
    // {
    //     base.ClearContainerForItemOverride(element);
    //
    //     var index = IndexFromContainer(element);
    //     if (index >= 0)
    //     {
    //         for (var i = index; i < Items.Count; i++)
    //         {
    //             if (ContainerFromIndex(i) is not StepItem item)
    //             {
    //                 continue;
    //             }
    //
    //             item.Index = 0;
    //             item.IsSelected = i == SelectedIndex;
    //             item.IsFirst = i == 0;
    //             item.IsLast = i == Items.Count - 1;
    //             item.IsCompleted = i < SelectedIndex;
    //         }
    //     }
    // }


    protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
    {
        base.PrepareContainerForItemOverride(container, item, index);

        if (container is StepItem stepItem)
        {
            stepItem.Index = index;
            stepItem.IsFirst = index == 0;
            stepItem.IsLast = index == Items.Count - 1;
            stepItem.IsCompleted = index < SelectedIndex || SelectedIndex == -1;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SelectedIndexProperty)
        {
            var selectedIndex = change.GetNewValue<int>();
            for (var i = 0; i < Items.Count; i++)
            {
                if (ContainerFromIndex(i) is StepItem item)
                {
                    item.IsCompleted = i < selectedIndex || selectedIndex == -1;
                }
            }
        }
    }
}
