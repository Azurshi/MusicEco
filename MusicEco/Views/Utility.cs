using MusicEco.Views.Shell;
using System.Runtime.CompilerServices;
namespace MusicEco.Views;

public static class Utility {
    public static T GetResource<T>(string key) {
        return (T)App.Current!.Resources[key];
    }
    public static BindableProperty Create<TDeclare, TReturn>(
        object? defaultValue = null,
        [CallerMemberName] string propertyName = "",
        BindableProperty.BindingPropertyChangedDelegate? propertyChanged = null,
        BindingMode bindingMode = BindingMode.OneWay
    ) {
        return Create(typeof(TDeclare), typeof(TReturn), defaultValue, propertyName, propertyChanged, bindingMode);
    }
    public static BindableProperty Create<TReturn>(
        Type declareType,
        object? defaultValue = null,
        [CallerMemberName] string propertyName = "",
        BindableProperty.BindingPropertyChangedDelegate? propertyChanged = null,
        BindingMode bindingMode = BindingMode.OneWay
    ) {
        return Create(declareType, typeof(TReturn), defaultValue, propertyName, propertyChanged, bindingMode);
    }
    public static BindableProperty Create(
        Type declareType,
        Type returnType,
        object? defaultValue = null,
        [CallerMemberName] string propertyName = "",
        BindableProperty.BindingPropertyChangedDelegate? propertyChanged = null,
        BindingMode bindingMode = BindingMode.OneWay
    ) {
        propertyName = propertyName.Replace("Property", "");
        return BindableProperty.Create(
            propertyName,
            returnType,
            declareType,
            defaultBindingMode: bindingMode,
            propertyChanged: propertyChanged,
            defaultValue: defaultValue
        );
    }
}
