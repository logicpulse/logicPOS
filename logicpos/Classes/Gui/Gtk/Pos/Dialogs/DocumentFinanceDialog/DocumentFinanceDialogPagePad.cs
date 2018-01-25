using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.shared;
using System;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog
{
    class DocumentFinanceDialogPagePad : PagePad
    {
        private Window _sourceWindow;
        public Window SourceWindow
        {
            get { return _sourceWindow; }
            set { _sourceWindow = value; }
        }

        private Session _session;
        public Session Session
        {
            get { return _session; }
            set { _session = value; }
        }

        private ERP_Customer _customer;
        public ERP_Customer Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        //Used to assign to WayBill Dates
        private DateTime _initalDateTime;
        public DateTime InitalDateTime
        {
            get { return _initalDateTime; }
            set { _initalDateTime = value; }
        }

        private string _dateTimeFormat;
        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { _dateTimeFormat = value; }
        }

        public DocumentFinanceDialogPagePad(Window pSourceWindow)
        {
            //Parameters
            _sourceWindow = pSourceWindow;
            //Init Private Vars 
            _session = GlobalFramework.SessionXpo;
            //Init Other
            _dateTimeFormat = SettingsApp.DateTimeFormat;
            _initalDateTime = FrameworkUtils.CurrentDateTimeAtomic();
        }
    }
}
