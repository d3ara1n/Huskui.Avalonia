using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Layout;

namespace Huskui.Avalonia.Controls
{
    public class StepControl : SelectingItemsControl
    {
        private static readonly FuncTemplate<Panel?> DefaultPanel =
            new(() => new StackPanel { Orientation = Orientation.Horizontal });

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
            new StepItem();

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
            NeedsContainer<StepItem>(item, out recycleKey);

        // protected override void ClearContainerForItemOverride(Control element) => UpdateSelectedIndex();
        //
        // protected override void PrepareContainerForItemOverride(Control container, object? item, int index) =>
        //     UpdateSelectedIndex();

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == SelectedIndexProperty)
            {
                UpdateSelectedIndex();
            }
        }

        private void UpdateSelectedIndex()
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (ContainerFromIndex(i) is not StepItem item)
                {
                    continue;
                }

                item.IsFirst = i == 0;
                item.IsLast = i == Items.Count - 1;
                item.IsCompleted = i < SelectedIndex;
            }
        }

        static StepControl()
        {
            ItemsPanelProperty.OverrideDefaultValue<StepControl>(DefaultPanel);
            KeyboardNavigation.TabNavigationProperty.OverrideDefaultValue<StepControl>(KeyboardNavigationMode.Once);
        }
    }
}