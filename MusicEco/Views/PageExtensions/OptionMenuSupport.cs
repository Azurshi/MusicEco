using Domain;
using MusicEco.Views.Components;

namespace MusicEco.Views.PageExtensions;
public interface IOptionMenuSupportPage: IBasePage {
    public void OptionMenu_Clicked(object sender, TappedEventArgs e);
    public void GetPageEventHandler(object sender, Buttons.GetBasePageEventArgs e);
}
public class OptionMenuExtension {
    private readonly IOptionMenuSupportPage page;
    private VisualElement Page => (VisualElement)page;
    public OptionMenuExtension(IOptionMenuSupportPage page) {
        this.page = page;
    }
    public void StartOptionMenu(object sender, TappedEventArgs e) {
        DataTemplate template = (DataTemplate)Page.Resources["OptionMenuTemplate"];
        OptionMenu optionMenu = (OptionMenu)template.CreateContent();
        optionMenu.SetBindingContext(((VisualElement)sender).BindingContext, page.MainBindingContext);
        Point? point = e.GetPosition(Page);
        Vector2 size = new((float)optionMenu.WidthRequest, -1);
        if (point != null) {
            Vector2 position = new((float)point.Value.X - size.X, (float)point.Value.Y);
            page.PageOverlay.Start(optionMenu, position, size, autoMove: true);
        }
    }
}