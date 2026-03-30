using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Huskui.Avalonia.Converters;

namespace Huskui.Avalonia.MarkupExtensions;

public class DynamicResourceBindingExtension : MarkupExtension
{
    public string ResourceKey { get; }
    public IValueConverter? Converter { get; set; }
    public object? ConverterParameter { get; set; }

    public DynamicResourceBindingExtension(string resourceKey)
    {
        ResourceKey = resourceKey;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var dyExt = new DynamicResourceExtension(ResourceKey);
        var binding = dyExt.ProvideValue(serviceProvider);

        return new MultiBinding
        {
            Bindings = [binding],
            Converter = new ResourceConverter(Converter, ConverterParameter),
        };
    }
}
