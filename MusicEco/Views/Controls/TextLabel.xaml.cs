using MusicEco.Core;
using MusicEco.SourceGeneration;
using MusicEco.ViewModels;
using System.Diagnostics;

namespace MusicEco.Views.Controls;

public partial class TextLabel: ContentView, IAnimatedView {
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
    [BindedProperty]
    public partial bool IsAnimationEnabled { get; set; }
    public static readonly BindableProperty IsAnimationEnabledProperty
        = Utility.Create<bool>(ThisType, false,
            propertyChanged: (b, _, v) => {
                var This = (TextLabel)b;
                var value = (bool)v;
                // Switch to static -> animation
                if (value) {
                    This.OnEnterViewport();
                }
                // Switch to animation -> static
                else {
                    This.OnExitViewport();
                }
            });
    private readonly DelayedDispatcher _dispatcher;
    public TextLabel() {
        InitializeComponent();
        this._dispatcher = new(Config.ResizeInputDelay);
        this.Viewport.SizeChanged += this.OnSizeChanged;
        this.TextElement.SizeChanged += this.OnSizeChanged;
    }
    private async void OnSizeChanged(object? sender, EventArgs e) {
        await this._dispatcher.Dispatch(this.QueueAnimation);
    }
    private void QueueAnimation() {
        this.Dispatcher.Dispatch(this.UpdateAnimation);
    }
    private double _lastViewportWidth;
    private double _lastTextWidth;
    private const double UnitPerSeconds = 10;
    // Same to delay when resize to avoid restart affect visual
    private static readonly uint PauseMs = (uint)Config.ResizeInputDelay.TotalMilliseconds;
    private const string AnimationName = "LeftToRight";
    private void UpdateAnimation() {
        if (!this.IsLoaded
            || this.Viewport.Width <= 0
            || this.TextElement.Width <= 0) {
            return;
        }
        if (!this.IsAnimationEnabled) {
            //this.StopAnimation();
            return;
        }

        var viewportWidth = this.Viewport.Width;
        var textWidth = this.TextElement.Width;
        //// Avoid restarting after unrelated layout passes
        // This won't work when resize ?
        //if (Math.Abs(viewportWidth - this._lastViewportWidth) < 0.5
        //    || Math.Abs(textWidth - this._lastTextWidth) < 0.5) {
        //    return;
        //}
        Debug.WriteLine("Start animation");
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
        var travelMs = (uint)Math.Clamp(overflow / UnitPerSeconds * 1000, 500, 1000_000);
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
            repeat: this.CanRepeat
            );
    }
    private bool CanRepeat() {
        return this.IsLoaded
            && this.IsAnimationEnabled
            && this.TextElement.Width > this.Viewport.Width + 0.5;
    }
    private void StopAnimation() {
        this.TextElement.AbortAnimation(AnimationName);
        this.TextElement.TranslationX = 0;
    }
    private void ResetCachedSizes() {
        this._lastTextWidth = double.NaN;
        this._lastViewportWidth = double.NaN;
    }
    public void OnEnterViewport() {
        //Debug.WriteLine($"Entered: {this.GetHashCode()} {this.IsAnimationEnabled}");
        this.ResetCachedSizes();
        this.QueueAnimation();
    }

    public void OnExitViewport() {
        //Debug.WriteLine($"Exited: {this.GetHashCode()} {this.IsAnimationEnabled}");
        this.StopAnimation();
    }
}

// Bridge property
public partial class TextLabel {
    [BindedProperty]
    public partial FontAttributes FontAttributes { get; set; }
    public static readonly BindableProperty FontAttributesProperty
        = Utility.Create<FontAttributes>(ThisType, FontAttributes.None);
    [BindedProperty]
    public partial double FontSize { get; set; }
    public static readonly BindableProperty FontSizeProperty
        = Utility.Create<double>(ThisType, 12.0);
}