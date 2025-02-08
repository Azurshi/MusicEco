using MusicEco.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicEco.ViewModels.Slots;
public class TextSlot : BaseSlot {
    #region Databinding
    public string? Title { get; private set; }
    #endregion

    protected override Task OnActive() {
        Title = _key;
        OnPropertyChanged(nameof(Title));
        return Task.CompletedTask;
    }
}