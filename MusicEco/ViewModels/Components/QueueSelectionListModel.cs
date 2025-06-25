using Domain.Models;

namespace MusicEco.ViewModels.Components; 
public partial class QueueSelectionListModel: PlaylistSelectionListModel {
    public QueueSelectionListModel() : base() {
        Type = DefaultValue.Queue;
    }
}

