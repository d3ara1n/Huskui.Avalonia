using System.Text;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Styling;
using Huskui.Avalonia.Code.Highlighting;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Code.Controls;

[TemplatePart(PART_CodeText, typeof(TextBlock))]
[TemplatePart(PART_LineNumbers, typeof(TextBlock))]
public class CodeViewer : TemplatedControl
{
    public const string PART_CodeText = nameof(PART_CodeText);
    public const string PART_LineNumbers = nameof(PART_LineNumbers);

    public static readonly StyledProperty<string> CodeProperty = AvaloniaProperty.Register<
        CodeViewer,
        string
    >(nameof(Code), string.Empty);

    public static readonly StyledProperty<string> LanguageProperty = AvaloniaProperty.Register<
        CodeViewer,
        string
    >(nameof(Language), "xml");

    public static readonly StyledProperty<bool> IsLineNumbersVisibleProperty =
        AvaloniaProperty.Register<CodeViewer, bool>(nameof(IsLineNumbersVisible), true);

    public static readonly StyledProperty<bool> IsCopyButtonVisibleProperty =
        AvaloniaProperty.Register<CodeViewer, bool>(nameof(IsCopyButtonVisible), true);

    private static readonly CodeViewerRegistryOptions RegistryOptions = new();
    private static readonly TextMateInlineFormatter InlineFormatter = new(RegistryOptions);

    private TextBlock? _codeTextBlock;
    private TextBlock? _lineNumbersTextBlock;

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

    public bool IsLineNumbersVisible
    {
        get => GetValue(IsLineNumbersVisibleProperty);
        set => SetValue(IsLineNumbersVisibleProperty, value);
    }

    public bool IsCopyButtonVisible
    {
        get => GetValue(IsCopyButtonVisibleProperty);
        set => SetValue(IsCopyButtonVisibleProperty, value);
    }

    public ICommand CopyCodeCommand { get; }

    public CodeViewer()
    {
        CopyCodeCommand = new InternalAsyncCommand(CopyCode);
        ActualThemeVariantChanged += SyncContentProxy;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _codeTextBlock = e.NameScope.Find<TextBlock>(PART_CodeText);
        _lineNumbersTextBlock = e.NameScope.Find<TextBlock>(PART_LineNumbers);

        SyncContent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (
            change.Property == CodeProperty
            || change.Property == LanguageProperty
            || change.Property == IsLineNumbersVisibleProperty
        )
        {
            SyncContent();
        }
    }

    private async Task CopyCode()
    {
        var text = Code;
        if (!string.IsNullOrEmpty(text))
        {
            var task = TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(text);
            if (task != null)
                await task;
        }
    }

    private void SyncContentProxy(object? sender, EventArgs args) => SyncContent();

    private void SyncContent()
    {
        if (_codeTextBlock is null || _lineNumbersTextBlock is null)
            return;

        var text = Code;

        if (IsLineNumbersVisible)
        {
            var lineNumbers = BuildLineNumbers(text);
            _lineNumbersTextBlock.Text = lineNumbers;
        }

        var scopeName = RegistryOptions.ResolveScopeName(Language);
        if (string.IsNullOrWhiteSpace(scopeName) || string.IsNullOrEmpty(text))
        {
            _codeTextBlock.Inlines = null;
            _codeTextBlock.Text = text;
            return;
        }

        var theme =
            ActualThemeVariant == ThemeVariant.Dark
                ? CodeViewerTextMateThemes.Dark
                : CodeViewerTextMateThemes.Light;

        var inlines = InlineFormatter.FormatInlines(text, scopeName, theme);
        if (inlines is { Count: > 0 })
        {
            _codeTextBlock.Text = null;
            _codeTextBlock.Inlines = inlines;
            return;
        }

        _codeTextBlock.Inlines = null;
        _codeTextBlock.Text = text;
    }

    private static string BuildLineNumbers(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        var normalized = NormalizeLineEndings(text);
        var lineCount = 1;

        foreach (var ch in normalized)
        {
            if (ch == '\n')
                lineCount++;
        }

        var builder = new StringBuilder();
        for (var i = 1; i <= lineCount; i++)
        {
            if (i > 1)
                builder.AppendLine();

            builder.Append(i);
        }

        return builder.ToString();
    }

    private static string NormalizeLineEndings(string text) =>
        text.Replace("\r\n", "\n").Replace('\r', '\n');
}
