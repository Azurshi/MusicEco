[global::System.AttributeUsage(global::System.AttributeTargets.Property)]
public sealed class AppSettingPropertyAttribute: global::System.Attribute {
    public object? DefaultValue;
    public string? StorageFieldName { get; set; }
    public bool IsObservableObject { get; set; }
    public AppSettingPropertyAttribute(object? defaultValue) {
        this.DefaultValue = defaultValue;
        this.StorageFieldName = null;
        this.IsObservableObject = true;
    }
}