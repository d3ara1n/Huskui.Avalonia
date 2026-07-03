using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(
    HORIZONTAL_COLLAPSED,
    PC_SELECTED)]
public class NavMenuItem : ContentControl
{
    public const string HORIZONTAL_COLLAPSED = ":horizontal-collapsed";
    public const string PC_SELECTED = ":selected";

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<NavMenuItem, string?>(nameof(Title));

    public static readonly StyledProperty<string?> DescriptionProperty =
        AvaloniaProperty.Register<NavMenuItem, string?>(nameof(Description));

    public static readonly StyledProperty<object?> IconProperty =
        AvaloniaProperty.Register<NavMenuItem, object?>(nameof(Icon));

    public static readonly StyledProperty<IDataTemplate?> IconTemplateProperty =
        AvaloniaProperty.Register<NavMenuItem, IDataTemplate?>(nameof(IconTemplate));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        Button.CommandProperty.AddOwner<NavMenuItem>();

    public static readonly StyledProperty<object?> CommandParameterProperty =
        Button.CommandParameterProperty.AddOwner<NavMenuItem>();

    public static readonly StyledProperty<bool> IsHorizontalCollapsedProperty =
        NavMenu.IsHorizontalCollapsedProperty.AddOwner<NavMenuItem>();

    public static readonly StyledProperty<bool> IsSeparatorProperty =
        AvaloniaProperty.Register<NavMenuItem, bool>(nameof(IsSeparator));

    private bool _isPointerDown;

    static NavMenuItem()
    {
        PressedMixin.Attach<NavMenuItem>();
        FocusableProperty.OverrideDefaultValue<NavMenuItem>(true);
        IsHorizontalCollapsedProperty.Changed.AddClassHandler<NavMenuItem, bool>(
         (item, args) => item.PseudoClasses.Set(HORIZONTAL_COLLAPSED, args.NewValue.Value));
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public object? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public IDataTemplate? IconTemplate
    {
        get => GetValue(IconTemplateProperty);
        set => SetValue(IconTemplateProperty, value);
    }

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool IsHorizontalCollapsed
    {
        get => GetValue(IsHorizontalCollapsedProperty);
        set => SetValue(IsHorizontalCollapsedProperty, value);
    }

    public bool IsSeparator
    {
        get => GetValue(IsSeparatorProperty);
        set => SetValue(IsSeparatorProperty, value);
    }

    private NavMenu? RootMenu { get; set; }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        RootMenu = this.FindAncestorOfType<NavMenu>();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (RootMenu is not null)
        {
            this[!IconTemplateProperty] = RootMenu[!NavMenu.IconTemplateProperty];
            this[!ContentTemplateProperty] = RootMenu[!NavMenu.ContentTemplateProperty];
            this[!IsHorizontalCollapsedProperty] = RootMenu[!NavMenu.IsHorizontalCollapsedProperty];
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Handled) return;
        if (e.Key is Key.Enter or Key.Space)
        {
            if (IsSeparator)
            {
                e.Handled = true;
                return;
            }
            SelectAndExecute();
            e.Handled = true;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (IsSeparator)
        {
            e.Handled = true;
            return;
        }

        base.OnPointerPressed(e);
        if (e.Handled) return;

        var p = e.GetCurrentPoint(this);
        if (p.Properties.PointerUpdateKind is not PointerUpdateKind.LeftButtonPressed) return;

        if (p.Pointer.Type == PointerType.Mouse)
            ActivateMenuItem(e);
        else
            _isPointerDown = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.Handled || !_isPointerDown) return;

        _isPointerDown = false;

        if (e.InitialPressMouseButton is MouseButton.Left)
        {
            var point = e.GetCurrentPoint(this);
            if (new Rect(Bounds.Size).ContainsExclusive(point.Position) && e.Pointer.Type == PointerType.Touch)
                ActivateMenuItem(e);
        }
    }

    internal void SetSelected(bool value) => PseudoClasses.Set(PC_SELECTED, value);

    internal bool SelectItem(NavMenuItem item)
    {
        if (item == this && RootMenu is not null)
        {
            RootMenu.SetCurrentValue(NavMenu.SelectedItemProperty, DataContext ?? this);
        }
        return true;
    }

    private void SelectAndExecute()
    {
        if (SelectItem(this))
        {
            Command?.Execute(CommandParameter);
        }
    }

    private void ActivateMenuItem(RoutedEventArgs e)
    {
        SelectAndExecute();
        e.Handled = true;
    }
}
