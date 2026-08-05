using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_Badge, typeof(Control))]
public sealed class BadgeAdorner : ContentControl
{
    public const string PART_Badge = nameof(PART_Badge);

    public static readonly StyledProperty<BadgePlacement> PlacementProperty =
        AvaloniaProperty.Register<BadgeAdorner, BadgePlacement>(nameof(Placement), BadgePlacement.TopRight);

    private Control? _badge;

    public BadgePlacement Placement
    {
        get => GetValue(PlacementProperty);
        set => SetValue(PlacementProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(BadgeAdorner);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        _badge?.RemoveHandler(SizeChangedEvent, OnBadgeSizeChanged);
        base.OnApplyTemplate(e);
        _badge = e.NameScope.Find<Control>(PART_Badge);
        _badge?.AddHandler(SizeChangedEvent, OnBadgeSizeChanged);
        UpdateBadgePlacement();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == PlacementProperty)
        {
            UpdateBadgePlacement();
        }
    }

    private void OnBadgeSizeChanged(object? sender, SizeChangedEventArgs e) => UpdateBadgePlacement();

    private void UpdateBadgePlacement()
    {
        if (_badge is null)
        {
            return;
        }

        var horizontal = Placement is BadgePlacement.TopRight or BadgePlacement.BottomRight ? 1 : -1;
        var vertical = Placement is BadgePlacement.BottomLeft or BadgePlacement.BottomRight ? 1 : -1;

        _badge.RenderTransform = new TranslateTransform(horizontal * _badge.Bounds.Width / 2,
                                                        vertical * _badge.Bounds.Height / 2);
    }
}
