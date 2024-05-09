using logicpos.datalayer.DataLayer.Xpo;
using System;
using System.Collections.Generic;

namespace logicpos.shared.App
{
    public static class SharedFramework
    {
        public static string ServerVersion { get; set; }
        public static pos_worksessionperiod WorkSessionPeriodDay { get; set; }
        public static pos_worksessionperiod WorkSessionPeriodTerminal { get; set; }
        public static GlobalFrameworkSession SessionApp { get; set; }
        public static Dictionary<string, bool> LoggedUserPermissions { get; set; }

        public static bool AppUseParkingTicketModule = false;
        public static bool CheckStocks { get; set; } = true;
        public static bool CheckStockMessage { get; set; } = true;
        public static bool AppUseBackOfficeMode { get; set; } = false;
        public static Dictionary<string, Guid> PendentPayedParkingTickets { get; set; } = new Dictionary<string, Guid>();
        public static Dictionary<string, Guid> PendentPayedParkingCards { get; set; } = new Dictionary<string, Guid>();
        public static System.Drawing.Size ScreenSize { get; set; }
    }
}
