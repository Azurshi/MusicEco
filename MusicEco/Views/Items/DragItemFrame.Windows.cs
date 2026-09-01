#if WINDOWS
using Microsoft.Maui.Platform;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using MusicEco.Views.Buttons;
using System.Windows.Input;
using Windows.Graphics.Imaging;

namespace MusicEco.Views.Items;

public partial class DragItemFrame {
    private static async Task<SoftwareBitmap> CaptureAsync(View view) {
        var nativeView = view.ToPlatform(view.Handler!.MauiContext!);
        int width = Math.Max(1, (int)Math.Round(nativeView.ActualWidth));
        int height = Math.Max(1, (int)Math.Round(nativeView.ActualHeight));
        var render = new RenderTargetBitmap();
        await render.RenderAsync(nativeView, width, height);
        var buffer = await render.GetPixelsAsync();
        return SoftwareBitmap.CreateCopyFromBuffer(
            buffer,
            BitmapPixelFormat.Bgra8,
            render.PixelWidth,
            render.PixelHeight,
            BitmapAlphaMode.Premultiplied);
    }
    private async Task PlatformDrag(MoveButton moveButton, ICommand command, Microsoft.Maui.Controls.DragStartingEventArgs e) {
        if (this.Handler?.PlatformView is FrameworkElement nativeView) {
            var platformArgs = e.PlatformArgs;
            if (platformArgs != null) {
                var nativeArgs = platformArgs.DragStartingEventArgs;
                var deferal = nativeArgs.GetDeferral();
                var pointerPosition = nativeArgs.GetPosition(nativeView);
                try {
                    var bitmap = await CaptureAsync(this);
                    var anchor = new Windows.Foundation.Point(
                        Math.Clamp(pointerPosition.X, 0, bitmap.PixelWidth - 1),
                        Math.Clamp(pointerPosition.Y, 0, bitmap.PixelHeight - 1));
                    nativeArgs.DragUI.SetContentFromSoftwareBitmap(bitmap, anchor);
                    platformArgs.Handled = true;
                }
                finally {
                    deferal.Complete();
                }
            }
        }
        command.Execute(this.BindingContext);
        this.Reset();
    }
}
#endif
