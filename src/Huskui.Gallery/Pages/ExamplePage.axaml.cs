using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Huskui.Avalonia.Controls;

namespace Huskui.Gallery.Pages;

public partial class ExamplePage : Page
{
    public ExamplePage()
    {
        InitializeComponent();

        var bytes = new byte[3];
        Random.Shared.NextBytes(bytes);
        Background = new SolidColorBrush(Color.FromRgb(bytes[0], bytes[1], bytes[2]));
    }
}
