using System;
using DevExpress.Xpo;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class FIN_ArticleStock : XPGuidObject
    {
        public FIN_ArticleStock() : base() { }
        public FIN_ArticleStock(Session session) : base(session) { }

        UInt32 fOrd;
        public UInt32 Ord
        {
            get { return fOrd; }
            set { SetPropertyValue<UInt32>("Ord", ref fOrd, value); }
        }

        DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        ERP_Customer fCustomer;
        public ERP_Customer Customer
        {
            get { return fCustomer; }
            set { SetPropertyValue<ERP_Customer>("Customer", ref fCustomer, value); }
        }

        string fDocumentNumber;
        [Size(50)]
        //[Indexed(Unique = true)]
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        FIN_Article fArticle;
        public FIN_Article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<FIN_Article>("Article", ref fArticle, value); }
        }

        Decimal fQuantity;
        public Decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<Decimal>("Quantity", ref fQuantity, value); }
        }

        FIN_DocumentFinanceMaster fDocumentMaster;
        public FIN_DocumentFinanceMaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<FIN_DocumentFinanceMaster>("DocumentMaster", ref fDocumentMaster, value); }
        }

        FIN_DocumentFinanceDetail fDocumentDetail;
        public FIN_DocumentFinanceDetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue<FIN_DocumentFinanceDetail>("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}