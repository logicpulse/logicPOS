using DevExpress.Xpo;
using Gtk;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
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
            InitalDateTime = DataLayerUtils.CurrentDateTimeAtomic();
        }
    }
}
