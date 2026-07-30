using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Data.Database.Repositories;

namespace MusicEco.Data.Services;

internal class QueueService: IQueueService {
    public event EventHandler<QueueChangedEventArgs>? ItemsChanged;
    public event EventHandler? CurrentChanged;
    private readonly QueueRepository _queueRepo;
    public QueueService(QueueRepository queueRepository) {
        this._queueRepo = queueRepository;
    }

    public Task<bool> Delete(AudioQueue model, object? caller = null) {
        throw new NotImplementedException();
    }

    public async Task<bool> Exists(string name) {
        return await this._queueRepo.Exists(name);
    }

    public async Task<AudioQueue?> Get(string name) {
        return await this._queueRepo.Get(name);
    }

    public async Task<AudioQueue?> Get(DateTime createdTime) {
        return await this._queueRepo.Get(createdTime);
    }

    public async Task<List<AudioQueue>> GetAll() {
        return await this._queueRepo.GetAll();
    }

    public async Task<AudioQueue?> GetCurrent() {
        return await this._queueRepo.GetCurrent();
    }

    public Task<bool> Insert(AudioQueue model, object? caller = null) {
        throw new NotImplementedException();
    }

    public async Task<List<AudioQueue>> Query(string nameLike) {
        return await this._queueRepo.Query(nameLike);
    }

    public Task<bool> SetCurrent(AudioQueue? current, object? caller = null) {
        throw new NotImplementedException();
    }

    public Task<bool> Update(AudioQueue model, object? caller = null) {
        throw new NotImplementedException();
    }
}
