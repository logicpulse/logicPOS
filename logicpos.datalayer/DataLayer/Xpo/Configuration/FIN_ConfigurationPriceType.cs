using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ConfigurationPriceType : XPGuidObject
    {
        public FIN_ConfigurationPriceType() : base() { }
        public FIN_ConfigurationPriceType(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationPriceType", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("FIN_ConfigurationPriceType", "Code");
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

        Int32 fEnumValue;
        [Indexed(Unique = true)]
        public Int32 EnumValue
        {
            get { return fEnumValue; }
            set { SetPropertyValue<Int32>("EnumValue", ref fEnumValue, value); }
        }

        //ConfigurationPriceType One <> Many ConfigurationPlace
        [Association(@"ConfigurationPriceTypeReferencesConfigurationPlace", typeof(POS_ConfigurationPlace))]
        public XPCollection<POS_ConfigurationPlace> Place
        {
            get { return GetCollection<POS_ConfigurationPlace>("Place"); }
        }

        //ConfigurationPriceType One <> Many Customer
        [Association(@"ConfigurationPriceTypeReferencesCustomer", typeof(ERP_Customer))]
        public XPCollection<ERP_Customer> Customer
        {
            get { return GetCollection<ERP_Customer>("Customer"); }
        }
    }
}