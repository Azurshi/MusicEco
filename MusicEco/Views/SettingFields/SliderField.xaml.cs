using MusicEco.ViewModels.Items;
using System.Diagnostics;

namespace MusicEco.Views.SettingFields;

public partial class SliderField : ContentView
{
    private SettingFieldModel? ViewModel;
    private Type? numberType => ViewModel?.DefaultValue?.GetType();
    private double? step;
    public SliderField()
	{
		InitializeComponent();
        BindingContextChanged += SliderField_BindingContextChanged;
	}

    private void SliderField_BindingContextChanged(object? sender, EventArgs e) {
        ViewModel = (SettingFieldModel)BindingContext;
        ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        if (ViewModel != null) {
            step = null;
            ValueSlider.Minimum = Convert.ToDouble(ViewModel.Domain[0]);
            ValueSlider.Maximum = Convert.ToDouble(ViewModel.Domain[1]);
            ValueSlider.Value = Convert.ToDouble(ViewModel.TemporyValue);
            if (ViewModel.Domain.Count > 2) {
                step = Convert.ToDouble(ViewModel.Domain[2]);
            }
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(SettingFieldModel.Domain)) {
            if (ViewModel != null) {
                step = null;
                ValueSlider.Minimum = Convert.ToDouble(ViewModel.Domain[0]);
                ValueSlider.Maximum = Convert.ToDouble(ViewModel.Domain[1]);
                ValueSlider.Value = Convert.ToDouble(ViewModel.TemporyValue);
                if (ViewModel.Domain.Count > 2) {
                    step = Convert.ToDouble(ViewModel.Domain[2]);
                }
            }
        }
        if (e.PropertyName == nameof(SettingFieldModel.TemporyValue)) {
            if (ViewModel != null) {
                ValueSlider.Value = Convert.ToDouble(ViewModel.TemporyValue);
            }
        }
    }

    private void ValueSlider_ValueChanged(object sender, ValueChangedEventArgs e) {
        if (ViewModel != null && numberType != null && step != null) {
            double value = Math.Round(e.NewValue / step ?? 1) * step ?? 0;
            if (numberType == typeof(int)) {
                ViewModel.TemporyValue = (int)value;
            }
            else if (numberType == typeof(long)) {
                ViewModel.TemporyValue = (long)value;
            }
            else if (numberType == typeof(float)) {
                ViewModel.TemporyValue = (float)value;
            }
            else if (numberType == typeof(double)) {
                ViewModel.TemporyValue = value;
            }
            else {
                value = 0;
                ViewModel.TemporyValue = null;
            }
            ValueSlider.Value = value;
        }
    }
}