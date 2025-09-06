using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":loading", ":finished", ":failed")]
public class Page : HeaderedContentControl
{
    public static readonly StyledProperty<bool> CanGoBackProperty =
        AvaloniaProperty.Register<Page, bool>(nameof(CanGoBack));

    public static readonly DirectProperty<Page, bool> IsHeaderVisibleProperty =
        AvaloniaProperty.RegisterDirect<Page, bool>(nameof(IsHeaderVisible),
                                                    o => o.IsHeaderVisible,
                                                    (o, v) => o.IsHeaderVisible = v);

    public static readonly DirectProperty<Page, bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.RegisterDirect<Page, bool>(nameof(IsBackButtonVisible),
                                                    o => o.IsBackButtonVisible,
                                                    (o, v) => o.IsBackButtonVisible = v);

    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        AvaloniaProperty.Register<Page, BoxShadows>(nameof(BoxShadow));


    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }

    public bool IsBackButtonVisible
    {
        get;
        set => SetAndRaise(IsBackButtonVisibleProperty, ref field, value);
    } = true;

    public IPageModel? Model { get; set; }

    public bool CanGoBack
    {
        get => GetValue(CanGoBackProperty);
        set => SetValue(CanGoBackProperty, value);
    }

    public bool IsHeaderVisible
    {
        get;
        set => SetAndRaise(IsHeaderVisibleProperty, ref field, value);
    } = true;

    protected override Type StyleKeyOverride => typeof(Page);


    protected override async void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (!Design.IsDesignMode)
        {
            if (Model is not null)
            {
                SetState(true);
                try
                {
                    await Model.InitializeAsync(_cancellationTokenSource.Token);
                    SetState(false, true);
                }
                catch
                {
                    SetState(false, false, true);
                }
            }
        }
    }

    protected override async void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);

        if (!Design.IsDesignMode)
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                await _cancellationTokenSource.CancelAsync();
            }

            if (Model is not null)
            {
                await Model.DeinitializeAsync(CancellationToken.None);
            }
        }
    }

    private void SetState(bool loading = false, bool finished = false, bool failed = false)
    {
        PseudoClasses.Set(":loading", loading);
        PseudoClasses.Set(":finished", finished);
        PseudoClasses.Set(":failed", failed);
    }
}
