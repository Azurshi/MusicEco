using MusicEco.ViewModels;
using MusicEco.Views.Components;

namespace MusicEco.Views.PageExtensions;
public interface IBasePage {
    public abstract Overlay PageOverlay { get; }
    public abstract PropertyObject MainBindingContext { get; }
}
