using MusicEco.Views.Shell;
using Windows.Storage.Pickers;

namespace MusicEco;

public partial class ExplorerPicker {
    public static async Task<string?> PickFolder() {
        string? folderPath = null;
        var picker = new FolderPicker();
        var hwnd = ((MauiWinUIWindow)App.Current!.Windows[0].Handler.PlatformView!).WindowHandle;
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var folder = await picker.PickSingleFolderAsync();
        folderPath = folder?.Path;
        return folderPath;
    }
    public static async Task<string?> PickFile(List<string> extensions) {
        string? filePath = null;
        var picker = new FileOpenPicker();
        var hwnd = ((MauiWinUIWindow)App.Current!.Windows[0].Handler.PlatformView!).WindowHandle;
        foreach (string fileType in extensions) {
            picker.FileTypeFilter.Add(fileType);
        }
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);
        var file = await picker.PickSingleFileAsync();
        filePath = file?.Path;
        return filePath;
    }
    
}