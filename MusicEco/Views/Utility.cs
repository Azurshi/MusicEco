using System.Runtime.CompilerServices;

namespace MusicEco.Views;
public static class Utility {
    public static BindableProperty Create<Treturn, Tdeclare>(BindableProperty.BindingPropertyChangedDelegate? propertyChanged = null, [CallerMemberName] string propertyName = "", BindingMode bindingMode = BindingMode.OneWay, object? defaultValue = null) {
        return Create<Treturn>(typeof(Tdeclare), propertyChanged, propertyName, bindingMode, defaultValue);
    }
    public static BindableProperty Create<Treturn>(Type declaretType, BindableProperty.BindingPropertyChangedDelegate? propertyChanged = null, [CallerMemberName] string propertyName = "", BindingMode bindingMode = BindingMode.OneWay, object? defaultValue = null) {
        propertyName = propertyName.Replace("Property", "");
        return BindableProperty.Create(
            propertyName, typeof(Treturn), declaretType, defaultBindingMode: bindingMode, propertyChanged: propertyChanged, defaultValue: defaultValue
            );
    }
}

public static class Validator {
    public static string Name(string value) {
        return value.Trim();
    }
    public static string? Float(string value) {
        if (float.TryParse(value, out float result)) {
            return result.ToString();
        }
        else {
            return null;
        }
    }
    public static string? Int(string value) {
        if (int.TryParse(value, out int result)) {
            return result.ToString();
        }
        else {
            return null;
        }
    }
    public static string? Long(string value) {
        if (long.TryParse(value, out long result)) {
            return result.ToString();
        }
        else {
            return null;
        }
    }
}