using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Huskui.Gallery.Models;
using Huskui.Gallery.Services;

namespace Huskui.Gallery.Controls;

/// <summary>
///     A control for selecting themes and accent colors
/// </summary>
public class ThemeSelector : TemplatedControl
{
    public static readonly StyledProperty<IThemeService?> ThemeServiceProperty =
        AvaloniaProperty.Register<ThemeSelector, IThemeService?>(nameof(ThemeService));

    private ComboBox? _accentComboBox;
    private ComboBox? _backgroundComboBox;
    private ComboBox? _themeComboBox;
    private IThemeService? _themeService;

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
        _backgroundComboBox = e.NameScope.Find("PART_BackgroundComboBox") as ComboBox;

        SetupThemeComboBox();
        SetupAccentComboBox();
        SetupBackgroundComboBox();
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
        if (_themeComboBox == null)
        {
            return;
        }

        _themeComboBox.ItemsSource = ThemeVariantItem.All;
        _themeComboBox.SelectionChanged += (_, _) =>
        {
            if (_themeComboBox.SelectedItem is ThemeVariantItem item && _themeService != null)
            {
                _themeService.SetTheme(item.Variant);
            }
        };
    }

    private void SetupAccentComboBox()
    {
        if (_accentComboBox == null)
        {
            return;
        }

        _accentComboBox.ItemsSource = AccentColorItem.All;
        _accentComboBox.SelectionChanged += (_, _) =>
        {
            if (_accentComboBox.SelectedItem is AccentColorItem item && _themeService != null)
            {
                _themeService.SetAccent(item.Color);
            }
        };
    }

    private void SetupBackgroundComboBox()
    {
        if (_backgroundComboBox == null)
        {
            return;
        }

        _backgroundComboBox.ItemsSource = BackgroundMaterialItem.All;
        _backgroundComboBox.SelectionChanged += (_, _) =>
        {
            if (_backgroundComboBox.SelectedItem is BackgroundMaterialItem item && _themeService != null)
            {
                _themeService.SetBackground(item.Material);
            }
        };
    }

    private void UpdateCurrentSelections()
    {
        if (_themeService == null)
        {
            return;
        }

        // Update theme selection
        if (_themeComboBox != null)
        {
            var currentTheme = ThemeVariantItem.All.FirstOrDefault(t => t.Variant == _themeService.CurrentTheme);
            _themeComboBox.SelectedItem = currentTheme;
        }

        // Update accent selection
        if (_accentComboBox != null)
        {
            var currentAccent = AccentColorItem.All.FirstOrDefault(a => a.Color == _themeService.CurrentAccent);
            _accentComboBox.SelectedItem = currentAccent;
        }

        // Update background selection
        if (_backgroundComboBox != null)
        {
            var currentBackground =
                BackgroundMaterialItem.All.FirstOrDefault(b => b.Material == _themeService.CurrentBackground);
            _backgroundComboBox.SelectedItem = currentBackground;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e) => UpdateCurrentSelections();

    // Records removed - now using user-friendly models from ThemeModels.cs
}
