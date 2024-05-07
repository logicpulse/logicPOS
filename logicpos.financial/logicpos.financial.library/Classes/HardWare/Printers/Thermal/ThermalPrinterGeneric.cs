using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.Classes.Hardware.Printers.Thermal.Enums;
using logicpos.printer.generic;
using logicpos.shared.App;
using System;

namespace logicpos.financial.library.Classes.Hardware.Printers.Thermal
{
    public class ThermalPrinterGeneric : ThermalPrinter
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Object Fields        
        private readonly string _encoding = string.Empty;
        private readonly string _line = string.Empty;
        private readonly char _lineChar = '-';

        public sys_configurationprinters Printer { get; set; }
        private int _maxCharsPerLineNormal = 0;
        public int MaxCharsPerLineNormal
        {
            get { return _maxCharsPerLineNormal; }
            set { _maxCharsPerLineNormal = value; }
        }

        public int MaxCharsPerLineNormalBold { get; set; } = 0;

        public int MaxCharsPerLineSmall { get; set; }

        public ThermalPrinterGeneric(sys_configurationprinters pPrinter)
            : this(pPrinter, SharedSettings.PrinterThermalEncoding)
        {
        }

        public ThermalPrinterGeneric(sys_configurationprinters pPrinter, string pEncoding)
            // Old HardCoded Method Settings
            //TK016249 - Impressoras - Diferenciação entre Tipos
            //: this(pPrinter, pEncoding, SettingsApp.PrinterThermalMaxCharsPerLineNormal, SettingsApp.PrinterThermalMaxCharsPerLineNormalBold, SettingsApp.PrinterThermalMaxCharsPerLineSmall)
            : this(pPrinter, pEncoding,
                  (XPOSettings.LoggedTerminal.ThermalPrinter != null && XPOSettings.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormal > 0) ? XPOSettings.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormal : SharedSettings.PrinterThermalMaxCharsPerLineNormal,
                  (XPOSettings.LoggedTerminal.ThermalPrinter != null && XPOSettings.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormalBold > 0) ? XPOSettings.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormalBold : SharedSettings.PrinterThermalMaxCharsPerLineNormalBold,
                  (XPOSettings.LoggedTerminal.ThermalPrinter != null && XPOSettings.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineSmall > 0) ? XPOSettings.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineSmall : SharedSettings.PrinterThermalMaxCharsPerLineSmall
                  )
        {
        }

        public ThermalPrinterGeneric(sys_configurationprinters pPrinter, string pEncoding, int pMaxCharsPerLineNormal, int pMaxCharsPerLineNormalBold, int pMaxCharsPerLineSmall)
            : base(pEncoding)
        {
            //Parameters
            Printer = pPrinter;
            _encoding = pEncoding;
            _maxCharsPerLineNormal = pMaxCharsPerLineNormal;
            MaxCharsPerLineNormalBold = pMaxCharsPerLineNormalBold;
            MaxCharsPerLineSmall = pMaxCharsPerLineSmall;
            //Other
            _line = new string(_lineChar, _maxCharsPerLineNormal);
        }

        public void PrintBuffer()
        {
            try
            {

                switch (Printer.PrinterType.Token)
                {
                    case "THERMAL_PRINTER_WINDOWS":
                        printer.genericusb.Print.USBPrintWindows(Printer.Designation, getByteArray());
                        break;
                    case "THERMAL_PRINTER_SOCKET":
                        printer.genericusb.Print.USBPrintWindows(Printer.NetworkName, getByteArray());
                        break;
                }


            }
            catch (Exception ex)
            {
                _logger.Error("void PrintBuffer() :: " + ex.Message, ex);
                throw ex;
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
        public new void WriteLine(string pLine, ThermalPrinter.PrintingStyle pPrintingStyle)
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
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _maxCharsPerLineNormal));
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, _maxCharsPerLineNormal), (byte)PrintingStyle.Bold);
            WriteLine("ÁÉÍÓÚ-1234567890-1234567890-1234567890-1234567890-1234567890-1234567890".Substring(0, MaxCharsPerLineSmall), (byte)PrintingStyle.DoubleWidth);
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
