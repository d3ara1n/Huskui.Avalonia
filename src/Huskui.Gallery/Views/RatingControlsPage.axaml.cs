using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Huskui.Gallery.Controls;

namespace Huskui.Gallery.Views;

public partial class RatingControlsPage : ControlPage
{
    public RatingControlsPage() => InitializeComponent();

    private void OnBasicRatingChanged(object? sender, RangeBaseValueChangedEventArgs e)
    {
        if (BasicValueText != null)
        {
            BasicValueText.Text = e.NewValue.ToString("0.#");
        }
    }

    private void OnResetBasicClick(object? sender, RoutedEventArgs e)
    {
        BasicRating.Value = 3;
    }

    private void OnSubmitReviewClick(object? sender, RoutedEventArgs e)
    {
        // In a real application, you would submit the review here
        // For demo purposes, we just reset the rating
        ReviewRating.Value = 0;
    }
}
