using System;

namespace Huskui.Avalonia.Attributes;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class HuskuiExtensionAttribute(string bundleUri) : Attribute
{
    public string BundleUri { get; init; } = bundleUri;
}
