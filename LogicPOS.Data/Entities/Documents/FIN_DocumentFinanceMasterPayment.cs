using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinancemasterpayment : XPGuidObject
    {
        public fin_documentfinancemasterpayment() : base() { }
        public fin_documentfinancemasterpayment(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private decimal fCreditAmount;
        public decimal CreditAmount
        {
            get { return fCreditAmount; }
            set { SetPropertyValue<decimal>("CreditAmount", ref fCreditAmount, value); }
        }

        private decimal fDebitAmount;
        public decimal DebitAmount
        {
            get { return fDebitAmount; }
            set { SetPropertyValue<decimal>("DebitAmount", ref fDebitAmount, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        private fin_documentfinancemaster fDocumentFinanceMaster;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMaster")]
        public fin_documentfinancemaster DocumentFinanceMaster
        {
            get { return fDocumentFinanceMaster; }
            set { SetPropertyValue("DocumentFinanceMaster", ref fDocumentFinanceMaster, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinancePayment
        private fin_documentfinancepayment fDocumentFinancePayment;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinancePayment")]
        public fin_documentfinancepayment DocumentFinancePayment
        {
            get { return fDocumentFinancePayment; }
            set { SetPropertyValue("DocumentFinancePayment", ref fDocumentFinancePayment, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        private fin_documentfinancemaster fDocumentFinanceMasterCreditNote;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMasterCreditNote")]
        public fin_documentfinancemaster DocumentFinanceMasterCreditNote
        {
            get { return fDocumentFinanceMasterCreditNote; }
            set { SetPropertyValue("DocumentFinanceMasterCreditNote", ref fDocumentFinanceMasterCreditNote, value); }
        }
    }
}