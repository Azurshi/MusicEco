using MusicEco.Core.Types;

namespace MusicEco.ViewModels.Pages;

public partial class ExplorerTreePageViewModel {
    private sealed class FolderStack {
        private readonly List<string> _stacks;
        private int _currentIndex;
        public FolderStack() {
            this._stacks = [];
            this._currentIndex = -1;
        }
        public void ToFolder(string folder) {
            this._stacks.RemoveRange(this._currentIndex + 1, this._stacks.Count - (this._currentIndex + 1));
            this._stacks.Add(folder);
            this._currentIndex++;
        }
        public string NextFolder() {
            if (CanNext()) {
                this._currentIndex++;
                return this._stacks[this._currentIndex];
            } else {
                throw new InvalidOperationException();
            }
        }
        public string PreviousFolder() {
            if (CanPrevious()) {
                this._currentIndex--;
                return this._stacks[this._currentIndex];
            }
            else {
                throw new InvalidOperationException();
            }
        }
        public bool CanNext() {
            return this._currentIndex >= 0 && this._currentIndex < this._stacks.Count - 1;
        }
        public bool CanPrevious() {
            return this._currentIndex > 0;
        }
        public void Reset() {
            this._stacks.Clear();
            this._currentIndex = -1;
        }
    }
}
