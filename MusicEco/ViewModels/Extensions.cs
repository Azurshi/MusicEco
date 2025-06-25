using Domain.Models;

namespace MusicEco.ViewModels; 
//public interface IPlaylistAdd {
//    public Task AddToQueue(long songId, long queueId);
//    public Task AddToPlaylist(long songId, long playlistId);
//}

//public class PlaylistAddExtension: IServiceAccess {
//    public async Task AddToQueue(long songId, long queueId) {
//        await AddToPlaylist(songId, queueId);
//    }
//    public async Task AddToPlaylist(long songId, long playlistId) {
//        IPlaylistModel? playlistModel = IServiceAccess.ModelGetter.Playlist(playlistId);
//        ISongModel? song = IServiceAccess.ModelGetter.Song(songId);
//        if (playlistModel != null && song != null) {
//            playlistModel.AddSong(song);
//            playlistModel.Save();
//            await Task.CompletedTask;
//        }
//    }
//}
