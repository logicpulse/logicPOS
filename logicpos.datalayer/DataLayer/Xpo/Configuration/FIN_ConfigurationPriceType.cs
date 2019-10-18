using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_configurationpricetype : XPGuidObject
    {
        public fin_configurationpricetype() : base() { }
        public fin_configurationpricetype(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_configurationpricetype), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_configurationpricetype), "Code");
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
        [Association(@"ConfigurationPriceTypeReferencesConfigurationPlace", typeof(pos_configurationplace))]
        public XPCollection<pos_configurationplace> Place
        {
            get { return GetCollection<pos_configurationplace>("Place"); }
        }

        //ConfigurationPriceType One <> Many Customer
        [Association(@"ConfigurationPriceTypeReferencesCustomer", typeof(erp_customer))]
        public XPCollection<erp_customer> Customer
        {
            get { return GetCollection<erp_customer>("Customer"); }
        }
    }
}