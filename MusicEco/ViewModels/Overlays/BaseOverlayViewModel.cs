using MusicEco.Core.Services;
using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Overlays;

public partial class BaseOverlayViewModel: ObservableObject {
    public AssemblyLocalization L { get; init; }
    public BaseOverlayViewModel(ILocalizationService localizationService) {
        this.L = localizationService.Get(typeof(BaseOverlayViewModel));
    }
}
