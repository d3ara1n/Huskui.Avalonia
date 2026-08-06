using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Metadata;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ContentPresenter, typeof(ContentPresenter))]
public class LazyContainer : TemplatedControl
{
    public const string PART_ContentPresenter = nameof(PART_ContentPresenter);

    public static readonly StyledProperty<object?> FaultContentProperty = AvaloniaProperty.Register<
        LazyContainer,
        object?
    >(nameof(FaultContent));

    public static readonly StyledProperty<bool> IsFaultedProperty = AvaloniaProperty.Register<
        LazyContainer,
        bool
    >(nameof(IsFaulted));

    public static readonly StyledProperty<LazyObject?> SourceProperty = AvaloniaProperty.Register<
        LazyContainer,
        LazyObject?
    >(nameof(Source));

    public static readonly StyledProperty<IDataTemplate?> SourceTemplateProperty =
        AvaloniaProperty.Register<LazyContainer, IDataTemplate?>(nameof(SourceTemplate));

    private ContentPresenter? _contentPresenter;

    [DependsOn(nameof(SourceTemplate))]
    public LazyObject? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public IDataTemplate? SourceTemplate
    {
        get => GetValue(SourceTemplateProperty);
        set => SetValue(SourceTemplateProperty, value);
    }

    [Content]
    public object? FaultContent
    {
        get => GetValue(FaultContentProperty);
        set => SetValue(FaultContentProperty, value);
    }

    public bool IsFaulted
    {
        get => GetValue(IsFaultedProperty);
        set => SetValue(IsFaultedProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _contentPresenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);

        if (Source != null)
        {
            _ = LoadContentAsync(Source);
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            if (change.OldValue is LazyObject { IsCancelled: false, IsInProgress: true } oldLazy)
            {
                oldLazy.Cancel();
            }

            if (change.NewValue is LazyObject newLazy && _contentPresenter is not null)
            {
                _ = LoadContentAsync(newLazy);
            }
        }
    }

    private async Task LoadContentAsync(LazyObject lazy)
    {
        if (Design.IsDesignMode || _contentPresenter is null)
        {
            return;
        }

        try
        {
            IsFaulted = false;

            if (lazy.Value != null)
            {
                Present(lazy.Value);
                return;
            }

            await lazy.FetchAsync();

            // NOTE: 加载期间 Source 可能已被替换为新的 LazyObject（翻页等场景）。
            // 过期的加载不得写 Content，否则会把当前展示覆盖成 null 并触发模板内
            // 编译绑定对错误 DataContext 的强 cast，抛 InvalidCastException。
            if (!ReferenceEquals(Source, lazy))
            {
                return;
            }

            if (lazy.Value != null)
            {
                Present(lazy.Value);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            if (!ReferenceEquals(Source, lazy))
            {
                return;
            }

            _contentPresenter.ContentTemplate = null;
            _contentPresenter.Content = FaultContent;
            IsFaulted = true;
            Debug.WriteLine($"LazyContainer failed to load content: {ex.Message}");
        }
    }

    private void Present(object value)
    {
        _contentPresenter!.ContentTemplate = SourceTemplate;
        _contentPresenter.Content = value;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        if (Source is { IsCancelled: false, IsInProgress: true })
        {
            Source.Cancel();
        }
    }
}
