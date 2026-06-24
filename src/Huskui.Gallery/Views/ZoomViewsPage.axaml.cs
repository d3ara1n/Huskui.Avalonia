using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Layout;
using Avalonia.Media;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class ZoomViewsPage : ControlPage
{
    public ZoomViewsPage()
    {
        InitializeComponent();
        PrimaryZoomView.Content = CreateDemoCanvas();
        ImageZoomView.Content = CreatePatternCanvas();
    }

    private static Canvas CreateDemoCanvas()
    {
        var canvas = new Canvas { Width = 1200, Height = 900 };
        var colors = new[]
        {
            Color.FromRgb(0x4F, 0x8F, 0xF9), // blue
            Color.FromRgb(0xF9, 0x5A, 0x5A), // red
            Color.FromRgb(0x5A, 0xD9, 0x66), // green
            Color.FromRgb(0xF9, 0xD8, 0x5A), // yellow
            Color.FromRgb(0x9B, 0x72, 0xF9), // purple
            Color.FromRgb(0xF9, 0x94, 0x5A), // orange
            Color.FromRgb(0x5A, 0xE0, 0xE0), // teal
            Color.FromRgb(0xF9, 0x72, 0xB8), // pink
        };

        const int cols = 8;
        const int rows = 6;
        const double cellW = 140;
        const double cellH = 140;
        const double gap = 10;

        for (var r = 0; r < rows; r++)
        {
            for (var c = 0; c < cols; c++)
            {
                var color = colors[(r * cols + c) % colors.Length];
                var rect = new Rectangle
                {
                    Width = cellW,
                    Height = cellH,
                    Fill = new SolidColorBrush(color),
                    RadiusX = 8,
                    RadiusY = 8,
                };
                Canvas.SetLeft(rect, gap + c * (cellW + gap));
                Canvas.SetTop(rect, gap + r * (cellH + gap));
                canvas.Children.Add(rect);

                var label = new TextBlock
                {
                    Text = $"{color.R:X2}{color.G:X2}{color.B:X2}",
                    FontSize = 14,
                    FontWeight = FontWeight.Bold,
                    Foreground = Brushes.White,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = cellW,
                    Height = cellH,
                    TextAlignment = TextAlignment.Center,
                };
                Canvas.SetLeft(label, gap + c * (cellW + gap));
                Canvas.SetTop(label, gap + r * (cellH + gap));
                canvas.Children.Add(label);
            }
        }

        return canvas;
    }

    private static Canvas CreatePatternCanvas()
    {
        var canvas = new Canvas { Width = 800, Height = 600, Background = Brushes.Transparent };

        // concentric gradient of circles
        for (var i = 0; i < 30; i++)
        {
            var t = i / 29.0;
            var r = (byte)(0x40 + t * 0xBF);
            var g = (byte)(0x80 - t * 0x40);
            var b = (byte)(0xF0 - t * 0x80);
            var size = 600 - i * 20;
            var ellipse = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = new SolidColorBrush(Color.FromRgb(r, g, b)),
                Stroke = new SolidColorBrush(Color.FromRgb((byte)(r + 40), (byte)(g + 20), (byte)(b + 20))),
                StrokeThickness = 1,
            };
            Canvas.SetLeft(ellipse, (800 - size) / 2);
            Canvas.SetTop(ellipse, (600 - size) / 2);
            canvas.Children.Add(ellipse);
        }

        return canvas;
    }
}
