using MusicEco.ViewModels.Items;

namespace MusicEco.Views.TemplateSelector;

public class ExplorerTemplateSelector: DataTemplateSelector {
    public DataTemplate? FolderTemplate { get; set; }
    public DataTemplate? FileTemplate { get; set; }
    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) {
        if (FolderTemplate == null || FileTemplate == null) {
            throw new InvalidOperationException();
        }
        if (item is FolderEntryViewModel) {
            return FolderTemplate;
        }
        else if (item is FileEntryViewModel) {
            return FileTemplate;
        } else {
            throw new ArgumentOutOfRangeException(nameof(item));
        }
    }
}
