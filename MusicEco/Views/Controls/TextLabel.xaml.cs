using MusicEco.SourceGeneration;

namespace MusicEco.Views.Controls;

public partial class TextLabel: ContentView {
    private static readonly Type ThisType = typeof(TextLabel);
    [BindedProperty]
    public partial string Text { get; set; }
    public static readonly BindableProperty TextProperty
        = Utility.Create<string>(ThisType, string.Empty,
            propertyChanged: (b, _, v) => {
                var This = (TextLabel)b;
                var value = (string)v;
                This.ResetCachedSizes();
                This.QueueAnimation();
            });

    public TextLabel() {
        InitializeComponent();
        this.Viewport.SizeChanged += this.OnSizeChanged;
        this.TextElement.SizeChanged += this.OnSizeChanged;
    }

    private void OnSizeChanged(object? sender, EventArgs e) {
        this.QueueAnimation();
    }
    private void QueueAnimation() {
        this.Dispatcher.Dispatch(this.UpdateAnimation);
    }
    private double _lastViewportWidth;
    private double _lastTextWidth;
    private const double UnitPerSeconds = 50;
    private const int PauseMs = 1000;
    private const string AnimationName = "MarqueeText";
    private void UpdateAnimation() {
        if (!this.IsLoaded
            || this.Viewport.Width <= 0
            || this.TextElement.Width <= 0) {
            return;
        }
        var viewportWidth = this.Viewport.Width;
        var textWidth = this.TextElement.Width;
        // Avoid restarting after unrelated layout passes
        if (Math.Abs(viewportWidth - this._lastViewportWidth) < 0.5
            || Math.Abs(textWidth - this._lastTextWidth) < 0.5) {
            return;
        }
        this._lastViewportWidth = viewportWidth;
        this._lastTextWidth = textWidth;
        this.StopAnimation();
        var overflow = textWidth - viewportWidth;
        // Text already fit inside layout
        if (overflow <= 0.5) {
            return;
        }
        var from = 0.0;
        var to = -overflow;
        var travelMs = (uint)Math.Clamp(overflow / UnitPerSeconds * 1000, 500, 10_000);
        var totalMs = travelMs + (PauseMs * 2);
        var pauseFraction = PauseMs / (double)totalMs;
        this.TextElement.TranslationX = from;
        async void Animation(double progress) {
            double translation;
            if (progress <= pauseFraction) {
                translation = from;
            }
            else if (progress >= 1 - pauseFraction) {
                translation = to;
            }
            else {
                double movementProgress = (progress - pauseFraction) / (1 - (2 * pauseFraction));
                translation = from + ((to - from) * movementProgress);
            }
            if (this.TextElement.TranslationX != translation) {
                this.TextElement.TranslationX = translation;
            }
        }
        this.TextElement.Animate(
            AnimationName,
            Animation,
            rate: 16,
            length: totalMs,
            easing: Easing.Linear,
            repeat: () => this.IsLoaded && this.TextElement.Width > this.Viewport.Width + 0.5
            );
    }
    private void StopAnimation() {
        this.TextElement.AbortAnimation(AnimationName);
        this.TextElement.TranslationX = 0;
    }
    private void ResetCachedSizes() {
        this._lastTextWidth = double.NaN;
        this._lastViewportWidth = double.NaN;
    }
}