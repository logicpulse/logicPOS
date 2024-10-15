﻿using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    internal class DocumentFinanceDialogPagePad : PagePad
    {
        public Window SourceWindow { get; set; }

        public Session Session { get; set; }

        public erp_customer Customer { get; set; }

        public DateTime InitalDateTime { get; set; }

        public string DateTimeFormat { get; set; }

        public DocumentFinanceDialogPagePad(Window parentWindow)
        {
            //Parameters
            SourceWindow = parentWindow;
            //Init Private Vars 
            Session = XPOSettings.Session;
            //Init Other
            DateTimeFormat = LogicPOS.Settings.CultureSettings.DateTimeFormat;
            InitalDateTime = XPOUtility.CurrentDateTimeAtomic();
        }
    }
}
