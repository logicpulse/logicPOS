using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class POS_ConfigurationCashRegister : XPGuidObject
    {
        public POS_ConfigurationCashRegister() : base() { }
        public POS_ConfigurationCashRegister(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(POS_ConfigurationCashRegister), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(POS_ConfigurationCashRegister), "Code");
        }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fPrinter;
        public string Printer
        {
            get { return fPrinter; }
            set { SetPropertyValue<string>("Printer", ref fPrinter, value); }
        }

        string fDrawer;
        public string Drawer
        {
            get { return fDrawer; }
            set { SetPropertyValue<string>("Drawer", ref fDrawer, value); }
        }

        string fAutomaticDrawer;
        public string AutomaticDrawer
        {
            get { return fAutomaticDrawer; }
            set { SetPropertyValue<string>("AutomaticDrawer", ref fAutomaticDrawer, value); }
        }

        string fActiveSales;
        public string ActiveSales
        {
            get { return fActiveSales; }
            set { SetPropertyValue<string>("ActiveSales", ref fActiveSales, value); }
        }

        string fAllowChargeBacks;
        public string AllowChargeBacks
        {
            get { return fAllowChargeBacks; }
            set { SetPropertyValue<string>("AllowChargeBacks", ref fAllowChargeBacks, value); }
        }
    }
}