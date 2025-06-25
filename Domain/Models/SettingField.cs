namespace Domain.Models;
public enum SettingFieldFormat {
    Null,
    TextInput,
    /// <summary>
    /// Type of DefaultValue is type of all values
    /// ValueDomain[0] for min and [1] for max
    /// </summary>
    NumberInput,
    /// <summary>
    /// Type of DefaultValue is type of all values
    /// ValueDomain[0] for min and [1] for max, [2] for step
    /// </summary>
    Slider,
    /// <summary>
    /// Type of DefaultValue is type of all values
    /// </summary>
    ComboBox,
    /// <summary>
    /// all values are boolean
    /// </summary>
    CheckBox,
    /// <summary>
    /// all values are int32 that hold 8bit for each channel RGBA
    /// </summary>
    ColorPicker,
    /// <summary>
    /// DefaultValue is string of default path
    /// </summary>
    FolderPicker,
    /// <summary>
    /// DefaultValue is string of default path
    /// </summary>
    FilePicker
}
public interface ISettingField: IBaseModel {
    public string UniqueName { get; set; }
    public string Name { get; set; }
    public object? Value { get; set; }
    public object DefaultValue { get; set; }
    public List<object> ValueDomain { get; set; }
    public string Info { get; set; }
    public object? ExtraArg { get; set; }
    public string ValueType { get; set; }
    public SettingFieldFormat Format { get; set; }
}