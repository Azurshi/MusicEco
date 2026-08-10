using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.ViewModels.Items;
using MusicEco.Views.Overlays;

namespace MusicEco.Views.Commands;

public static class MenuCommands {
    public static AsyncCommand<object> AddToQueueCommand { get; } = new(AddToQueue);
    private static async Task AddToQueue(object? vm) {
        Hash256 hash;
        if (vm is AudioEntryViewModel audioEntry) {
            hash = audioEntry.FileHash;
        }
        else if (vm is FileEntryViewModel fileEntry) {
            hash = fileEntry.FileHash;
        }
        else {
            return;
        }
        var provider = AppLifeCycle.Provider;
        await Task.Delay(500); // Await for previous overlay to close :XD
        var overlay = provider.GetRequiredService<IOverlayService>();
        var view = provider.GetRequiredService<AddToQueueOverlay>();
        overlay.ShowDynamic(new(0.5f, 0.9f), view);
        await view.Initialize(hash);
    }
}
