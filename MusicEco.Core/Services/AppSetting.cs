using System.Text.Json.Serialization;

namespace MusicEco.Core.Services;

public class SettingChangedEventArgs(string name, object? value): EventArgs {
    public string Name { get; } = name;
    public object? Value { get; } = value;
}
public interface IAppSetting {
    public event EventHandler<SettingChangedEventArgs> ItemChanged;
    public void Set(string key, object? value);
    public T Get<T>(string key, T defaultValue);
    public bool Register(Type type, JsonConverter converter);
    public bool Register<T>(JsonConverter<T> converter);
}