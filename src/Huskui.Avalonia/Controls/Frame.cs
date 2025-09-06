using System.Windows.Input;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.LogicalTree;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_ContentPresenter, typeof(ContentPresenter))]
[TemplatePart(PART_ContentPresenter2, typeof(ContentPresenter))]
public class Frame : TemplatedControl
{
    #region Delegates

    public delegate object? PageActivatorDelegate(Type page, object? parameter);

    #endregion

    public const string PART_ContentPresenter = nameof(PART_ContentPresenter);
    public const string PART_ContentPresenter2 = nameof(PART_ContentPresenter2);


    public static readonly StyledProperty<object?> ContentProperty =
        AvaloniaProperty.Register<Frame, object?>(nameof(Content));

    public static readonly StyledProperty<int> HistoryCountProperty =
        AvaloniaProperty.Register<Frame, int>(nameof(HistoryCount));

    public static readonly StyledProperty<IPageTransition> DefaultTransitionProperty =
        AvaloniaProperty.Register<Frame, IPageTransition>(nameof(DefaultTransition),
                                                          TransitioningContentControl.PageTransitionProperty
                                                             .GetDefaultValue(typeof(TransitioningContentControl))
                                                       ?? new CrossFade(TimeSpan.FromMilliseconds(197)));

    public static readonly StyledProperty<bool> CanGoBackProperty =
        AvaloniaProperty.Register<Frame, bool>(nameof(CanGoBack));

    public bool CanGoBack
    {
        get => GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    public static readonly StyledProperty<bool> CanGoBackOutOfStackProperty =
        AvaloniaProperty.Register<Frame, bool>(nameof(CanGoBackOutOfStack));

    public bool CanGoBackOutOfStack
    {
        get => GetValue(CanGoBackOutOfStackProperty);
        set => SetValue(CanGoBackOutOfStackProperty, value);
    }

    public IPageTransition DefaultTransition
    {
        get => GetValue(DefaultTransitionProperty);
        set => SetValue(DefaultTransitionProperty, value);
    }

    public int HistoryCount
    {
        get => GetValue(HistoryCountProperty);
        set => SetValue(HistoryCountProperty, value);
    }


    private readonly InternalCommand _goBackCommand;

    private readonly Stack<FrameFrame> _history = new();

    private FrameFrame? _currentFrame;
    private CancellationTokenSource? _currentToken;
    private bool _toggle;

    private ContentPresenter? _presenter;
    private ContentPresenter? _presenter2;

    public Frame()
    {
        _goBackCommand = new(GoBack, () => CanGoBack);
    }

    public object? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public ICommand GoBackCommand => _goBackCommand;

    public PageActivatorDelegate PageActivator { get; set; } = ActivatePage;

    private static object? ActivatePage(Type page, object? parameter)
    {
        var obj = Activator.CreateInstance(page);
        if (obj is StyledElement element)
        {
            element.DataContext = parameter;
        }

        return obj;
    }

    public void ClearHistory() => _history.Clear();

    public void Navigate(Type page, object? parameter, IPageTransition? transition)
    {
        var content = PageActivator(page, parameter)
                   ?? throw new InvalidOperationException($"Activating {page.Name} gets null page model");
        var old = CanGoBack;
        if (_currentFrame is not null)
        {
            _history.Push(_currentFrame);
        }

        _currentFrame = new(page, parameter, transition);

        UpdateContent(content, transition ?? DefaultTransition, false);
        _goBackCommand.OnCanExecuteChanged();
    }

    public void GoBack()
    {
        if (_history.TryPop(out var frame))
        {
            var content = PageActivator(frame.Page, frame.Parameter) ?? throw new ArgumentNullException();
            UpdateContent(content, _currentFrame?.Transition ?? DefaultTransition, true);
            _currentFrame = frame;
        }
        else if (CanGoBackOutOfStack)
        {
            _currentFrame = null;
            UpdateContent(null, _currentFrame?.Transition ?? DefaultTransition, true);
        }
        else
        {
            throw new InvalidOperationException("No previous page in the stack");
        }

        _goBackCommand.OnCanExecuteChanged();
    }

    private void UpdateContent(object? content, IPageTransition transition, bool reverse)
    {
        HistoryCount = _history.Count;

        ArgumentNullException.ThrowIfNull(_presenter);
        ArgumentNullException.ThrowIfNull(_presenter2);

        CanGoBack = _history.Count > 0 || CanGoBackOutOfStack;
        _currentToken?.Cancel();
        var cancel = new CancellationTokenSource();
        _currentToken = cancel;

        var currentPresenter = _toggle ? _presenter : _presenter2;
        var targetPresenter = _toggle ? _presenter2 : _presenter;
        _toggle = !_toggle;

        (currentPresenter.ZIndex, targetPresenter.ZIndex) = (0, 1);
        (currentPresenter.IsVisible, targetPresenter.IsVisible) = (true, true);

        targetPresenter.Content = content;

        var (fromPresenter, toPresenter) = reverse
                                               ? (targetPresenter, currentPresenter)
                                               : (currentPresenter, targetPresenter);
        transition
           .Start(fromPresenter, toPresenter, !reverse, cancel.Token)
           .ContinueWith(_ =>
                         {
                             if (cancel.IsCancellationRequested)
                             {
                                 return;
                             }

                             (currentPresenter.IsVisible, targetPresenter.IsVisible) = (false, true);
                             currentPresenter.Content = null;

                             // NOTE: ContentControl.Content 改变会移除 from.Content 自 LogicalChildren，这会导致 from.Content 的 StaticResource 全部失效
                             //  因此要放在动画结束 from 退出时对 Content 进行设置
                             Content = targetPresenter.Content;
                         },
                         TaskScheduler.FromCurrentSynchronizationContext());
    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _presenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);
        _presenter2 = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter2);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ContentProperty)
        {
            if (change.OldValue is ILogical oldChild)
            {
                LogicalChildren.Remove(oldChild);
            }

            if (change.NewValue is ILogical newChild)
            {
                LogicalChildren.Add(newChild);
            }
        }
    }

    #region Nested type: FrameFrame

    // protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    // {
    //     base.OnApplyTemplate(e);
    //     _presenter = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter);
    //     _presenter2 = e.NameScope.Find<ContentPresenter>(PART_ContentPresenter2);
    // }

    public record FrameFrame(Type Page, object? Parameter, IPageTransition? Transition);

    #endregion
}
