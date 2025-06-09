using Avalonia;
using Avalonia.Controls;

namespace Huskui.Gallery.Controls;

public class ControlPresenter : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ControlPresenter, string>(nameof(Title));

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<ControlPresenter, string>(nameof(Subtitle));

    public static readonly StyledProperty<object?> OperationProperty =
        AvaloniaProperty.Register<ControlPresenter, object?>(nameof(Operation));

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

    public object? Operation
    {
        get => GetValue(OperationProperty);
        set => SetValue(OperationProperty, value);
    }
}