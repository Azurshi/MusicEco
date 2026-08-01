using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace MusicEco.Core.Services;

public class SettingChangedEventArgs(string name, object? value): EventArgs {
    public string Name { get; } = name;
    public object? Value { get; } = value;
}
public interface IAppSetting {
    public event EventHandler<SettingChangedEventArgs> ItemChanged;
    public void Set(object? value, [CallerMemberName] string key = "");
    public T Get<T>(T defaultValue, [CallerMemberName] string key = "");
    public bool Register(Type type, JsonConverter converter);
    public bool Register<T>(JsonConverter<T> converter);
}