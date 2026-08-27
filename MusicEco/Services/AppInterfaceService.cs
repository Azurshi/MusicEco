using MusicEco.Core.Services;
using MusicEco.Views.Shell;

namespace MusicEco.Services;

internal partial class AppInterfaceService: IAppInterfaceService {
    private readonly IAppSetting _setting;
    private readonly AssemblyLocalization L;
    private App App => (App)(Application.Current ?? throw new Exception());
    public AppInterfaceService(IAppSetting appSetting, ILocalizationService localizationService) {
        this._setting = appSetting;
        this.L = localizationService.Get(this.GetType());
    }    
}
