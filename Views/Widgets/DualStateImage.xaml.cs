
namespace MusicEco.Views.Widgets;

public partial class DualStateImage : ContentView
{
    public event EventHandler? Clicked;
    public event EventHandler? Enabled;
    public event EventHandler? Disabled;
	public ImageSource? EnabledImageSouce { get; set; }
	public ImageSource? DisabledImageSoure { get; set; }
    public bool AutoChangeState { get; set; } = true;
    private bool _enabled = false;
    public bool IsEnable {
        get => _enabled;
        set {
            _enabled = value;
            if (_enabled) {
                InnerImage.Source = EnabledImageSouce;
                Enabled?.Invoke(this, EventArgs.Empty);
            } else {
                InnerImage.Source = DisabledImageSoure;
                Disabled?.Invoke(this, EventArgs.Empty);
            }    
        }
    }
	public DualStateImage()
	{
		InitializeComponent();
    }
    //public void SetImageSourcePath(string enabledPath, string disabledPath) {
    //    EnabledImageSouce = ImageSource.FromFile(enabledPath);
    //    DisabledImageSoure = ImageSource.FromFile(disabledPath);
    //    IsEnable = _enabled;
    //}
    #region PrivateSignal
    private void OnPressed(object sender, EventArgs e) {
        InnerImage.Scale = 0.9;
        InnerImage.BackgroundColor = Colors.Transparent;
    }
    private void OnReleased(object sender, EventArgs e) {
        InnerImage.Scale = 1;
        InnerImage.BackgroundColor = Colors.LightBlue;
    }
    private void OnEntered(object sender, EventArgs e) {
        InnerImage.BackgroundColor = Colors.LightBlue;
    }
    private void OnExited(object sender, EventArgs e) {
        InnerImage.BackgroundColor = Colors.Transparent;
    }
    private void OnClicked(object sender, EventArgs e) {
        if (AutoChangeState) IsEnable = !_enabled;
        Clicked?.Invoke(this, e);
    }
    #endregion
}