using MusicEco.Common.Events;
using MusicEco.Global;

namespace MusicEco.Common.Value;
public static class UI {
    private static int numRow;
    public static int NumRow => numRow;
    private static int rowHeight = 50;
    public static int RowHeight => rowHeight;
    public static void SetNumRow(double height, int value) {
        numRow = value;
        int newRowHeight = (int)(height / numRow);
        if (newRowHeight != rowHeight) {
            rowHeight = newRowHeight;
            EventSystem.Publish<IntEventArgs>(Signal.UI_RowHeight_Changed, null, new(rowHeight));
        }
    }
}