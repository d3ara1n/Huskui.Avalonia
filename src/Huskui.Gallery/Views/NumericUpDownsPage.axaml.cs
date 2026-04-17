using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class NumericUpDownsPage : ControlPage
{
    public NumericUpDownsPage()
    {
        InitializeComponent();

        DefaultNumericUpDown.PropertyChanged += OnBasicNumericUpDownPropertyChanged;
        RangeNumericUpDown.PropertyChanged += OnBasicNumericUpDownPropertyChanged;
        DecimalNumericUpDown.PropertyChanged += OnBasicNumericUpDownPropertyChanged;
        ClippedNumericUpDown.PropertyChanged += OnBasicNumericUpDownPropertyChanged;

        NullableNumericUpDown.PropertyChanged += OnBehaviorNumericUpDownPropertyChanged;
        CurrencyNumericUpDown.PropertyChanged += OnBehaviorNumericUpDownPropertyChanged;

        SeatsNumericUpDown.PropertyChanged += OnPricingNumericUpDownPropertyChanged;
        UsageNumericUpDown.PropertyChanged += OnPricingNumericUpDownPropertyChanged;
        DiscountNumericUpDown.PropertyChanged += OnPricingNumericUpDownPropertyChanged;

        ResetBasicValues();
        ResetBehaviorValues();
        ResetPricingValues();
    }

    private void OnClearValuesClick(object? sender, RoutedEventArgs e) =>
        SetBasicValues(null, null, null, null);

    private void OnResetValuesClick(object? sender, RoutedEventArgs e) => ResetBasicValues();

    private void OnSeedNullableClick(object? sender, RoutedEventArgs e)
    {
        NullableNumericUpDown.Value = 18;
        CurrencyNumericUpDown.Value = 5250;
    }

    private void OnResetBehaviorClick(object? sender, RoutedEventArgs e) => ResetBehaviorValues();

    private void OnResetPricingClick(object? sender, RoutedEventArgs e) => ResetPricingValues();

    private void ResetBasicValues() => SetBasicValues(42m, 35m, 12.5m, 12m);

    private void SetBasicValues(
        decimal? defaultValue,
        decimal? rangeValue,
        decimal? decimalValue,
        decimal? wrapValue
    )
    {
        DefaultNumericUpDown.Value = defaultValue;
        RangeNumericUpDown.Value = rangeValue;
        DecimalNumericUpDown.Value = decimalValue;
        ClippedNumericUpDown.Value = wrapValue;
        UpdateBasicStateText();
    }

    private void ResetBehaviorValues()
    {
        NullableNumericUpDown.Value = null;
        CurrencyNumericUpDown.Value = 4250;
        UpdateBehaviorStateText();
    }

    private void ResetPricingValues()
    {
        SeatsNumericUpDown.Value = 12;
        UsageNumericUpDown.Value = 40;
        DiscountNumericUpDown.Value = 10;
        UpdatePricingSummary();
    }

    private void UpdateBasicStateText()
    {
        BasicStateText.Text =
            $"Default: {FormatNullable(DefaultNumericUpDown.Value)} | Range: {FormatNullable(RangeNumericUpDown.Value)} | Decimal: {FormatNullable(DecimalNumericUpDown.Value, "F2")} | Clipped: {FormatNullable(ClippedNumericUpDown.Value)}";
    }

    private void UpdateBehaviorStateText()
    {
        BehaviorStateText.Text =
            $"Nullable field: {FormatNullable(NullableNumericUpDown.Value)} | Budget: {FormatNullable(CurrencyNumericUpDown.Value, "F0")}";
    }

    private void UpdatePricingSummary()
    {
        var seats = Math.Max(1m, SeatsNumericUpDown.Value ?? 1m);
        var usage = Math.Max(0m, UsageNumericUpDown.Value ?? 0m);
        var discount = Math.Clamp(DiscountNumericUpDown.Value ?? 0m, 0m, 30m);

        const decimal seatPrice = 18m;
        const decimal usageUnitPrice = 6m;

        var seatSubtotal = seats * seatPrice;
        var usageSubtotal = usage * usageUnitPrice;
        var totalBeforeDiscount = seatSubtotal + usageSubtotal;
        var total = totalBeforeDiscount * (1 - discount / 100m);

        PricingSeatsText.Text = $"Seats: {seats:0} x ${seatPrice:0}/month = ${seatSubtotal:0}";
        PricingUsageText.Text =
            $"Usage: {usage:0}k calls x ${usageUnitPrice:0} = ${usageSubtotal:0} | Discount: {discount:0}%";
        PricingTotalText.Text = $"Estimated total: ${total:0}/month";
        PricingPreviewText.Text =
            $"{seats:0} seats with {usage:0}k monthly API calls results in an estimated ${total:0} monthly charge after {discount:0}% discount.";
    }

    private void OnBasicNumericUpDownPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (e.Property == NumericUpDown.ValueProperty)
        {
            UpdateBasicStateText();
        }
    }

    private void OnBehaviorNumericUpDownPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (e.Property == NumericUpDown.ValueProperty)
        {
            UpdateBehaviorStateText();
        }
    }

    private void OnPricingNumericUpDownPropertyChanged(
        object? sender,
        AvaloniaPropertyChangedEventArgs e
    )
    {
        if (e.Property == NumericUpDown.ValueProperty)
        {
            UpdatePricingSummary();
        }
    }

    private static string FormatNullable(decimal? value, string format = "F0") =>
        value is null ? "empty" : value.Value.ToString(format);
}
