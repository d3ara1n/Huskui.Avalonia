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

    public static readonly StyledProperty<object?> BadContentProperty =
        AvaloniaProperty.Register<LazyContainer, object?>(nameof(BadContent));

    public static readonly StyledProperty<bool> IsBadProperty =
        AvaloniaProperty.Register<LazyContainer, bool>(nameof(IsBad));

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
    public object? BadContent
    {
        get => GetValue(BadContentProperty);
        set => SetValue(BadContentProperty, value);
    }

    public bool IsBad
    {
        get => GetValue(IsBadProperty);
        set => SetValue(IsBadProperty, value);
    }

    protected override async void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        if (_contentPresenter != null)
        {
            _contentPresenter.PropertyChanged -= ContentPresenterOnPropertyChanged;
        }

        _contentPresenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);
        if (_contentPresenter != null)
        {
            _contentPresenter.PropertyChanged += ContentPresenterOnPropertyChanged;
        }

        if (Source != null)
        {
            await LoadAsync(Source);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

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


    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == SourceProperty)
        {
            if (change.OldValue is LazyObject { IsCancelled: false, IsInProgress: true } old)
            {
                old.Cancel();
            }

            if (change.NewValue is LazyObject lazy && _contentPresenter is not null)
            {
                await LoadAsync(lazy);
            }
        }
    }

    private async Task LoadAsync(LazyObject lazy)
    {
        if (Design.IsDesignMode)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(_contentPresenter);

        _contentPresenter.Content = null;
        _contentPresenter.ContentTemplate = null;
        IsBad = false;
        if (lazy.Value != null)
        {
            _contentPresenter.Content = lazy.Value;
            _contentPresenter.ContentTemplate = SourceTemplate;
        }
        else
        {
            try
            {
                await lazy.FetchAsync();
                _contentPresenter.Content = lazy.Value;
                _contentPresenter.ContentTemplate = SourceTemplate;
            }
            catch
            {
                _contentPresenter.Content = BadContent;
                _contentPresenter.ContentTemplate = null;
                IsBad = true;
            }
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
