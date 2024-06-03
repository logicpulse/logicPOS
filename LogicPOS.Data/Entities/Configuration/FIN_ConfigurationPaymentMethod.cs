using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using LogicPOS.Data.XPO.Utility;
using System;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_configurationpaymentmethod : XPGuidObject
    {
        public fin_configurationpaymentmethod() : base() { }
        public fin_configurationpaymentmethod(Session session) : base(session) { }

        protected override void OnAfterConstruction()
        {
            Ord = XPOHelper.GetNextTableFieldID(nameof(fin_configurationpaymentmethod), "Ord");
            Code = XPOHelper.GetNextTableFieldID(nameof(fin_configurationpaymentmethod), "Code");
        }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private uint fCode;
        [Indexed(Unique = true)]
        public uint Code
        {
            get { return fCode; }
            set { SetPropertyValue("Code", ref fCode, value); }
        }

        private string fToken;
        [Size(100)]
        [Indexed(Unique = true)]
        public string Token
        {
            get { return fToken; }
            set { SetPropertyValue<string>("Token", ref fToken, value); }
        }

        private string fDesignation;
        [Indexed(Unique = true)]
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        private string fResourceString;
        [Indexed(Unique = true)]
        public string ResourceString
        {
            get { return fResourceString; }
            set { SetPropertyValue<string>("ResourceString", ref fResourceString, value); }
        }

        private string fButtonIcon;
        [Size(255)]
        public string ButtonIcon
        {
            get { return fButtonIcon; }
            set { SetPropertyValue<string>("ButtonIcon", ref fButtonIcon, value); }
        }

        private string fAcronym;
        public string Acronym
        {
            get { return fAcronym; }
            set { SetPropertyValue<string>("Acronym", ref fAcronym, value); }
        }

        private string fAllowPayback;
        public string AllowPayback
        {
            get { return fAllowPayback; }
            set { SetPropertyValue<string>("AllowPayback", ref fAllowPayback, value); }
        }

        private string fSymbol;
        public string Symbol
        {
            get { return fSymbol; }
            set { SetPropertyValue<string>("Symbol", ref fSymbol, value); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinanceMaster
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinanceMaster", typeof(fin_documentfinancemaster))]
        public XPCollection<fin_documentfinancemaster> DocumentMaster
        {
            get { return GetCollection<fin_documentfinancemaster>("DocumentMaster"); }
        }

        //ConfigurationPaymentMethod One <> Many DocumentFinancePayment
        [Association(@"ConfigurationPaymentMethodReferencesDocumentFinancePayment", typeof(fin_documentfinancepayment))]
        public XPCollection<fin_documentfinancepayment> DocumentPayment
        {
            get { return GetCollection<fin_documentfinancepayment>("DocumentPayment"); }
        }
    }
}
