using System;
using Avalonia;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Controls;

public class ControlPage : Page
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ControlPage, string>(nameof(Title));

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<ControlPage, string>(nameof(Subtitle));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    protected override Type StyleKeyOverride => typeof(ControlPage);
}
