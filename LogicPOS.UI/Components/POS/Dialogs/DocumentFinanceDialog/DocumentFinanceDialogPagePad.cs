using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
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

        public DocumentFinanceDialogPagePad(Window pSourceWindow)
        {
            //Parameters
            SourceWindow = pSourceWindow;
            //Init Private Vars 
            Session = XPOSettings.Session;
            //Init Other
            DateTimeFormat = LogicPOS.Settings.CultureSettings.DateTimeFormat;
            InitalDateTime = XPOHelper.CurrentDateTimeAtomic();
        }
    }
}
