namespace MusicEco.Common.Events;

#region Scalar
public class StringEventArgs(string value) : EventArgs {
    public string Value { get; set; } = value;
}
public class BoolEventArgs(bool value) : EventArgs {
    public bool Value { get; set; } = value;
}
public class FloatEventArgs(float value) : EventArgs {
    public float Value { get; set; } = value;
}
public class IntEventArgs(int value) : EventArgs {
    public int Value { get; set; } = value;
}
public class Vector2EventArgs : EventArgs {
    public Vector2 Value { get; set; }
    public Vector2EventArgs(double x, double y) {
        this.Value = new Vector2((float)x, (float)y);
    }
    public Vector2EventArgs(float x, float y) {
        this.Value = new Vector2(x, y);
    }
    public Vector2EventArgs(Vector2 value) {
        this.Value = value;
    }
}
public class Vector2IEventArgs : EventArgs {
    public Vector2I Value { get; set; }
    public Vector2IEventArgs(int x, int y) {
        this.Value = new Vector2I(x, y);
    }
    public Vector2IEventArgs(Vector2I value) {
        this.Value = value;
    }
}
#endregion
#region Change
public class IntChangeEventArgs(int oldValue, int newValue): EventArgs {
    public int OldValue { get; set; } = oldValue;
    public int NewVlaue { get; set; } = newValue;
}
public class StringChangeEventArgs(string oldValue, string newValue) : EventArgs {
    public string OldValue { get; set; } = oldValue;
    public string NewValue { get; set; } = newValue;
}
#endregion

#region Custom
public class PlayAudioEventArgs(int songId, int queueId) : EventArgs {
    public int SongId { get; set; } = songId;
    public int QueueId { get; set; } = queueId;
}
public class OptionMenuEventArgs(VisualElement element, TappedEventArgs mouseEvent) : EventArgs {
    public VisualElement Element { get; set; } = element;
    public TappedEventArgs MouseEvent { get; set; } = mouseEvent;
}
public class FormMenuEventArgs(VisualElement element) : EventArgs {
    public VisualElement Element { get; set; } = element;
}
#endregion