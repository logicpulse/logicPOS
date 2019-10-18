using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_documentfinancedetailorderreference : XPGuidObject
    {
        public fin_documentfinancedetailorderreference() : base() { }
        public fin_documentfinancedetailorderreference(Session session) : base(session) { }

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

        string fOriginatingON;
        [Size(60)]
        public string OriginatingON
        {
            get { return fOriginatingON; }
            set { SetPropertyValue<string>("OriginatingON", ref fOriginatingON, value); }
        }

        DateTime fOrderDate;
        public DateTime OrderDate
        {
            get { return fOrderDate; }
            set { SetPropertyValue<DateTime>("OrderDate", ref fOrderDate, value); }
        }

        //DocumentFinanceDetail One <> Many DocumentFinanceOrderReferences
        fin_documentfinancedetail fDocumentDetail;
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailOrderReference")]
        public fin_documentfinancedetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue<fin_documentfinancedetail>("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}