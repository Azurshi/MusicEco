using MusicEco.Core;

namespace MusicEco.Services;

public class NavigationStack {
    private class NavigationRoute {
        public PageRoute FromPage;
        public PageRoute ToPage;
        private readonly object _sentinel;
        public NavigationRoute(object sentinel, PageRoute fromPage, PageRoute toPage) {
            this.FromPage = fromPage;
            this.ToPage = toPage;
            this._sentinel = sentinel;
        }
        public NavigateEventArgs ToNagvigateEventArgs() {
            return new(this._sentinel, this.FromPage, this.ToPage, []);
        }
        public NavigateEventArgs ToNagvigateEventArgsReverse() {
            return new(this._sentinel, this.ToPage, this.FromPage, []);
        }
    }
    private readonly List<NavigationRoute> _stacks;
    private int _currentIndex;
    private readonly object _sentinel = new();
    public event EventHandler<PageRoute>? RouteChanged;
    public NavigationStack() {
        this._stacks = [];
        this._currentIndex = -1;
        EventSystem.Connect<NavigatedEventArgs>(OnPageNavigated);
    }
    private void OnPageNavigated(object? sender, NavigatedEventArgs e) {
        if (e.NavigateSender != this._sentinel) {
            if (this._currentIndex != this._stacks.Count - 1) {
                this._stacks.RemoveRange(this._currentIndex + 1, this._stacks.Count - (this._currentIndex + 1));
            }
            NavigationRoute item = new(this._sentinel, e.FromPage, e.ToPage);
            this._stacks.Add(item);
            this._currentIndex = this._stacks.Count - 1;
        }
        RouteChanged?.Invoke(this, e.ToPage);
    }
    public void PreviousPage() {
        if (CanNavigateToPreviousPage()) {
            var currentItem = this._stacks[_currentIndex];
            this._currentIndex--;
            var args = currentItem.ToNagvigateEventArgsReverse();
            EventSystem.Publish(this._sentinel, args);
        }
        else {
            throw new RaceExeption();
        }
    }
    public void NextPage() {
        if (CanNavigateToNextPage()) {
            this._currentIndex++;
            var nextItem = _stacks[this._currentIndex];
            var args = nextItem.ToNagvigateEventArgs();
            EventSystem.Publish(this._sentinel, args);
        }
        else {
            throw new RaceExeption();
        }
    }
    public bool CanNavigateToPreviousPage() {
        if (this._currentIndex > 0) {
            return true;
        }
        else {
            return false;
        }
    }
    public bool CanNavigateToNextPage() {
        if (this._currentIndex < this._stacks.Count - 1) {
            return true;
        }
        else {
            return false;
        }
    }
    public PageRoute? CurrentRoute {
        get {
            if (this._currentIndex >= 0) {
                return this._stacks[this._currentIndex].ToPage;
            } else {
                return null;
            }
        }
    }
}
