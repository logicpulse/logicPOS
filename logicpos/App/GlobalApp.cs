using Gtk;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using logicpos.financial.library.Classes.Utils;
using System.Collections.Generic;

namespace logicpos.App
{
    public static class GlobalApp
    {
        public static StartupWindow StartupWindow;
        public static PosMainWindow PosMainWindow;
        public static BackOfficeMainWindow BackOfficeMainWindow;
        public static BackOfficeReportWindow BackOfficeReportWindow;
        public static EventBox EventBoxDashboard;
        public static Dialog DialogThreadWork;
        public static ThreadNotify DialogThreadNotify;
        public static PosKeyboardDialog DialogPosKeyboard;
        public static Dictionary<string, bool> Notifications;
        public static bool MultiUserEnvironment;
        public static bool UseVirtualKeyBoard;
        public static dynamic Theme;
        public static ExpressionEvaluator ExpressionEvaluator = new ExpressionEvaluator();
        public static System.Drawing.Size ScreenSize;
        public static System.Drawing.Size MaxWindowSize;
        public static System.Drawing.Size boScreenSize;
        public static string FilePickerStartPath;

        public static UsbDisplayDevice UsbDisplay;
        public static InputReader BarCodeReader;
        public static WeighingBalance WeighingBalance;

        public static ProtectedFiles ProtectedFiles;

        public static ParkingTicket ParkingTicket;
    }
}
