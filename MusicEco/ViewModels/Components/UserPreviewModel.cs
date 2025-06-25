using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;

namespace MusicEco.ViewModels.Components; 
public partial class UserPreviewModel: PropertyObject {
    public UserPreviewModel() {
        Debug.WriteLine($"~~~~~ Create new instance of {nameof(UserPreviewModel)}");
    }
    [RelayCommand]
    public async Task NavigateFavourite() {
        await Utility.GoToAsync("favourite", false);
    }
    [RelayCommand]
    public async Task NavigatePlaycount() {
        await Utility.GoToAsync("playcount", false);
    }
}
