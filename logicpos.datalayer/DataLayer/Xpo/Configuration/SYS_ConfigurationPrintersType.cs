using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_ConfigurationPrintersType : XPGuidObject
    {
        public SYS_ConfigurationPrintersType() : base() { }
        public SYS_ConfigurationPrintersType(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("SYS_ConfigurationPrintersType", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("SYS_ConfigurationPrintersType", "Code");
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

        string fToken;
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        Boolean fThermalPrinter;
        public Boolean ThermalPrinter
        {
            get { return fThermalPrinter; }
            set { SetPropertyValue<Boolean>("ThermalPrinter", ref fThermalPrinter, value); }
        }

        //ConfigurationPrintersType One <> Many Article
        [Association(@"ConfigurationPrintersTypeConfigurationPrinters", typeof(SYS_ConfigurationPrinters))]
        public XPCollection<SYS_ConfigurationPrinters> Printers
        {
            get { return GetCollection<SYS_ConfigurationPrinters>("Printers"); }
        }
    }
}