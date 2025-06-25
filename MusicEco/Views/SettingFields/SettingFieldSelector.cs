using MusicEco.ViewModels.Items;

namespace MusicEco.Views.SettingFields;
public class SettingFieldSelector : DataTemplateSelector {
    public DataTemplate? NullTemplate { get; set; }
    public DataTemplate? TextInputTemplate { get; set; }
    public DataTemplate? SliderFieldTemplate { get; set; }
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
        if (item is SettingFieldModel model) {
            model.Refresh();
            switch (model.SettingFieldFormat) {
                case Domain.Models.SettingFieldFormat.TextInput:
                    return TextInputTemplate ?? throw new ArgumentNullException(nameof(TextInputTemplate));
                case Domain.Models.SettingFieldFormat.Slider:
                    return SliderFieldTemplate ?? throw new ArgumentNullException(nameof(SliderFieldTemplate));
                default:
                    return NullTemplate ?? throw new Exception("null");
            }
        }
        else {
            throw new Exception($"Type not supported {item.GetType()}");
        }
    }
}
