using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input.Platform;
using Huskui.Avalonia.Models;

namespace Huskui.Avalonia.Controls;

[TemplatePart(PART_CodeText, typeof(SelectableTextBlock))]
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

    private SelectableTextBlock? _codeTextBlock;

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
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _codeTextBlock = e.NameScope.Find<SelectableTextBlock>(PART_CodeText);

        SyncContent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CodeProperty || change.Property == InlinesProperty)
        {
            SyncContent();
        }
    }

    private void SyncContent()
    {
        if (_codeTextBlock is null)
            return;


        if (Inlines is { Count: > 0 })
        {
            _codeTextBlock.Inlines = Inlines;
        }
        else
        {
            _codeTextBlock.Inlines = null;

            _codeTextBlock.Text = Code;
        }
    }
}
