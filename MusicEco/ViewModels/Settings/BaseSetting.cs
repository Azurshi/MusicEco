using Domain.Models;
using System.Diagnostics;

namespace MusicEco.ViewModels.Settings; 
public abstract class BaseSetting: IServiceAccess {
    public abstract string FieldName { get; }
    public string GetUniqueName() {
        return "Setting_" + FieldName;
    }
    public ISettingField? SettingField;
    public T? GetValue<T>() where T: notnull {
        if (SettingField == null) throw new ArgumentNullException(nameof(SettingField));
        return (T?)SettingField.Value;
    }
    public T GetValueOrDefault<T>() where T: notnull {
        if (SettingField == null) throw new ArgumentNullException(nameof(SettingField));
        return (T)(SettingField.Value ?? SettingField.DefaultValue);
    }
    protected ISettingField Create(string valueType) {
        ISettingField field = IServiceAccess.Service.GetRequiredService<ISettingField>();
        field.ValueType = valueType;
        field.AssignId();
        return field;
    }
    protected void Refresh() {
        ISettingField? field = IServiceAccess.ModelQuery.SettingFieldByUniqueName(GetUniqueName());
        SettingField = field;
        if (field == null) {
            throw new ArgumentNullException(GetUniqueName());
        }
    }
    public abstract ISettingField Create();
    public BaseSetting() { }
}
