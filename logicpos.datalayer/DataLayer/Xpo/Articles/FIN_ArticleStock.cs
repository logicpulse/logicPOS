using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;

namespace logicpos.datalayer.DataLayer.Xpo
{
    [DeferredDeletion(false)]
    public class fin_articlestock : XPGuidObject
    {
        public fin_articlestock() : base() { }
        public fin_articlestock(Session session) : base(session) { }

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

        erp_customer fCustomer;
        public erp_customer Customer
        {
            get { return fCustomer; }
            set { SetPropertyValue<erp_customer>("Customer", ref fCustomer, value); }
        }

        string fDocumentNumber;
        [Size(50)]
        //[Indexed(Unique = true)]
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        fin_articleserialnumber fArticleSerialNumber;
        //[Indexed(Unique = true)]
        public fin_articleserialnumber ArticleSerialNumber
        {
            get { return fArticleSerialNumber; }
            set { SetPropertyValue<fin_articleserialnumber>("ArticleSerialNumber", ref fArticleSerialNumber, value); }
        }

        fin_article fArticle;
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue<fin_article>("Article", ref fArticle, value); }
        }

        Decimal fQuantity;
        public Decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<Decimal>("Quantity", ref fQuantity, value); }
        }

        decimal fPurchasePrice;
        public decimal PurchasePrice
        {
            get { return fPurchasePrice; }
            set { SetPropertyValue<decimal>("PurchasePrice", ref fPurchasePrice, value); }
        }

        byte[] fAttachedFile;
        public byte[] AttachedFile
        {
            get { return fAttachedFile; }
            set { SetPropertyValue<byte[]>("AttachedFile", ref fAttachedFile, value); }
        }

        fin_documentfinancemaster fDocumentMaster;
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue<fin_documentfinancemaster>("DocumentMaster", ref fDocumentMaster, value); }
        }

        fin_documentfinancedetail fDocumentDetail;
        public fin_documentfinancedetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue<fin_documentfinancedetail>("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}