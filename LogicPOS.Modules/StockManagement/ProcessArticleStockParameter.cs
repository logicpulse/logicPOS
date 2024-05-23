using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using System;
using System.Collections.Generic;

namespace LogicPOS.Modules.StockManagement
{
    public class ProcessArticleStockParameter
    {
        public erp_customer Customer { get; }
        private readonly DateTime _documentDate;
        public DateTime DocumentDate
        {
            get { return _documentDate; }
        }

        public string DocumentNumber { get; }
        private readonly Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>> _articleCollection;
        public Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>> ArticleCollection
        {
            get { return _articleCollection; }
        }

        public Dictionary<fin_article, decimal> ArticleCollectionSimple { get; set; }
        private fin_article _article;
        public fin_article Article
        {
            get { return _article; }
            set { _article = value; }
        }

        public decimal Quantity { get; set; }
        private decimal _purchasePrice;
        public decimal PurchasePrice
        {
            get { return _purchasePrice; }
            set { _purchasePrice = value; }
        }

        public string SerialNumber { get; set; }
        private List<fin_articleserialnumber> _associatedArticles;
        public List<fin_articleserialnumber> AssociatedArticles
        {
            get { return _associatedArticles; }
            set { _associatedArticles = value; }
        }

        public fin_warehouselocation WarehouseLocation { get; set; }
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public byte[] AttachedFile { get; set; }
        //Gestão de Stocks [IN:016527]
        public ProcessArticleStockParameter()  { }
        public ProcessArticleStockParameter(erp_customer pCustomer, DateTime pDocumentDate, string pDocumentNumber, Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal,fin_warehouselocation>> pArticleCollection, decimal pQuantity, string pNotes, byte[] pAttachedFile)
        {
            //if (pCustomer.Oid == Guid.Parse("00000000-0000-0000-0000-000000000002") && string.IsNullOrEmpty(_documentNumber)) pDocumentNumber = CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_internal_document_footer1");
            Customer = pCustomer;
            _documentDate = pDocumentDate;
            DocumentNumber = pDocumentNumber;
            _articleCollection = pArticleCollection;
            Quantity = pQuantity;
            _notes = pNotes;
            AttachedFile = pAttachedFile;
        }
        public ProcessArticleStockParameter(erp_customer pCustomer, DateTime pDocumentDate, string pDocumentNumber, Dictionary<fin_article, decimal> pArticleCollection, decimal pQuantity, string pNotes, byte[] pAttachedFile)
        {
            Customer = pCustomer;
            _documentDate = pDocumentDate;
            DocumentNumber = pDocumentNumber;
            ArticleCollectionSimple = pArticleCollection;
            Quantity = pQuantity;
            _notes = pNotes;
            AttachedFile = pAttachedFile;
        }
    }
}
