using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Metadata;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ContentPresenter, typeof(ContentPresenter))]
public class PlaceholderContainer : TemplatedControl
{
    public const string PART_ContentPresenter = nameof(PART_ContentPresenter);

    public static readonly StyledProperty<object?> PlaceholderProperty = AvaloniaProperty.Register<
        PlaceholderContainer,
        object?
    >(nameof(Placeholder));

    public static readonly StyledProperty<object?> SourceProperty = AvaloniaProperty.Register<
        PlaceholderContainer,
        object?
    >(nameof(Source));

    public static readonly StyledProperty<IDataTemplate?> SourceTemplateProperty =
        AvaloniaProperty.Register<PlaceholderContainer, IDataTemplate?>(nameof(SourceTemplate));

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _contentPresenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);

        UpdateContent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if ((change.Property == SourceProperty || change.Property == SourceTemplateProperty)
         && _contentPresenter != null)
        {
            UpdateContent();
        }
    }

    private void UpdateContent()
    {
        if (_contentPresenter == null)
        {
            return;
        }

        // NOTE: 不要手动操作 LogicalChildren。ContentPresenter 在 Avalonia 11 中会自动把
        // 它的 child 注册进模板父级的逻辑树；手动 Remove/Add 会在切换瞬间把旧 child 摘出
        // 逻辑树，触发继承式 DataContext 重算，模板内编译绑定（强 cast 到具体类型）就会在
        // 这一瞬拿到外层 DataContext 而抛 InvalidCastException。交由 ContentPresenter 原子切换即可。
        if (Source != null)
        {
            _contentPresenter.ContentTemplate = SourceTemplate;
            _contentPresenter.Content = Source;
        }
        else
        {
            _contentPresenter.ContentTemplate = null;
            _contentPresenter.Content = Placeholder;
        }
    }
}
