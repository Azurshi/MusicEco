using System;
using System.Numerics;

namespace MusicEco.Image;

internal static class Common {
    public static Vector2 ComputeSize(double width, double height, Vector2 maxSize) {
        double maxWidth = maxSize.X;
        double maxHeight = maxSize.Y;
        if (width <= maxWidth && height <= maxHeight) {
            // Both width and height are smaller than max size
        }
        else {
            double ratio = width / height;
            if (width > maxWidth && height > maxHeight) {
                // Both with and height is oversized
                if (ratio <= 1) {
                    height = maxHeight; 
                    width = height * ratio;
                }
                else {
                    width = maxWidth; 
                    height = width / ratio;
                }
            }
            else if (width > maxWidth && height <= maxHeight) {
                // Width is oversized
                width = maxWidth;
                height = width / ratio;
            }
            else if (width <= maxWidth && height > maxHeight) {
                // Height is oversized
                height = maxHeight;
                width = height * ratio;
            }
            else {
                throw new Exception();
            }
        }
        return new(MathF.Ceiling((float)width), MathF.Ceiling((float)height));
    }
}