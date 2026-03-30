using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Huskui.Avalonia.Converters;

internal sealed class ResourceConverter(IValueConverter? converter, object? parameter) : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter2, CultureInfo culture)
    {
        if (converter is null)
            return values[0];

        return converter.Convert(values[0], targetType, parameter, culture);
    }
}
