using Windows.Storage.Pickers;
using MusicEco.Common;


namespace MusicEco.Platforms.Windows;
public static partial class SettingModify {
    public static async Task Scan() {
        string? folderPath = await OpenFolderPicker();
        if (folderPath != null) {
            WindowMusicScanner.ScanAndPush(folderPath);
        }
    }
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
        foreach (string fileType in Setting.SupportedExtension) {
            picker.FileTypeFilter.Add(fileType);
        }
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var file = await picker.PickSingleFileAsync();
        filePath = file?.Path;
        return filePath;
    }
}