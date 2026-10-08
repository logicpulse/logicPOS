using SkiaSharp;

namespace LogicPOS.App.Hardware;

/// <summary>
/// ESC/POS bit-image (ESC * 24-dot double-density), same approach as GTK ThermalPrinter.PrintImage.
/// Used for QRCODE_METHOD=1 on printers that ignore native GS ( k QR.
/// </summary>
internal static class ThermalEscPosBitmap
{
    public static byte[] FromPng(byte[] png)
    {
        if (png.Length == 0)
        {
            return Array.Empty<byte>();
        }

        using var bitmap = SKBitmap.Decode(png);
        return bitmap is null ? Array.Empty<byte>() : FromBitmap(bitmap);
    }

    public static byte[] FromBitmap(SKBitmap source)
    {
        var width = source.Width;
        var height = source.Height;
        if (width <= 0 || height <= 0)
        {
            return Array.Empty<byte>();
        }

        // Pad height to a multiple of 24 (three 8-dot bands).
        var paddedHeight = ((height + 23) / 24) * 24;
        var dots = new bool[width * paddedHeight];
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var pixel = source.GetPixel(x, y);
                // Dark enough → black dot
                var luminance = (pixel.Red * 299 + pixel.Green * 587 + pixel.Blue * 114) / 1000;
                dots[(y * width) + x] = luminance < 128 && pixel.Alpha > 64;
            }
        }

        var buffer = new List<byte>(width * paddedHeight / 8 + 64);
        // Line spacing 24 dots
        buffer.Add(0x1B);
        buffer.Add(0x33);
        buffer.Add(24);

        var widthLo = (byte)(width % 256);
        var widthHi = (byte)(width / 256);
        for (var offset = 0; offset < paddedHeight; offset += 24)
        {
            buffer.Add(0x1B); // ESC
            buffer.Add(0x2A); // *
            buffer.Add(33);   // 24-dot double-density
            buffer.Add(widthLo);
            buffer.Add(widthHi);

            for (var x = 0; x < width; x++)
            {
                for (var band = 0; band < 3; band++)
                {
                    byte slice = 0;
                    for (var bit = 0; bit < 8; bit++)
                    {
                        var y = offset + (band * 8) + bit;
                        if (y < paddedHeight && dots[(y * width) + x])
                        {
                            slice |= (byte)(1 << (7 - bit));
                        }
                    }

                    buffer.Add(slice);
                }
            }

            buffer.Add(0x0A);
        }

        // Restore default line spacing (~30 dots)
        buffer.Add(0x1B);
        buffer.Add(0x33);
        buffer.Add(30);
        return buffer.ToArray();
    }
}
