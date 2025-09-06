using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Huskui.Gallery.Controls;

/// <summary>
///     A container for displaying examples with title, description, and optional code
/// </summary>
public class ExampleContainer : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<ExampleContainer, string>(nameof(Title), string.Empty);

    public static readonly StyledProperty<string> DescriptionProperty =
        AvaloniaProperty.Register<ExampleContainer, string>(nameof(Description), string.Empty);

    public static readonly StyledProperty<string> XamlCodeProperty =
        AvaloniaProperty.Register<ExampleContainer, string>(nameof(XamlCode), string.Empty);

    public static readonly StyledProperty<string> CSharpCodeProperty =
        AvaloniaProperty.Register<ExampleContainer, string>(nameof(CSharpCode), string.Empty);

    public static readonly StyledProperty<bool> ShowCodeProperty =
        AvaloniaProperty.Register<ExampleContainer, bool>(nameof(ShowCode));

    public static readonly StyledProperty<object?> OptionsProperty =
        AvaloniaProperty.Register<ExampleContainer, object?>(nameof(Options));

    public static readonly StyledProperty<object?> ControlsProperty =
        AvaloniaProperty.Register<ExampleContainer, object?>(nameof(Controls));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string XamlCode
    {
        get => GetValue(XamlCodeProperty);
        set => SetValue(XamlCodeProperty, value);
    }

    public string CSharpCode
    {
        get => GetValue(CSharpCodeProperty);
        set => SetValue(CSharpCodeProperty, value);
    }

    public bool ShowCode
    {
        get => GetValue(ShowCodeProperty);
        set => SetValue(ShowCodeProperty, value);
    }

    public object? Options
    {
        get => GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    public object? Controls
    {
        get => GetValue(ControlsProperty);
        set => SetValue(ControlsProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Find and wire up enable toggle button
        if (e.NameScope.Find("PART_EnableToggle") is ToggleButton enableToggle)
        {
            enableToggle.IsCheckedChanged += (_, _) =>
            {
                if (e.NameScope.Find("PART_ContentContainer") is Control contentContainer)
                {
                    contentContainer.IsEnabled = !enableToggle.IsChecked ?? true;
                }
            };
        }

        // Find and wire up copy buttons
        if (e.NameScope.Find("PART_CopyXaml") is Button copyXaml)
        {
            copyXaml.Click += async (_, _) =>
            {
                if (!string.IsNullOrEmpty(XamlCode))
                {
                    await TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(XamlCode)!;
                }
            };
        }

        if (e.NameScope.Find("PART_CopyCSharp") is Button copyCSharp)
        {
            copyCSharp.Click += async (_, _) =>
            {
                if (!string.IsNullOrEmpty(CSharpCode))
                {
                    await TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(CSharpCode)!;
                }
            };
        }
    }
}
