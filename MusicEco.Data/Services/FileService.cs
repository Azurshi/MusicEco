using MusicEco.Core.Data;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Data.Database.Repositories;

namespace MusicEco.Data.Services;

internal class FileService: IFileService {
    public event EventHandler<FileChangedEventArgs>? ItemsChanged;
    private readonly FileRepository _fileRepo;
    public FileService(FileRepository fileRepository) {
        this._fileRepo = fileRepository;
    }
    public Task<FileEntry?> Get(string path) {
        return this._fileRepo.Get(path);
    }

    public Task<List<FileEntry>> GetAll() {
        return this._fileRepo.GetAll();
    }

    public Task<List<FileEntry>> GetByHash(Hash256 hash) {
        return this._fileRepo.GetByHash(hash);
    }

    public Task<bool> IsAvailable(FileEntry model) {
#if WINDOWS
        if (File.Exists(model.Path)) {
            return Task.FromResult(true);
        } else {
            return Task.FromResult(false);
        }
#else
        return Task.FromResult(true);
#endif
    }

    public Task<List<FileEntry>> Query(string path) {
        return this._fileRepo.Query(path);
    }

    public Task<bool> UpdateMetadata(string path, Hash256 fileHash, AudioMetadata metadata, object? caller = null) {
        throw new NotImplementedException();
    }
}
