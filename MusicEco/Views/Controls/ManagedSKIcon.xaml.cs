using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Core.Types;
using MusicEco.Image.Decoder;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using System.Diagnostics;
using System.Numerics;

namespace MusicEco.Views.Controls;

public partial class ManagedSKIcon: SKCanvasView {
    private enum IconViewState {
        Empty,
        Default,
        HasImage
    }
    private static readonly Type ThisType = typeof(ManagedSKIcon);
    [BindableAutoGen]
    public static readonly BindableProperty FileHashProperty
        = Utility.Create<Hash256>(ThisType, new Hash256(),
            propertyChanged: (b, _, v) => {
                var This = (ManagedSKIcon)b;
                var hash = (Hash256)v;
                This.SetImage(hash);
            });
    [BindableAutoGen]
    public static readonly BindableProperty FileHashesProperty
    = Utility.Create<IReadOnlyList<Hash256>>(ThisType, new List<Hash256>(),
        propertyChanged: (b, _, v) => {
            var This = (ManagedSKIcon)b;
            var hashes = (IReadOnlyList<Hash256>)v;
            This.SetImage(hashes);
        });
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
    private IDecodeResult? _decodeResult;
    public CoverSize Option { get; set; } = CoverSize.Small;
    private readonly IIconService _iconService;
    private IconViewState _state = IconViewState.Empty;
    public ManagedSKIcon() {
        InitializeComponent();
        this.IgnorePixelScaling = true;
        this.EnableTouchEvents = false;
        this._iconService = AppLifeCycle.Provider.GetRequiredService<IIconService>();
        //InstanceCount++;
        //Debug.WriteLine($"Instance count: {InstanceCount}");
    }
    private Hash256? _lastIconHash = null;
    private void SetEmpty() {
        this._decodeResult = null;
    }
    private void SetDefault() {
        this._decodeResult = this._iconService.GetDefault(this.Option);
    }
    private void SetIcon(IDecodeResult result) {
        this._decodeResult = result;
    }
    private bool HandleResult(IDecodeResult? result) {
        bool needRedraw;
        // If result is null
        if (result == null) {
            // Switch to default mode
            if (this._state == IconViewState.Default) {
                // Already default icon
                needRedraw = false;
                // Skip and return control
            }
            else {
                // Switch to default icon
                this.SetDefault();
                // Request redraw since state and image changed
                needRedraw = true;
                // Return control
            }
            this._state = IconViewState.Default;
        }
        // If has result and success
        else if (result.Success) {
            // Switch to new icon
            // All case handle with same operation
            this.SetIcon(result);
            this._state = IconViewState.HasImage;
            // Request redraw since state and image changed
            needRedraw = true;
            // Return control
        }
        // Result is not success
        else {
            // Switch to empty mode
            if (this._state == IconViewState.Empty) {
                needRedraw = false;
            }
            else {
                this.SetEmpty();
                needRedraw = true;
            }
            this._state = IconViewState.Empty;
        }
        return needRedraw;
    }
    private async Task SetIcon(Hash256? iconHash) {
        if (iconHash == null) {
            this._lastIconHash = iconHash;
            if (this._cts != null) {
                await this._cts.CancelAsync();
            }
            // Handle case where no IconHash is found
            // Switch to default mode if available else empty mode
            if (this._state != IconViewState.Default) {
                var result = this._iconService.GetDefault(this.Option);
                // If success, switch to default mode
                if (result.Success) {
                    this.SetIcon(result);
                    this._state = IconViewState.HasImage;
                    this.InvalidateSurface();
                }
                // Else when fail, switch to empty mode
                else {
                    this.SetEmpty();
                    this._state = IconViewState.Empty;
                    this.InvalidateSurface();
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
            bool needRedraw;
            Task<IDecodeResult?> task = this._iconService.GetIcon(iconHash.Value, this.Option, new(this, token));
            // Already have result
            if (task.IsCompletedSuccessfully) {
                needRedraw = HandleResult(task.Result);
            }
            // Need to wait for result
            else {
                // Clear canvas to prepare for result
                if (this._state != IconViewState.Empty) {
                    this._state = IconViewState.Empty;
                    this.InvalidateSurface();
                }
                await task;
                needRedraw = HandleResult(task.Result);
            }
            if (needRedraw) {
                this.InvalidateSurface();
            }
            this._cts = null;
        }
    }
    private static long DrawCount = 0;
    private static long ClearCount = 0;
    private static long InstanceCount = 0;
    private void SKCanvasView_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e) {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        //ClearCount++;
        if (this._decodeResult != null && this._decodeResult is SkiaDecodeResult skiaResult) {
            try {
                var image = skiaResult.Image;
                //Thread.Sleep(100);
                //DrawCount++;
                //Debug.WriteLine($"Draw count: {DrawCount} / {ClearCount}");
                //Debug.WriteLine("Redraw");
                float imageWidth = image.Width;
                float imageHeight = image.Height;
                float surfaceWidth = e.Info.Width;
                float surfaceHeight = e.Info.Height;
                float scale = Math.Min(surfaceWidth / imageWidth, surfaceHeight / imageHeight);
                float width = imageWidth * scale;
                float height = imageHeight * scale;
                float offsetX = (surfaceWidth - width) / 2;
                float offsetY = (surfaceHeight - height) / 2;
                var rect = new SKRect(
                    offsetX,
                    offsetY,
                    offsetX + width,
                    offsetY + height);
                canvas.DrawImage(image, rect, SKSamplingOptions.Default);
            }
            catch (ObjectDisposedException) {
                this._decodeResult = null;
                this._state = IconViewState.Empty;
                canvas.Clear(SKColors.Transparent);
            }
        }
    }
}