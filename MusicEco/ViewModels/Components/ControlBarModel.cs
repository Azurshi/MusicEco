using CommunityToolkit.Mvvm.Input;
using Domain.EventSystem;
using Domain.Models;
using System.Diagnostics;

namespace MusicEco.ViewModels.Components;
public partial class ControlBarModel : PropertyObject, IServiceAccess {
    #region EventArgs
    public class FavouriteChangedEventArgs(bool state, long songId): EventArgs {
        public bool State { get; set; } = state;
        public long SongId { get; set; } = songId;
    }

    #endregion
    private readonly Dictionary<string, ImageState> imageStates = [];
    private void Initialize() {
        // Save and load state here
        GlobalData.IsPlaying = false;
        imageStates[nameof(FavouriteImage)] = new("favourite.png", "no_favourite.png");
        imageStates[nameof(PlayImage)] = new("pause.png", "play.png", GlobalData.IsPlaying);
        imageStates[nameof(ShuffleImage)] = new("shuffle_on.png", "no_shuffle.png", GlobalData.IsShuffle);
        imageStates[nameof(RepeatImage)] = new("repeat_on.png", "no_repeat.png", GlobalData.IsRepeat);
    }
    private readonly ImageSource volumeImage = ImageSource.FromFile("volume_on.png");
    private readonly ImageSource forwardImage = ImageSource.FromFile("forward.png");
    private readonly ImageSource nextImage = ImageSource.FromFile("next.png");
    #region Binding
    public ImageSource FavouriteImage => imageStates[nameof(FavouriteImage)].Source;
    public ImageSource VolumeImage => volumeImage;
    public ImageSource PlayImage => imageStates[nameof(PlayImage)].Source;
    public ImageSource PreviousImage => nextImage;
    public ImageSource BackwardImage => forwardImage;
    public ImageSource ForwardImage => forwardImage;
    public ImageSource NextImage => nextImage;
    public ImageSource RepeatImage => imageStates[nameof(RepeatImage)].Source;
    public ImageSource ShuffleImage => imageStates[nameof(ShuffleImage)].Source;
    public float PlayerProgress {
        get {
            //Debug.WriteLine(GlobalData.PlayerProgress);
            return GlobalData.PlayerProgress;
        }
        set {
            if (GlobalData.PlayerProgress != value) {
                Debug.WriteLine($"Set percent {value}");
                MusicPlayer.SeekTo(value).GetAwaiter().GetResult();
            }
        }
    }
    public float PlayerVolume {
        get => GlobalData.PlayerVolume;
        set {
            if (GlobalData.PlayerVolume != value) {
                Debug.WriteLine($"Set volume {value}");
                MusicPlayer.SetVolume(value);
            }
        }
    }
    #endregion
    public ControlBarModel() {
        Initialize();
        Debug.WriteLine($"~~~~~ Create new instance of {nameof(ControlBarModel)}");
        EventSystem.Connect<PlayerPlayingChangedEventArgs>((s, e) => {
            Toggle(nameof(PlayImage), e.IsPlaying);
        });
        EventSystem.Connect<PlayerIsShuffleChangedEventArgs>((s, e) => {
            Toggle(nameof(ShuffleImage), e.IsShuffle);
        });
        EventSystem.Connect<PlayerIsRepeatChangedEventArgs>((s, e) => {
            Toggle(nameof(RepeatImage), e.IsRepeat);
        });
        EventSystem.Connect<PlayerProgressChangedEventArgs>((s, e) => {
            OnPropertyChanged(nameof(PlayerProgress));
        });
        EventSystem.Connect<PlayingSongChangedEventArgs>((s, e) => {
            ISongModel? model = IServiceAccess.ModelGetter.Song(e.SongId);
            if (model != null) {
                imageStates[nameof(FavouriteImage)].IsOn = model.Favourite;
                OnPropertyChanged(nameof(FavouriteImage));
            }
        });
        EventSystem.Connect<FavouriteChangedEventArgs>((s, e) => {
            ISongModel? model = IServiceAccess.ModelGetter.Song(e.SongId);
            if (model != null) {
                imageStates[nameof(FavouriteImage)].IsOn = model.Favourite;
                OnPropertyChanged(nameof(FavouriteImage));
            }
        });
    }
    private bool Toggle(string key, bool? inputValue = null) {
        bool value = inputValue ?? !imageStates[key].IsOn;
        imageStates[key].IsOn = value;
        OnPropertyChanged(key);
        return value;
    }
    #region Command
    [RelayCommand]
    private void PlayChange() {
        MusicPlayer.PauseResume();
    }
    [RelayCommand]
    private void FavouriteChange() {
        Toggle(nameof(FavouriteImage));
        IServiceAccess.Service.GetRequiredService<GlobalModel>().Favourite(GlobalData.PlayingSongId);
        OnPropertyChanged(nameof(FavouriteImage));
    }
    [RelayCommand]
    private void VolumeChange() {
        OnPropertyChanged(nameof(PlayerVolume));
    }
    [RelayCommand]
    private void PreviousChange() {
        long currentSongId = GlobalData.PlayingSongId;
        MusicPlayer.PlayPrevious();
    }
    [RelayCommand]
    private async Task BackwardChange() {
        await MusicPlayer.Backward(30);
    }
    [RelayCommand]
    private async Task ForwardChange() {
        await MusicPlayer.Forward(30);
    }
    [RelayCommand]
    private void NextChange() {
        long currentSongId = GlobalData.PlayingSongId;
        MusicPlayer.PlayNext();
    }
    [RelayCommand]
    private void RepeatChange() {
        GlobalData.IsRepeat = !GlobalData.IsRepeat;
    }
    [RelayCommand]
    private void ShuffleChange() {
        GlobalData.IsShuffle = !GlobalData.IsShuffle;
    }
    #endregion
}


