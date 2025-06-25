using Domain.Models;
using System.Diagnostics;
using DefaultValue_ = Domain.Models.DefaultValue;

namespace MusicEco.ViewModels.Items;
/// <summary>
/// Force load on create (not support lazy loading due to format change)
/// </summary>
public partial class SettingFieldModel : BaseItem, IServiceAccess {
    private ISettingField? target;
    public ISettingField Target {
        get => target ?? throw new ArgumentNullException(nameof(target));
        set => target = value;
    }
    private object? temporyValue;
    public object? Value => Target.Value;
    public object DefaultValue => Target.DefaultValue;
    public bool Loaded = false;
    public object? TemporyValue {
        get => temporyValue;
        set {
            temporyValue = value;
            OnPropertyChanged();
        }
    }
    public SettingFieldFormat SettingFieldFormat => Target.Format;
    public string UniqueName => Target.UniqueName;
    public string Name => Target.Name;
    public List<object> Domain => Target.ValueDomain;
    public object? ExtraArg => Target.ExtraArg;
    public string Info => Target.Info;
    public string ValueType => Target.ValueType;
    private static readonly List<string> _propertyNames = [
        nameof(Key), nameof(UniqueName), nameof(Name), nameof(Domain),
        nameof(ExtraArg), nameof(Info), nameof(ValueType)
        ];
    public SettingFieldModel() { }
    public SettingFieldModel(ISettingField field) {
        key = field.Id.ToString();
        Target = field;
        temporyValue = Value ?? DefaultValue;
    }
    protected override async Task OnActive() {
        Refresh();
        await Task.CompletedTask;
    }
    public void Refresh() {
        ISettingField? model = IServiceAccess.ModelGetter.SettingField(long.Parse(Key));
        if (model != null) {
            Target = model;
            temporyValue = Value ?? DefaultValue;
            foreach (var propertyName in _propertyNames) {
                OnPropertyChanged(propertyName);
            }
        }
        Loaded = true;
    }
    public void Apply() {
        if (Target == null || temporyValue == null) return;
        Target.Value = temporyValue;
        Target.Save();
        OnPropertyChanged(nameof(Value));
    }
    public void Cancel() {
        temporyValue = Target.Value ?? Target.DefaultValue;
        OnPropertyChanged(nameof(TemporyValue));
    }
}