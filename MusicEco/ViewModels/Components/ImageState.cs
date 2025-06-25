namespace MusicEco.ViewModels.Components;
internal class ImageState {
    public ImageSource OnSource;
    public ImageSource OffSource;
    public bool IsOn;
    public ImageState(ImageSource onSource, ImageSource offSource, bool isOn = false) {
        OnSource = onSource;
        OffSource = offSource;
        IsOn = isOn;
    }
    public ImageState(string onPath, string offPath, bool isOn = false) {
        OnSource = ImageSource.FromFile(onPath);
        OffSource = ImageSource.FromFile(offPath);
        IsOn = isOn;
    }
    public ImageSource Source {
        get {
            if (IsOn) {
                return OnSource;
            }
            else {
                return OffSource;
            }
        }
    }
}