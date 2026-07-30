using MusicEco.Core.Services;

namespace MusicEco.Image;

internal class BaseImageCodec: IImageCodec {
    protected SemaphoreSlim? Semaphore;
    public int NumWorkers { get; private set; }
    public void Initialize(int nWorkers) {
        if (Semaphore == null) {
            this.NumWorkers = nWorkers;
            this.Semaphore = new(nWorkers);
        } else {
            throw new InvalidOperationException("Object already initialized");
        }
    }
}
