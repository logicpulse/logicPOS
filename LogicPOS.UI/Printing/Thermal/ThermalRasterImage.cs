using System;
using System.Collections.Generic;
using System.Drawing;
using ESC_POS_USB_NET.Printer;

namespace LogicPOS.UI.Printing
{
    /// <summary>
    /// Encodes bitmaps as ESC/POS GS v 0 raster (more reliable than ESC * 24-dot bands).
    /// </summary>
    public static class ThermalRasterImage
    {
        public static void Print(Printer printer, Bitmap image, int maxDotsWidth)
        {
            if (printer == null || image == null || maxDotsWidth <= 0)
            {
                return;
            }

            using (var prepared = Prepare(image, maxDotsWidth))
            {
                printer.Append(EncodeGsV0(prepared));
            }
        }

        private static Bitmap Prepare(Bitmap source, int maxDotsWidth)
        {
            var width = source.Width;
            var height = source.Height;

            if (width > maxDotsWidth)
            {
                height = Math.Max(1, (int)Math.Round(height * (maxDotsWidth / (double)width)));
                width = maxDotsWidth;
            }

            // GS v 0 width must be a multiple of 8 dots.
            var targetWidth = Math.Max(8, (width + 7) / 8 * 8);

            var mono = new Bitmap(targetWidth, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(mono))
            {
                g.Clear(Color.White);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

                var drawWidth = Math.Min(source.Width, width);
                g.DrawImage(source, 0, 0, drawWidth, height);
            }

            return mono;
        }

        private static byte[] EncodeGsV0(Bitmap bmp)
        {
            var widthBytes = (bmp.Width + 7) / 8;
            var height = bmp.Height;
            var data = new List<byte>(8 + widthBytes * height)
            {
                0x1D, // GS
                0x76, // v
                0x30, // 0
                0x00, // normal mode
                (byte)(widthBytes & 0xFF),
                (byte)((widthBytes >> 8) & 0xFF),
                (byte)(height & 0xFF),
                (byte)((height >> 8) & 0xFF)
            };

            for (var y = 0; y < height; y++)
            {
                for (var xByte = 0; xByte < widthBytes; xByte++)
                {
                    byte slice = 0;
                    for (var bit = 0; bit < 8; bit++)
                    {
                        var x = xByte * 8 + bit;
                        if (x >= bmp.Width)
                        {
                            continue;
                        }

                        var c = bmp.GetPixel(x, y);
                        var luminance = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                        if (luminance < 128)
                        {
                            slice |= (byte)(0x80 >> bit);
                        }
                    }

                    data.Add(slice);
                }
            }

            return data.ToArray();
        }
    }
}
