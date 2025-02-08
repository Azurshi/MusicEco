using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MusicEco.Common;
public abstract class PropertyObject : INotifyPropertyChanged {
    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = "null") {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public void PublicOnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

