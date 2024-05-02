using logicpos.datalayer.DataLayer.Xpo;
using logicpos.plugin.contracts;
using logicpos.plugin.library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace logicpos.shared.App
{
    public static class SharedFramework
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Localization
        public static CultureInfo CurrentCulture { get; set; }
        public static CultureInfo CurrentCultureNumberFormat { get; set; }

        //TK016248 - BackOffice - Check New Version 
        public static string ServerVersion { get; set; }
        //AT - Only Used in logicerp.Modules.FINANCIAL | LogicposHelper
        public static Hashtable AT;
        //Database
        public static string DatabaseServer { get; set; }
        public static string DatabaseName { get; set; }
        public static string DatabaseUser { get; set; }
        public static string DatabasePassword { get; set; }
        //WorkSession
        public static pos_worksessionperiod WorkSessionPeriodDay;
        public static pos_worksessionperiod WorkSessionPeriodTerminal;
        //Session
        public static GlobalFrameworkSession SessionApp;
        

        //User/Terminal/Permissions
        public static Dictionary<string, bool> LoggedUserPermissions;
        //PreferenceParameters
       
        //FastReport
        public static Dictionary<string, string> FastReportSystemVars;
        public static Dictionary<string, string> FastReportCustomVars;
        //TK013134: HardCoded Modules
        public static bool AppUseParkingTicketModule = false;
        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        public static bool PrintQRCode = true;
        //Gestão de Stocks : Janela de Gestão de Stocks [IN:016534]
        public static bool CheckStocks = true;
        public static bool CheckStockMessage = true;
        //TK016235 BackOffice - Mode
        public static bool AppUseBackOfficeMode = false;
        public static Dictionary<string, Guid> PendentPayedParkingTickets = new Dictionary<string, Guid>();
        public static Dictionary<string, Guid> PendentPayedParkingCards = new Dictionary<string, Guid>();
        //TK016249 - Impressoras - Diferenciação entre Tipos
        public static bool UsingThermalPrinter;
        public static System.Drawing.Size ScreenSize { get; set; }
        public static bool CanOpenFiles { get; set; } = true;
      
    }
}
