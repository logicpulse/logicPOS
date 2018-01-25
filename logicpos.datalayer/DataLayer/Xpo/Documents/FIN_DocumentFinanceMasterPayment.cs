using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceMasterPayment : XPGuidObject
    {
        public FIN_DocumentFinanceMasterPayment() : base() { }
        public FIN_DocumentFinanceMasterPayment(Session session) : base(session) { }

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
        FIN_DocumentFinanceMaster fDocumentFinanceMaster;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMaster")]
        public FIN_DocumentFinanceMaster DocumentFinanceMaster
        {
            get { return fDocumentFinanceMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentFinanceMaster", ref fDocumentFinanceMaster, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinancePayment
        FIN_DocumentFinancePayment fDocumentFinancePayment;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinancePayment")]
        public FIN_DocumentFinancePayment DocumentFinancePayment
        {
            get { return fDocumentFinancePayment; }
            set { SetPropertyValue<FIN_DocumentFinancePayment>("DocumentFinancePayment", ref fDocumentFinancePayment, value); }
        }

        //DocumentFinanceMasterPayment Many <> Many DocumentFinanceMaster
        FIN_DocumentFinanceMaster fDocumentFinanceMasterCreditNote;
        [Association(@"DocumentFinanceMasterPaymentReferencesDocumentFinanceMasterCreditNote")]
        public FIN_DocumentFinanceMaster DocumentFinanceMasterCreditNote
        {
            get { return fDocumentFinanceMasterCreditNote; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentFinanceMasterCreditNote", ref fDocumentFinanceMasterCreditNote, value); }
        }
    }
}