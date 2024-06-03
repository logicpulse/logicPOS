using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class pos_configurationcashregister : Entity
    {
        public pos_configurationcashregister() : base() { }
        public pos_configurationcashregister(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(pos_configurationcashregister), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(pos_configurationcashregister), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
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