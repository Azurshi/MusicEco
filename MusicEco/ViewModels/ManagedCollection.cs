using MusicEco.Core.Types;
using System.Collections.ObjectModel;

namespace MusicEco.ViewModels;

public class ManagedCollection<T> where T: IUpdateble {
    public ObservableCollection<T> Items => this._processedCollection.Items;
    protected readonly List<T> _originalItems;
    protected readonly ObservableCollectionExtend<T> _processedCollection;
    protected Func<IReadOnlyList<T>, IReadOnlyList<T>>? _filter;
    public ManagedCollection() {
        this._originalItems = [];
        this._processedCollection = new();
    }
    public ManagedCollection(Func<IReadOnlyList<T>, IReadOnlyList<T>> filter) {
        this._filter = filter;
        this._originalItems = [];
        this._processedCollection = new();
    }
    private void UpdateInner(Action<T>? changeState) {
        if (this._filter != null) {
            this._processedCollection.Update(this._filter(this._originalItems), changeState);
        }
        else {
            this._processedCollection.Update(this._originalItems, changeState);
        }
    }
    public virtual void Update(IReadOnlyList<T> items, Action<T>? changeState = null) {
        this._originalItems.Clear();
        this._originalItems.AddRange(items);
        this.UpdateInner(changeState);
    }
    public virtual void SetFilter(Func<IReadOnlyList<T>, IReadOnlyList<T>>? filter, Action<T>? changeState = null) {
        this._filter = filter;
        this.UpdateInner(changeState);
    }
    public virtual void Refresh(Action<T>? changeState) {
        this.UpdateInner(changeState);
    }
    public virtual void Refresh() {
        this.UpdateInner(null);
    }
}
