using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.printer.generic;
using System;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal
{
    public class ThermalPrinterGeneric : ThermalPrinter
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Object Fields        
        private string _encoding = string.Empty;
        private string _line = string.Empty;
        private char _lineChar = '-';
        //Public
        private SYS_ConfigurationPrinters _printer;
        public SYS_ConfigurationPrinters Printer
        {
            get { return _printer; }
            set { _printer = value; }
        }
        private int _maxCharsPerLineNormal = 0;
        public int MaxCharsPerLineNormal
        {
            get { return _maxCharsPerLineNormal; }
            set { _maxCharsPerLineNormal = value; }
        }
        private int _maxCharsPerLineNormalBold = 0;
        public int MaxCharsPerLineNormalBold
        {
            get { return _maxCharsPerLineNormalBold; }
            set { _maxCharsPerLineNormalBold = value; }
        }
        private int _maxCharsPerLineSmall;
        public int MaxCharsPerLineSmall
        {
            get { return _maxCharsPerLineSmall; }
            set { _maxCharsPerLineSmall = value; }
        }

        public ThermalPrinterGeneric(SYS_ConfigurationPrinters pPrinter)
            : this(pPrinter, SettingsApp.PrinterThermalEncoding)
        {
        }

        public ThermalPrinterGeneric(SYS_ConfigurationPrinters pPrinter, string pEncoding)
            : this(pPrinter, pEncoding, SettingsApp.PrinterThermalMaxCharsPerLineNormal, SettingsApp.PrinterThermalMaxCharsPerLineNormalBold, SettingsApp.PrinterThermalMaxCharsPerLineSmall)
        {
        }

        public ThermalPrinterGeneric(SYS_ConfigurationPrinters pPrinter, string pEncoding, int pMaxCharsPerLineNormal, int pMaxCharsPerLineNormalBold, int pMaxCharsPerLineSmall)
            : base(pEncoding)
        {
            //Parameters
            _printer = pPrinter;
            _encoding = pEncoding;
            _maxCharsPerLineNormal = pMaxCharsPerLineNormal;
            _maxCharsPerLineNormalBold = pMaxCharsPerLineNormalBold;
            _maxCharsPerLineSmall = pMaxCharsPerLineSmall;
            //Other
            _line = new String(_lineChar, _maxCharsPerLineNormal);
        }

        public void PrintBuffer()
        {
            switch (_printer.PrinterType.Token)
            {
                case "THERMAL_PRINTER_WINDOWS":
                    printer.genericwindows.Print.WindowsPrint(_printer.NetworkName, getByteArray());
                    break;
                case "THERMAL_PRINTER_LINUX":
                    printer.genericlinux.Print.LinuxPrint(_printer.NetworkName, getByteArray());
                    break;
                case "THERMAL_PRINTER_SOCKET":
                    printer.genericsocket.Print.SocketPrint(_printer.NetworkName, getByteArray());
                    break;
                case "THERMAL_PRINTER_USB":
                    printer.genericusb.Print.USBPrint(_printer.NetworkName, getByteArray());
                    break;
            }
        }

        //Override base method
        public new void WriteLine(string pLine)
        {
            if (! string.IsNullOrEmpty(pLine))
            {
                base.WriteLine(pLine);
            }
        }

        public void WriteLine(string pLine, bool pPrintIfValueIsEmpty)
        {
            if (! string.IsNullOrEmpty(pLine) || (! string.IsNullOrEmpty(pLine) && pPrintIfValueIsEmpty == true))
            {
                base.WriteLine(pLine);
            }
        }

        //Used to Send Label + Value (pPrintIfValueIsEmpty if true, prints Label even if Value is Empty ex "Name: ")
        public void WriteLine(string pLabel, string pValue, bool pPrintIfValueIsEmpty = true)
        {
            if (! string.IsNullOrEmpty(pLabel) && (! string.IsNullOrEmpty(pValue) || pPrintIfValueIsEmpty))
            {
                base.WriteLine(string.Format("{0} : {1}", pLabel, pValue));
            }
        }

        //Override base method
        public new void WriteLine(string pLine, ThermalPrinter.PrintingStyle pPrintingStyle)
        {
            if (! string.IsNullOrEmpty(pLine))
            {
                base.WriteLine(pLine, pPrintingStyle);
            }
        }

        public void WriteLine(string pLine, WriteLineTextMode pTextMode)
        {
            if (! string.IsNullOrEmpty(pLine))
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
                        base.WriteLine_DoubleWidth(pLine);
                        break;
                    case WriteLineTextMode.DoubleHeight:
                        base.WriteLine_DoubleHeight(pLine);
                        break;
                    case WriteLineTextMode.DoubleWidthBold:
                        base.WriteLine_DoubleWidthBold(pLine);
                        break;
                    case WriteLineTextMode.DoubleHeightBold:
                        base.WriteLine_DoubleHeightBold(pLine);
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
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _maxCharsPerLineNormal));
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _maxCharsPerLineNormal), (byte)ThermalPrinter.PrintingStyle.Bold);
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _maxCharsPerLineSmall), (byte)ThermalPrinter.PrintingStyle.DoubleWidth);
            WriteLine("LINE");
            LineFeed();
            Cut(true);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helpers

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
