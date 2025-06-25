using DataStorage.DataAccess;
using Domain.DataAccess;
using Domain.Models;

namespace MusicEco; 
public interface IServiceAccess {
    public static IServiceProvider? _service;
    protected static IServiceProvider Service => _service ?? throw new NullReferenceException();
    internal static readonly IModelGetter _modelGetter = new ModelGetter();
    protected static IModelGetter ModelGetter => _modelGetter;
    private static readonly IModelQuery _modelQuery = new ModelQuery();
    protected static IModelQuery ModelQuery => _modelQuery;
    private static readonly IDataGetter _dataGetter = new DataGetter();
    protected static IDataGetter DataGetter => _dataGetter;
    private static readonly IGlobalData _globalData = new DataStorage.DataAccess.GlobalData();
    protected static IGlobalData GlobalData => _globalData;

    private static void PlayTest() {
        IPlaylistModel? queue = _service?.GetService<IPlaylistModel>();
        if (queue != null) {
            queue.Type = DefaultValue.Queue;
            queue.AssignId();
            queue.Name = "Test";
            ISongModel? song = ModelGetter.Song(1746180791974484);
            if (song != null) {
                queue.AddSong(song);
                queue.Current = song;
                queue.Save();
                MusicEco.GlobalData.PlayingQueueId = queue.Id;
                MusicPlayer.Play();
            }
        }
    }
    protected static void PlayQueue(long songId, List<ISongModel> songs, string queueName) {
        IPlaylistModel? existsQueue = IServiceAccess.ModelGetter.PlaylistList()
                .Where(s => s.Type == DefaultValue.Queue && s.Name == queueName).FirstOrDefault();
        if (existsQueue != null) {
            IReadOnlyList<ISongModel> old_songs = existsQueue.Songs;
            foreach (ISongModel song in old_songs) {
                existsQueue.RemoveSong(song);
            }
            foreach (ISongModel song in songs) {
                existsQueue.AddSong(song);
                if (song.Id == songId) {
                    existsQueue.Current = song;
                }
            }
            existsQueue.Order = int.MaxValue;
            existsQueue.Save();
            existsQueue.ConsolideOrder(existsQueue.Type);
            MusicEco.GlobalData.PlayingQueueId = existsQueue.Id;
        }
        else {
            IPlaylistModel queueModel = IServiceAccess.Service.GetRequiredService<IPlaylistModel>();
            queueModel.Type = DefaultValue.Queue;
            queueModel.Name = queueName;
            foreach (var song in songs) {
                queueModel.AddSong(song);
                if (song.Id == songId) {
                    queueModel.Current = song;
                }
            }
            queueModel.AssignId();
            queueModel.Save();
            queueModel.ConsolideOrder(queueModel.Type);
            MusicEco.GlobalData.PlayingQueueId = queueModel.Id;
        }
    }
}
