namespace MusicEco.Core.Types;

public sealed partial class SemaphoreSingle: SemaphoreSlim {
    public SemaphoreSingle() : base(1, 1) {
    }
}