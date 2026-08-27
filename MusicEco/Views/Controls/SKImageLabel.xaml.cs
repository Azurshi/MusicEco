using MusicEco.Core;
using MusicEco.Core.Services;
using MusicEco.Image.Decoder;
using MusicEco.Services;
using MusicEco.SourceGeneration;
using SkiaSharp;
using System.Diagnostics;
using System.Numerics;

namespace MusicEco.Views.Controls;
/// <summary>
/// Always call <see cref="Dispose"/> on removed.
/// </summary>
public partial class SKImageLabel: ContentView, IDisposable {
    private static readonly Type ThisType = typeof(SKImageLabel);
    [BindedProperty]
    public partial Vector2 MaxSize { get; set; }
    public static readonly BindableProperty MaxSizeProperty
        = Utility.Create<Vector2>(ThisType, Vector2.Zero,
            propertyChanged: (b, _, v) => {
                var This = (SKImageLabel)b;
                var value = (Vector2)v;
                if (This._bitmap != null) {
                    This._bitmap = This._bitmap.Resize(new SKSizeI((int)value.X, (int)value.Y), This.Option);
                }
            });
    [BindedProperty]
    public partial string ResourcePath { get; set; }
    public static readonly BindableProperty ResourcePathProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (SKImageLabel)b;
                var value = (string)v;
                This.LoadResource(value).FireAndForgetAsync();
            });
    [BindedProperty]
    public partial Color? TintColor { get; set; }
    public static readonly BindableProperty TintColorProperty
        = Utility.Create<Color?>(ThisType,
            propertyChanged: (b, _, v) => {
                var This = (SKImageLabel)b;
                var value = (Color?)v;
                This.Canvas.InvalidateSurface();
            });
    [BindedProperty]
    public partial SKSamplingOptions Option { get; set; }
    public static readonly BindableProperty OptionProperty
        = Utility.Create<SKSamplingOptions>(ThisType, SamplingOptions.None,
            propertyChanged: (b, _, v) => {
                var This = (SKImageLabel)b;
                var value = (SKSamplingOptions)v;
                This.Canvas.InvalidateSurface();
            });
    public SKImageLabel() {
        InitializeComponent();
        this._limiter = new(1);
        this._codec = AppLifeCycle.Provider.GetRequiredService<SharedImageCodec>();
    }
    ~SKImageLabel() {
        this._bitmap?.Dispose();
    }
    // Since Resource usually does not change.
    // We use SemaphoreSlim
    // to void handle dispose resource during cancellation
    private readonly SemaphoreSlim _limiter;
    private readonly SharedImageCodec _codec;
    private bool _disposed = false;
    private SKBitmap? _bitmap;
    private async Task LoadResource(string path) {
        if (path.Trim().Length == 0) {
            return;
        }
        await this._limiter.WaitAsync();
        try {
            if (this._disposed) {
                return;
            }
            Debug.WriteLine($"Resource path: {path}");
            using (var file = await FileSystem.OpenAppPackageFileAsync(path)) {
                using (var memory = new MemoryStream()) {
                    await file.CopyToAsync(memory);
                    var decodeResult = await this._codec.IconDecoder.DecodeAsync(memory.ToArray());
                    if (decodeResult is SkiaDecodeResult skiaDecodeResult) {
                        if (this._disposed) {
                            skiaDecodeResult.Image.Dispose();
                            return;
                        }
                        else {
                            // Prevent race with Paint method.
                            // But both this and Paint method run on same thread.
                            // So this is just for safeguard.
                            var oldBitmap = this._bitmap;
                            SKSizeI size = new((int)this.MaxSize.X, (int)this.MaxSize.Y);
                            this._bitmap = SKBitmap.FromImage(skiaDecodeResult.Image).Resize(size, this.Option);
                            oldBitmap?.Dispose();
                            skiaDecodeResult.Image.Dispose();
                            this.Canvas.InvalidateSurface();
                        }
                    }
                }
            }
        }
        finally {
            this._limiter.Release();
        }
    }
    private void Canvas_PaintSurface(object sender, SkiaSharp.Views.Maui.SKPaintSurfaceEventArgs e) {
        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        if (this._bitmap != null) {
            try {
                var bitmap = this._bitmap;
                float imageWidth = bitmap.Width;
                float imageHeight = bitmap.Height;
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
                var tintColor = this.TintColor;
                if (tintColor != null) {
                    SKColor skTintColor = new((byte)(tintColor.Red * 255), (byte)(tintColor.Green * 255), (byte)(tintColor.Blue * 255), (byte)(tintColor.Alpha * 255));
                    TintBitmap(bitmap, skTintColor);
                }
                canvas.DrawBitmap(bitmap, rect, this.Option);
            }
            finally {

            }
        }
    }
    private unsafe static void TintBitmap(SKBitmap bitmap, SKColor color) {
        SKColor* pixels = (SKColor*)bitmap.GetPixels();
        int size = bitmap.Width * bitmap.Height;
        for(int i=0; i<size; i++) {
            byte alpha = pixels[i].Alpha;
            if (alpha != 0) {
                pixels[i] = color.WithAlpha(alpha);
            }
        }
    }

    public virtual void Dispose() {
        if (this._disposed) {
            // Prevent double call
            return;
        }
        // Avoid collide with finalizer
        this._disposed = true;
        var bitmap = this._bitmap;
        this._bitmap = null;
        bitmap?.Dispose();
        GC.SuppressFinalize(this);
    }
}