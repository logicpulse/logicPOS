using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class sys_systemprint : XPGuidObject
    {
        public sys_systemprint() : base() { }
        public sys_systemprint(Session session) : base(session) { }

        DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        string fDesignation;
        public string Designation
        {
            get { return fDesignation; }
            set { SetPropertyValue<string>("Designation", ref fDesignation, value); }
        }

        string fCopyNames;
        [Size(50)]
        public string CopyNames
        {
            get { return fCopyNames; }
            set { SetPropertyValue<string>("CopyNames", ref fCopyNames, value); }
        }
  
        int fPrintCopies;
        [Size(50)]
        public int PrintCopies
        {
            get { return fPrintCopies; }
            set { SetPropertyValue<int>("PrintCopies", ref fPrintCopies, value); }
        }

        string fPrintMotive;
        [Size(255)]
        public string PrintMotive
        {
            get { return fPrintMotive; }
            set { SetPropertyValue<string>("PrintMotive", ref fPrintMotive, value); }
        }

        //2ª Via
        bool fSecondPrint;
        public bool SecondPrint
        {
            get { return fSecondPrint; }
            set { SetPropertyValue<bool>("SecondPrint", ref fSecondPrint, value); }
        }
        
        sys_userdetail fUserDetail;
        public sys_userdetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue<sys_userdetail>("UserDetail", ref fUserDetail, value); }
        }

        pos_configurationplaceterminal fTerminal;
        public pos_configurationplaceterminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<pos_configurationplaceterminal>("Terminal", ref fTerminal, value); }
        }

        //DocumentFinanceMaster One <> Many SystemPrint
        fin_documentfinancemaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesSystemPrint")]
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }

        //DocumentFinancePayment One <> Many SystemPrint
        fin_documentfinancepayment fDocumentPayment;
        [Association(@"DocumentFinancePaymentReferencesSystemPrint")]
        public fin_documentfinancepayment DocumentPayment
        {
            get { return fDocumentPayment; }
            set { SetPropertyValue<fin_documentfinancepayment>("DocumentPayment", ref fDocumentPayment, value); }
        }
    }
}