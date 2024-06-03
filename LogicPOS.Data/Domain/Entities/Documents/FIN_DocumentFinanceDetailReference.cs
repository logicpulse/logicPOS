using System;
using DevExpress.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_documentfinancedetailreference : Entity
    {
        public fin_documentfinancedetailreference() : base() { }
        public fin_documentfinancedetailreference(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private fin_documentfinancemaster fDocumentMaster;
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue("DocumentMaster", ref fDocumentMaster, value); }
        }

        private string fReference;
        [Size(60)]
        public string Reference
        {
            get { return fReference; }
            set { SetPropertyValue<string>("Reference)", ref fReference, value); }
        }

        private string fReason;
        [Size(50)]
        public string Reason
        {
            get { return fReason; }
            set { SetPropertyValue<string>("Reason)", ref fReason, value); }
        }

        //DocumentFinanceDetail One <> Many DocumentFinanceReferences
        private fin_documentfinancedetail fDocumentDetail;
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailReference")]
        public fin_documentfinancedetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}