using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using FluentIcons.Common;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":readonly")]
[TemplatePart(PART_StarsPanel, typeof(ItemsControl))]
public class RatingControl : RangeBase
{
    public const string PART_StarsPanel = nameof(PART_StarsPanel);

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        AvaloniaProperty.Register<RatingControl, bool>(nameof(IsReadOnly));

    public static readonly StyledProperty<bool> IsClearEnabledProperty =
        AvaloniaProperty.Register<RatingControl, bool>(nameof(IsClearEnabled), true);

    public static readonly StyledProperty<bool> AllowHalfStarsProperty =
        AvaloniaProperty.Register<RatingControl, bool>(nameof(AllowHalfStars));

    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<RatingControl, double>(nameof(Spacing), 2d);

    public static readonly StyledProperty<Symbol> FilledIconProperty =
        AvaloniaProperty.Register<RatingControl, Symbol>(nameof(FilledIcon), Symbol.Star);

    public static readonly StyledProperty<Symbol> UnfilledIconProperty =
        AvaloniaProperty.Register<RatingControl, Symbol>(nameof(UnfilledIcon), Symbol.Star);

    public static readonly StyledProperty<IBrush?> FilledColorProperty =
        AvaloniaProperty.Register<RatingControl, IBrush?>(nameof(FilledColor));

    public static readonly StyledProperty<IBrush?> UnfilledColorProperty =
        AvaloniaProperty.Register<RatingControl, IBrush?>(nameof(UnfilledColor));

    public static readonly DirectProperty<RatingControl, double> PreviewValueProperty =
        AvaloniaProperty.RegisterDirect<RatingControl, double>(nameof(PreviewValue), o => o.PreviewValue);

    public static readonly DirectProperty<RatingControl, bool> IsPreviewingProperty =
        AvaloniaProperty.RegisterDirect<RatingControl, bool>(nameof(IsPreviewing), o => o.IsPreviewing);

    public static readonly DirectProperty<RatingControl, ObservableCollection<RatingStar>> StarsProperty =
        AvaloniaProperty.RegisterDirect<RatingControl, ObservableCollection<RatingStar>>(nameof(Stars), o => o.Stars);

    private ItemsControl? _starsPanel;

    static RatingControl()
    {
        MaximumProperty.OverrideDefaultValue<RatingControl>(5d);
        MinimumProperty.OverrideDefaultValue<RatingControl>(0d);
    }

    public RatingControl() => UpdateStarsCollection();

    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public bool IsClearEnabled
    {
        get => GetValue(IsClearEnabledProperty);
        set => SetValue(IsClearEnabledProperty, value);
    }

    public bool AllowHalfStars
    {
        get => GetValue(AllowHalfStarsProperty);
        set => SetValue(AllowHalfStarsProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public Symbol FilledIcon
    {
        get => GetValue(FilledIconProperty);
        set => SetValue(FilledIconProperty, value);
    }

    public Symbol UnfilledIcon
    {
        get => GetValue(UnfilledIconProperty);
        set => SetValue(UnfilledIconProperty, value);
    }

    public IBrush? FilledColor
    {
        get => GetValue(FilledColorProperty);
        set => SetValue(FilledColorProperty, value);
    }

    public IBrush? UnfilledColor
    {
        get => GetValue(UnfilledColorProperty);
        set => SetValue(UnfilledColorProperty, value);
    }

    public double PreviewValue
    {
        get;
        private set => SetAndRaise(PreviewValueProperty, ref field, value);
    }

    public bool IsPreviewing
    {
        get;
        private set => SetAndRaise(IsPreviewingProperty, ref field, value);
    }

    public ObservableCollection<RatingStar> Stars
    {
        get;
        private init => SetAndRaise(StarsProperty, ref field, value);
    } = [];

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsReadOnlyProperty)
            PseudoClasses.Set(":readonly", change.GetNewValue<bool>());

        if (change.Property == MaximumProperty)
            UpdateStarsCollection();

        if (change.Property == ValueProperty || change.Property == PreviewValueProperty || change.Property == IsPreviewingProperty)
            UpdateStarStates();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_starsPanel != null)
        {
            _starsPanel.ContainerPrepared -= OnContainerPrepared;
            _starsPanel.ContainerClearing -= OnContainerClearing;
        }

        _starsPanel = e.NameScope.Find<ItemsControl>(PART_StarsPanel);

        if (_starsPanel != null)
        {
            _starsPanel.ContainerPrepared += OnContainerPrepared;
            _starsPanel.ContainerClearing += OnContainerClearing;
        }

        UpdateStarStates();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        IsPreviewing = false;
        PreviewValue = 0;
    }

    private void OnContainerPrepared(object? sender, ContainerPreparedEventArgs e)
    {
        e.Container.PointerMoved += OnStarPointerMoved;
        e.Container.PointerPressed += OnStarPointerPressed;
    }

    private void OnContainerClearing(object? sender, ContainerClearingEventArgs e)
    {
        e.Container.PointerMoved -= OnStarPointerMoved;
        e.Container.PointerPressed -= OnStarPointerPressed;
    }

    private void OnStarPointerMoved(object? sender, PointerEventArgs e)
    {
        if (IsReadOnly || sender is not Control { DataContext: RatingStar starItem } container)
            return;

        PreviewValue = CalculateValue(container, starItem.Index, e);
        IsPreviewing = true;
    }

    private void OnStarPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (IsReadOnly || sender is not Control { DataContext: RatingStar starItem } container)
            return;

        var newValue = CalculateValue(container, starItem.Index, e);
        Value = IsClearEnabled && Math.Abs(Value - newValue) < 0.01 ? 0 : newValue;
        e.Handled = true;
    }

    private double CalculateValue(Control container, int index, PointerEventArgs e)
    {
        if (!AllowHalfStars || container.Bounds.Width <= 0)
            return index + 1;

        var relativeX = e.GetPosition(container).X / container.Bounds.Width;
        return relativeX <= 0.5 ? index + 0.5 : index + 1;
    }

    private void UpdateStarsCollection()
    {
        Stars.Clear();
        for (var i = 0; i < (int)Maximum; i++)
            Stars.Add(new(i));
        UpdateStarStates();
    }

    private void UpdateStarStates()
    {
        var displayValue = IsPreviewing ? PreviewValue : Value;

        foreach (var star in Stars)
        {
            var threshold = star.Index + 1;
            star.FillState = displayValue >= threshold ? RatingStarFillState.Full :
                             displayValue > star.Index ? RatingStarFillState.Half : RatingStarFillState.Empty;
        }
    }
}
