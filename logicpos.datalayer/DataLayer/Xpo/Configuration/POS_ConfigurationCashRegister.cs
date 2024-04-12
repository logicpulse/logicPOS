using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class pos_configurationcashregister : XPGuidObject
    {
        public pos_configurationcashregister() : base() { }
        public pos_configurationcashregister(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationcashregister), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(pos_configurationcashregister), "Code");
        }

        private UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fPrinter;
        public string Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<string>("Printer", ref fPrinter, value); }
        }

        private string fDrawer;
        public string Drawer
        {
            get { return fDrawer; }
            set { SetPropertyValue<string>("Drawer", ref fDrawer, value); }
        }

        private string fAutomaticDrawer;
        public string AutomaticDrawer
        {
            get { return fAutomaticDrawer; }
            set { SetPropertyValue<string>("AutomaticDrawer", ref fAutomaticDrawer, value); }
        }

        private string fActiveSales;
        public string ActiveSales
        {
            get { return fActiveSales; }
            set { SetPropertyValue<string>("ActiveSales", ref fActiveSales, value); }
        }

        private string fAllowChargeBacks;
        public string AllowChargeBacks
        {
            get { return fAllowChargeBacks; }
            set { SetPropertyValue<string>("AllowChargeBacks", ref fAllowChargeBacks, value); }
        }
    }
}