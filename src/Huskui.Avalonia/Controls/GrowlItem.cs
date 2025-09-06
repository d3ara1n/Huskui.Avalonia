using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[PseudoClasses(":progress", ":information", ":success", ":warning", ":danger")]
public sealed class GrowlItem : ContentControl
{
    public static readonly StyledProperty<GrowlLevel> LevelProperty =
        AvaloniaProperty.Register<GrowlItem, GrowlLevel>(nameof(Level));

    public static readonly DirectProperty<GrowlItem, AvaloniaList<GrowlAction>> ActionsProperty =
        AvaloniaProperty.RegisterDirect<GrowlItem, AvaloniaList<GrowlAction>>(nameof(Actions),
                                                                              o => o.Actions,
                                                                              (o, v) => o.Actions = v);

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<GrowlItem, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<bool> IsCloseButtonVisibleProperty =
        AvaloniaProperty.Register<GrowlItem, bool>(nameof(IsCloseButtonVisible));

    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<GrowlItem, double>(nameof(Progress));

    public static readonly StyledProperty<bool> IsProgressIndeterminateProperty =
        AvaloniaProperty.Register<GrowlItem, bool>(nameof(IsProgressIndeterminate));


    public static readonly StyledProperty<double> ProgressMaximumProperty =
        AvaloniaProperty.Register<GrowlItem, double>(nameof(ProgressMaximum), 100d);

    public static readonly StyledProperty<bool> IsProgressBarVisibleProperty =
        AvaloniaProperty.Register<GrowlItem, bool>(nameof(IsProgressBarVisible));

    public static readonly RoutedEvent<DismissRequestedEventArgs> DismissRequestedEvent =
        RoutedEvent.Register<GrowlItem, DismissRequestedEventArgs>(nameof(DismissRequested), RoutingStrategies.Bubble);

    private readonly CancellationTokenSource _cts = new();
    private readonly InternalCommand _dismissCommand;

    public GrowlItem() => _dismissCommand = new(Dismiss, () => !_cts.IsCancellationRequested);

    protected override Type StyleKeyOverride { get; } = typeof(GrowlItem);

    public ICommand DismissCommand => _dismissCommand;

    public bool IsCloseButtonVisible
    {
        get => GetValue(IsCloseButtonVisibleProperty);
        set => SetValue(IsCloseButtonVisibleProperty, value);
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public bool IsProgressIndeterminate
    {
        get => GetValue(IsProgressIndeterminateProperty);
        set => SetValue(IsProgressIndeterminateProperty, value);
    }

    public double ProgressMaximum
    {
        get => GetValue(ProgressMaximumProperty);
        set => SetValue(ProgressMaximumProperty, value);
    }

    public bool IsProgressBarVisible
    {
        get => GetValue(IsProgressBarVisibleProperty);
        set => SetValue(IsProgressBarVisibleProperty, value);
    }

    public GrowlLevel Level
    {
        get => GetValue(LevelProperty);
        set => SetValue(LevelProperty, value);
    }

    public AvaloniaList<GrowlAction> Actions
    {
        get;
        set => SetAndRaise(ActionsProperty, ref field, value);
    } = [];

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public CancellationToken Token => _cts.Token;

    public event EventHandler<DismissRequestedEventArgs>? DismissRequested
    {
        add => AddHandler(DismissRequestedEvent, value);
        remove => RemoveHandler(DismissRequestedEvent, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == LevelProperty)
        {
            SetPseudoClass(change.NewValue switch
            {
                GrowlLevel.Information => ":information",
                GrowlLevel.Success => ":success",
                GrowlLevel.Warning => ":warning",
                GrowlLevel.Danger => ":danger",
                _ => ":information"
            });
        }

        if (change.Property == IsProgressBarVisibleProperty)
        {
            var visible = change.GetNewValue<bool>();

            PseudoClasses.Set(":progress", visible);
        }
    }

    public void Dismiss()
    {
        if (!_cts.IsCancellationRequested)
        {
            _cts.Cancel();
        }

        _dismissCommand.OnCanExecuteChanged();

        RaiseEvent(new DismissRequestedEventArgs(this));
    }

    private void SetPseudoClass(string name)
    {
        foreach (var i in (string[])[":information", ":success", ":warning", ":danger"])
        {
            PseudoClasses.Set(i, name == i);
        }
    }

    #region Nested type: DismissRequestedEventArgs

    public class DismissRequestedEventArgs(object? source = null) : RoutedEventArgs(DismissRequestedEvent, source);

    #endregion
}
