using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinancedetailreference : XPGuidObject
    {
        public fin_documentfinancedetailreference() : base() { }
        public fin_documentfinancedetailreference(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        fin_documentfinancemaster fDocumentMaster;
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
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
        fin_documentfinancedetail fDocumentDetail;
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailReference")]
        public fin_documentfinancedetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue<fin_documentfinancedetail>("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}