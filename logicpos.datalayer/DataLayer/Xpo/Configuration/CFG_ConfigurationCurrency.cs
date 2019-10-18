using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class cfg_configurationcurrency : XPGuidObject
    {
        public cfg_configurationcurrency() : base() { }
        public cfg_configurationcurrency(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID(nameof(cfg_configurationcurrency), "Ord");
            Code = FrameworkUtils.GetNextTableFieldID(nameof(cfg_configurationcurrency), "Code");
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

        string fAcronym;
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        string fSymbol;
        [Size(10)]
        public string Symbol
        {
            get { return fSymbol; }
            set { SetPropertyValue<string>("Symbol", ref fSymbol, value); }
        }

        string fEntity;
        [Size(512)]
        public string Entity
        {
            get { return fEntity; }
            set { SetPropertyValue<string>("Entity", ref fEntity, value); }
        }

        decimal fExchangeRate;
        public decimal ExchangeRate
        {
            get { return fExchangeRate; }
            set { SetPropertyValue<decimal>("ExchangeRate", ref fExchangeRate, value); }
        }

        //ConfigurationCurrency One <> Many DocumentFinanceMaster
        [Association(@"ConfigurationCurrencyReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
        public XPCollection<fin_documentfinancemaster> DocumentMaster
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
        }

        //ConfigurationCurrency One <> Many DocumentFinancePayment
        [Association(@"ConfigurationCurrencyReferencesDocumentFinancePayment", typeof(fin_documentfinancepayment))]
        public XPCollection<fin_documentfinancemaster> DocumentPayment
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentPayment"); }
        }
    }
}