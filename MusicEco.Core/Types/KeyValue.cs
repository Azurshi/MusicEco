namespace MusicEco.Core.Types;

public sealed record KeyValue {
    private readonly string _key;
    private readonly string _value;
    public string Key => _key;
    public string Value => _value;
    public KeyValue(string key, string value) {
        _key = key;
        _value = value;
    }
}