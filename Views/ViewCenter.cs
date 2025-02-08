using MusicEco.Common.Events;
using MusicEco.Common.Value;
using MusicEco.Global;
using MusicEco.Models;

namespace MusicEco.Views;
public static class ViewCenter {
    public static void AddOrUpdateQueue(string queueName, int targetSongId, List<int> songIds, bool play = true) {
        PlaylistModel? model = PlaylistModel.GetAll<PlaylistModel>()
            .Where(e => e.Name == queueName && e.Type == Data.Playlist_QueueType)
            .FirstOrDefault();
        if (model == null) {
            model = new();
            model.Name = queueName;
            model.AssignId();
            model.Type = Data.Playlist_QueueType;
        }
        model.SongIds = songIds;
        model.CurrentSongId = targetSongId;
        model.Save();

        if (play) {
            MusicPlayer.PlayAudio(targetSongId, model.Id);
        }
    }
    public static void PlayAudio(string queueName, int songId) {
        PlaylistModel? model = PlaylistModel.GetAll<PlaylistModel>()
            .Where(e => e.Name == queueName && e.Type == Data.Playlist_QueueType)
            .FirstOrDefault() 
            ?? throw new KeyNotFoundException("Queue does not exists");
        model.CurrentSongId = songId;
        model.Save();

        MusicPlayer.PlayAudio(songId, model.Id);
    }
    public static void PlayAudio(int queuId, int songId) {
        PlaylistModel? model = PlaylistModel.Get(queuId);
        if (model != null) {
            model.CurrentSongId = songId;
            model.Save();
            MusicPlayer.PlayAudio(songId, model.Id);
        } else {
            throw new KeyNotFoundException("Queue id not found");
        }

    }
}