﻿using System.Collections;
using Avalonia.Data.Converters;

namespace Huskui.Avalonia.Converters;

public static class CollectionConverters
{
    public static IValueConverter IsEmpty { get; } = new RelayConverter((v, _) => !IsObjectNotEmpty(v));
    public static IValueConverter IsNotEmpty { get; } = new RelayConverter((v, _) => IsObjectNotEmpty(v));

    private static bool IsObjectNotEmpty(object? value)
    {
        if (value is IEnumerable i)
            return i.Cast<object>().Any();

        return false;
    }
}