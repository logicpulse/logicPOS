using LogicPOS.Printing.Enums;
using LogicPOS.Printing.Utility;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace LogicPOS.Printing.Common
{
    public partial class ThermalPrinter
    {
        private readonly MemoryStream _memoryStream = new MemoryStream();
        public BinaryWriter BinaryStream { get; set; }

        /// <summary>
        /// Delay between two picture lines. (in ms)
        /// </summary>
        public int PictureLineSleepTimeInMs = 40;

        /// <summary>
        /// Delay between two text lines. (in ms)
        /// </summary>
        public int WriteLineSleepTimeInMs = 0;

        /// <summary>
        /// Current encoding used by the printer.
        /// </summary>
        public string Encoding { get; private set; }

        public ThermalPrinter(string encoding)
        {
            Encoding = encoding;
            BinaryStream = new BinaryWriter(this._memoryStream, new UTF8Encoding());
            Reset();
            SetEncoding(this.Encoding);
        }

        public void WriteLine(string text)
        {
            //martelado pelo carlos
            //SetFont(49);
            //
            WriteToBuffer(text);
            WriteByte(PrintingControlSignals.PrintAndLineFeed);
        }

        /// <summary>
        /// Sends the text to the printer buffer. Does not print until a line feed (0x10) is sent.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteToBuffer(string text)
        {
            string textStrip = ReplaceDiacritics(text);

            BinaryStream.Write(System.Text.Encoding.Default.GetBytes(textStrip));
        }

        public string ReplaceDiacritics(string source)
        {
            source = ReplaceDiacriticsChars(source);

            string sourceInFormD = source.Normalize(NormalizationForm.FormD);

            var output = new StringBuilder();
            foreach (char c in sourceInFormD)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    output.Append(c);
            }

            return (output.ToString().Normalize(NormalizationForm.FormC));
        }

        public string ReplaceDiacriticsChars(string source)
        {
            string result = source;

            try
            {
                Dictionary<char, char> replaceChars = new Dictionary<char, char>
                {
                    { 'º', 'o' },
                    { 'ª', 'a' }
                };

                foreach (var item in replaceChars)
                {
                    result = result.Replace(item.Key, item.Value);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// Prints the line of text, white on black.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_Invert(string text)
        {
            //Sets inversion on
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.ReversePrintingMode);
            WriteByte(1);

            //Sends the text
            WriteLine(text);

            //Sets inversion off
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.ReversePrintingMode);
            WriteByte(0);

            LineFeed();
        }

        /// <summary>
        /// Prints the line of text, double size.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_Big(string text)
        {
            const byte DoubleHeight = 1 << 4;
            const byte DoubleWidth = 1 << 5;
            const byte Bold = 1 << 3;

            //big on 
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(DoubleHeight + DoubleWidth + Bold);

            //Sends the text
            WriteLine(text);

            //big off
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(0);
        }

        /// <summary>
        /// Prints the line of text, double height.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_DoubleWidth(string text)
        {
            const byte DoubleWidth = 1 << 5;
            const byte Bold = 1 << 3;

            //big on 
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(DoubleWidth + Bold);

            //Sends the text
            WriteLine(text);

            //big off
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(0);
        }

        /// <summary>
        /// Prints the line of text, double height bold.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_DoubleWidthBold(string text)
        {
            const byte DoubleWidth = 1 << 5;
            const byte Bold = 1 << 3;

            //big on 
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(DoubleWidth + Bold);

            //bold on
            BoldOn();
            //Sends the text
            WriteLine(text);
            //bold on
            BoldOff();

            //big off
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(0);
        }

        /// <summary>
        /// Prints the line of text, double height.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_DoubleHeight(string text)
        {
            const byte DoubleHeight = 1 << 4;
            const byte Bold = 1 << 3;

            //big on 
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(DoubleHeight + Bold);

            //Sends the text
            WriteLine(text);

            //big off
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(0);
        }

        /// <summary>
        /// Prints the line of text, double height.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_DoubleHeightBold(string text)
        {
            const byte DoubleHeight = 1 << 4;
            const byte Bold = 1 << 3;

            //big on 
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(DoubleHeight + Bold);

            //bold on
            BoldOn();
            //Sends the text
            WriteLine(text);
            //bold on
            BoldOff();

            //big off
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(0);
        }

        /// <summary>
        /// Prints the line of text.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        /// <param name='style'>
        /// Style of the text.
        /// </param> 
        public void WriteLine(string text, PrintingStyle style)
        {
            WriteLine(text, (byte)style);
        }

        /// <summary>
        /// Prints the line of text.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        /// <param name='style'>
        /// Style of the text. Can be the sum of PrintingStyle enums.
        /// </param>
        public void WriteLine(string text, byte style)
        {
            byte underlineHeight = 0;

            if (_BitTest(style, 0))
            {
                style = _BitClear(style, 0);
                underlineHeight = 1;
            }

            if (_BitTest(style, 7))
            {
                style = _BitClear(style, 7);
                underlineHeight = 2;
            }

            if (underlineHeight != 0)
            {
                WriteByte(PrintingControlSignals.Escape);
                WriteByte(PrintingControlSignals.TurnUnderline);
                WriteByte(underlineHeight);
            }

            //style on
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte((byte)style);

            //Sends the text
            WriteLine(text);

            //style off
            if (underlineHeight != 0)
            {
                WriteByte(PrintingControlSignals.Escape);
                WriteByte(PrintingControlSignals.TurnUnderline);
                WriteByte(0);
            }

            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte(0);

        }

        /// <summary>
        /// Prints the line of text in bold.
        /// </summary>
        /// <param name='text'>
        /// Text to print.
        /// </param>
        public void WriteLine_Bold(string text)
        {
            //bold on
            BoldOn();

            //Sends the text
            WriteLine(text);

            //bold off
            BoldOff();

            //mario Commented : This always print extra Blank Line
            //LineFeed();
        }

        /// <summary>
        /// Sets bold mode on.
        /// </summary>
        public void BoldOn()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.CharacterSpacing);
            WriteByte(1);
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.TurnEmphasized);
            WriteByte(1);
        }

        /// <summary>
        /// Sets bold mode off.
        /// </summary>
        public void BoldOff()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.CharacterSpacing);
            WriteByte(0);
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.TurnEmphasized);
            WriteByte(0);
        }

        /// <summary>
        /// Sets white on black mode on.
        /// </summary>
        public void WhiteOnBlackOn()
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.ReversePrintingMode);
            WriteByte(1);
        }

        /// <summary>
        /// Sets white on black mode off.
        /// </summary>
        public void WhiteOnBlackOff()
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.ReversePrintingMode);
            WriteByte(0);
        }

        /// <summary>
        /// Sets the text size.
        /// </summary>
        /// <param name='doubleWidth'>
        /// Double width
        /// </param>
        /// <param name='doubleHeight'>
        /// Double height
        /// </param>
        public void SetSize(bool doubleWidth, bool doubleHeight)
        {
            int sizeValue = (Convert.ToInt32(doubleWidth)) * (0xF0) + (Convert.ToInt32(doubleHeight)) * (0x0F);
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte((byte)sizeValue);
        }

        public void SetSize2(int sizeValue) //0; 16;32; 48; 64; 80; 96; 112
        {
            WriteByte(PrintingControlSignals.SelectCharacterSize);
            WriteByte(PrintingControlSignals.SelectPrintMode);
            WriteByte((byte)sizeValue);
        }

        public void SetFont(int font)
        {
            WriteByte(27);
            WriteByte(77);
            WriteByte((byte)font);
        }

        ///	<summary>
        /// Prints the contents of the buffer and feeds one line.
        /// </summary>
        public void LineFeed()
        {
            WriteByte(PrintingControlSignals.PrintAndLineFeed);
        }

        ///	<summary>
        /// Prints the contents of the buffer and feeds one line.
        /// </summary>
        public void InvertText()
        {
            WriteByte(27);
            WriteByte(123);
            WriteByte(1);
        }

        /// <summary>
        /// Prints the contents of the buffer and feeds n lines.
        /// </summary>
        /// <param name='lines'>
        /// Number of lines to feed.
        /// </param>
        public void LineFeed(byte lines)
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.PrintAndFeedNLines);
            WriteByte(lines);
        }

        /// <summary>
        /// Idents the text.
        /// </summary>
        /// <param name='columns'>
        /// Number of columns.
        /// </param>
        public void Indent(byte columns)
        {
            if (columns < 0 || columns > 31)
            {
                columns = 0;
            }

            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.ReversePrintingMode);
            WriteByte(columns);
        }

        /// <summary>
        /// Sets the line spacing.
        /// </summary>
        /// <param name='lineSpacing'>
        /// Line spacing (in dots), default value: 32 dots.
        /// </param>
        public void SetLineSpacing(byte lineSpacing)
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SetLineSpacing);
            WriteByte(lineSpacing);
        }

        /// <summary>
        /// Aligns the text to the left.
        /// </summary>
        public void SetAlignLeft()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectJustification);
            WriteByte(0);
        }

        /// <summary>
        /// Centers the text.
        /// </summary>		
        public void SetAlignCenter()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectJustification);
            WriteByte(1);
        }

        /// <summary>
        /// Aligns the text to the right.
        /// </summary>
        public void SetAlignRight()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SelectJustification);
            WriteByte(2);
        }

        /// <summary>
        /// Prints a horizontal line.
        /// </summary>
        /// <param name='length'>
        /// Line length (in characters) (max 32).
        /// </param>
        public void HorizontalLine(int length)
        {
            if (length > 0)
            {
                if (length > 48)
                {
                    length = 48;
                }

                for (int i = 0; i < length; i++)
                {
                    BinaryStream.Write('-');
                }

                WriteByte(PrintingControlSignals.PrintAndLineFeed);
            }
        }

        /// <summary>
        /// Resets the printer.
        /// </summary>
        public void Reset()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.InitializePrinter);
            System.Threading.Thread.Sleep(50);
        }

        /// <summary>
        /// Prints the barcode data.
        /// </summary>
        /// <param name='type'>
        /// Type of barcode.
        /// </param>
        /// <param name='data'>
        /// Data to print.
        /// </param>
        public void PrintBarcode(BarcodeType type, string data)
        {
            byte[] originalBytes;
            byte[] outputBytes;

            if (type == BarcodeType.code93 || type == BarcodeType.code128)
            {
                originalBytes = System.Text.Encoding.UTF8.GetBytes(data);
                outputBytes = originalBytes;
            }
            else
            {
                originalBytes = System.Text.Encoding.UTF8.GetBytes(data.ToUpper());
                outputBytes = System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.UTF8, originalBytes);
            }

            switch (type)
            {
                case BarcodeType.upc_a:
                    if (data.Length == 11 || data.Length == 12)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(0);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.upc_e:
                    if (data.Length == 11 || data.Length == 12)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(1);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.ean13:
                    if (data.Length == 12 || data.Length == 13)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(2);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.ean8:
                    if (data.Length == 7 || data.Length == 8)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(3);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.code39:
                    if (data.Length > 1)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(4);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.i25:
                    if (data.Length > 1 || data.Length % 2 == 0)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(5);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.codebar:
                    if (data.Length > 1)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(6);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.code93: //TODO: overload PrintBarcode method with a byte array parameter
                    if (data.Length > 1)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(7); //TODO: use format 2 (init string : 29,107,72) (0x00 can be a value, too)
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.code128: //TODO: overload PrintBarcode method with a byte array parameter
                    if (data.Length > 1)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(8); //TODO: use format 2 (init string : 29,107,73) (0x00 can be a value, too)
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.code11:
                    if (data.Length > 1)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(9);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.msi:
                    if (data.Length > 1)
                    {
                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteByte(10);
                        WriteBytes(outputBytes);
                        WriteByte(0);
                    }
                    break;
                case BarcodeType.qrcode:
                    if (data.Length > 1)
                    {
                        int store_len = (data).Length + 3;
                        byte store_pL = (byte)(store_len % 256);
                        byte store_pH = (byte)(store_len / 256);

                        WriteByte(PrintingControlSignals.GroupSeparator);
                        WriteByte(PrintingControlSignals.PrintBarcode);
                        WriteBytes(new byte[] { 29, 40, 107, 4, 0, 49, 65, 50, 0 });
                        WriteBytes(new byte[] { 29, 40, 107, 4, 0, 49, 65, 50, 0 });
                        WriteBytes(new byte[] { 29, 40, 107, 3, 0, 49, 67, 8 });
                        WriteBytes(new byte[] { 29, 40, 107, 3, 0, 49, 69, 48 });
                        WriteBytes(new byte[] { 29, 40, 107, store_pL, store_pH, 49, 80, 48 });
                        WriteBytes(outputBytes);
                        WriteBytes(new byte[] { 29, 40, 107, 3, 0, 49, 81, 48 });
                        WriteByte(0);
                    }
                    break;
            }
        }

        /// <summary>
        /// Selects Width barcode.
        /// </summary>
        /// <param name="size">2 a 6 </param>
        public void SetBarcodeWidth(int size)
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.SetBarcodeWidth);
            WriteByte((byte)size);
        }

        /// <summary>
        /// Selects Height barcode.
        /// </summary>
        /// <param name="size">1 a 255</param>
        public void SetBarcodeHeight(int size)
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.SetBarcodeHeight);
            WriteByte((byte)size);
        }

        /// <summary>
        /// Sets the barcode left space.
        /// </summary>
        /// <param name='spacingDots'>
        /// Spacing dots.
        /// </param>
        [Obsolete("Cannot find in manual")]
        public void SetBarcodeLeftSpace(byte spacingDots)
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(120);
            WriteByte(spacingDots);
        }

        public void PrintImage(string bmpFilename, bool QrCode = false)
        {
            float widthBMP = QrCode ? 300 : 328;
            float heightBMP = QrCode ? 300 : 126;
            var brush = new SolidBrush(Color.White);

            if (!File.Exists(Path.GetDirectoryName(bmpFilename) + "\\" + Path.GetFileNameWithoutExtension(bmpFilename) + "_temp" + ".bmp"))
            {
                Image Dummy = Image.FromFile(bmpFilename);
                Dummy.Save("image.bmp", ImageFormat.Bmp);
                var fullPath = Path.GetDirectoryName(bmpFilename) + "\\" + Path.GetFileNameWithoutExtension(bmpFilename) + "_temp" + ".bmp";


                Bitmap myBitmap = new Bitmap(Dummy);
                if (!QrCode) ToGrayScale(myBitmap);


                var bmp = new Bitmap((int)widthBMP, (int)heightBMP);
                var graph = Graphics.FromImage(bmp);

                float scale = Math.Min(widthBMP / myBitmap.Width, heightBMP / myBitmap.Height);

                var scaleWidth = (int)(myBitmap.Width * scale);
                var scaleHeight = (int)(myBitmap.Height * scale);

                graph.FillRectangle(brush, new RectangleF(0, 0, widthBMP, heightBMP));
                graph.DrawImage(myBitmap, ((int)widthBMP - scaleWidth) / 2, ((int)heightBMP - scaleHeight) / 2, scaleWidth, scaleHeight);

                bmp.Save(Path.GetDirectoryName(bmpFilename) + "\\" + Path.GetFileNameWithoutExtension(bmpFilename) + "_temp" + ".bmp", ImageFormat.Bmp);
                bmpFilename = Path.GetDirectoryName(bmpFilename) + "\\" + Path.GetFileNameWithoutExtension(bmpFilename) + "_temp" + ".bmp";

            }
            else
            {
                bmpFilename = Path.GetDirectoryName(bmpFilename) + "\\" + Path.GetFileNameWithoutExtension(bmpFilename) + "_temp" + ".bmp";
            }
            var data = BitmapData.GetBitmapData(bmpFilename);
            var dots = data.Dots;
            var width = BitConverter.GetBytes(data.Width);

            // So we have our bitmap data sitting in a bit array called "dots."
            // This is one long array of 1s (black) and 0s (white) pixels arranged
            // as if we had scanned the bitmap from top to bottom, left to right.
            // The printer wants to see these arranged in bytes stacked three high.
            // So, essentially, we need to read 24 bits for x = 0, generate those
            // bytes, and send them to the printer, then keep increasing x. If our
            // image is more than 24 dots high, we have to send a second bit image
            // command.

            // Set the line spacing to 24 dots, the height of each "stripe" of the
            // image that we're drawing.
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SetLineSpacing);
            WriteByte((byte)24);

            // OK. So, starting from x = 0, read 24 bits down and send that data
            // to the printer.
            int offset = 0;

            while (offset < data.Height)
            {
                WriteByte(PrintingControlSignals.Escape);
                WriteByte(PrintingControlSignals.SelectBitImageMode);                // bit-image mode
                WriteByte(PrintingControlSignals.SelectPrintMode);                   // 24-dot double-density
                WriteByte((byte)width[0]);                                      // width low byte
                WriteByte((byte)width[1]);                                      // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;

                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;

                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }

                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        WriteByte(slice);
                    }
                }

                offset += 24;
                WriteByte(PrintingControlSignals.Newline);
            }

            // Restore the line spacing to the default of 30 dots.
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SetLineSpacing);
            WriteByte((byte)30);

        }

        public void PrintImage(Bitmap myBitmap)
        {

            var data = BitmapData.GetBitmapData(myBitmap);
            var dots = data.Dots;
            var width = BitConverter.GetBytes(data.Width);

            // So we have our bitmap data sitting in a bit array called "dots."
            // This is one long array of 1s (black) and 0s (white) pixels arranged
            // as if we had scanned the bitmap from top to bottom, left to right.
            // The printer wants to see these arranged in bytes stacked three high.
            // So, essentially, we need to read 24 bits for x = 0, generate those
            // bytes, and send them to the printer, then keep increasing x. If our
            // image is more than 24 dots high, we have to send a second bit image
            // command.

            // Set the line spacing to 24 dots, the height of each "stripe" of the
            // image that we're drawing.
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SetLineSpacing);
            WriteByte((byte)24);

            // OK. So, starting from x = 0, read 24 bits down and send that data
            // to the printer.
            int offset = 0;

            while (offset < data.Height)
            {
                WriteByte(PrintingControlSignals.Escape);
                WriteByte(PrintingControlSignals.SelectBitImageMode);                // bit-image mode
                WriteByte(PrintingControlSignals.SelectPrintMode);                   // 24-dot double-density
                WriteByte((byte)width[0]);                                      // width low byte
                WriteByte((byte)width[1]);                                      // width high byte

                for (int x = 0; x < data.Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;

                        for (int b = 0; b < 8; ++b)
                        {
                            int y = (((offset / 8) + k) * 8) + b;

                            // Calculate the location of the pixel we want in the bit array.
                            // It'll be at (y * width) + x.
                            int i = (y * data.Width) + x;

                            // If the image is shorter than 24 dots, pad with zero.
                            bool v = false;
                            if (i < dots.Length)
                            {
                                v = dots[i];
                            }

                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }

                        WriteByte(slice);
                    }
                }

                offset += 24;
                WriteByte(PrintingControlSignals.Newline);
            }

            // Restore the line spacing to the default of 30 dots.
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SetLineSpacing);
            WriteByte((byte)30);
        }

        public BitmapData GetBitmapData(Bitmap bmp) // (string bmpFileName)
        {
            //using (var bitmap = (Bitmap)Bitmap.FromFile(bmpFileName))
            using (var bitmap = bmp)
            {
                var threshold = 127;
                var index = 0;
                double multiplier = 570; // this depends on your printer
                double scale = (double)(multiplier / (double)bitmap.Width);
                int xheight = (int)(bitmap.Height * scale);
                int xwidth = (int)(bitmap.Width * scale);
                var dimensions = xwidth * xheight;
                var dots = new System.Collections.BitArray(dimensions);

                for (var y = 0; y < xheight; y++)
                {
                    for (var x = 0; x < xwidth; x++)
                    {
                        var _x = (int)(x / scale);
                        var _y = (int)(y / scale);
                        var color = bitmap.GetPixel(_x, _y);
                        var luminance = (int)(color.R * 0.3 + color.G * 0.59 + color.B * 0.11);
                        dots[index] = (luminance < threshold);
                        index++;
                    }
                }

                return new BitmapData()
                {
                    Dots = dots,
                    Height = (int)(bitmap.Height * scale),
                    Width = (int)(bitmap.Width * scale)
                };
            }
        }

        public void ToGrayScale(Bitmap Bmp)
        {
            int rgb;
            Color c;

            for (int y = 0; y < Bmp.Height; y++)
                for (int x = 0; x < Bmp.Width; x++)
                {
                    c = Bmp.GetPixel(x, y);
                    rgb = (int)Math.Round(.299 * c.R + .587 * c.G + .114 * c.B);
                    Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
                }
        }

        /// <summary>
        /// Sets the printing parameters.
        /// </summary>
        /// <param name='maxPrintingDots'>
        /// Max printing dots (0-255), unit: (n+1)*8 dots, default: 7 (beceause (7+1)*8 = 64 dots)
        /// </param>
        /// <param name='heatingTime'>
        /// Heating time (3-255), unit: 10µs, default: 80 (800µs)
        /// </param>
        /// <param name='heatingInterval'>
        /// Heating interval (0-255), unit: 10µs, default: 2 (20µs)
        /// </param>
        //public void SetPrintingParameters(byte maxPrintingDots, byte heatingTime, byte heatingInterval)
        //{
        //    WriteByte(AsciiControlChars.Escape);
        //    WriteByte(55);	
        //    WriteByte(maxPrintingDots);
        //    WriteByte(heatingTime);				
        //    WriteByte(heatingInterval);
        //}

        /// <summary>
        /// Sets the printer offine.
        /// </summary>
        public void Sleep()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(61);
            WriteByte(0);
        }

        /// <summary>
        /// Sets the printer online.
        /// </summary>		
        public void WakeUp()
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.SetPeripheralDevice);
            WriteByte(1);
        }

        //[Obsolete("Cannot find in manual")]

        public void Cut(bool full)
        {
            //Try to Get CutCommand from Config
            string printerThermalCutCommand = GeneralSettings.Settings["PrinterThermalCutCommand"];

            // Send Config CutCommand to Cut Overload
            Cut(full, printerThermalCutCommand);
        }

        public void Cut(bool full, string configCutCommand)
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.SelectCutModeAndCutPaper);

            //Use to convert
            //http://www.binaryhexconverter.com/decimal-to-hex-converter
            //http://www.binaryhexconverter.com/hex-to-decimal-converter
            //Ex.
            //Decimal Value (28) = Hexadecimal Value (1c)

            //Default CutCommand, used if dont have values from config or sent via parameter
            byte[] cutCommand = { 66, 0 };
            //string value = ASCIIEncoding.ASCII.GetString(cut);// { 66, 0 } => "B\0"
            //byte[] cutFromString = System.Text.Encoding.ASCII.GetBytes(value);

            if (configCutCommand != string.Empty)
            {
                if (configCutCommand != null)
                {
                    // Replace default cutCommand
                    //cutCommand = System.Text.Encoding.ASCII.GetBytes(printerThermalCutCommand);
                    cutCommand = configCutCommand.Split(new[] { ',' }).Select(s => Convert.ToByte(s, 16)).ToArray();
                }
            };

            //WriteByte(86);
            WriteBytes(cutCommand);

            //Custom TG2460H - FULL CUT
            //current
            //WriteByte((byte)28);//0x1C
            //WriteByte((byte)192);//0xC0
            //WriteByte((byte)52);//0x34

            //Other#1 - P12-USL, TM-T70 : Decimal 29 66 n : Acores
            //WriteByte((byte)66);0x42
            //WriteByte((byte)0);0x00

            //Other#2
            //WriteByte((byte)29);//0x1D
            //WriteByte((byte)248);//0xF8
            //WriteByte((byte)33);//0x1B

            //Other#3
            //WriteByte((byte)65);//0x41
            //WriteByte((byte)16);//0x10
            //WriteByte((byte)29);//0x1D
            //WriteByte((byte)248);//0xF8

            //Other#4 - Try for TM-T70 : Decimal 27 25 52 
            //WriteByte((byte)27);
            //WriteByte((byte)25);
            //WriteByte((byte)52);
        }

        /// <summary>
        /// FeedVerticalAndCut
        /// </summary>
        /// <param name="m"> 0 to 255</param>
        public void FeedVerticalAndCut(int m)
        {
            // Feed 3 vertical motion units and cut the paper with a 1 point cut
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte(PrintingControlSignals.SelectCutModeAndCutPaper);
            WriteByte((byte)66);
            WriteByte((byte)m);
        }

        /// <summary>
        /// Prints the contents of the buffer and feeds n dots.
        /// </summary>
        /// <param name='dotsToFeed'>
        /// Number of dots to feed.
        /// </param>
        public void FeedDots(byte dotsToFeed)
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.PrintAndFeedPaper);
            WriteByte(dotsToFeed);
        }

        public void SetEncoding(string encoding)
        {
            switch (encoding)
            {
                case "IBM437":
                    WriteByte(PrintingControlSignals.Escape);
                    WriteByte(PrintingControlSignals.SelectCharacterCodeTable);
                    WriteByte(0);
                    break;
                case "PC850":
                    WriteByte(PrintingControlSignals.Escape);
                    WriteByte(PrintingControlSignals.SelectCharacterCodeTable);
                    WriteByte(2);
                    break;
                case "PC860":
                    WriteByte(PrintingControlSignals.Escape);
                    WriteByte(PrintingControlSignals.SelectCharacterCodeTable);
                    WriteByte(3);
                    break;
            }
        }

        /// <summary>
        /// Output to connector realtime pulse
        /// </summary>
        /// <param name="connector">0 or 1</param>
        /// <param name="pulse">1 to 8</param>
        public void GeneratePulseRealtime(int connector, int pulse)
        {
            WriteByte(PrintingControlSignals.DLE);
            WriteByte(PrintingControlSignals.GeneratePulseRealtime);
            WriteByte((byte)1);
            WriteByte((byte)connector);
        }

        /// <summary>
        /// Output to connector pulse
        /// </summary>
        /// <param name="m">0, 1, 48, 49</param>
        /// <param name="t1">0,48</param>
        /// <param name="t2">1,49</param>
        public void GeneratePulse(int m, int t1, int t2)
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(PrintingControlSignals.GeneratePulse);
            WriteByte((byte)m);
            WriteByte((byte)t1);
            WriteByte((byte)t2);
        }

        /// <summary>
        /// Enable/disable panel button
        /// </summary>
        /// <param name="set"> LSB is 0 - enable; LSB is 1 - disable</param>
        public void SetPanelButton(int set)
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(99);
            WriteByte(53);
            WriteByte((byte)set);
        }

        /// <summary>
        /// Setup upside-down print mode
        /// </summary>
        /// <param name="set">LSB is 0 - off; LSB is 1 - on</param>
        public void SetupUpsideDownPrinting(int set)
        {
            WriteByte(PrintingControlSignals.Escape);
            WriteByte(123);
            WriteByte((byte)set);
        }

        /// <summary>
        /// When printing barcode select printing position of human readable characters
        /// </summary>
        /// <param name="set">0,1,2,3</param>
        public void SelectPrintingPositionHRIBarcode(int set)
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte((byte)72);
            WriteByte((byte)set);
        }

        /// <summary>
        /// select font for printing barcode human readable characters
        /// </summary>
        /// <param name="set">0,1</param>
        public void SelectFontHRIBarcode(int set)
        {
            WriteByte(PrintingControlSignals.GroupSeparator);
            WriteByte((byte)102);
            WriteByte((byte)set);
        }

        /// <summary>
        /// Tests the value of a given bit.
        /// </summary>
        /// <param name="valueToTest">The value to test</param>
        /// <param name="testBit">The bit number to test</param>
        /// <returns></returns>
        static private bool _BitTest(byte valueToTest, int testBit)
        {
            return ((valueToTest & (byte)(1 << testBit)) == (byte)(1 << testBit));
        }

        /// <summary>
        /// Return the given value with its n bit cleared.
        /// </summary>
        /// <param name="originalValue">The value to return</param>
        /// <param name="bit">The bit number to clear</param>
        /// <returns></returns>
        static private byte _BitClear(byte originalValue, int bit)
        {
            return originalValue &= (byte)(~(1 << bit));
        }

        protected void WriteByte(byte valueToWrite)
        {
            byte[] tempArray = { valueToWrite };
            BinaryStream.Write(tempArray);
        }

        private void WriteBytes(byte[] valueToWrite)
        {
            BinaryStream.Write(valueToWrite);
        }

        public byte[] GetByteArray()
        {
            BinaryStream.Flush();
            return _memoryStream.ToArray();
        }

    }
}

