using DevExpress.Xpo;
using logicpos.datalayer.App;
using System;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class CFG_ConfigurationCurrency : XPGuidObject
    {
        public CFG_ConfigurationCurrency() : base() { }
        public CFG_ConfigurationCurrency(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = FrameworkUtils.GetNextTableFieldID("CFG_ConfigurationCurrency", "Ord");
            Code = FrameworkUtils.GetNextTableFieldID("CFG_ConfigurationCurrency", "Code");
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
        [Association(@"ConfigurationCurrencyReferencesDocumentFinanceMaster", typeof(FIN_DocumentFinanceMaster))]
        public XPCollection<FIN_DocumentFinanceMaster> DocumentMaster
        {
            get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentMaster"); }
        }

        //ConfigurationCurrency One <> Many DocumentFinancePayment
        [Association(@"ConfigurationCurrencyReferencesDocumentFinancePayment", typeof(FIN_DocumentFinancePayment))]
        public XPCollection<FIN_DocumentFinanceMaster> DocumentPayment
        {
            get { return GetCollection<FIN_DocumentFinanceMaster>("DocumentPayment"); }
        }
    }
}