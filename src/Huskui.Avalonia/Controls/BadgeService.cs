using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

public sealed class BadgeService : AvaloniaObject
{
    private const string DotClass = "Dot";

    private BadgeService() { }

    public static readonly AttachedProperty<object?> ContentProperty = AvaloniaProperty.RegisterAttached<
        BadgeService,
        Control,
        object?
    >("Content");

    public static readonly AttachedProperty<IDataTemplate?> ContentTemplateProperty =
        AvaloniaProperty.RegisterAttached<BadgeService, Control, IDataTemplate?>(
            "ContentTemplate"
        );

    public static readonly AttachedProperty<string?> ClassesProperty = AvaloniaProperty.RegisterAttached<
        BadgeService,
        Control,
        string?
    >("Classes");

    public static readonly AttachedProperty<bool> IsVisibleProperty = AvaloniaProperty.RegisterAttached<
        BadgeService,
        Control,
        bool
    >("IsVisible", true);

    public static readonly AttachedProperty<BadgePlacement> PlacementProperty =
        AvaloniaProperty.RegisterAttached<BadgeService, Control, BadgePlacement>(
            "Placement",
            BadgePlacement.TopRight
        );

    private static readonly AttachedProperty<State?> StateProperty = AvaloniaProperty.RegisterAttached<
        BadgeService,
        Control,
        State?
    >("State");

    static BadgeService()
    {
        ContentProperty.Changed.AddClassHandler<Control>((control, _) => Update(control));
        ContentTemplateProperty.Changed.AddClassHandler<Control>((control, _) => Update(control));
        IsVisibleProperty.Changed.AddClassHandler<Control>((control, _) => Update(control));
        ClassesProperty.Changed.AddClassHandler<Control>((control, _) => Update(control));
        PlacementProperty.Changed.AddClassHandler<Control>((control, _) => Update(control));
    }

    public static object? GetContent(Control element) => element.GetValue(ContentProperty);

    public static void SetContent(Control element, object? value) =>
        element.SetValue(ContentProperty, value);

    public static IDataTemplate? GetContentTemplate(Control element) =>
        element.GetValue(ContentTemplateProperty);

    public static void SetContentTemplate(Control element, IDataTemplate? value) =>
        element.SetValue(ContentTemplateProperty, value);

    public static string? GetClasses(Control element) => element.GetValue(ClassesProperty);

    public static void SetClasses(Control element, string? value) =>
        element.SetValue(ClassesProperty, value);

    public static bool GetIsVisible(Control element) => element.GetValue(IsVisibleProperty);
    public static void SetIsVisible(Control element, bool value) =>
        element.SetValue(IsVisibleProperty, value);

    public static BadgePlacement GetPlacement(Control element) => element.GetValue(PlacementProperty);

    public static void SetPlacement(Control element, BadgePlacement value) =>
        element.SetValue(PlacementProperty, value);

    private static State? GetState(Control control) => control.GetValue(StateProperty);

    private static void SetState(Control control, State? state) => control.SetValue(StateProperty, state);

    private static void Update(Control control)
    {
        var state = GetState(control);
        var classes = GetClassNames(control);
        var hasContent = GetContent(control) is not null || classes.Contains(DotClass);
        var shouldShow = GetIsVisible(control) && hasContent;

        if (!shouldShow)
        {
            state?.Dispose();
            SetState(control, null);
            return;
        }

        if (state is null)
        {
            state = new(control);
            SetState(control, state);
        }

        state.Update(classes);
    }

    private static string[] GetClassNames(Control control) =>
        Classes.Parse(GetClasses(control) ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();

    private sealed class State : IDisposable
    {
        private readonly Control _control;
        private readonly BadgeAdorner _adorner = new();
        private AdornerLayer? _layer;
        private bool _disposed;

        public State(Control control)
        {
            _control = control;
            _control.AttachedToVisualTree += OnAttachedToVisualTree;
            _control.DetachedFromVisualTree += OnDetachedFromVisualTree;
        }

        public void Update(string[] classes)
        {
            if (_disposed)
            {
                return;
            }

            _adorner.Content = GetContent(_control);
            _adorner.ContentTemplate = GetContentTemplate(_control);
            _adorner.Placement = GetPlacement(_control);
            _adorner.Classes.Replace(classes);

            if (_control.IsAttachedToVisualTree())
            {
                Attach();
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Detach();
            _control.AttachedToVisualTree -= OnAttachedToVisualTree;
            _control.DetachedFromVisualTree -= OnDetachedFromVisualTree;
            _disposed = true;
        }

        private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e) => Attach();

        private void OnDetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e) => Detach();

        private void Attach()
        {
            var layer = AdornerLayer.GetAdornerLayer(_control);
            if (layer is null)
            {
                return;
            }

            if (ReferenceEquals(layer, _layer) && layer.Children.Contains(_adorner))
            {
                return;
            }

            Detach();

            _layer = layer;
            AdornerLayer.SetAdornedElement(_adorner, _control);
            AdornerLayer.SetIsClipEnabled(_adorner, false);
            ((ISetLogicalParent)_adorner).SetParent(_control);
            layer.Children.Add(_adorner);
        }

        private void Detach()
        {
            if (_layer is null)
            {
                return;
            }

            _layer.Children.Remove(_adorner);
            ((ISetLogicalParent)_adorner).SetParent(null);
            AdornerLayer.SetAdornedElement(_adorner, null);
            _layer = null;
        }
    }
}
