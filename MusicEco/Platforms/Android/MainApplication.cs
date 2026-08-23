using Android.App;
using Android.Runtime;
using MusicEco.Platform;

namespace MusicEco;

[Application]
public class MainApplication: MauiApplication {
    public MainApplication(IntPtr handle, JniHandleOwnership ownership)
        : base(handle, ownership) {
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}

public partial class ExplorerPicker {
    public static async Task<string?> PickFolder() {
        var activity = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity as MainActivity ?? throw new Exception("Main activity not found");
        var uri = await activity.FolderPickerInternal();
        if (uri == null || uri.Path == null) {
            return null;
        }
        else {
            string? path = uri.ToString();
            if (path == null) {
                return null;
            }
            UriUtility.Register(path, uri);
            return path;
        }
    }
}
