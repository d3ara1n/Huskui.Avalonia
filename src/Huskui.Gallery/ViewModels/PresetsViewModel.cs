using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Huskui.Gallery.ViewModels;

public partial class PresetsViewModel : ObservableObject
{
    #region Reactive

    [ObservableProperty]
    public partial double Number { get; set; }

    #endregion
    
    #region Commands
    
    [RelayCommand]
    private void Increase()
    {
        Number++;
    }
    
    [RelayCommand]
    private void Decrease()
    {
        Number--;
    }

    #endregion
}