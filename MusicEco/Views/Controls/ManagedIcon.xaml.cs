using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Services;
using MusicEco.SourceGeneration;

namespace MusicEco.Views.Controls;

public partial class ManagedIcon: Microsoft.Maui.Controls.Image {
    private static readonly Type ThisType = typeof(ManagedIcon);
    [BindableAutoGen]
    public static readonly BindableProperty FileHashProperty
        = Utility.Create<Hash256>(ThisType, new Hash256(),
            propertyChanged: (b, _, v) => {
                var This = (ManagedIcon)b;
                var hash = (Hash256)v;
                This.SetImage(hash);
            });
    [BindableAutoGen]
    public static readonly BindableProperty FileHashesProperty
        = Utility.Create<IReadOnlyList<Hash256>>(ThisType, new List<Hash256>(),
            propertyChanged: (b, _, v) => {
                var This = (ManagedIcon)b;
                var hashes = (IReadOnlyList<Hash256>)v;
                This.SetImage(hashes);
            });
    public CoverSize Option { get; set; } = CoverSize.Small;
    private readonly IIconService _iconService;
    public ManagedIcon() {
        InitializeComponent();
        this._iconService = AppLifeCycle.Provider.GetRequiredService<IIconService>();
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
        var iconHash = await this._iconService.GetIconHash(fileHash);
        await this.SetIcon(iconHash);
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
        var iconHash = await this._iconService.GetFirstIconHash(fileHashes);
        await this.SetIcon(iconHash);
    }

    private Hash256? _lastIconHash = null;
    private async Task SetIcon(Hash256? iconHash) {
        //if (iconHash == null) {
        //    this._lastIconHash = iconHash;
        //    if (this._cts != null) {
        //        await this._cts.CancelAsync();
        //    }
        //    this.Source = this._iconService.GetDefault(this.Option);
        //    return;
        //}
        //if (this._lastIconHash == iconHash) {
        //    return;
        //}
        //this._lastIconHash = iconHash;
        //if (this._cts != null) {
        //    await this._cts.CancelAsync();
        //}
        //else {
        //    this._cts = new();
        //    var token = this._cts.Token;
        //    Task<ImageSource> task = this._iconService.GetIcon(iconHash.Value, this.Option, new(this, token));
        //    if (task.IsCompletedSuccessfully) {
        //        this.Source = task.Result;
        //    }
        //    else {
        //        this.Source = null;
        //        await task;
        //        this.Source = task.Result;
        //    }
        //    this._cts = null;
        //}
    }
}