using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;

namespace Huskui.Avalonia.Controls
{
    [PseudoClasses(":selected", ":completed")]
    public class StepItem : HeaderedContentControl, ISelectable
    {
        public static readonly StyledProperty<bool> IsSelectedProperty = SelectingItemsControl.IsSelectedProperty
           .AddOwner<ListBoxItem>();

        public static readonly StyledProperty<bool> IsCompletedProperty =
            AvaloniaProperty.Register<StepItem, bool>(nameof(IsCompleted));

        public static readonly StyledProperty<bool> IsLastProperty =
            AvaloniaProperty.Register<StepItem, bool>(nameof(IsLast));

        public static readonly StyledProperty<bool> IsFirstProperty =
            AvaloniaProperty.Register<StepItem, bool>(nameof(IsFirst));

        static StepItem() => SelectableMixin.Attach<StepItem>(IsSelectedProperty);

        public bool IsFirst
        {
            get => GetValue(IsFirstProperty);
            set => SetValue(IsFirstProperty, value);
        }

        public bool IsLast
        {
            get => GetValue(IsLastProperty);
            set => SetValue(IsLastProperty, value);
        }

        public bool IsCompleted
        {
            get => GetValue(IsCompletedProperty);
            set => SetValue(IsCompletedProperty, value);
        }

        #region ISelectable Members

        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsCompletedProperty)
            {
                PseudoClasses.Set(":completed", change.GetNewValue<bool>());
            }
        }
    }
}
