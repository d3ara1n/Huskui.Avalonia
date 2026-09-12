using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Huskui.Avalonia;
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

    public static readonly StyledProperty<bool> ShowBackgroundSelectorProperty =
        AvaloniaProperty.Register<ThemeSelector, bool>(nameof(ShowBackgroundSelector), true);

    private ComboBox? _accentComboBox;
    private ComboBox? _backgroundComboBox;
    private ComboBox? _cornerComboBox;
    private ComboBox? _grayComboBox;
    private ComboBox? _paletteComboBox;
    private ComboBox? _themeComboBox;
    private IThemeService? _themeService;

    public IThemeService? ThemeService
    {
        get => GetValue(ThemeServiceProperty);
        set => SetValue(ThemeServiceProperty, value);
    }

    public bool ShowBackgroundSelector
    {
        get => GetValue(ShowBackgroundSelectorProperty);
        set => SetValue(ShowBackgroundSelectorProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _themeComboBox = e.NameScope.Find("PART_ThemeComboBox") as ComboBox;
        _paletteComboBox = e.NameScope.Find("PART_PaletteComboBox") as ComboBox;
        _accentComboBox = e.NameScope.Find("PART_AccentComboBox") as ComboBox;
        _grayComboBox = e.NameScope.Find("PART_GrayComboBox") as ComboBox;
        _cornerComboBox = e.NameScope.Find("PART_CornerComboBox") as ComboBox;
        _backgroundComboBox = e.NameScope.Find("PART_BackgroundComboBox") as ComboBox;

        SetupThemeComboBox();
        SetupPaletteComboBox();
        SetupAccentComboBox();
        SetupGrayComboBox();
        SetupCornerComboBox();
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

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        if (_themeService != null)
        {
            _themeService.ThemeChanged -= OnThemeChanged;
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

    private void SetupPaletteComboBox()
    {
        if (_paletteComboBox == null)
        {
            return;
        }

        _paletteComboBox.ItemsSource = PaletteItem.All;
        _paletteComboBox.SelectionChanged += (_, _) =>
        {
            if (_paletteComboBox.SelectedItem is PaletteItem item && _themeService != null)
            {
                _themeService.ApplyPalette(item.Palette);
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

    private void SetupGrayComboBox()
    {
        if (_grayComboBox == null)
        {
            return;
        }

        _grayComboBox.ItemsSource = GrayColorItem.All;
        _grayComboBox.SelectionChanged += (_, _) =>
        {
            if (_grayComboBox.SelectedItem is GrayColorItem item && _themeService != null)
            {
                _themeService.SetGray(item.Color);
            }
        };
    }

    private void SetupCornerComboBox()
    {
        if (_cornerComboBox == null)
        {
            return;
        }

        _cornerComboBox.ItemsSource = CornerStyleItem.All;
        _cornerComboBox.SelectionChanged += (_, _) =>
        {
            if (_cornerComboBox.SelectedItem is CornerStyleItem item && _themeService != null)
            {
                _themeService.SetCorner(item.Style);
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
            if (
                _backgroundComboBox.SelectedItem is BackgroundMaterialItem item
                && _themeService != null
            )
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
            var currentTheme = ThemeVariantItem.All.FirstOrDefault(t =>
                t.Variant == _themeService.CurrentTheme
            );
            _themeComboBox.SelectedItem = currentTheme;
        }

        // Update palette selection: highlighted only when gray and accent still match a preset
        if (_paletteComboBox != null)
        {
            var current = PaletteItem.All.FirstOrDefault(item =>
                item.Palette.Gray == _themeService.CurrentGray
                && item.Palette.Accent == _themeService.CurrentAccent
            );
            _paletteComboBox.SelectedItem = current;
        }

        // Update accent selection
        if (_accentComboBox != null)
        {
            var currentAccent = AccentColorItem.All.FirstOrDefault(a =>
                a.Color == _themeService.CurrentAccent
            );
            _accentComboBox.SelectedItem = currentAccent;
        }

        // Update gray selection
        if (_grayComboBox != null)
        {
            var currentGray = GrayColorItem.All.FirstOrDefault(g =>
                g.Color == _themeService.CurrentGray
            );
            _grayComboBox.SelectedItem = currentGray;
        }

        // Update corner selection
        if (_cornerComboBox != null)
        {
            var currentCorner = CornerStyleItem.All.FirstOrDefault(c =>
                c.Style == _themeService.CurrentCorner
            );
            _cornerComboBox.SelectedItem = currentCorner;
        }

        // Update background selection
        if (_backgroundComboBox != null)
        {
            var currentBackground = BackgroundMaterialItem.All.FirstOrDefault(b =>
                b.Material == _themeService.CurrentBackground
            );
            _backgroundComboBox.SelectedItem = currentBackground;
        }
    }

    private void OnThemeChanged(object? sender, EventArgs e) => UpdateCurrentSelections();

    // Records removed - now using user-friendly models from ThemeModels.cs
}
