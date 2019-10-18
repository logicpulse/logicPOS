using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class cfg_configurationcountry : XPGuidObject
    {
        public cfg_configurationcountry() : base() { }
        public cfg_configurationcountry(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(cfg_configurationcountry), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(cfg_configurationcountry), "Code");
        }

        //This Can be Optional
        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        //This Can be Optional
        UInt32 fCode;
        [Indexed(Unique = true)]
        public UInt32 Code
        {
            get { return fCode; }
            set { SetPropertyValue<UInt32>("Code", ref fCode, value); }
        }

        string fCode2;
        //Required 5 Chars to use PT-AC (Ex Açores Country Region)
        [Indexed(Unique = true), Size(5)]//2
        public string Code2
        {
            get { return fCode2; }
            set { SetPropertyValue<string>("Code2", ref fCode2, value); }
        }

        string fCode3;
        //Required 6 Chars to use PRT-AC (Ex Açores Country Region)
        [Indexed(Unique = true), Size(6)]//3
        public string Code3
        {
            get { return fCode3; }
            set { SetPropertyValue<string>("Code3", ref fCode3, value); }
        }

        string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fCapital;
        public string Capital
        {
            get { return fCapital; }
            set { SetPropertyValue<string>("Capital", ref fCapital, value); }
        }

        string fTLD;
        [Size(10)]
        public string TLD
        {
            get { return fTLD; }
            set { SetPropertyValue<string>("TLD", ref fTLD, value); }
        }

        string fCurrency;
        [Size(20)]
        public string Currency
        {
            get { return fCurrency; }
            set { SetPropertyValue<string>("Currency", ref fCurrency, value); }
        }

        string fCurrencyCode;
        [Size(3)]
        public string CurrencyCode
        {
            get { return fCurrencyCode; }
            set { SetPropertyValue<string>("CurrencyCode", ref fCurrencyCode, value); }
        }

        string fRegExFiscalNumber;
        [Size(255)]
        public string RegExFiscalNumber
        {
            get { return fRegExFiscalNumber; }
            set { SetPropertyValue<string>("RegExFiscalNumber", ref fRegExFiscalNumber, value); }
        }
        
        string fRegExZipCode;
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