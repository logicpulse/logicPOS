using LogicPOS.DTOs.Printing;
using LogicPOS.Settings;

namespace LogicPOS.Data.XPO.Settings
{
    public static partial class TerminalSettings
    {
        public static class ThermalPrinter
        {
            public static int MaxCharsPerLineNormal
            {
                get
                {
                    if(HasLoggedTerminal == false)
                    {
                       return PrintingSettings.ThermalPrinter.MaxCharsPerLineNormal;
                    }

                    var result = LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormal;
                    return (result > 0) ? result : PrintingSettings.ThermalPrinter.MaxCharsPerLineNormal;
                }
            }

            public static int MaxCharsPerLineNormalBold
            {
                get
                {
                    if(HasLoggedTerminal == false)
                    {
                        return PrintingSettings.ThermalPrinter.MaxCharsPerLineNormalBold;
                    }

                    var result = LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineNormalBold;
                    return result > 0 ? result : PrintingSettings.ThermalPrinter.MaxCharsPerLineNormalBold;
                }
            }

            public static int MaxCharsPerLineSmall
            {
                get
                {
                    if(HasLoggedTerminal == false)
                    {
                        return PrintingSettings.ThermalPrinter.MaxCharsPerLineSmall;
                    }

                    var result = LoggedTerminal.ThermalPrinter.ThermalMaxCharsPerLineSmall;
                    return result > 0 ? result : PrintingSettings.ThermalPrinter.MaxCharsPerLineSmall;
                }
            }

            public static PrinterReferenceDto GetLoggedTerminalPrinterReference()
            {
                return new PrinterReferenceDto
                {
                    Id = LoggedTerminal.ThermalPrinter.Oid,
                    Designation = LoggedTerminal.ThermalPrinter.Designation,
                    NetworkName = LoggedTerminal.ThermalPrinter.NetworkName,
                    Token = LoggedTerminal.ThermalPrinter.PrinterType.Token
                };
            }
        }
    }
}
