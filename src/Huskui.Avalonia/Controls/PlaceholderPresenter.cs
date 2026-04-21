using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ContentPresenter, typeof(ContentPresenter))]
public class PlaceholderPresenter : TemplatedControl
{
    public const string PART_ContentPresenter = nameof(PART_ContentPresenter);

    public static readonly StyledProperty<object?> PlaceholderProperty = AvaloniaProperty.Register<
        PlaceholderPresenter,
        object?
    >(nameof(Placeholder));

    public static readonly StyledProperty<object?> SourceProperty = AvaloniaProperty.Register<
        PlaceholderPresenter,
        object?
    >(nameof(Source));

    public static readonly StyledProperty<IDataTemplate?> SourceTemplateProperty =
        AvaloniaProperty.Register<PlaceholderPresenter, IDataTemplate?>(nameof(SourceTemplate));

    public static readonly DirectProperty<PlaceholderPresenter, object?> CurrentContentProperty =
        AvaloniaProperty.RegisterDirect<PlaceholderPresenter, object?>(
            nameof(CurrentContent),
            o => o.CurrentContent,
            (o, v) => o.CurrentContent = v,
            defaultBindingMode: BindingMode.OneWayToSource
        );

    public static readonly DirectProperty<PlaceholderPresenter, IDataTemplate?>
        CurrentContentTemplateProperty = AvaloniaProperty.RegisterDirect<
            PlaceholderPresenter,
            IDataTemplate?
        >(
            nameof(CurrentContentTemplate),
            o => o.CurrentContentTemplate,
            (o, v) => o.CurrentContentTemplate = v,
            defaultBindingMode: BindingMode.OneWayToSource
        );

    private ContentPresenter? _contentPresenter;

    [Content]
    public object? Placeholder
    {
        get => GetValue(PlaceholderProperty);
        set => SetValue(PlaceholderProperty, value);
    }

    public object? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public IDataTemplate? SourceTemplate
    {
        get => GetValue(SourceTemplateProperty);
        set => SetValue(SourceTemplateProperty, value);
    }

    public object? CurrentContent
    {
        get;
        private set => SetAndRaise(CurrentContentProperty, ref field, value);
    }

    public IDataTemplate? CurrentContentTemplate
    {
        get;
        private set => SetAndRaise(CurrentContentTemplateProperty, ref field, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (
            change.Property == SourceProperty
            || change.Property == SourceTemplateProperty
            || change.Property == PlaceholderProperty
        )
        {
            UpdateCurrentContent();
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        UnregisterContentPresenterHandlers();
        _contentPresenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);
        RegisterContentPresenterHandlers();
    }

    private void RegisterContentPresenterHandlers()
    {
        if (_contentPresenter != null)
        {
            _contentPresenter.PropertyChanged += ContentPresenterOnPropertyChanged;
        }
    }

    private void UnregisterContentPresenterHandlers()
    {
        if (_contentPresenter != null)
        {
            _contentPresenter.PropertyChanged -= ContentPresenterOnPropertyChanged;
        }
    }

    private void ContentPresenterOnPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (e.Property == ContentPresenter.ChildProperty)
        {
            if (e.OldValue is ILogical oldChild)
            {
                LogicalChildren.Remove(oldChild);
            }

            if (e.NewValue is ILogical newChild)
            {
                LogicalChildren.Add(newChild);
            }
        }
    }

    private void UpdateCurrentContent()
    {
        var source = Source;

        if (source != null)
        {
            CurrentContentTemplate = SourceTemplate;
            CurrentContent = source;
        }
        else
        {
            CurrentContentTemplate = null;
            CurrentContent = Placeholder;
        }
    }
}
