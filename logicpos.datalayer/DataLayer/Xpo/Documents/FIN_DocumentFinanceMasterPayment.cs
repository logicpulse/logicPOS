using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinancemasterpayment : XPGuidObject
    {
        public fin_documentfinancemasterpayment() : base() { }
        public fin_documentfinancemasterpayment(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        decimal fCreditAmount;
        public decimal CreditAmount
        {
            get { return fCreditAmount; }
            set { SetPropertyValue<decimal>("CreditAmount", ref fCreditAmount, value); }
        }

        decimal fDebitAmount;
        public decimal DebitAmount
        {
            get { return fDebitAmount; }
            set { SetPropertyValue<decimal>("DebitAmount", ref fDebitAmount, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        fin_documentfinancemaster fDocumentFinanceMaster;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMaster")]
        public fin_documentfinancemaster DocumentFinanceMaster
        {
            get { return fDocumentFinanceMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentFinanceMaster", ref fDocumentFinanceMaster, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinancePayment
        fin_documentfinancepayment fDocumentFinancePayment;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinancePayment")]
        public fin_documentfinancepayment DocumentFinancePayment
        {
            get { return fDocumentFinancePayment; }
            set { SetPropertyValue<fin_documentfinancepayment>("DocumentFinancePayment", ref fDocumentFinancePayment, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        fin_documentfinancemaster fDocumentFinanceMasterCreditNote;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMasterCreditNote")]
        public fin_documentfinancemaster DocumentFinanceMasterCreditNote
        {
            get { return fDocumentFinanceMasterCreditNote; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentFinanceMasterCreditNote", ref fDocumentFinanceMasterCreditNote, value); }
        }
    }
}