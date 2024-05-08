using System.Collections.Generic;

namespace LogicPOS.Settings
{
    public static class PrintingSettings
    {
        public static bool PrintQRCode { get; set; } = true;
        public static Dictionary<string, string> FastReportSystemVars { get; set; }
        public static Dictionary<string, string> FastReportCustomVars { get; set; }
        public static bool UsingThermalPrinter { get; set; }
    }
}
