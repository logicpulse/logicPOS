using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Shared.Article;
using System;
using System.Collections.Generic;

namespace LogicPOS.Finance.DocumentProcessing
{

    public class DocumentProcessingParameters
    {
        //Required: DocumentType
        private Guid _documentType;
        public Guid DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        public ArticleBag ArticleBag { get; set; }

        //Required: Customer
        private Guid _customer;
        public Guid Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        public DateTime DocumentDateTime { get; set; }

        //Optional: PaymentConditions
        private Guid _paymentConditions;
        public Guid PaymentCondition
        {
            get { return _paymentConditions; }
            set { _paymentConditions = value; }
        }

        //Optional: PaymentMethod
        private Guid _paymentMethod;
        public Guid PaymentMethod
        {
            get { return _paymentMethod; }
            set { _paymentMethod = value; }
        }

        //Optional: Currency
        private Guid _currency;
        public Guid Currency
        {
            get { return _currency; }
            set { _currency = value; }
        }

        public decimal ExchangeRate { get; set; }

        public PersistFinanceDocumentSourceMode SourceMode { get; set; }

        public fin_documentordermain SourceOrderMain { get; set; }

        //Optional: DocumentParent
        private Guid _documentParent;
        public Guid DocumentParent
        {
            get { return _documentParent; }
            set { _documentParent = value; }
        }

        public List<fin_documentfinancemaster> FinanceDocuments { get; set; }

        public decimal TotalDelivery { get; set; }

        public decimal TotalChange { get; set; }

        public List<fin_documentfinancemaster> OrderReferences { get; set; }

        public MovementOfGoodsProperties ShipTo { get; set; }

        public MovementOfGoodsProperties ShipFrom { get; set; }

        public string Notes { get; set; }

        public DocumentProcessingParameters(Guid pDocumentType, ArticleBag pArticleBag)
        {
            //Init Default Values
            DocumentDateTime = XPOHelper.CurrentDateTimeAtomic();
            SourceMode = PersistFinanceDocumentSourceMode.CurrentOrderMain;
            TotalDelivery = 0.0m;
            TotalChange = 0.0m;
            _currency = XPOSettings.ConfigurationSystemCurrency.Oid;
            //_discount = 0.0m;
            ExchangeRate = 1.0m;

            //Init Parameters
            _documentType = pDocumentType;
            ArticleBag = pArticleBag;

            //Validate();
        }
    }
}
