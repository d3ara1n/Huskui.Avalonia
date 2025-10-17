using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using Avalonia.Styling;

namespace Huskui.Avalonia;

public class HuskuiTheme : Styles
{
    public static readonly StyledProperty<AccentColor> AccentProperty =
        AvaloniaProperty.Register<HuskuiTheme, AccentColor>(nameof(Accent));

    public static readonly StyledProperty<CornerStyle> CornerProperty =
        AvaloniaProperty.Register<HuskuiTheme, CornerStyle>(nameof(Corner),
                                                            CornerStyle.Normal,
                                                            defaultBindingMode: BindingMode.OneWay);


    public HuskuiTheme()
    {
        AvaloniaXamlLoader.Load(this);

        if (Accent == AccentColor.System)
        {
            Resources.MergedDictionaries[1] = GenerateSystemAccentColorResourceDictionary();
        }
    }

    public AccentColor Accent
    {
        get => GetValue(AccentProperty);
        set => SetValue(AccentProperty, value);
    }

    public CornerStyle Corner
    {
        get => GetValue(CornerProperty);
        set => SetValue(CornerProperty, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AccentProperty)
        {
            var color = change.GetNewValue<AccentColor>();

            var source = $"avares://Huskui.Avalonia/Themes/Colors.Accent.{color}.axaml";
            Resources.MergedDictionaries[1] = color is AccentColor.System
                                                  ? GenerateSystemAccentColorResourceDictionary()
                                                  : new ResourceInclude(new Uri("avares://Huskui.Avalonia",
                                                                                    UriKind.Absolute))
                                                  {
                                                      Source = new(source, UriKind.Absolute)
                                                  };
        }

        if (change.Property == CornerProperty)
        {
            var corner = change.GetNewValue<CornerStyle>();
            var source = $"avares://Huskui.Avalonia/Themes/CornerRadius.{corner}.axaml";
            Resources.MergedDictionaries[0] =
                new ResourceInclude(new Uri("avares://Huskui.Avalonia", UriKind.Absolute))
                {
                    Source = new(source, UriKind.Absolute)
                };
        }
    }

    private ResourceDictionary GenerateSystemAccentColorResourceDictionary()
    {
        var systemAccent = Application.Current is { PlatformSettings: { } platformSettings }
                               ? platformSettings.GetColorValues().AccentColor1
                               : Color.FromRgb(0x00, 0x90, 0xFF);

        var lightScale = RadixColorGenerator.GenerateLightScale(systemAccent);
        var darkScale = RadixColorGenerator.GenerateDarkScale(systemAccent);

        var systemColorDict = new ResourceDictionary();

        var lightDict = new ResourceDictionary();
        for (var i = 0; i < 12; i++)
        {
            lightDict.Add($"Accent{i + 1}Color", lightScale[i]);
        }

        systemColorDict.ThemeDictionaries.Add(ThemeVariant.Default, lightDict);

        var darkDict = new ResourceDictionary();
        for (var i = 0; i < 12; i++)
        {
            darkDict.Add($"Accent{i + 1}Color", darkScale[i]);
        }

        systemColorDict.ThemeDictionaries.Add(ThemeVariant.Dark, darkDict);

        return systemColorDict;
    }
}
