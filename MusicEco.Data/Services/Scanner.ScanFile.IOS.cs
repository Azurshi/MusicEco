#if IOS
using MusicEco.Core.Data;
using MusicEco.Core.Services;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    private static Task<ScanFileDto> ScanFiles(List<FileEntry> existsFiles, IReadOnlyList<string> paths, HashSet<string> fileExtensions, int nWorkers, IProgress<ScanFileProgress> progress, TimeSpan updateInterval) {
        throw new NotImplementedException();
    }
}
#endif
