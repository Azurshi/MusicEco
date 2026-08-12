using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Image.Decoder;
using MusicEco.Services;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;
using System.Numerics;

namespace MusicEco.Views.Controls;

public partial class ManagedSKIcon: SKCanvasView {
    private static readonly Type ThisType = typeof(ManagedSKIcon);
    public static readonly BindableProperty FileHashProperty
        = Utility.Create<Hash256>(ThisType, new Hash256(),
            propertyChanged: (b, _, v) => {
                var This = (ManagedSKIcon)b;
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
            var This = (ManagedSKIcon)b;
            var hashes = (IReadOnlyList<Hash256>)v;
            This.SetImage(hashes);
        });
    public IReadOnlyList<Hash256> FileHashes {
        get => (IReadOnlyList<Hash256>)GetValue(FileHashesProperty);
        set => SetValue(FileHashesProperty, value);
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
    private static bool CheckHashListEquals(IReadOnlyList<Hash256> listA, IReadOnlyList<Hash256> listB) {
        if (listA.Count != listB.Count) {
            return false;
        }
        int count = listA.Count;
        for(int i=0; i<count; i++) {
            if (listA[i] != listB[i]) {
                return false;
            }
        }
        return true;
    }
    private async Task SetImageTask(IReadOnlyList<Hash256> fileHashes) {
        if (CheckHashListEquals(this._lastHashes, fileHashes)) {
            return;
        }
        this._lastHashes = fileHashes;
        if (this._cts != null) {
            await this._cts.CancelAsync();
        }
        var iconHash = await this._iconService.GetFirstIconHash(fileHashes);
        await this.SetIcon(iconHash);
    }
    private SKImage? _image;
    public CoverSize Option { get; set; } = CoverSize.Small;
    private readonly IIconService _iconService;
    public ManagedSKIcon() {
        InitializeComponent();
        this._iconService = AppLifeCycle.Provider.GetRequiredService<IIconService>();
        var result = this._iconService.GetDefault(CoverSize.Small);
        if (result is SkiaDecodeResult skiaResult) {
            this._image = skiaResult.Image;
        }
    }
    private Hash256? _lastIconHash = null;
    private async Task SetIcon(Hash256? iconHash) {
        if (iconHash == null) {
            this._lastIconHash = iconHash;
            if (this._cts != null) {
                await this._cts.CancelAsync();
            }
            var result = this._iconService.GetDefault(this.Option);
            if (result is SkiaDecodeResult skiaResult) {
                var oldImage = this._image;
                this._image = skiaResult.Image;
                if (this._image != oldImage) {
                    this.InvalidateMeasure();
                }
            }
            return;
        }
        if (this._lastIconHash == iconHash) {
            return;
        }
        this._lastIconHash = iconHash;
        if (this._cts != null) {
            await this._cts.CancelAsync();
        }
        else {
            this._cts = new();
            var token = this._cts.Token;
            Task<IDecodeResult> task = this._iconService.GetIcon(iconHash.Value, this.Option, new(this, token));
            if (task.IsCompletedSuccessfully) {
                bool needRedraw = false;
                if (this._image == null) {
                    needRedraw = true;
                }
                if (task.Result.Success && task.Result is SkiaDecodeResult skiaResult) {
                    this._image = skiaResult.Image;
                    needRedraw = true;
                } else {
                    this._image = null;
                }
                if (needRedraw) {
                    this.InvalidateSurface();
                }
            }
            else {
                if (this._image != null) {
                    this._image = null;
                    this.InvalidateSurface();
                }
                bool needRedraw = false;
                await task;
                if (task.Result.Success && task.Result is SkiaDecodeResult skiaResult) {
                    this._image = skiaResult.Image;
                    needRedraw = true;
                }
                else {
                    this._image = null;
                }
                if (needRedraw) {
                    this.InvalidateSurface();
                }
            }
            this._cts = null;
        }
    }
    private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e) {
        //var canvas = e.Surface.Canvas;
        //canvas.Clear(SKColors.Transparent);
        //if(this._image != null) {
        //    //Debug.WriteLine("Redraw");
        //    float imageWidth = this._image.Width;
        //    float imageHeight = this._image.Height;
        //    float surfaceWidth = e.Info.Width;
        //    float surfaceHeight = e.Info.Height;
        //    float scale = Math.Min(surfaceWidth / imageWidth, surfaceHeight / imageHeight);
        //    float width = imageWidth * scale;
        //    float height = imageHeight * scale;
        //    float offsetX = (surfaceWidth - width) / 2;
        //    float offsetY = (surfaceHeight - height) / 2;
        //    var rect = new SKRect(
        //        offsetX,
        //        offsetY, 
        //        offsetX + width,
        //        offsetY + height);
        //    //canvas.DrawImage(this._image, rect, SKSamplingOptions.Default);
        //}
    }
}