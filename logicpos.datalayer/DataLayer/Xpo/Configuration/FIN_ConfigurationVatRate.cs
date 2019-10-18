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
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(fin_configurationvatrate), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(fin_configurationvatrate), "Code");
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

        decimal fValue;
        public decimal Value
        {
            get { return fValue; }
            set { SetPropertyValue<decimal>("Value", ref fValue, value); }
        }

        string fReasonCode;
        public string ReasonCode
        {
            get { return fReasonCode; }
            set { SetPropertyValue<string>("ReasonCode", ref fReasonCode, value); }
        }

        //SAF-T PT
        string fTaxType;
        [Size(3)]
        public string TaxType
        {
            get { return fTaxType; }
            set { SetPropertyValue<string>("TaxType", ref fTaxType, value); }
        }

        string fTaxCode;
        [Size(10)]
        public string TaxCode
        {
            get { return fTaxCode; }
            set { SetPropertyValue<string>("TaxCode", ref fTaxCode, value); }
        }

        string fTaxCountryRegion;
        [Size(5)]
        public string TaxCountryRegion
        {
            get { return fTaxCountryRegion; }
            set { SetPropertyValue<string>("TaxCountryRegion", ref fTaxCountryRegion, value); }
        }

        DateTime fTaxExpirationDate;
        public DateTime TaxExpirationDate
        {
            get { return fTaxExpirationDate; }
            set { SetPropertyValue<DateTime>("TaxExpirationDate", ref fTaxExpirationDate, value); }
        }

        string fTaxDescription;
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