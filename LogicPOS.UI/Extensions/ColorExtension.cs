using System;
using System.ComponentModel;
using System.Drawing;

namespace logicpos.Extensions
{
    public static class ColorExtension
    {
        public static string ToHex(this Color color)
        {
            string red = Convert.ToString(color.R, 16);
            if (red.Length < 2) red = "0" + red;
            string green = Convert.ToString(color.G, 16);
            if (green.Length < 2) green = "0" + green;
            string blue = Convert.ToString(color.B, 16);
            if (blue.Length < 2) blue = "0" + blue;

            return red.ToUpper() + green.ToUpper() + blue.ToUpper();
        }

        public static Color HexToColor(this string hexString)
        {
            if (hexString.Length != 6)
            {
                return Color.Black;
            }

            string rText, gText, bText;
            int r, g, b;

            rText = hexString.Substring(0, 2);
            gText = hexString.Substring(2, 2);
            bText = hexString.Substring(4, 2);

            r = int.Parse(rText, System.Globalization.NumberStyles.HexNumber);
            g = int.Parse(gText, System.Globalization.NumberStyles.HexNumber);
            b = int.Parse(bText, System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(r, g, b);
        }

        public static Color AlphaHexToColor(this string hexString)
        {
            int tmpCutFirstChars = 0;
            string tmpAlfa, tmpRed, tmpGreen, tmpBlue;
            int tmpAlfaValue = 0, tmpRedValue, tmpGreenValue, tmpBlueValue;

            if (hexString.StartsWith("#"))
            {
                tmpCutFirstChars++;
            }

            if (hexString.Length == 9)
            {
                tmpAlfa = hexString.Substring(tmpCutFirstChars, 2);

                tmpAlfaValue = int.Parse(tmpAlfa, System.Globalization.NumberStyles.HexNumber);
                tmpCutFirstChars += 2;
            }

            tmpRed = hexString.Substring(0 + tmpCutFirstChars, 2);
            tmpGreen = hexString.Substring(2 + tmpCutFirstChars, 2);
            tmpBlue = hexString.Substring(4 + tmpCutFirstChars, 2);

            tmpRedValue = int.Parse(tmpRed, System.Globalization.NumberStyles.HexNumber);
            tmpGreenValue = int.Parse(tmpGreen, System.Globalization.NumberStyles.HexNumber);
            tmpBlueValue = int.Parse(tmpBlue, System.Globalization.NumberStyles.HexNumber);

            return Color.FromArgb(tmpAlfaValue, tmpRedValue, tmpGreenValue, tmpBlueValue);
        }

        public static Color Lighten(this Color color, float correctionFactor = 0.25f)
        {
            float red = (255 - color.R) * correctionFactor + color.R;
            float green = (255 - color.G) * correctionFactor + color.G;
            float blue = (255 - color.B) * correctionFactor + color.B;

            if (red > 255) red = 0;
            if (green > 255) green = 0;
            if (blue > 255) blue = 0;

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public static Color Darken(this Color color, float correctionFactor = 0.25f)
        {
            float red = -(255 - color.R) * correctionFactor + color.R;
            float green = -(255 - color.G) * correctionFactor + color.G;
            float blue = -(255 - color.B) * correctionFactor + color.B;

            if (red < 0) red = 0;
            if (green < 0) green = 0;
            if (blue < 0) blue = 0;

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public static Gdk.Color ToGdkColor(this System.Drawing.Color color)
        {
            return new Gdk.Color(color.R, color.G, color.B);
        }

        public static Color StringToColor(this string color)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(Color));
                return (Color)converter.ConvertFromInvariantString(color);
            }
            catch (Exception)
            {
                return (Color)Color.FromArgb(255, 0, 0);
            }
        }

        public static Gdk.Color StringToGdkColor(this string color)
        {
            return color.StringToColor().ToGdkColor();
        }
    }
}
