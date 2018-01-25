using logicpos.datalayer.DataLayer.Xpo;
using System;

namespace logicpos.financial.library.Classes.Stocks
{
    public class ProcessArticleStockParameter
    {
        private ERP_Customer _customer;
        public ERP_Customer Customer
        {
            get { return _customer; }
        }
        private DateTime _documentDate;
        public DateTime DocumentDate
        {
            get { return _documentDate; }
        }
        private string _documentNumber;
        public string DocumentNumber
        {
            get { return _documentNumber; }
        }
        private FIN_Article _article;
        public FIN_Article Article
        {
            get { return _article; }
        }
        private decimal _quantity;
        public decimal Quantity
        {
            get { return _quantity; }
        }
        private string _notes;
        public string Notes
        {
            get { return _notes; }
        }

        public ProcessArticleStockParameter()  { }
        public ProcessArticleStockParameter(ERP_Customer pCustomer, DateTime pDocumentDate, string pDocumentNumber, FIN_Article pArticle, decimal pQuantity, string pNotes)
        {
            _customer = pCustomer;
            _documentDate = pDocumentDate;
            _documentNumber = pDocumentNumber;
            _article = pArticle;
            _quantity = pQuantity;
            _notes = pNotes;
        }
    }
}
