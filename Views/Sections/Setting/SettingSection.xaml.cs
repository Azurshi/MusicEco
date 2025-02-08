#if WINDOWS
using MusicEco.Platforms.Windows;
#else
using MusicEco.Platforms.Android;
#endif
using MusicEco.Common;
using MusicEco.Common.Events;
using MusicEco.Global;
using MusicEco.Global.AbstractLayers;
using System.Diagnostics;

namespace MusicEco.Views.Sections;

public partial class SettingSection : ContentView
{
	public SettingSection()
	{
		InitializeComponent();
        //EventSystem.Connect(Signal.System.ExtensionLoaded, this, OnExtensionLoaded);
    }
    #region Main
    private async void FileScanButton_Clicked(object sender, EventArgs e) {
        await SettingModify.Scan();
        DataStorage.ForceSave();
        DataStorage.Load();
    }
    private void DeleteUserDataButton_Clicked(object sender, EventArgs e) {
        string localStateFolderPath = Global.AbstractLayers.File.GetPersPath();
        if (Directory.Exists(localStateFolderPath)) {
            MusicEco.Common.Value.System.AppRunning = false;
            Directory.Delete(localStateFolderPath, true);
            Directory.CreateDirectory(localStateFolderPath);
            DataStorage.Load();
            MusicEco.Common.Value.System.AppRunning = true;
        }

    }
    #endregion
    #region Extension
    private void PluginSettingButton_Clicked(object sender, EventArgs e) {
        PluginSettingView.IsVisible = false;
        DeveloperView.IsVisible = false;
        PluginListView.IsVisible = true;
        AppSettingView.IsVisible = false;
    }
    private void PluginBackButton_Clicked(object sender, EventArgs e) {
        PluginSettingView.IsVisible = false;
        DeveloperView.IsVisible = false;
        PluginListView.IsVisible = false;
        AppSettingView.IsVisible = true;
    }
    private void OnPluginSetting_Clicked(object? sender, StringEventArgs e) {
        PluginListView.IsVisible = false;
        PluginSettingView.IsVisible = true;
        SetupSettingView(e.Value);
    }
    private void PluginSettingCancelButton_Clicked(object sender, EventArgs e) {
        PluginSettingView.IsVisible = false;
        PluginListView.IsVisible = true;
        FinalizeSetting(false);
    }

    private void PluginSettingApplyButton_Clicked(object sender, EventArgs e) {
        PluginSettingView.IsVisible = false;
        PluginListView.IsVisible = true;
        FinalizeSetting(true);
    }
    //    private void OnExtensionLoaded(object? sender, EventArgs e) {

    //#if WINDOWS

    //        foreach (IViewExtension extension in ExtensionManager.Extensions) {
    //            PluginSettingModel.LoadSetting(extension.Info.Setting);
    //            SettingField field = new();
    //            field.SetData(extension.Name, extension.XId);
    //            PluginHolder.Add(field);
    //            field.SetBinding(SettingField.HeightRequestProperty, new Binding(nameof(ViewModel.SlotHeight)));
    //            field.SettingClicked += OnPluginSetting_Clicked;
    //            ManagedExtensions.Add(extension);
    //        }
    //#endif
    //    }
    private void FinalizeSetting(bool apply) {
        //int settingCount = PluginSettingHolder.Count;
        //for (int i = 1; i < settingCount; i++) {
        //    Microsoft.Maui.IView child = PluginSettingHolder.Children[i];
        //    if (child is TextInputField textInputField) {
        //        if (apply) textInputField.Apply();
        //        else textInputField.Cancel();
        //    }
        //    else if (child is NumberInputField numberInputField) {
        //        if (apply) numberInputField.Apply();
        //        else numberInputField.Cancel();
        //    }
        //    else if (child is SliderField sliderField) {
        //        if (apply) sliderField.Apply();
        //        else sliderField.Cancel();
        //    }
        //    else if (child is ComboBoxField comboBoxField) {
        //        if (apply) comboBoxField.Apply();
        //        else comboBoxField.Cancel();
        //    }
        //    else if (child is CheckBoxField checkBoxField) {
        //        if (apply) checkBoxField.Apply();
        //        else checkBoxField.Cancel();
        //    }
        //    else if (child is ColorPickerField colorPickerField) {
        //        if (apply) colorPickerField.Apply();
        //        else colorPickerField.Cancel();
        //    }
        //    else if (child is FilePickerField filePickerField) {
        //        if (apply) filePickerField.Apply();
        //        else filePickerField.Cancel();
        //    }
        //    else if (child is FolderPickerField folderPickerField) {
        //        if (apply) folderPickerField.Apply();
        //        else folderPickerField.Cancel();
        //    }
        //    else {
        //        continue ;
        //    }
        //}
        //if (apply && lastSettingOpened != null) {
        //    PluginSettingModel.SaveSetting(lastSettingOpened);
        //}
    }
    //private XSetting? lastSettingOpened;
    private void SetupSettingView(string id) {
        //IViewExtension? target = null;
        //foreach(var extension in ManagedExtensions) {
        //    if (extension.XId == id) {
        //        target = extension;
        //        lastSettingOpened = extension.Info.Setting;
        //        break;
        //    }
        //}
        //int settingCount = PluginSettingHolder.Count;
        //for (int i = 1; i < settingCount; i++) {
        //    PluginSettingHolder.Children.RemoveAt(1);
        //}
        //if (target == null) return;
        //List<XSettingField> fieldInfos = target.Info.Setting.Fields;
        //foreach (var field in fieldInfos) {
        //    switch(field.Format) {
        //        case XSettingFieldFormat.TextInput:
        //            TextInputField textInputField = new();
        //            textInputField.SetData(field);
        //            PluginSettingHolder.Add(textInputField);
        //            break;
        //        case XSettingFieldFormat.NumberInput:
        //            NumberInputField numberInputField = new();
        //            numberInputField.SetData(field);
        //            PluginSettingHolder.Add(numberInputField);
        //            break;
        //        case XSettingFieldFormat.Slider:
        //            SliderField sliderField = new();
        //            sliderField.SetData(field);
        //            PluginSettingHolder.Add(sliderField);
        //            break;
        //        case XSettingFieldFormat.ComboBox:
        //            ComboBoxField comboBoxField = new();
        //            comboBoxField.SetData(field);
        //            PluginSettingHolder.Add(comboBoxField);
        //            break;
        //        case XSettingFieldFormat.CheckBox:
        //            CheckBoxField checkBoxField = new();
        //            checkBoxField.SetData(field);
        //            PluginSettingHolder.Add(checkBoxField);
        //            break;
        //        case XSettingFieldFormat.ColorPicker:
        //            ColorPickerField colorPickerField = new();
        //            colorPickerField.SetData(field);
        //            PluginSettingHolder.Add(colorPickerField);
        //            break;
        //        case XSettingFieldFormat.FilePicker:
        //            FilePickerField filePickerField = new();
        //            filePickerField.SetData(field);
        //            PluginSettingHolder.Add(filePickerField);
        //            break;
        //        case XSettingFieldFormat.FolderPicker:
        //            FolderPickerField folderPickerField = new();
        //            folderPickerField.SetData(field);
        //            PluginSettingHolder.Add(folderPickerField);
        //            break;
        //    }
        //}
    }
    #endregion
    #region Developer
    private void DeveloperSectionButton_Clicked(object sender, EventArgs e) {
        PluginSettingView.IsVisible = false;
        DeveloperView.IsVisible = true;
        PluginListView.IsVisible = false;
        AppSettingView.IsVisible = false;
    }

    private void DeveloperBackButton_Clicked(object sender, EventArgs e) {
        PluginSettingView.IsVisible = false;
        DeveloperView.IsVisible = false;
        PluginListView.IsVisible = false;
        AppSettingView.IsVisible = true;
    }

    private void Button_1_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("!!!! Dev button 1 clicked");
        EventSystem.Publish(Signal.Dev_Button1, this);
    }
    private void Button_2_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("!!!! Dev button 2 clicked");
        EventSystem.Publish(Signal.Dev_Button2, this);
    }
    private void Button_3_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("!!!! Dev button 3 clicked");
        EventSystem.Publish(Signal.Dev_Button3, this);
    }
    private void Button_4_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("!!!! Dev button 4 clicked");
        EventSystem.Publish(Signal.Dev_Button4, this);
    }
    private void Button_5_Clicked(object sender, EventArgs e) {
        Debug.WriteLine("!!!! Dev button 5 clicked");
        EventSystem.Publish(Signal.Dev_Button5, this);
    }
    #endregion
}