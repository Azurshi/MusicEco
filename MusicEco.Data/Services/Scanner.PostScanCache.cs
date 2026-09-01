using SQLiteORM;

namespace MusicEco.Data.Services;

internal partial class Scanner {
    /// <summary>
    /// Update cached value like DisplayTitle
    /// </summary>
    /// <param name="connection"></param>
    private static void PostScanCache(SQLiteWriteConnection connection) {
        connection.Execute("""
            UPDATE AudioEntity AS a
            SET DisplayTitle = (
                SELECT f.Name
                FROM FileEntity AS f
                WHERE f.Hash = a.FileHash
                ORDER BY f.Path
                LIMIT 1
            )
            WHERE a.Title IS NULL
                AND EXISTS (
                    SELECT 1
                    FROM FileEntity AS f
                    WHERE f.Hash = a.FileHash
                    );
            """);
    }
}
