using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Huskui.Avalonia.Converters;

namespace Huskui.Avalonia.MarkupExtensions;

public class StaticResourceBindingExtension : MarkupExtension
{
    public string ResourceKey { get; }
    public IValueConverter? Converter { get; set; }
    public object? ConverterParameter { get; set; }

    public StaticResourceBindingExtension(string resourceKey)
    {
        ResourceKey = resourceKey;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var stExt = new StaticResourceExtension(ResourceKey);
        var binding = stExt.ProvideValue(serviceProvider);

        return new MultiBinding
        {
            Bindings = [(BindingBase)binding!],
            Converter = new ResourceConverter(Converter, ConverterParameter),
        };
    }
}
