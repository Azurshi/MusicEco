using CommunityToolkit.Mvvm.Input;
using Domain.DataAccess;
using Domain.EventSystem;
using System.Diagnostics;

#if WINDOWS
using Windows.Storage.Pickers;
#elif ANDROID
using MusicEco.Platforms.Android;
#endif
namespace MusicEco.ViewModels.Pages;
public partial class SettingPageModel : PropertyObject, IServiceAccess {
    public SettingPageModel() {
    }

#if WINDOWS
    public static async Task<string?> OpenFolderPicker() {
        string? folderPath = null;
        var picker = new FolderPicker();
        var hwnd = ((MauiWinUIWindow)App.Current!.Windows[0].Handler.PlatformView!).WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var folder = await picker.PickSingleFolderAsync();
        folderPath = folder?.Path;
        return folderPath;
    }
    public static async Task<string?> OpenFilePicker() {
        string? filePath = null;
        var picker = new FileOpenPicker();
        var hwnd = ((MauiWinUIWindow)App.Current!.Windows[0].Handler.PlatformView!).WindowHandle;
        foreach (string fileType in DataStorage.Config.AudioFileExtensions) {
            picker.FileTypeFilter.Add(fileType);
        }
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var file = await picker.PickSingleFileAsync();
        filePath = file?.Path;
        return filePath;
    }
#elif ANDROID
    public static async Task<string?> OpenFolderPicker() {
        var intent = new Android.Content.Intent(Android.Content.Intent.ActionOpenDocumentTree);
        intent.AddFlags(Android.Content.ActivityFlags.GrantReadUriPermission);
        intent.AddFlags(Android.Content.ActivityFlags.GrantWriteUriPermission);
        intent.AddFlags(Android.Content.ActivityFlags.GrantPersistableUriPermission);
        var activity = Platform.CurrentActivity;
        activity!.StartActivityForResult(intent, 39);
        if (activity is MainActivity mainActivity) {
            mainActivity.waiting = true;
            while (mainActivity.waiting) {
                System.Diagnostics.Debug.WriteLine("Waiting");
                await Task.Delay(100);
            }
            if (mainActivity.folderScanUri != null) {
                var uri = mainActivity.folderScanUri;
                string path = UriUtility.GetItemPathFromUri(uri);
                UriUtility.Register(path, uri);
                return path;
            } else {
                return null;
            }
        }
        throw new Exception("Failed to process folder picker");
    }
#endif
    [RelayCommand]
    public async Task OpenScanPage() {
        await Utility.GoToAsync("scan", false);
    }
}
