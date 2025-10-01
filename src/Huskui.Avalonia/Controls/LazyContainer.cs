using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Metadata;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ContentPresenter, typeof(ContentPresenter))]
public class LazyContainer : TemplatedControl
{
    public const string PART_ContentPresenter = nameof(PART_ContentPresenter);

    public static readonly StyledProperty<object?> FaultContentProperty =
        AvaloniaProperty.Register<LazyContainer, object?>(nameof(FaultContent));

    public static readonly StyledProperty<bool> IsFaultedProperty =
        AvaloniaProperty.Register<LazyContainer, bool>(nameof(IsFaulted));

    public static readonly StyledProperty<LazyObject?> SourceProperty =
        AvaloniaProperty.Register<LazyContainer, LazyObject?>(nameof(Source));

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

        UnregisterHandlers();

        _contentPresenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);
        if (_contentPresenter != null)
        {
            _contentPresenter.PropertyChanged += ContentPresenterOnPropertyChanged;
        }

        // Start loading if source is available
        if (Source != null)
        {
            _ = LoadContentAsync(Source);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        UnregisterHandlers();
    }

    private void UnregisterHandlers()
    {
        if (_contentPresenter != null)
        {
            _contentPresenter.PropertyChanged -= ContentPresenterOnPropertyChanged;
        }
    }

    private void ContentPresenterOnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
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


    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            if (change.OldValue is LazyObject oldLazy && !oldLazy.IsCancelled && oldLazy.IsInProgress)
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
        if (Design.IsDesignMode || Source is null || _contentPresenter is null)
        {
            return;
        }

        try
        {
            // 以下会在第二次替换 Source 时触发，并产生未知原因报错
            // _contentPresenter.ContentTemplate = null;
            // _contentPresenter.Content = null;
            IsFaulted = false;

            if (Source.Value != null)
            {
                _contentPresenter.ContentTemplate = SourceTemplate;
                _contentPresenter.Content = Source.Value;
                return;
            }

            await lazy.FetchAsync();
            _contentPresenter.ContentTemplate = SourceTemplate;
            _contentPresenter.Content = Source.Value;
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            _contentPresenter.ContentTemplate = null;
            _contentPresenter.Content = FaultContent;
            IsFaulted = true;
            Debug.WriteLine($"LazyContainer failed to load content: {ex.Message}");
        }
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
