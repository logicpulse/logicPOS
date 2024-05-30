using LogicPOS.Data.XPO.Utility;
using LogicPOS.DTOs.Printing;

namespace LogicPOS.Data.XPO.Settings.Terminal
{
    public static class LoggedTerminalSettings
    {
        public static bool HasPrinter => TerminalSettings.LoggedTerminal.Printer != null;

        public static PrintingPrinterDto GetPrinterDto()
        {
            return MappingUtils.GetPrinterDto(TerminalSettings.LoggedTerminal.ThermalPrinter);
        }

        public static ThermalPrinterOpenDrawerValues GetThermalPrinterOpenDrawerValues()
        {
            return new ThermalPrinterOpenDrawerValues
            {
                M = TerminalSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueM,
                T1 = TerminalSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueT1,
                T2 = TerminalSettings.LoggedTerminal.ThermalPrinter.ThermalOpenDrawerValueT2
            };
        }
    }
}
