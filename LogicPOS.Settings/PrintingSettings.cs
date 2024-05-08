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

        public static Guid XpoOidConfigurationPrinterGeneric { get; set; } = new Guid("b0c917c7-2ea1-4e08-afa5-4744c19e1c5c");

        public static string PrinterThermalEncoding { get; set; } = "PC860";
        public static string PrinterThermalImageCompanyLogo { get; set; } = "Images/Tickets/company_loggero_thermal.bmp";
        public static int PrinterThermalMaxCharsPerLineNormal { get; set; } = 48;
        public static int PrinterThermalMaxCharsPerLineNormalBold { get; set; } = 44;
        public static int PrinterThermalMaxCharsPerLineSmall { get; set; } = 64;
        public static string PrinterThermalCutCommand { get; set; } = "0x42,0x00";
        public static int PrinterThermalOpenDrawerValueM { get; set; } = 0;
        public static int PrinterThermalOpenDrawerValueT1 { get; set; } = 3;
        public static int PrinterThermalOpenDrawerValueT2 { get; set; } = 49;
    }
}
