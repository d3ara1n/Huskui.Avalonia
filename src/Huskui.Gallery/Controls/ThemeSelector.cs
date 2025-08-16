using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Styling;
using Huskui.Avalonia;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.Controls;

/// <summary>
/// A control for selecting themes and accent colors
/// </summary>
public class ThemeSelector : TemplatedControl
{
    private IThemeService? _themeService;
    private ComboBox? _themeComboBox;
    private ComboBox? _accentComboBox;
    private ComboBox? _cornerComboBox;

    public static readonly StyledProperty<IThemeService?> ThemeServiceProperty =
        AvaloniaProperty.Register<ThemeSelector, IThemeService?>(nameof(ThemeService));

    public IThemeService? ThemeService
    {
        get => GetValue(ThemeServiceProperty);
        set => SetValue(ThemeServiceProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _themeComboBox = e.NameScope.Find("PART_ThemeComboBox") as ComboBox;
        _accentComboBox = e.NameScope.Find("PART_AccentComboBox") as ComboBox;
        _cornerComboBox = e.NameScope.Find("PART_CornerComboBox") as ComboBox;

        SetupThemeComboBox();
        SetupAccentComboBox();
        SetupCornerComboBox();
        UpdateCurrentSelections();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ThemeServiceProperty)
        {
            if (_themeService != null)
            {
                _themeService.ThemeChanged -= OnThemeChanged;
            }

            _themeService = change.NewValue as IThemeService;

            if (_themeService != null)
            {
                _themeService.ThemeChanged += OnThemeChanged;
                UpdateCurrentSelections();
            }
        }
    }

    private void SetupThemeComboBox()
    {
        if (_themeComboBox == null) return;

        var themes = new List<ThemeItem>
        {
            new("System", ThemeVariant.Default),
            new("Light", ThemeVariant.Light),
            new("Dark", ThemeVariant.Dark)
        };

        _themeComboBox.ItemsSource = themes;
        _themeComboBox.SelectionChanged += (_, _) =>
        {
            if (_themeComboBox.SelectedItem is ThemeItem item && _themeService != null)
            {
                _themeService.SetTheme(item.Variant);
            }
        };
    }

    private void SetupAccentComboBox()
    {
        if (_accentComboBox == null) return;

        var accents = Enum.GetValues<AccentColor>()
            .Select(a => new AccentItem(a.ToString(), a))
            .ToList();

        _accentComboBox.ItemsSource = accents;
        _accentComboBox.SelectionChanged += (_, _) =>
        {
            if (_accentComboBox.SelectedItem is AccentItem item && _themeService != null)
            {
                _themeService.SetAccent(item.Color);
            }
        };
    }

    private void SetupCornerComboBox()
    {
        if (_cornerComboBox == null) return;

        var corners = Enum.GetValues<CornerStyle>()
            .Select(c => new CornerItem(c.ToString(), c))
            .ToList();

        _cornerComboBox.ItemsSource = corners;
        _cornerComboBox.SelectionChanged += (_, _) =>
        {
            if (_cornerComboBox.SelectedItem is CornerItem item && _themeService != null)
            {
                _themeService.SetCorner(item.Style);
            }
        };
    }

    private void UpdateCurrentSelections()
    {
        if (_themeService == null) return;

        // Update theme selection
        if (_themeComboBox?.ItemsSource is IEnumerable<ThemeItem> themes)
        {
            var currentTheme = themes.FirstOrDefault(t => t.Variant == _themeService.CurrentTheme);
            _themeComboBox.SelectedItem = currentTheme;
        }

        // Update accent selection
        if (_accentComboBox?.ItemsSource is IEnumerable<AccentItem> accents)
        {
            var currentAccent = accents.FirstOrDefault(a => a.Color == _themeService.CurrentAccent);
            _accentComboBox.SelectedItem = currentAccent;
        }

        // Update corner selection
        if (_cornerComboBox?.ItemsSource is IEnumerable<CornerItem> corners)
        {
            var currentCorner = corners.FirstOrDefault(c => c.Style == _themeService.CurrentCorner);
            _cornerComboBox.SelectedItem = currentCorner;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e)
    {
        UpdateCurrentSelections();
    }

    private record ThemeItem(string Name, ThemeVariant Variant);
    private record AccentItem(string Name, AccentColor Color);
    private record CornerItem(string Name, CornerStyle Style);
}
