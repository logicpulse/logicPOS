using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class SYS_SystemPrint : XPGuidObject
    {
        public SYS_SystemPrint() : base() { }
        public SYS_SystemPrint(Session session) : base(session) { }

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
        
        SYS_UserDetail fUserDetail;
        public SYS_UserDetail UserDetail
        {
            get { return fUserDetail; }
            set { SetPropertyValue<SYS_UserDetail>("UserDetail", ref fUserDetail, value); }
        }

        POS_ConfigurationPlaceTerminal fTerminal;
        public POS_ConfigurationPlaceTerminal Terminal
        {
            get { return fTerminal; }
            set { SetPropertyValue<POS_ConfigurationPlaceTerminal>("Terminal", ref fTerminal, value); }
        }

        //DocumentFinanceMaster One <> Many SystemPrint
        FIN_DocumentFinanceMaster fDocumentMaster;
        [Association(@"DocumentFinanceMasterReferencesSystemPrint")]
        public FIN_DocumentFinanceMaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentMaster", ref fDocumentMaster, value); }
        }

        //DocumentFinancePayment One <> Many SystemPrint
        FIN_DocumentFinancePayment fDocumentPayment;
        [Association(@"DocumentFinancePaymentReferencesSystemPrint")]
        public FIN_DocumentFinancePayment DocumentPayment
        {
            get { return fDocumentPayment; }
            set { SetPropertyValue<FIN_DocumentFinancePayment>("DocumentPayment", ref fDocumentPayment, value); }
        }
    }
}