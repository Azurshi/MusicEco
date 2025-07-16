namespace MusicEco.ViewModels;
public static class Navigator {
    private static SemaphoreSlim semaphore = new (1);
    public static async Task GoToAsync (string route) {
        await semaphore.WaitAsync();
        try {
            await Shell.Current.GoToAsync(route);
        }
        finally {
            semaphore.Release();
        }
    }
    public static async Task GoToAsync(string route, long id) {
        await GoToAsync($"{route}?id={id}");
    }
    public static async Task GoToAsync(string route, string name) {
        name = Uri.EscapeDataString(name);
        await GoToAsync($"{route}?name={name}");
    }
}
