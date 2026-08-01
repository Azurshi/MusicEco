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
            return new(_sentinel, this.FromPage, this.ToPage, []);
        }
        public NavigateEventArgs ToNagvigateEventArgsReverse() {
            return new(_sentinel, this.ToPage, this.FromPage, []);
        }
    }
    private readonly List<NavigationRoute> _stacks;
    private int _currentIndex;
    private readonly object _sentinel = new();
    public event EventHandler<PageRoute>? RouteChanged;
    public NavigationStack() {
        _stacks = [];
        _currentIndex = -1;
        EventSystem.Connect<NavigatedEventArgs>(OnPageNavigated);
    }
    private void OnPageNavigated(object? sender, NavigatedEventArgs e) {
        if (e.NavigateSender != _sentinel) {
            if (_currentIndex != _stacks.Count - 1) {
                _stacks.RemoveRange(_currentIndex + 1, _stacks.Count - (_currentIndex + 1));
            }
            NavigationRoute item = new(_sentinel, e.FromPage, e.ToPage);
            _stacks.Add(item);
            _currentIndex = _stacks.Count - 1;
        }
        RouteChanged?.Invoke(this, e.ToPage);
    }
    public void PreviousPage() {
        if (CanNavigateToPreviousPage()) {
            var currentItem = _stacks[_currentIndex];
            _currentIndex--;
            var args = currentItem.ToNagvigateEventArgsReverse();
            EventSystem.Publish(_sentinel, args);
        }
        else {
            throw new RaceExeption();
        }
    }
    public void NextPage() {
        if (CanNavigateToNextPage()) {
            _currentIndex++;
            var nextItem = _stacks[_currentIndex];
            var args = nextItem.ToNagvigateEventArgs();
            EventSystem.Publish(_sentinel, args);
        }
        else {
            throw new RaceExeption();
        }
    }
    public bool CanNavigateToPreviousPage() {
        if (_currentIndex > 0) {
            return true;
        }
        else {
            return false;
        }
    }
    public bool CanNavigateToNextPage() {
        if (_currentIndex < _stacks.Count - 1) {
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
