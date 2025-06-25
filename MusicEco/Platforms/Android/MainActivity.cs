using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using static Android.Provider.Settings;

namespace MusicEco;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public bool waiting = false;
    public Android.Net.Uri? folderScanUri = null;
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data) {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 39) {
            System.Diagnostics.Debug.WriteLine(data);
            if (data != null) {
                folderScanUri = data!.Data!;
                var takeFlag = data.Flags & (ActivityFlags.GrantReadUriPermission | ActivityFlags.GrantWriteUriPermission);
                Platform.CurrentActivity!.ContentResolver!.TakePersistableUriPermission(folderScanUri!, takeFlag);
            }
            waiting = false;
        }
    }
}
