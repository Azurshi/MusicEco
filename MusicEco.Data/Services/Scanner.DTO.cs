using MusicEco.Core.Data;
using MusicEco.Core.Types;
using MusicEco.Core.Utility;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    private sealed record IconDatabaseDto(
        Hash256 IconHash,
        TempFile SmallFile,
        TempFile MediumFile,
        TempFile LargeFile);
    private sealed record AudioMetadataDatabaseDto(
        AudioEntity Audio,
        List<AudioTagRelation> Tags);
    private sealed record TimeChangedFile(
        string Path,
        DateTime ModifiedTime);
    private sealed record ScanFileDto(
        List<FileEntry> NewFiles,
        List<FileEntry> ContentChangedFiles,
        List<TimeChangedFile> TimeChangeFiles) {
        public ScanFileDto() : this([], [], []) { }
    }
    private sealed record HandleFileDto(
        List<IconDatabaseDto> Icons,
        List<AudioMetadataDatabaseDto> Audios,
        List<FileEntity> NewFiles,
        List<FileEntity> ContentChangedFiles,
        List<TimeChangedFile> TimeChangedFiles) {
        public HandleFileDto() : this([], [], [], [], []) { }
    }
}
