using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;

namespace MusicEco.Views.Controls;

public partial class ManagedIcon: Microsoft.Maui.Controls.Image {
    private static readonly Type ThisType = typeof(ManagedIcon);
    public static readonly BindableProperty FileHashProperty
        = Utility.Create<Hash256>(ThisType, new Hash256(),
            propertyChanged: (b, _, v) => {
                var This = (ManagedIcon)b;
                var hash = (Hash256)v;
                This.SetImage(hash);
            });
    public Hash256 FileHash {
        get => (Hash256)GetValue(FileHashProperty);
        set => SetValue(FileHashProperty, value);
    }
    public static readonly BindableProperty FileHashesProperty
        = Utility.Create<IReadOnlyList<Hash256>>(ThisType, new List<Hash256>(),
            propertyChanged: (b, _, v) => {
                var This = (ManagedIcon)b;
                var hashes = (IReadOnlyList<Hash256>)v;
                This.SetImage(hashes);
            });
    public IReadOnlyList<Hash256> FileHashes {
        get => (IReadOnlyList<Hash256>)GetValue(FileHashesProperty);
        set => SetValue(FileHashesProperty, value);
    }
    public CoverSize Option { get; set; } = CoverSize.Small;
    public ManagedIcon() {
        InitializeComponent();
    }
    private Hash256 _lastHash = new();
    private CancellationTokenSource? _cts;
    private void SetImage(Hash256 fileHash) {
        SetImageTask(fileHash).FireAndForgetAsync();
    }
    private async Task SetImageTask(Hash256 fileHash) {
        if (this._lastHash == fileHash) { // vulable to change
            return;
        }
        this._lastHash = fileHash;
        if (this._cts != null) {
            await this._cts.CancelAsync();
        }
        this._cts = new();
        var token = this._cts.Token;
        var service = AppLifeCycle.Provider.GetRequiredService<IIconService>();
        Task<ImageSource> task = service.GetIcon(fileHash, this.Option, new(this, token));
        if (task.IsCompletedSuccessfully) {
            this.Source = task.Result;
        } else {
            this.Source = null;
            await task;
            this.Source = task.Result;
        }
        this._cts = null;
    }
    private IReadOnlyList<Hash256> _lastHashes = [];
    private void SetImage(IReadOnlyList<Hash256> fileHashes) {
        SetImageTask(fileHashes).FireAndForgetAsync();
    }
    private async Task SetImageTask(IReadOnlyList<Hash256> fileHashes) {
        if (this._lastHashes.SequenceEqual(fileHashes)) {
            return;
        }
        this._lastHashes = fileHashes;
        if (this._cts != null) {
            await this._cts.CancelAsync();
        }
        this._cts = new();
        var token = this._cts.Token;
        var service = AppLifeCycle.Provider.GetRequiredService<IIconService>();
        Task<ImageSource> task = service.GetFirstIcon(fileHashes, this.Option, new(this, token));
        if (task.IsCompletedSuccessfully) {
            this.Source = task.Result;
        }
        else {
            this.Source = null;
            await task;
            if (!AppLifeCycle.Closed) {
                this.Source = task.Result;
            }
        }
        this._cts = null;
    }
}