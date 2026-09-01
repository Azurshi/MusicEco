namespace MusicEco.Core.Types;

public interface IUpdateble {
    public IComparable Identify { get; }
}
public interface IBackgroundItem {
    public void SetOddBackgroundColor();
    public void SetEvenBackgroundColor();
    public void AutoBackgroundColor(int index);
}
