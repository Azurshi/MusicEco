using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MusicEco;

using Uri = Android.Net.Uri;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity: MauiAppCompatActivity {
    public TaskCompletionSource<Uri?>? FolderPickerTcs;
    protected override void OnCreate(Bundle? savedInstanceState) {
        base.OnCreate(savedInstanceState);
    }
    public async Task<Uri?> FolderPickerInternal() {
        FolderPickerTcs = new();
        var intent = new Intent(Intent.ActionOpenDocumentTree);
        intent.AddFlags(ActivityFlags.GrantReadUriPermission);
        intent.AddFlags(ActivityFlags.GrantWriteUriPermission);
        intent.AddFlags(ActivityFlags.GrantPersistableUriPermission);
        StartActivityForResult(intent, 39);
        return await FolderPickerTcs.Task;
    }
    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data) {
        base.OnActivityResult(requestCode, resultCode, data);
        if (requestCode == 39 && FolderPickerTcs != null) {
            if (resultCode == Result.Ok) {
                FolderPickerTcs.TrySetResult(data?.Data);
            }
            else {
                FolderPickerTcs.TrySetResult(null);
            }
        }
    }
}