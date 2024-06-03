using System;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;

namespace LogicPOS.Domain.Entities
{
    [DeferredDeletion(false)]
    public class fin_articlestock : XPGuidObject
    {
        public fin_articlestock() : base() { }
        public fin_articlestock(Session session) : base(session) { }

        private uint fOrd;
        public uint Ord
        {
            get { return fOrd; }
            set { SetPropertyValue("Ord", ref fOrd, value); }
        }

        private DateTime fDate;
        public DateTime Date
        {
            get { return fDate; }
            set { SetPropertyValue<DateTime>("Date", ref fDate, value); }
        }

        private erp_customer fCustomer;
        public erp_customer Customer
        {
            get { return fCustomer; }
            set { SetPropertyValue("Customer", ref fCustomer, value); }
        }

        private string fDocumentNumber;
        [Size(50)]
        //[Indexed(Unique = true)]
        public string DocumentNumber
        {
            get { return fDocumentNumber; }
            set { SetPropertyValue<string>("DocumentNumber", ref fDocumentNumber, value); }
        }

        private fin_articleserialnumber fArticleSerialNumber;
        //[Indexed(Unique = true)]
        public fin_articleserialnumber ArticleSerialNumber
        {
            get { return fArticleSerialNumber; }
            set { SetPropertyValue("ArticleSerialNumber", ref fArticleSerialNumber, value); }
        }

        private fin_article fArticle;
        public fin_article Article
        {
            get { return fArticle; }
            set { SetPropertyValue("Article", ref fArticle, value); }
        }

        private decimal fQuantity;
        public decimal Quantity
        {
            get { return fQuantity; }
            set { SetPropertyValue<decimal>("Quantity", ref fQuantity, value); }
        }

        private decimal fPurchasePrice;
        public decimal PurchasePrice
        {
            get { return fPurchasePrice; }
            set { SetPropertyValue<decimal>("PurchasePrice", ref fPurchasePrice, value); }
        }

        private byte[] fAttachedFile;
        public byte[] AttachedFile
        {
            get { return fAttachedFile; }
            set { SetPropertyValue("AttachedFile", ref fAttachedFile, value); }
        }

        private fin_documentfinancemaster fDocumentMaster;
        public fin_documentfinancemaster DocumentMaster
        {
            get { return fDocumentMaster; }
            set { SetPropertyValue("DocumentMaster", ref fDocumentMaster, value); }
        }

        private fin_documentfinancedetail fDocumentDetail;
        public fin_documentfinancedetail DocumentDetail
        {
            get { return fDocumentDetail; }
            set { SetPropertyValue("DocumentDetail", ref fDocumentDetail, value); }
        }
    }
}