using System.Collections;
using System.Drawing;

namespace LogicPOS.Printing.Common
{
    public class BitmapData
    {
        public BitArray Dots { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public static BitmapData GetBitmapData(string fileName)
        {
            return GetBitmapData((Bitmap)Image.FromFile(fileName));
        }

        public static BitmapData GetBitmapData(Bitmap bitMap)
        {
            using (var bitmap = bitMap)
            {
                var threshold = 127;
                var index = 0;
                var dimensions = bitmap.Width * bitmap.Height;
                var dots = new BitArray(dimensions);

                for (var y = 0; y < bitmap.Height; y++)
                {
                    for (var x = 0; x < bitmap.Width; x++)
                    {
                        var color = bitmap.GetPixel(x, y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = bitmap.Height,
                    Width = bitmap.Width
                };
            }
        }
    }
}
