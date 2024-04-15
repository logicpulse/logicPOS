using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_configurationprinterstype : XPGuidObject
    {
        public sys_configurationprinterstype() : base() { }
        public sys_configurationprinterstype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationprinterstype), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(sys_configurationprinterstype), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
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

        private string fToken;
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        private bool fThermalPrinter;
        public bool ThermalPrinter
        {
            get { return fThermalPrinter; }
            set { SetPropertyValue<bool>("ThermalPrinter", ref fThermalPrinter, value); }
        }

        //ConfigurationPrintersType One <> Many Article
        [Association(@"ConfigurationPrintersTypeConfigurationPrinters", typeof(sys_configurationprinters))]
        public XPCollection<sys_configurationprinters> Printers
        {
            get { return GetCollection<sys_configurationprinters>("Printers"); }
        }
    }
}