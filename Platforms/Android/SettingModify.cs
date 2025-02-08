using Android.Content;
using Android.Net;
using Uri = Android.Net.Uri;


namespace MusicEco.Platforms.Android;
public static partial class SettingModify {
    public static readonly int FileScanRequestCode = 39;
    public static async Task Scan() {
        Uri? uri = await OpenFolderPicker();
        AndroidMusicScanner.ScanAndPush(uri!);
    }
    public static async Task<Uri?> OpenFolderPicker() {
        var intent = new Intent(Intent.ActionOpenDocumentTree);
        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
        intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
        intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
        var activity = Platform.CurrentActivity;
        activity!.StartActivityForResult(intent, FileScanRequestCode);
        if (activity is MainActivity mainActivity) {
            mainActivity.scanCompleted = false;
            while (!mainActivity.scanCompleted) {
                System.Diagnostics.Debug.WriteLine("Waiting");
                await Task.Delay(100);
            }
            System.Diagnostics.Debug.WriteLine(mainActivity.folderScanPath);
            return mainActivity.folderScanUri;
        }
        throw new Exception("Failed to process folder picker");
    }   
}
