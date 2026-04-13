using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Avalonia.Styling;
using ColorCode;
using Huskui.Avalonia.Code.Highlighting;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Code.Controls;

[TemplatePart(PART_CodeText, typeof(TextBlock))]
public class CodeViewer : TemplatedControl
{
    public const string PART_CodeText = nameof(PART_CodeText);

    public static readonly StyledProperty<string> CodeProperty = AvaloniaProperty.Register<
        CodeViewer,
        string
    >(nameof(Code), string.Empty);

    public static readonly StyledProperty<InlineCollection?> InlinesProperty =
        AvaloniaProperty.Register<CodeViewer, InlineCollection?>(nameof(Inlines));

    public static readonly StyledProperty<string> LanguageProperty = AvaloniaProperty.Register<
        CodeViewer,
        string
    >(nameof(Language), "xml");

    public static readonly StyledProperty<bool> IsLineNumbersVisibleProperty =
        AvaloniaProperty.Register<CodeViewer, bool>(nameof(IsLineNumbersVisible), true);

    public static readonly StyledProperty<bool> IsCopyButtonVisibleProperty =
        AvaloniaProperty.Register<CodeViewer, bool>(nameof(IsCopyButtonVisible), true);

    public string Code
    {
        get => GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public InlineCollection? Inlines
    {
        get => GetValue(InlinesProperty);
        set => SetValue(InlinesProperty, value);
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

    private TextBlock? _codeTextBlock;
    private static readonly AvaloniaInlineFormatter LightFormatter = new(
        CodeViewerStyleDictionaries.Light
    );
    private static readonly AvaloniaInlineFormatter DarkFormatter = new(
        CodeViewerStyleDictionaries.Dark
    );

    private static ILanguage? ResolveLanguage(string language)
    {
        if (string.IsNullOrWhiteSpace(language))
            return null;

        var lang = Languages.FindById(language);
        if (lang is not null)
            return lang;

        foreach (var known in Languages.All)
        {
            if (known.HasAlias(language))
                return known;
        }

        return language.ToLowerInvariant() switch
        {
            "cs" or "csharp" => Languages.CSharp,
            "js" => Languages.JavaScript,
            "ts" or "tsx" => Languages.Typescript,
            "py" => Languages.Python,
            "xaml" or "axaml" or "html" => Languages.Xml,
            "fs" or "fsharp" => Languages.FSharp,
            "md" => Languages.Markdown,
            _ => null,
        };
    }

    private async Task CopyCode()
    {
        if (!string.IsNullOrEmpty(Code))
        {
            await TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(Code)!;
        }
    }

    public CodeViewer()
    {
        CopyCodeCommand = new InternalAsyncCommand(CopyCode);
        ActualThemeVariantChanged += SyncContentProxy;
    }

    ~CodeViewer()
    {
        ActualThemeVariantChanged -= SyncContentProxy;
    }

    private AvaloniaInlineFormatter GetFormatter() =>
        ActualThemeVariant == ThemeVariant.Dark ? DarkFormatter : LightFormatter;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _codeTextBlock = e.NameScope.Find<TextBlock>(PART_CodeText);

        SyncContent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (
            change.Property == CodeProperty
            || change.Property == InlinesProperty
            || change.Property == LanguageProperty
        )
        {
            SyncContent();
        }
    }

    private void SyncContentProxy(object? sender, EventArgs args) => SyncContent();

    private void SyncContent()
    {
        if (_codeTextBlock is null)
            return;

        if (Inlines is { Count: > 0 })
        {
            _codeTextBlock.Inlines = Inlines;
        }
        else if (!string.IsNullOrEmpty(Code))
        {
            var language = ResolveLanguage(Language);

            if (language is not null)
            {
                var inlines = GetFormatter().FormatInlines(Code, language);
                _codeTextBlock.Text = null;
                _codeTextBlock.Inlines = inlines;
            }
            else
            {
                _codeTextBlock.Inlines = null;
                _codeTextBlock.Text = Code;
            }
        }
        else
        {
            _codeTextBlock.Inlines = null;
            _codeTextBlock.Text = Code;
        }
    }
}
