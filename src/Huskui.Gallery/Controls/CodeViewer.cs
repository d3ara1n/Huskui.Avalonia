using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Huskui.Gallery.Controls;

/// <summary>
/// A control for displaying syntax-highlighted code
/// </summary>
public class CodeViewer : TemplatedControl
{
    public static readonly StyledProperty<string> CodeProperty =
        AvaloniaProperty.Register<CodeViewer, string>(nameof(Code), string.Empty);

    public static readonly StyledProperty<string> LanguageProperty =
        AvaloniaProperty.Register<CodeViewer, string>(nameof(Language), "xml");

    public static readonly StyledProperty<bool> ShowLineNumbersProperty =
        AvaloniaProperty.Register<CodeViewer, bool>(nameof(ShowLineNumbers), true);

    public static readonly StyledProperty<bool> ShowCopyButtonProperty =
        AvaloniaProperty.Register<CodeViewer, bool>(nameof(ShowCopyButton), true);

    public string Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public string Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public bool ShowLineNumbers
    {
        get => GetValue(ShowLineNumbersProperty);
        set => SetValue(ShowLineNumbersProperty, value);
    }

    public bool ShowCopyButton
    {
        get => GetValue(ShowCopyButtonProperty);
        set => SetValue(ShowCopyButtonProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Wire up copy button
        if (e.NameScope.Find("PART_CopyButton") is Button copyButton)
        {
            copyButton.Click += async (_, _) =>
            {
                if (!string.IsNullOrEmpty(Code))
                {
                    await TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(Code)!;
                }
            };
        }

        // Update code display
        UpdateCodeDisplay(e.NameScope);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CodeProperty)
        {
            UpdateCodeDisplay(null);
        }
    }

    private void UpdateCodeDisplay(INameScope? nameScope)
    {
        nameScope ??= this.FindNameScope();
        if (nameScope?.Find("PART_CodeText") is TextBlock codeText)
        {
            codeText.Text = Code;
            codeText.FontFamily = new("Consolas,Monaco,Menlo,monospace");
        }
    }
}
