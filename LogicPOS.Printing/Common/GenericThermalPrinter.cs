using LogicPOS.Data.XPO.Settings;
using LogicPOS.DTOs.Printing;
using LogicPOS.Printing.Enums;
using LogicPOS.Settings;

namespace LogicPOS.Printing.Common
{
    public class GenericThermalPrinter : ThermalPrinter
    {
        private readonly string _encoding = string.Empty;
        private readonly string _line = string.Empty;
        private readonly char _lineChar = '-';

        public PrintingPrinterDto Printer { get; private set; }

        public int MaxCharsPerLineNormal { get; set; }
        public int MaxCharsPerLineNormalBold { get; set; }
        public int MaxCharsPerLineSmall { get; set; }

        public GenericThermalPrinter(
            PrintingPrinterDto printer)
            : this(
                  printer,
                  PrintingSettings.ThermalPrinter.Encoding)
        {
        }

        public GenericThermalPrinter(
            PrintingPrinterDto printer,
            string encoding)
            : this(
                  printer,
                  encoding,
                  TerminalSettings.ThermalPrinter.MaxCharsPerLineNormal,
                  TerminalSettings.ThermalPrinter.MaxCharsPerLineNormalBold,
                  TerminalSettings.ThermalPrinter.MaxCharsPerLineSmall)
        {
        }

        public GenericThermalPrinter(
            PrintingPrinterDto printer,
            string encoding,
            int maxCharsPerLineNormal,
            int maxCharsPerLineNormalBold,
            int maxCharsPerLineSmall)
            : base(encoding)
        {
            Printer = printer;
            _encoding = encoding;
            MaxCharsPerLineNormal = maxCharsPerLineNormal;
            MaxCharsPerLineNormalBold = maxCharsPerLineNormalBold;
            MaxCharsPerLineSmall = maxCharsPerLineSmall;
            _line = new string(_lineChar, MaxCharsPerLineNormal);
        }

        public void PrintBuffer()
        {
            switch (Printer.Token)
            {
                case "THERMAL_PRINTER_WINDOWS":
                    Usb.Print.USBPrintWindows(Printer.Designation, GetByteArray());
                    break;
                case "THERMAL_PRINTER_SOCKET":
                    Usb.Print.USBPrintWindows(Printer.NetworkName, GetByteArray());
                    break;
            }
        }

        //Override base method
        public new void WriteLine(string pLine)
        {
            if (!string.IsNullOrEmpty(pLine))
            {
                base.WriteLine(pLine);
            }
        }

        /// <summary>
        /// Prints the line when value is not empty or even the value is empty when "pPrintIfValueIsEmpty" is set to true.
        /// </summary>
        /// <param name="pLine"></param>
        /// <param name="pPrintIfValueIsEmpty"></param>
        public void WriteLine(string pLine, bool pPrintIfValueIsEmpty)
        {
            /* IN009055 */
            if (!string.IsNullOrEmpty(pLine) || pPrintIfValueIsEmpty)
            {
                base.WriteLine(pLine);
            }
        }

        //Used to Send Label + Value (pPrintIfValueIsEmpty if true, prints Label even if Value is Empty ex "Name: ")
        public void WriteLine(string pLabel, string pValue, bool pPrintIfValueIsEmpty = true)
        {
            if (!string.IsNullOrEmpty(pLabel) && (!string.IsNullOrEmpty(pValue) || pPrintIfValueIsEmpty))
            {
                base.WriteLine(string.Format("{0}: {1}", pLabel, pValue)); /* IN009055 */
            }
        }

        //Override base method
        public new void WriteLine(string pLine, PrintingStyle pPrintingStyle)
        {
            if (!string.IsNullOrEmpty(pLine))
            {
                base.WriteLine(pLine, pPrintingStyle);
            }
        }

        public void WriteLine(string pLine, WriteLineTextMode pTextMode)
        {
            if (!string.IsNullOrEmpty(pLine))
            {
                switch (pTextMode)
                {
                    case WriteLineTextMode.Normal:
                        base.WriteLine(pLine);
                        break;
                    case WriteLineTextMode.Big:
                        WriteLine_Big(pLine);
                        break;
                    case WriteLineTextMode.Small:
                        SetFont(1);
                        base.WriteLine(pLine);
                        SetFont(0);
                        break;
                    case WriteLineTextMode.DoubleWidth:
                        WriteLine_DoubleWidth(pLine);
                        break;
                    case WriteLineTextMode.DoubleHeight:
                        WriteLine_DoubleHeight(pLine);
                        break;
                    case WriteLineTextMode.DoubleWidthBold:
                        WriteLine_DoubleWidthBold(pLine);
                        break;
                    case WriteLineTextMode.DoubleHeightBold:
                        WriteLine_DoubleHeightBold(pLine);
                        break;
                    case WriteLineTextMode.Bold:
                        WriteLine_Bold(pLine);
                        break;
                    case WriteLineTextMode.Invert:
                        WriteLine_Invert(pLine);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Test()
        {
            WriteLine("LINE");
            WriteLine(_line);

            WriteToBuffer("X1");
            WriteToBuffer("X2");
            WriteToBuffer("X3");
            LineFeed();

            WriteLine(_line);
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, MaxCharsPerLineNormal));
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, MaxCharsPerLineNormal), (byte)PrintingStyle.Bold);
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, MaxCharsPerLineSmall), (byte)PrintingStyle.DoubleWidth);
            WriteLine("LINE");
            LineFeed();
            Cut(true);
        }

        public static string TextJustified(string pLeft, string pRight, int pMaxPerLine)
        {
            return TextJustified(pLeft, pRight, pMaxPerLine, "{0,-10}{1,10}");
        }

        public static string TextJustified(string pLeft, string pRight, int pMaxPerLine, string pFormat)
        {
            return string.Format(pFormat, pLeft, pRight);
        }

        public static string TextCentered(string stringToCenter, int pCharactersPerLine)
        {
            return stringToCenter.PadLeft(((pCharactersPerLine - stringToCenter.Length) / 2) + stringToCenter.Length).PadRight(pCharactersPerLine);
        }

        public static string TextCentered(string stringToCenter, int totalLength, char paddingCharacter)
        {
            return stringToCenter.PadLeft(((totalLength - stringToCenter.Length) / 2) + stringToCenter.Length, paddingCharacter).PadRight(totalLength, paddingCharacter);
        }
    }
}
