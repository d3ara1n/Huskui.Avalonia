using System.ComponentModel;

namespace Huskui.Avalonia.Models;

public class RatingStar(int index) : INotifyPropertyChanged
{
    public int Index { get; } = index;

    public RatingStarFillState FillState
    {
        get;
        set
        {
            if (field == value)
                return;
            field = value;
            PropertyChanged?.Invoke(this, new(nameof(FillState)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
