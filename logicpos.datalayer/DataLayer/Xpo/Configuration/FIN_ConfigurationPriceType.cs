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
            Ord = DataLayerUtils.GetNextTableFieldID(nameof(fin_configurationpricetype), "Ord");
            Code = DataLayerUtils.GetNextTableFieldID(nameof(fin_configurationpricetype), "Code");
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

        private int fEnumValue;
        [Indexed(Unique = true)]
        public int EnumValue
        {
            get { return fEnumValue; }
            set { SetPropertyValue<int>("EnumValue", ref fEnumValue, value); }
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