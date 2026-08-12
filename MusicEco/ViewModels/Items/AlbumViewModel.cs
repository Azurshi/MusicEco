using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Items;

//public class AlbumViewModelIdentify: IComparer<AlbumViewModelIdentify> {
//    public string Name;
//    public IReadOnlyList<Hash256> FileHashes;
//    public AlbumViewModelIdentify(string name, IReadOnlyList<Hash256> fileHashes) {
//        this.Name = name;
//        this.FileHashes = fileHashes;
//    }

//    public int Compare(AlbumViewModelIdentify? x, AlbumViewModelIdentify? y) {
//        if (x == null && y == null) {
//            return 0;
//        }
//        else if (x == null) {
//            return -1;
//        }
//        else if (y == null) {
//            return 1;
//        }
//        else {
//            if (x.Name != y.Name) {
//                return x.Name.CompareTo(y.Name);
//            }
//            else {
//                if (x.FileHashes.Count != y.FileHashes.Count) {
//                    return x.FileHashes.Count.CompareTo(y.FileHashes.Count);
//                }
//                for(int i=0; i<x.FileHashes.Count; i++) {
//                    int result = x.FileHashes[i].Compare(x.FileHashes[i], y.FileHashes[i]);
//                    if (result != 0) {
//                        return result;
//                    }
//                }
//                return 0;
//            }
//        }
//    }
//}

public sealed class AlbumViewModel: IUpdateble {
    public object Identify { get; init; }
    public string Name { get; init; }
    public IReadOnlyList<Hash256> FileHashes { get; init; }
    public AlbumViewModel(string name, IReadOnlyList<Hash256> fileHashes) {
        this.Name = name;
        this.FileHashes = fileHashes;
        this.Identify = (this.Name, this.FileHashes);
    }
}
