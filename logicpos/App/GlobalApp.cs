using Gtk;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using logicpos.financial.library.Classes.Utils;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace logicpos.App
{
    public static class GlobalApp
    {
        public static StartupWindow StartupWindow { get; set; }
        public static PosMainWindow PosMainWindow { get; set; }
        public static BackOfficeMainWindow BackOfficeMainWindow { get; set; }
        public static BackOfficeReportWindow BackOfficeReportWindow { get; set; }
        public static Dialog DialogThreadWork { get; set; }
        public static ThreadNotify DialogThreadNotify { get; set; }
        public static PosKeyboardDialog DialogPosKeyboard { get; set; }
        public static Dictionary<string, bool> Notifications { get; set; }
        public static bool MultiUserEnvironment { get; set; }
        public static bool UseVirtualKeyBoard { get; set; }
        public static dynamic Theme { get; set; }
        public static ExpressionEvaluator ExpressionEvaluator { get; set; } = new ExpressionEvaluator();
        public static System.Drawing.Size ScreenSize { get; set; }
        public static System.Drawing.Size MaxWindowSize { get; set; }
        public static System.Drawing.Size BoScreenSize { get; set; }
        public static string FilePickerStartPath { get; set; }
        public static UsbDisplayDevice UsbDisplay { get; set; }
        public static InputReader BarCodeReader { get; set; }
        public static WeighingBalance WeighingBalance { get; set; }
        public static ProtectedFiles ProtectedFiles { get; set; }
        public static ParkingTicket ParkingTicket { get; set; }
    }
}
