namespace Domain.DataAccess;
public interface IGlobalData {
    public bool TryGet(string fieldName, out object? value);
    public void Set(string fieldName, object? value);
    public object? GetValueOrDefault(string fieldName, object? defaultValue = null);
    public T GetValueOrDefault<T>(string fieldName, T defaultValue) where T : notnull;
    public T? GetValueOrDefault<T>(string fieldName) where T : struct;
}