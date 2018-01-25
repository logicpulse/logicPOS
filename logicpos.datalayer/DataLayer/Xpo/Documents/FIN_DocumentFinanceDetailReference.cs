using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceDetailReference : XPGuidObject
    {
        public FIN_DocumentFinanceDetailReference() : base() { }
        public FIN_DocumentFinanceDetailReference(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        FIN_DocumentFinanceMaster fDocumentMaster;
        public FIN_DocumentFinanceMaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentMaster", ref fDocumentMaster, value); }
        }

        string fReference;
        [Size(60)]
        public string Reference
        {
            get { return fReference; }
            set { SetPropertyValue<string>("Reference)", ref fReference, value); }
        }

        string fReason;
        [Size(50)]
        public string Reason
        {
            get { return fReason; }
            set { SetPropertyValue<string>("Reason)", ref fReason, value); }
        }

        //DocumentFinanceDetail One <> Many DocumentFinanceReferences
        FIN_DocumentFinanceDetail fDocumentDetail;
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailReference")]
        public FIN_DocumentFinanceDetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue<FIN_DocumentFinanceDetail>("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}