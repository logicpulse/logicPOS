using System;
using System.Drawing;
using System.IO;

namespace logicpos
{
    class UtilsGraphic
    {
        public static void DrawLine(Graphics pGraphics, Pen pPen, Point pPoint1, Point pPoint2)
        {
            pGraphics.DrawLine(pPen, pPoint1, pPoint2);
        }

        public static void DrawRectangle(Graphics pGraphics, Brush pBrush, Pen pPen, int pX, int pY, int pWidth, int pHeight, bool pFill)
        {
            Rectangle rectangle = new Rectangle(pX, pY, pWidth, pHeight);
            if (pFill)
            {
                if (pBrush != null)
                {
                    pGraphics.FillRectangle(pBrush, rectangle);
                }
            }
            if (pPen != null)
            {
                pGraphics.DrawRectangle(pPen, rectangle);
            }
        }

        public static void DrawEllipse(Graphics pGraphics, Pen pPen, int pX, int pY, int pWidth, int pHeight)
        {
            Rectangle rectangle = new Rectangle(pX, pY, pWidth, pHeight);
            pGraphics.DrawEllipse(pPen, rectangle);
        }

        public static void DrawImageUnscaled(Graphics pGraphics, string pFileName, int pX, int pY)
        {
            Image image = Image.FromFile(pFileName);
            Point point = new Point(pX, pY);
            pGraphics.DrawImageUnscaled(image, point);
        }

        public static void DrawImage(Graphics pGraphics, string pFileName, int pX, int pY, int pWidth, int pHeight)
        {
            if (File.Exists(pFileName))
            {
                Image image = Image.FromFile(pFileName);
                Rectangle rectangle = new Rectangle(pX, pY, pWidth, pHeight);
                pGraphics.DrawImage(image, rectangle);
            }
        }

        //Returns Height of Text
        public static int DrawStringMultiLine(Graphics pGraphics, string pText, int pX, int pY, int pLineWidth, string pFontName, int pFontSize, FontStyle pFontStyle, StringAlignment pStringAlignment, Color pBrushColor)
        {

            StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
            stringFormat.Alignment = pStringAlignment;

            Brush brush = new SolidBrush(pBrushColor);

            //GraphicsUnit.Pixel Required for working with DPI
            Font font = new Font(pFontName, pFontSize, pFontStyle, GraphicsUnit.Pixel);
            SizeF sizeF = pGraphics.MeasureString(pText, font, pLineWidth);
            RectangleF rectangleF = new RectangleF(pX, pY, sizeF.Width, sizeF.Height);


            pGraphics.DrawString(pText, font, brush, rectangleF, stringFormat);

            return Convert.ToInt16(sizeF.Height);
        }
    }
}
