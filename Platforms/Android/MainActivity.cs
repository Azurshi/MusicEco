using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using MusicEco.Platforms.Android;

namespace MusicEco;
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity {
    public string folderScanPath = "null";
    public bool scanCompleted = false;
    public Android.Net.Uri? folderScanUri = null;
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data) {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == SettingModify.FileScanRequestCode) {
            System.Diagnostics.Debug.WriteLine(data);
            if (data != null) {
                folderScanUri = data!.Data!;
                if (folderScanUri != null) {
                    folderScanPath = UriUtility.GetItemPathFromUri(folderScanUri);
                }
                var takeFlag = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                Platform.CurrentActivity!.ContentResolver!.TakePersistableUriPermission(folderScanUri!, takeFlag);
            }
            scanCompleted = true;
        }
    }
}
