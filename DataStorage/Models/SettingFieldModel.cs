using Domain.Exceptions;
using Domain.Models;
using System.Text.Json.Serialization;

namespace DataStorage.Models; 
public class ValueTypeNotSupportException: BaseException {
    public ValueTypeNotSupportException(string type) {
        this.Type = "Data error";
        this.Info = $"Type: {type} not supported";
    }
}
public class SettingFieldModel: BaseModel, ISettingField {
    private object Parse(string value) {
        switch (ValueType) {
            case "string":
                return value;
            case "int":
                return int.Parse(value);
            case "long":
                return long.Parse(value);
            case "float":
                return float.Parse(value);
            case "double":
                return double.Parse(value);
            case "bool":
                return bool.Parse(value);
            default:
                throw new ValueTypeNotSupportException(this.ValueType);
        }
    }
    private string Convert(object value) {
        switch (ValueType) {
            case "string":
                return ((string)value).ToString();
            case "int":
                return ((int)value).ToString();
            case "long":
                return ((long)value).ToString();
            case "float":
                return ((float)value).ToString();
            case "double":
                return ((double)value).ToString();
            case "bool":
                return ((bool)value).ToString();
            default:
                throw new ValueTypeNotSupportException(this.ValueType);
        }
    }
    [JsonInclude] public string UniqueName { get; set; } = Domain.Models.DefaultValue.Empty;
    [JsonInclude] public string Name { get; set; } = Domain.Models.DefaultValue.Unknow;
    [JsonInclude] public string Info { get; set; } = Domain.Models.DefaultValue.Empty;
    [JsonInclude] public string? ValueString { get; set; }
    [JsonInclude] public string DefaultValueString { get; set; } = Domain.Models.DefaultValue.Empty;
    [JsonInclude] public string? ExtraArgString { get; set; }
    [JsonInclude] public List<string> ValueDomainString { get; set; } = [];
    [JsonInclude] public string ValueType { get; set; } = "string";
    [JsonInclude] public SettingFieldFormat Format { get; set; } = Domain.Models.DefaultValue.FieldFormat;
    
    [JsonIgnore]
    public object? Value {
        get {
            if (ValueString != null) {
                return Parse(ValueString);
            }
            else {
                return null;
            }
        }
        set {
            if (value != null) {
                ValueString = Convert(value);
            }
            else {
                ValueString = null;
            }
        }
    }
    [JsonIgnore] public object DefaultValue {
        get => Parse(DefaultValueString);
        set => DefaultValueString = Convert(value);
    }
    [JsonIgnore] public List<object> ValueDomain {
        get {
            List<object> values = [];
            foreach(string valueString in ValueDomainString) {
                values.Add(Parse(valueString));
            }
            return values;

        }
        set {
            List<string> valueStrings = [];
            foreach(object value_ in value) {
                valueStrings.Add(Convert(value_));
            }
            this.ValueDomainString = valueStrings;
        }
    }
    [JsonIgnore] public object? ExtraArg {
        get {
            if (ExtraArgString == null) {
                return null;
            }
            else {
                return Parse(ExtraArgString);
            }
        }
        set {
            if (value == null) {
                ExtraArgString = null;
            }
            else {
                ExtraArgString = Convert(value);
            }
        }
    }

}
