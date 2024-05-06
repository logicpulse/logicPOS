using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Collections;
using System.Collections.Generic;

namespace logicpos.shared.App
{
    public static class SharedFramework
    {
        public static string ServerVersion { get; set; }
        public static Hashtable AT;
        public static pos_worksessionperiod WorkSessionPeriodDay;
        public static pos_worksessionperiod WorkSessionPeriodTerminal;
        public static GlobalFrameworkSession SessionApp;
        
        public static Dictionary<string, bool> LoggedUserPermissions;
        public static Dictionary<string, string> FastReportSystemVars;
        public static Dictionary<string, string> FastReportCustomVars;
        public static bool AppUseParkingTicketModule = false;
        public static bool PrintQRCode { get; set; } = true;
        public static bool CheckStocks { get; set; } = true;
        public static bool CheckStockMessage { get; set; } = true;
        public static bool AppUseBackOfficeMode { get; set; } = false;
        public static Dictionary<string, Guid> PendentPayedParkingTickets { get; set; } = new Dictionary<string, Guid>();
        public static Dictionary<string, Guid> PendentPayedParkingCards { get; set; } = new Dictionary<string, Guid>();
        public static bool UsingThermalPrinter { get; set; }
        public static System.Drawing.Size ScreenSize { get; set; }
        public static bool CanOpenFiles { get; set; } = true;
      
    }
}
