using Gtk;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using logicpos.financial.library.Classes.Utils;
using System;
using System.Collections.Generic;

namespace logicpos.App
{
    public class GlobalApp
    {
        //Windows
        public static StartupWindow WindowStartup;
        public static PosMainWindow WindowPos;
        //Windows
        public static BackOfficeMainWindow WindowBackOffice;
        public static BackOfficeReportWindow WindowReports;

        public static EventBox eventboxDashboard;
        //Dialogs/Thread
        public static Dialog DialogThreadWork;
        public static ThreadNotify DialogThreadNotify;
        //Windows.Forms
        //public static System.Windows.Forms.Form WindowReportsWinForm;
        //Dialogs
        public static PosKeyboardDialog DialogPosKeyboard;
        //Application Notifications
        public static Dictionary<string, bool> Notifications;
        //Other
        public static Boolean MultiUserEnvironment;
        public static Boolean UseVirtualKeyBoard;
        //Theme
        public static dynamic Theme;
        public static ExpressionEvaluator ExpressionEvaluator = new ExpressionEvaluator();
        public static System.Drawing.Size ScreenSize;
        public static System.Drawing.Size MaxWindowSize;
        public static System.Drawing.Size boScreenSize;
        //System
        public static string FilePickerStartPath;
        //Hardware
        public static UsbDisplayDevice UsbDisplay;
        public static InputReader BarCodeReader;
        public static WeighingBalance WeighingBalance;
        //Protected Files
        public static ProtectedFiles ProtectedFiles;
        // TK013134: HardCoded Modules
        public static ParkingTicket ParkingTicket;
    }
}
