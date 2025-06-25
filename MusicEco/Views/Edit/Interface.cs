using MusicEco.ViewModels;
namespace MusicEco.Views.Edit; 
public interface IEditableView {
    public static BindableProperty CreateBinding<T>() where T: VisualElement {
        return Utility.Create<ViewMode, T>(PropertyChanged, nameof(Mode));
    }
    private static void PropertyChanged(BindableObject bindable, object oldValue, object newValue) {
        IEditableView view = (IEditableView)bindable;
        ViewMode state = (ViewMode)newValue;
        if (state == ViewMode.Edit) {
            view.OnEdit();
        }
        else if (state == ViewMode.Confirm) {
            view.OnConfirm();
        }
        else if (state == ViewMode.Cancel) {
            view.OnCancel();
        }
    }
    public abstract ViewMode Mode { get; set; }
    protected abstract void OnEdit();
    protected abstract void OnConfirm();
    protected abstract void OnCancel();
}