using Domain.DataAccess;
namespace DataStorage.DataAccess;
public class GlobalData : IGlobalData {
    public void Set(string fieldName, object? value) => GlobalDataStorage.Set(fieldName, value);
    public bool TryGet(string fieldName, out object? value) => GlobalDataStorage.TryGet(fieldName, out value);
    public object? GetValueOrDefault(string fieldName, object? defaultValue = null) => GlobalDataStorage.GetValueOrDefault(fieldName, defaultValue);
    public T GetValueOrDefault<T>(string fieldName, T defaultValue) where T : notnull => GlobalDataStorage.GetValueOrDefault(fieldName, defaultValue);
    public T? GetValueOrDefault<T>(string fieldName) where T : struct => GlobalDataStorage.GetValueOrDefault<T>(fieldName);
}
