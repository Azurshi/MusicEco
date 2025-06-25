using System.Diagnostics;

namespace MusicEco.ViewModels;
public static class Utility {
    public static bool Busy = false;
    public static async Task GoToAsync(string topRoute, bool prefix = true) {
        while (Busy) {
            Debug.WriteLine("BUSY");
            await Task.Delay(10);
        }
        Busy = true;
        if (prefix) {
            await Shell.Current.GoToAsync($"//{topRoute}");
        }
        else {
            await Shell.Current.GoToAsync(topRoute);
        }
        Busy = false;
    }
    public static async Task GoToAsync(string route, long id) {
        while (Busy) {
            Debug.WriteLine("BUSY");
            await Task.Delay(10);
        }
        Busy = true;
        await Shell.Current.GoToAsync($"{route}?id={id}");
        Busy = false;
    }
    public static async Task GoToAsync(string route, string name) {
        while (Busy) {
            Debug.WriteLine("BUSY");
            await Task.Delay(10);
        }
        Busy = true;
        await Shell.Current.GoToAsync($"{route}?name={name}");
        Busy = false;
    }
    /// <summary>
    /// Go back
    /// </summary>
    /// <returns></returns>
    public static async Task GoToAsync() {
        while (Busy) {
            Debug.WriteLine("BUSY");
            await Task.Delay(10);
        }
        Busy = true;
        await Shell.Current.GoToAsync("..");
        Busy = false;
    }
}
