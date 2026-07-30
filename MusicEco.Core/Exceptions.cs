namespace MusicEco.Core;

public class RaceExeption : Exception {
    public RaceExeption() {
    }
}
public class NotInitializedException : Exception {
    public NotInitializedException() { }
    public NotInitializedException(string message) : base(message) { }
}
public class ValueNotExistsExeption : Exception {
    public ValueNotExistsExeption() { }
    public ValueNotExistsExeption(string value) : base($"Value does not exists: {value}") { }
}