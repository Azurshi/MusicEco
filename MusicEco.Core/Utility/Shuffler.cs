namespace MusicEco.Core.Utility;

public static class Shuffler {
    public static List<T> Shuffle<T>(IEnumerable<T> source) {
        var list = source.ToList();
        var rng = Random.Shared;
        for(int i=list.Count-1; i>0; i--) {
            int j = rng.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
        return list;
    }
}
