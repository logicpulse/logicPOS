using System;
using System.Collections.Generic;

namespace LogicPOS.Settings
{
    public static class PrintingSettings
    {
        public static bool PrintQRCode { get; set; } = true;
        public static Dictionary<string, string> FastReportSystemVars { get; set; }
        public static Dictionary<string, string> FastReportCustomVars { get; set; }
        public static bool UsingThermalPrinter { get; set; }

        //THERMAL_PRINTER_WINDOWS
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterWindows { get; set; } = new Guid("e7143ea5-391b-46ef-a28d-4843fd7e21ac");
        //THERMAL_PRINTER_SOCKET
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterSocket { get; set; } = new Guid("faeb45cd-2989-4e92-9907-3038444e4849");
        //GENERIC_PRINTER_WINDOWS
        public static Guid XpoOidConfigurationPrinterTypeGenericWindows { get; set; } = new Guid("4be662e6-67c9-4063-bd6c-574ae4df7f3f");
        //REPORT_EXPORT_PDF
        public static Guid XpoOidConfigurationPrinterTypeExportPdf { get; set; } = new Guid("e5e20cd0-d9d2-443d-9d3f-3478949db30f");
        //THERMAL _PRINTER_USB
        public static Guid XpoOidConfigurationPrinterTypeThermalPrinterUsb { get; set; } = new Guid("39b58a4e-b860-49c1-81a5-8bb5f7186940");
        //Modelo para impressão de Fecho de Dia/Caixa
        public static Guid XpoOidConfigurationPrintersTemplateWorkSessionMovement { get; set; } = new Guid("f6a25476-40b0-4287-9284-d5db3280d7f1");
        //Modelo para impressão de Entradas/Saidas de Numerário
        public static Guid XpoOidConfigurationPrintersTemplateCashDrawerOpenAndMoneyInOut { get; set; } = new Guid("f6565476-28b0-4287-9284-d5db3280d421");
    }
}
