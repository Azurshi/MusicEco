namespace MusicEco.Core.Utility;

public class FileNameComparer : IComparer<string> {
    public int Compare(string? x, string? y) {
        if (x is null && y is null) return 0;
        if (x is null) return -1;
        if (y is null) return 1;
        if (!int.TryParse(x.Split(".")[0], out int valueX)) {
            valueX = int.MaxValue;
        }
        if (!int.TryParse(y.Split(".")[0], out int valueY)) {
            valueY = int.MaxValue;
        }
        if (valueX != int.MaxValue && valueY != int.MaxValue) {
            return valueX.CompareTo(valueY);
        }
        else {
            return x.CompareTo(y);
        }
    }
}