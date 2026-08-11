using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.ViewModels.Items;
using MusicEco.ViewModels.Pages;
using MusicEco.Views.Overlays;
using System.Diagnostics.CodeAnalysis;

namespace MusicEco.Views.Commands;

public static class MenuCommands {
    private static bool TryGetHash(object? vm, [MaybeNullWhen(false)] out Hash256 hash) {
        if (vm is AudioEntryViewModel audioEntry) {
            hash = audioEntry.FileHash;
            return true;
        }
        else if (vm is FileEntryViewModel fileEntry) {
            hash = fileEntry.FileHash;
            return true;
        }
        else if (vm is HomePageViewModel homeVM) {
            hash = homeVM.FileHash;
            return true;
        }
        else {
            hash = default;
            return false;
        }
    }
    public static AsyncCommand<object> AddToQueueCommand { get; } = new(AddToQueue);
    private static async Task AddToQueue(object? vm) {
        if (TryGetHash(vm, out var hash)) {
            var provider = AppLifeCycle.Provider;
            var overlay = provider.GetRequiredService<IOverlayService>();
            var view = provider.GetRequiredService<AddToQueueOverlay>();
            overlay.ShowDynamic(new(0.5f, 0.9f), view);
            await view.Initialize(hash);
        }
    }
    public static AsyncCommand<object> AddToPlaylistCommand { get; } = new(AddToPlaylist);
    private static async Task AddToPlaylist(object? vm) {
        if (TryGetHash(vm, out var hash)) {
            var provider = AppLifeCycle.Provider;
            var overlay = provider.GetRequiredService<IOverlayService>();
            var view = provider.GetRequiredService<AddToPlaylistOverlay>();
            overlay.ShowDynamic(new(0.5f, 0.9f), view);
            await view.Initialize(hash);
        }
    }
    public static AsyncCommand<object> ShowAudioInfoCommand { get; } = new(ShowAudioInfo);
    private static async Task ShowAudioInfo(object? vm) {
        if (TryGetHash(vm, out var hash)) {
            var provider  = AppLifeCycle.Provider;
            var overlay = provider.GetRequiredService<IOverlayService>();
            var view = provider.GetRequiredService<AudioInfoOverlay>();
            overlay.ShowDynamic(new(0.5f, 0.9f), view);
            await view.Initialize(hash);
        }
    }
    private const string PathSeparator = "\\";
    public static AsyncCommand<object> PlayListCommand { get; } = new(PlayList);
    private static async Task PlayList(object? vm) {
        var provider = AppLifeCycle.Provider;
        var playbackService = provider.GetRequiredService<IPlaybackService>();
        if (vm is QueueItemViewModel queueVM) {
            var queueService = provider.GetRequiredService<IQueueService>();
            var queue = await queueService.Get(queueVM.CreationTime);
            if (queue != null && queue.Audios.Count > 0) {
                await playbackService.PlayQueue(queue, vm);
            }
        }
        else if (vm is PlaylistItemViewModel playlistVM) {
            var playlistService = provider.GetRequiredService<IPlaylistService>();
            var playlist = await playlistService.Get(playlistVM.CreationTime);
            if (playlist != null && playlist.Audios.Count > 0) {
                List<AudioEntry> audios = playlist.Audios.ToList();
                if (audios.Count > 0) {
                    string format = Localization.L["Queue_Template_Playlist"];
                    string queueName = string.Format(format, playlistVM.Name);
                    await playbackService.PlayQueue(queueName, audios, audios[0], vm);
                }
            }
        }
        else if (vm is FolderEntryViewModel folderVM) {
            var fileService = provider.GetRequiredService<IFileService>();
            var files = await fileService.Query(folderVM.Path);
            var rootPathLength = folderVM.Path.Length + PathSeparator.Length;
            List<AudioEntry> audios = [];
            foreach (var file in files) {
                string relPath = file.Path[rootPathLength..];
                if (!relPath.Contains(PathSeparator)) {
                    audios.Add(new(file.Hash, string.Empty));
                }
            }
            if (audios.Count > 0) {
                string format = Localization.L["Queue_Template_Folder"];
                string queueName = string.Format(format, folderVM.Name);
                await playbackService.PlayQueue(queueName, audios, audios[0], vm);
            }
        }
        else if (vm is AlbumViewModel albumVM) {
            var audioQueryService = provider.GetRequiredService<IAudioQueryService>();
            var album = await audioQueryService.GetAlbum(albumVM.Name);
            if (album != null) {
                List<AudioEntry> audios = album.Audios.ToList();
                if (audios.Count > 0) {
                    string format = Localization.L["Queue_Template_Album"];
                    string queueName = string.Format(format, albumVM.Name);
                    await playbackService.PlayQueue(queueName, audios, audios[0], vm);
                }
            }
        }
    }
}
