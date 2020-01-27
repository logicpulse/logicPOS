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
        private sys_configurationprinters _printer;
        public sys_configurationprinters Printer
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

        public ThermalPrinterGeneric(sys_configurationprinters pPrinter)
            : this(pPrinter, SettingsApp.PrinterThermalEncoding)
        {
        }

        public ThermalPrinterGeneric(sys_configurationprinters pPrinter, string pEncoding)
            // Old HardCoded Method Settings
			//TK016249 - Impressoras - Diferenciação entre Tipos
            //: this(pPrinter, pEncoding, SettingsApp.PrinterThermalMaxCharsPerLineNormal, SettingsApp.PrinterThermalMaxCharsPerLineNormalBold, SettingsApp.PrinterThermalMaxCharsPerLineSmall)
            : this(pPrinter, pEncoding,
                  (GlobalFramework.LoggedTerminal.ThermalPrinter != null && GlobalFramework.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormal > 0) ? GlobalFramework.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormal : SettingsApp.PrinterThermalMaxCharsPerLineNormal,
                  (GlobalFramework.LoggedTerminal.ThermalPrinter != null && GlobalFramework.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormalBold > 0) ? GlobalFramework.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormalBold : SettingsApp.PrinterThermalMaxCharsPerLineNormalBold,
                  (GlobalFramework.LoggedTerminal.ThermalPrinter != null && GlobalFramework.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineSmall > 0) ? GlobalFramework.LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineSmall : SettingsApp.PrinterThermalMaxCharsPerLineSmall
                  )
        {
        }

        public ThermalPrinterGeneric(sys_configurationprinters pPrinter, string pEncoding, int pMaxCharsPerLineNormal, int pMaxCharsPerLineNormalBold, int pMaxCharsPerLineSmall)
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
            try
            {
				//TK016310 Configuração Impressoras Windows 
                if (IsLinux)
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
                    }
                }
                else
                {
                    switch (_printer.PrinterType.Token)
                    {
                        case "THERMAL_PRINTER_WINDOWS":
                            printer.genericusb.Print.USBPrintWindows(_printer.Designation, getByteArray());
                            break;
                        case "THERMAL_PRINTER_LINUX":
                            printer.genericusb.Print.USBPrintWindows(_printer.NetworkName, getByteArray());
                            break;
                        case "THERMAL_PRINTER_SOCKET":
                            printer.genericusb.Print.USBPrintWindows(_printer.NetworkName, getByteArray());
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                _log.Error("void PrintBuffer() :: " + ex.Message, ex);
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
		
		//TK016310 Configuração Impressoras Windows 
        //Verifica se é linux
        public static bool IsLinux
        {
            get
            {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }
    }
}
