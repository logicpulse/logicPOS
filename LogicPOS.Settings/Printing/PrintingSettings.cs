using System;
using System.Collections.Generic;

namespace LogicPOS.Settings
{
    public static partial class PrintingSettings
    {
        public static bool PrintPDFEnabled { get; set; } = false;

        public static bool PrintQRCode { get; set; } = true;
        public static Dictionary<string, string> FastReportSystemVars { get; set; }
        public static Dictionary<string, string> FastReportCustomVars { get; set; }

        public static Guid WindowsThermalPrinterId { get; set; } = new Guid("e7143ea5-391b-46ef-a28d-4843fd7e21ac");
        public static Guid ThermalSocketPrinterId { get; set; } = new Guid("faeb45cd-2989-4e92-9907-3038444e4849");
        public static Guid WindowsGenericPrinterId { get; set; } = new Guid("4be662e6-67c9-4063-bd6c-574ae4df7f3f");
        public static Guid ExportToPdfPrinterId { get; set; } = new Guid("e5e20cd0-d9d2-443d-9d3f-3478949db30f");
        public static Guid UsbThermalPrinterId { get; set; } = new Guid("39b58a4e-b860-49c1-81a5-8bb5f7186940");
        public static Guid WorkSessionMovementPrintingTemplateId { get; set; } = new Guid("f6a25476-40b0-4287-9284-d5db3280d7f1");
        public static Guid CashDrawerMoneyMovementPrintingTemplateId { get; set; } = new Guid("f6565476-28b0-4287-9284-d5db3280d421");
        public static Guid GenericPrinterId { get; set; } = new Guid("b0c917c7-2ea1-4e08-afa5-4744c19e1c5c");
    }
}
