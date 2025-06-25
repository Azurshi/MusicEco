using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;

namespace MusicEco.Views.Buttons;

/// <summary>
/// Auto bind ItemModel Key as CommandParameter
/// </summary>
public partial class DeleteButton : BaseButton {
    public DeleteButton() {
        InitializeComponent();
        PreviousColor = BackgroundColor;
    }
    protected override Color HightLightColor => Colors.Red;
}