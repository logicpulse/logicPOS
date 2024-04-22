using System;
using DevExpress.Xpo;
using logicpos.datalayer.App;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_configurationvatrate : XPGuidObject
    {
        public fin_configurationvatrate() : base() { }
        public fin_configurationvatrate(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = DataLayerUtils.GetNextTableFieldID(nameof(fin_configurationvatrate), "Ord");
            Code = DataLayerUtils.GetNextTableFieldID(nameof(fin_configurationvatrate), "Code");
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

        private decimal fValue;
        public decimal Value
        {
            get { return fValue; }
            set { SetPropertyValue<decimal>("Value", ref fValue, value); }
        }

        private string fReasonCode;
        public string ReasonCode
        {
            get { return fReasonCode; }
            set { SetPropertyValue<string>("ReasonCode", ref fReasonCode, value); }
        }

        //SAF-T PT
        private string fTaxType;
        [Size(3)]
        public string TaxType
        {
            get { return fTaxType; }
            set { SetPropertyValue<string>("TaxType", ref fTaxType, value); }
        }

        private string fTaxCode;
        [Size(10)]
        public string TaxCode
        {
            get { return fTaxCode; }
            set { SetPropertyValue<string>("TaxCode", ref fTaxCode, value); }
        }

        private string fTaxCountryRegion;
        [Size(5)]
        public string TaxCountryRegion
        {
            get { return fTaxCountryRegion; }
            set { SetPropertyValue<string>("TaxCountryRegion", ref fTaxCountryRegion, value); }
        }

        private DateTime fTaxExpirationDate;
        public DateTime TaxExpirationDate
        {
            get { return fTaxExpirationDate; }
            set { SetPropertyValue<DateTime>("TaxExpirationDate", ref fTaxExpirationDate, value); }
        }

        private string fTaxDescription;
        public string TaxDescription
        {
            get { return fTaxDescription; }
            set { SetPropertyValue<string>("TaxDescription", ref fTaxDescription, value); }
        }

        //ConfigurationVatRate One <> Many SubFamily
        [Association(@"ConfigurationVatRateReferencesSubFamily_ForVatOnTable", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> SubFamilyVatOnTable
        {
            get { return GetCollection<fin_articlesubfamily>("SubFamilyVatOnTable"); }
        }

        //ConfigurationVatRate One <> Many SubFamily
        [Association(@"ConfigurationVatRateReferencesSubFamily_ForVatDirectSelling", typeof(fin_articlesubfamily))]
        public XPCollection<fin_articlesubfamily> SubFamilyVatDirectSelling
        {
            get { return GetCollection<fin_articlesubfamily>("SubFamilyVatDirectSelling"); }
        }

        //ConfigurationVatRate One <> Many Article
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatOnTable", typeof(fin_article))]
        public XPCollection<fin_article> ArticleVatOnTable
        {
            get { return GetCollection<fin_article>("ArticleVatOnTable"); }
        }

        //ConfigurationVatRate One <> Many Article
        [Association(@"ConfigurationVatRateReferencesArticle_ForVatDirectSelling", typeof(fin_article))]
        public XPCollection<fin_article> ArticleVatDirectSelling
        {
            get { return GetCollection<fin_article>("ArticleVatDirectSelling"); }
        }

        //ConfigurationVatRate One <> Many DocumentFinanceDetail
        [Association(@"ConfigurationVatRateReferencesDocumentDocumentFinanceDetail", typeof(fin_documentfinancedetail))]
        public XPCollection<fin_documentfinancedetail> DocumentFinanceDetail
        {
            get { return GetCollection<fin_documentfinancedetail>("DocumentFinanceDetail"); }
        }
    }
}