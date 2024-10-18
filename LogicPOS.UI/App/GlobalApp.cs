﻿using Gtk;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Logic.Hardware;
using logicpos.Classes.Logic.Others;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI
{
    public static class GlobalApp
    {
        public static StartupWindow StartupWindow { get; set; }
        public static POSMainWindow PosMainWindow { get; set; }
        public static BackOfficeWindow BackOffice { get; set; }
        public static Dialog LoadingDialog { get; set; }
        public static ThreadNotify DialogThreadNotify { get; set; }
        public static PosKeyboardDialog DialogPosKeyboard { get; set; }
        public static Dictionary<string, bool> Notifications { get; set; }
        public static bool MultiUserEnvironment { get; set; }
        public static bool UseVirtualKeyBoard { get; set; }

        public static dynamic Theme
        {
            get;
            set;
        }

        public static ExpressionEvaluationService ExpressionEvaluator { get; set; } = new ExpressionEvaluationService();

        public static System.Drawing.Size ScreenSize
        {
            get;
            set;
        }

        public static System.Drawing.Size MaxWindowSize { get; set; }
        public static System.Drawing.Size BackOfficeScreenSize { get; set; }
        public static string OpenFileDialogStartPath { get; set; }
        public static UsbDisplayDevice UsbDisplay { get; set; }
        public static InputReader BarCodeReader { get; set; }
        public static WeighingBalance WeighingBalance { get; set; }
        public static ProtectedFiles ProtectedFiles { get; set; }
        public static ParkingTicket ParkingTicket { get; set; }
    }
}
