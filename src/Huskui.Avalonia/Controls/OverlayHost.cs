using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Styling;
using Avalonia.Threading;
using Huskui.Avalonia.Transitions;

namespace Huskui.Avalonia.Controls
{
    [PseudoClasses(":present")]
    [TemplatePart(PART_ItemsPresenter, typeof(ItemsPresenter))]
    public class OverlayHost : ItemsControl
    {
        public const string PART_ItemsPresenter = nameof(PART_ItemsPresenter);

        public static readonly DirectProperty<OverlayHost, bool> IsPresentProperty =
            AvaloniaProperty.RegisterDirect<OverlayHost, bool>(nameof(IsPresent),
                                                               o => o.IsPresent,
                                                               (o, v) => o.IsPresent = v);


        public static readonly DirectProperty<OverlayHost, IPageTransition> TransitionProperty =
            AvaloniaProperty.RegisterDirect<OverlayHost, IPageTransition>(nameof(Transition),
                                                                          o => o.Transition,
                                                                          (o, v) => o.Transition = v);

        public static readonly RoutedEvent<PropertyChangedRoutedEventArgs<bool>> IsPresentChangedEvent =
            RoutedEvent.Register<OverlayHost, PropertyChangedRoutedEventArgs<bool>>(nameof(IsPresentChanged),
                RoutingStrategies.Bubble);

        public bool IsPresent
        {
            get;
            set => SetAndRaise(IsPresentProperty, ref field, value);
        }

        public IPageTransition Transition
        {
            get;
            set => SetAndRaise(TransitionProperty, ref field, value);
        } = new PageCoverOverTransition(null, DirectionFrom.Bottom);

        protected override Type StyleKeyOverride => typeof(OverlayHost);

        public event EventHandler<PropertyChangedRoutedEventArgs<bool>>? IsPresentChanged
        {
            add => AddHandler(IsPresentChangedEvent, value);
            remove => RemoveHandler(IsPresentChangedEvent, value);
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
            new OverlayItem();

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
            NeedsContainer<OverlayItem>(item, out recycleKey);

        // protected override void ClearContainerForItemOverride(Control container) =>
        //     base.ClearContainerForItemOverride(container);

        protected override void ContainerForItemPreparedOverride(Control container, object? item, int index)
        {
            base.ContainerForItemPreparedOverride(container, item, index);

            if (container is OverlayItem overlay)
            {
                var transition = overlay.Transition ?? Transition;
                transition.Start(null, overlay, true, CancellationToken.None);

                if (Items.Count == 1)
                {
                    IsPresent = true;
                }
            }

            UpdateDistanceFrom(index);
        }

        protected override void PrepareContainerForItemOverride(Control container, object? item, int index) =>
            base.PrepareContainerForItemOverride(container, item, index);

        protected override void ContainerIndexChangedOverride(Control container, int oldIndex, int newIndex)
        {
            base.ContainerIndexChangedOverride(container, oldIndex, newIndex);

            UpdateDistanceFrom(newIndex, oldIndex);
        }

        private void UpdateDistanceFrom(int newIndex, int oldIndex = -1)
        {
            var start = ItemCount - newIndex - 1;
            for (var i = newIndex; i > oldIndex; i--)
            {
                if (ContainerFromIndex(i) is OverlayItem item)
                {
                    item.Distance = start;
                    start++;
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsPresentProperty)
            {
                PseudoClasses.Set(":present", change.GetNewValue<bool>());
                RaiseEvent(new PropertyChangedRoutedEventArgs<bool>(IsPresentChangedEvent,
                                                                    this,
                                                                    change.GetOldValue<bool>(),
                                                                    change.GetNewValue<bool>()));
            }
        }

        // public void Pop(object control)
        // {
        //     var item = new OverlayItem { Content = control, Distance = 0 };
        //     foreach (var i in Items.OfType<OverlayItem>())
        //     {
        //         i.Distance++;
        //     }
        //
        //     Items.Add(item);
        //
        //
        //     // Make control attached to visual tree ensuring its parent is valid
        //     // Make OnApplyTemplate called
        //     UpdateLayout();
        //     // if (control is Visual visual) Transition.Start(null, visual, true, CancellationToken.None);
        //     var transition = item.Transition ?? Transition;
        //     transition.Start(null, item.ContentPresenter, true, CancellationToken.None);
        //
        //     if (Items.Count == 1)
        //     {
        //         IsPresent = true;
        //     }
        // }

        private void Dismiss(OverlayItem item)
        {
            var transition = item.Transition ?? Transition;
            transition
               .Start(item, null, false, CancellationToken.None)
               .ContinueWith(_ => Dispatcher.UIThread.Post(Clean));
            return;

            void Clean()
            {
                for (var i = 0; i < Items.IndexOf(item); i++)
                {
                    if (Items[i] is OverlayItem inner)
                    {
                        inner.Distance--;
                    }
                }

                // TODO: 移出不再在这个 OverlayHost 完成
                Items.Remove(item);
                if (Items.Count == 0)
                {
                    IsPresent = false;
                }
            }
        }

        // public void Dismiss()
        // {
        //     if (Items is [.., OverlayItem last])
        //     {
        //         Dismiss(last);
        //     }
        // }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            AddHandler(OverlayItem.DismissRequestedEvent, DismissRequestedHandler);
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            base.OnUnloaded(e);
            RemoveHandler(OverlayItem.DismissRequestedEvent, DismissRequestedHandler);
        }

        private void DismissRequestedHandler(object? sender, OverlayItem.DismissRequestedEventArgs e)
        {
            if (e.Source is not null && ContainerFromItem(e.Source) is OverlayItem item)
            {
                Dismiss(item);
            }

            e.Handled = true;
        }

        #region StageInAnimation & StageOutAnimation

        private static readonly Animation StageInAnimation = new()
        {
            FillMode = FillMode.Forward,
            Duration = TimeSpan.FromMilliseconds(146),
            Easing = new SineEaseOut(),
            Children =
            {
                new() { Cue = new(0d), Setters = { new Setter { Property = OpacityProperty, Value = 0d } } },
                new() { Cue = new(1d), Setters = { new Setter { Property = OpacityProperty, Value = 1d } } }
            }
        };

        private static readonly Animation StageOutAnimation = new()
        {
            FillMode = FillMode.Forward,
            Duration = TimeSpan.FromMilliseconds(146),
            Easing = new SineEaseOut(),
            Children =
            {
                new() { Cue = new(0d), Setters = { new Setter { Property = OpacityProperty, Value = 1d } } },
                new() { Cue = new(1d), Setters = { new Setter { Property = OpacityProperty, Value = 0d } } }
            }
        };

        #endregion

        #region ContentAlignment

        public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty =
            ContentControl.HorizontalContentAlignmentProperty.AddOwner<OverlayHost>();

        public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty =
            ContentControl.VerticalContentAlignmentProperty.AddOwner<OverlayHost>();

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => GetValue(HorizontalContentAlignmentProperty);
            set => SetValue(HorizontalContentAlignmentProperty, value);
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get => GetValue(VerticalContentAlignmentProperty);
            set => SetValue(VerticalContentAlignmentProperty, value);
        }

        #endregion
    }
}
