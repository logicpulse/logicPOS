using DevExpress.Xpo;
using LogicPOS.Data.XPO.Utility;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class cfg_configurationcountry : Entity
    {
        public cfg_configurationcountry() : base() { }
        public cfg_configurationcountry(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOUtility.GetNextTableFieldID(nameof(cfg_configurationcountry), "Ord");
            Code = XPOUtility.GetNextTableFieldID(nameof(cfg_configurationcountry), "Code");
        }

        //This Can be Optional
        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        //This Can be Optional
        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fCode2;
        //Required 5 Chars to use PT-AC (Ex Açores Country Region)
        [Indexed(Unique = true), Size(5)]//2
        public string Code2
        {
            get { return fCode2; }
            set { SetPropertyValue<string>("Code2", ref fCode2, value); }
        }

        private string fCode3;
        //Required 6 Chars to use PRT-AC (Ex Açores Country Region)
        [Indexed(Unique = true), Size(6)]//3
        public string Code3
        {
            get { return fCode3; }
            set { SetPropertyValue<string>("Code3", ref fCode3, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fCapital;
        public string Capital
        {
            get { return fCapital; }
            set { SetPropertyValue<string>("Capital", ref fCapital, value); }
        }

        private string fTLD;
        [Size(10)]
        public string TLD
        {
            get { return fTLD; }
            set { SetPropertyValue<string>("TLD", ref fTLD, value); }
        }

        private string fCurrency;
        [Size(20)]
        public string Currency
        {
            get { return fCurrency; }
            set { SetPropertyValue<string>("Currency", ref fCurrency, value); }
        }

        private string fCurrencyCode;
        [Size(3)]
        public string CurrencyCode
        {
            get { return fCurrencyCode; }
            set { SetPropertyValue<string>("CurrencyCode", ref fCurrencyCode, value); }
        }

        private string fRegExFiscalNumber;
        [Size(255)]
        public string RegExFiscalNumber
        {
            get { return fRegExFiscalNumber; }
            set { SetPropertyValue<string>("RegExFiscalNumber", ref fRegExFiscalNumber, value); }
        }

        private string fRegExZipCode;
        [Size(255)]
        public string RegExZipCode
        {
            get { return fRegExZipCode; }
            set { SetPropertyValue<string>("RegExZipCode", ref fRegExZipCode, value); }
        }

        //ConfigurationCountry One <> Many Customer
        [Association(@"ConfigurationCountryReferencesCustomer", typeof(erp_customer))]
        public XPCollection<erp_customer> Customer
        {
            get { return GetCollection<erp_customer>("Customer"); }
        }
    }
}