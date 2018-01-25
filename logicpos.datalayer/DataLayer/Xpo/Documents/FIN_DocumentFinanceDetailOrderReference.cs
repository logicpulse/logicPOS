using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_DocumentFinanceDetailOrderReference : XPGuidObject
    {
        public FIN_DocumentFinanceDetailOrderReference() : base() { }
        public FIN_DocumentFinanceDetailOrderReference(Session session) : base(session) { }

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
        FIN_DocumentFinanceDetail fDocumentDetail;
        [Association(@"DocumentFinanceDetailReferencesDocumentFinanceDetailOrderReference")]
        public FIN_DocumentFinanceDetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue<FIN_DocumentFinanceDetail>("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}