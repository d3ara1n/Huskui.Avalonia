using System.Collections.Generic;
using Avalonia;
using Avalonia.Interactivity;
using Huskui.Avalonia.Transitions;
using Huskui.Gallery.Controls;
using Huskui.Gallery.Models;
using Huskui.Gallery.Pages;

namespace Huskui.Gallery.Views;

public partial class FramesPage : ControlPage
{
    public static readonly DirectProperty<FramesPage, TransitionInfo> SelectedTransitionInfoProperty =
        AvaloniaProperty.RegisterDirect<FramesPage, TransitionInfo>(nameof(SelectedTransitionInfo),
                                                                    o => o.SelectedTransitionInfo,
                                                                    (o, v) => o.SelectedTransitionInfo = v);

    public FramesPage()
    {
        InitializeComponent();

        SelectedTransitionInfo = TransitionInfos[0];
    }

    public IReadOnlyList<TransitionInfo> TransitionInfos { get; } =
    [
        new(new PageCoverOverTransition()),
        new(new FocusOnTransition()),
        new(new HookUpTransition()),
        new(new PopUpTransition())
    ];

    public TransitionInfo SelectedTransitionInfo
    {
        get;
        set => SetAndRaise(SelectedTransitionInfoProperty, ref field, value);
    }

    private void NextButton_OnClick(object? sender, RoutedEventArgs e) =>
        Root.Navigate(typeof(ExamplePage), Root.HistoryCount, SelectedTransitionInfo.Transition);

    private void BackButton_OnClick(object? sender, RoutedEventArgs e) => Root.GoBack();
}
