using System;
using System.Linq;
using System.Reflection;

namespace SQLiteORM.Internal;

internal static class ORMCommon {
    public static TableAttribute? GetTable(Type type) {
        return type.GetCustomAttribute<TableAttribute>(inherit: true);
    }
    public static bool IsNullable(PropertyInfo property) {
        if (property.PropertyType.IsValueType) {
            return Nullable.GetUnderlyingType(property.PropertyType) != null;
        } else {
            NullabilityInfoContext context = new();
            return context.Create(property).ReadState == NullabilityState.Nullable;
        }
    }
}
public static class SQLiteKeyword {
    public const string Integer = "INTEGER";
    public const string Text = "TEXT";
    public const string Real = "REAL";
    public const string Blob = "BLOB";
    public const string Json = "JSON";
    public const string JsonB = "JSONB";
    public const string NotNull = "NOT NULL";
    public const string AutoIncremental = "PRIMARY KEY AUTOINCREMENT";
}