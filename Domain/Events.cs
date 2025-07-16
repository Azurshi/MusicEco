using Domain.Models;

namespace Domain.Events;
public class ScanStartedEventArgs(System.IProgress<Tuple<int, int>> progress, List<string> folders, bool quickScan, ItemSource source) : EventArgs {
    public System.IProgress<Tuple<int, int>> Progress { get; set; } = progress;
    public List<string> Folders { get; set; } = folders;
    public bool QuickScan { get; set; } = quickScan;
    public ItemSource Source { get; set; } = source;
}

public class ScanCompletedEventArgs : EventArgs {

}

public class ScanCommitedEventArgs : EventArgs {

}