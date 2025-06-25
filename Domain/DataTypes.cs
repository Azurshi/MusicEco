namespace Domain;

public struct Vector2 {
    private float[] data;
    public float X {
        get => data[0];
        set => data[0] = value;
    }
    public float Y {
        get => data[1];
        set => data[1] = value;
    }
    public Vector2(float x, float y) {
        this.data = [x, y];
    }
}
public struct Vector2I {
    private int[] data;
    public int X {
        get => data[0];
        set => data[0] = value;
    }
    public int Y {
        get => data[1];
        set => data[1] = value;
    }
    public Vector2I(int x, int y) {
        this.data = [x, y];
    }
}