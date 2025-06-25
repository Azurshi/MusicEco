namespace DataStorage;
internal enum DataType {
    Null,
    Int,
    Float,
    String,
    Bool,
    Long,
    Double,
    ListInt,
    ListFloat,
    ListString,
    ListBool,
    ListLong,
    ListDouble,
    DateTime
}

internal static class GlobalDataStorage {
    private static readonly Dictionary<string, object?> _data = [];
    internal static Dictionary<string, object?> Data => _data;
    private static readonly Dictionary<string, DataType> _dataTypes = [];
    internal static Dictionary<string, DataType> DataTypes => _dataTypes;
    internal static bool _changed = false;
    internal static bool TryGet(string fieldName, out object? value) {
        if (_data.TryGetValue(fieldName, out object? dataValue)) {
            value = dataValue;
            return true;
        }
        else {
            value = default;
            return false;
        }
    }
    internal static object? GetValueOrDefault(string fieldName, object? defaultValue = null) {
        if (TryGet(fieldName, out object? value)) {
            return value;
        }
        else {
            return defaultValue;
        }
    }
    internal static T GetValueOrDefault<T>(string fieldName, T defaultValue) where T : notnull {
        if (TryGet(fieldName, out object? value) && value != null) {
            return (T)value;
        }
        else {
            return defaultValue;
        }
    }
    internal static T? GetValueOrDefault<T>(string fieldName) where T : struct {
        if (TryGet(fieldName, out object? value) && value != null) {
            return (T)value;
        }
        else {
            return null;
        }
    }
    internal static void Set(string fieldName, object? value) {
        _changed = true;
        _data[fieldName] = value;
        if (value == null) {
            _dataTypes[fieldName] = DataType.Null;
        }
        else if (_dataTypes.TryGetValue(fieldName, out DataType dataType)) {
            if (dataType != DataType.Null) {
                return;
            }
        }
        if (value is int) {
            _dataTypes[fieldName] = DataType.Int;
        }
        else if (value is float) {
            _dataTypes[fieldName] = DataType.Float;
        }
        else if (value is string) {
            _dataTypes[fieldName] = DataType.String;
        }
        else if (value is bool) {
            _dataTypes[fieldName] = DataType.Bool;
        }
        else if (value is long) {
            _dataTypes[fieldName] = DataType.Long;
        }
        else if (value is double) {
            _dataTypes[fieldName] = DataType.Double;
        }
        else if (value is List<int>) {
            _dataTypes[fieldName] = DataType.ListInt;
        }
        else if (value is List<float>) {
            _dataTypes[fieldName] = DataType.ListFloat;
        }
        else if (value is List<string>) {
            _dataTypes[fieldName] = DataType.ListString;
        }
        else if (value is List<bool>) {
            _dataTypes[fieldName] = DataType.ListBool;
        }
        else if (value is List<long>) {
            _dataTypes[fieldName] = DataType.ListLong;
        }
        else if (value is List<double>) {
            _dataTypes[fieldName] = DataType.ListDouble;
        }
        else if (value is DateTime) {
            _dataTypes[fieldName] = DataType.DateTime;
        }
        else {
            throw new NotSupportedException($"Type not supported {value?.GetType()}");
        }
    }
}