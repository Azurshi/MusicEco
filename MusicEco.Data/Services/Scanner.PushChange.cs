using MusicEco.Core.Services;
using MusicEco.Data.Database.Entities;
using MusicEco.Data.Database.Relations;
using SQLiteORM;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    /// <summary>
    /// Old Entity still remain
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="dto"></param>
    /// <exception cref="Exception"></exception>
    private static void PushChange(SQLiteWriteConnection connection, HandleFileDto dto, IProgress<PushChangeProgress> progress) {
        int result;
        foreach (var file in dto.TimeChangedFiles) {
            result = connection.Update("""
                UPDATE FileEntity SET TimeModified = ? WHERE Path = ?
                """, file.ModifiedTime, file.Path);
        }
        foreach (var file in dto.ContentChangedFiles) {
            result = connection.Update(file, """
                WHERE Path = ?
                """, file.Path);
        }
        _ = connection.Insert(dto.NewFiles, false);
        List<AudioEntity> audios = [];
        List<AudioTagRelation> tags = [];
        foreach (var item in dto.Audios) {
            audios.Add(item.Audio);
            tags.AddRange(item.Tags);
        }
        _ = connection.Insert(audios, false);
        _ = connection.Insert(tags, false);
        IconEncoderBuffer buffer = new();
        int totalCount = dto.Icons.Count;
        for(int i=0; i<totalCount; i++) {
            var icon = dto.Icons[i];
            var iconHash = icon.IconHash;
            int smallLength = (int)icon.SmallFile.ReadAndDispose(buffer.SmallIconBuffer);
            int mediumLength = (int)icon.MediumFile.ReadAndDispose(buffer.MediumIconBuffer);
            int largeLength = (int)icon.LargeFile.ReadAndDispose(buffer.LargeIconBuffer);
            if (smallLength == 0 || mediumLength == 0 || largeLength == 0) {
                throw new Exception("File is empty");
            }
            _ = connection.Execute($"""
                INSERT INTO IconEntity (Hash, SmallIcon, MediumIcon, LargeIcon)
                VALUES (?,?,?,?)
                """,
                iconHash,
                buffer.GetSmallIcon(smallLength),
                buffer.GetMediumIcon(mediumLength),
                buffer.GetLargeIcon(largeLength));
            progress.Report(new(i+1, totalCount));
        }
    }
}
