using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using System;
using System.Collections;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Stocks
{
    public class ProcessArticleStockParameter
    {
        private readonly erp_customer _customer;
        public erp_customer Customer
        {
            get { return _customer; }
        }
        private readonly DateTime _documentDate;
        public DateTime DocumentDate
        {
            get { return _documentDate; }
        }
        private readonly string _documentNumber;
        public string DocumentNumber
        {
            get { return _documentNumber; }
        }
        private readonly Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>> _articleCollection;
        public Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal, fin_warehouselocation>> ArticleCollection
        {
            get { return _articleCollection; }
        }
        private Dictionary<fin_article, decimal> _articleCollectionSimple;
        public Dictionary<fin_article, decimal> ArticleCollectionSimple
        {
            get { return _articleCollectionSimple; }
            set { _articleCollectionSimple = value; }
        }
        private fin_article _article;
        public fin_article Article
        {
            get { return _article; }
            set { _article = value; }
        }
        private decimal _quantity;
        public decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }
        private decimal _purchasePrice;
        public decimal PurchasePrice
        {
            get { return _purchasePrice; }
            set { _purchasePrice = value; }
        }
        private string _serialNumber;
        public string SerialNumber
        {
            get { return _serialNumber; }
            set { _serialNumber = value; }
        }
        private List<fin_articleserialnumber> _associatedArticles;
        public List<fin_articleserialnumber> AssociatedArticles
        {
            get { return _associatedArticles; }
            set { _associatedArticles = value; }
        }
        private fin_warehouselocation _warehouselocation;
        public fin_warehouselocation WarehouseLocation
        {
            get { return _warehouselocation; }
            set { _warehouselocation = value; }
        }
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }
        private byte[] _attachedFile;
        public byte[] AttachedFile
        {
            get { return _attachedFile; }
            set { _attachedFile = value; }
        }
        //Gestão de Stocks [IN:016527]
        public ProcessArticleStockParameter()  { }
        public ProcessArticleStockParameter(erp_customer pCustomer, DateTime pDocumentDate, string pDocumentNumber, Dictionary<fin_article, Tuple<decimal, Dictionary<string, List<fin_articleserialnumber>>, decimal,fin_warehouselocation>> pArticleCollection, decimal pQuantity, string pNotes, byte[] pAttachedFile)
        {
            //if (pCustomer.Oid == Guid.Parse("00000000-0000-0000-0000-000000000002") && string.IsNullOrEmpty(_documentNumber)) pDocumentNumber = resources.CustomResources.GetCustomResources(DataLayerFramework.Settings["customCultureResourceDefinition"], "global_internal_document_footer1");
            _customer = pCustomer;
            _documentDate = pDocumentDate;
            _documentNumber = pDocumentNumber;
            _articleCollection = pArticleCollection;
            _quantity = pQuantity;
            _notes = pNotes;
            _attachedFile = pAttachedFile;
        }
        public ProcessArticleStockParameter(erp_customer pCustomer, DateTime pDocumentDate, string pDocumentNumber, Dictionary<fin_article, decimal> pArticleCollection, decimal pQuantity, string pNotes, byte[] pAttachedFile)
        {
            _customer = pCustomer;
            _documentDate = pDocumentDate;
            _documentNumber = pDocumentNumber;
            _articleCollectionSimple = pArticleCollection;
            _quantity = pQuantity;
            _notes = pNotes;
            _attachedFile = pAttachedFile;
        }
    }
}
