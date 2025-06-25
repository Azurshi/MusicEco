using Domain.Models;

namespace Domain.DataAccess; 
public interface IScanner {
    public Task ScanAsync(
        System.IProgress<Tuple<int, int>> progress,
        List<string> inputFolderPaths,
        List<string> extensions,
        bool quickScan,
        ItemSource source
    );
    public Dictionary<string, HashSet<long>> CheckDiff();
    /// <summary>
    /// Undo last scan.
    /// </summary>
    public void Rollback();
    /// <summary>
    /// Commit sacnned data, keep misiing files.
    /// </summary>
    public void Commit();
    public void DeleteAllData();
}