using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;

namespace Huskui.Avalonia;

public class HuskuiTheme : Styles
{
    public static readonly StyledProperty<AccentColor> AccentProperty =
        AvaloniaProperty.Register<HuskuiTheme, AccentColor>(nameof(Accent),
                                                            defaultValue: AccentColor.Amber,
                                                            defaultBindingMode: BindingMode.OneTime);


    public HuskuiTheme()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public AccentColor Accent
    {
        get => GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AccentProperty)
        {
            var color = change.GetNewValue<AccentColor>();
            var source = $"avares://Huskui.Avalonia/Themes/Colors.Accent.{color}.axaml";
            Resources.MergedDictionaries[0] =
                new ResourceInclude(new Uri("avares://Huskui.Avalonia", UriKind.Absolute))
                {
                    Source = new Uri(source, UriKind.Absolute)
                };
        }
    }
}