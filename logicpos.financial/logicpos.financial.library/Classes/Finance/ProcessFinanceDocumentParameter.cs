using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Finance
{
    public class DocumentReference
    {
        private fin_documentfinancemaster _reference;
        public fin_documentfinancemaster Reference
        {
            get { return _reference; }
            set { _reference = value; }
        }

        private string _reason;
        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public DocumentReference(fin_documentfinancemaster pDocumentFinanceMaster, string pReason)
        {
            _reference = pDocumentFinanceMaster;
            _reason = pReason;
        }
    }

    public class MovementOfGoodsProperties
    {
        private string _deliveryID;
        public string DeliveryID
        {
            get { return _deliveryID; }
            set { _deliveryID = value; }
        }

        private DateTime _deliveryDate;
        public DateTime DeliveryDate
        {
            get { return _deliveryDate; }
            set { _deliveryDate = value; }
        }

        private string _warehouseID;
        public string WarehouseID
        {
            get { return _warehouseID; }
            set { _warehouseID = value; }
        }

        private string _locationID;
        public string LocationID
        {
            get { return _locationID; }
            set { _locationID = value; }
        }

        private string _buildingNumber;
        public string BuildingNumber
        {
            get { return _buildingNumber; }
            set { _buildingNumber = value; }
        }

        private string _streetName;
        public string StreetName
        {
            get { return _streetName; }
            set { _streetName = value; }
        }

        private string _addressDetail;
        public string AddressDetail
        {
            get { return _addressDetail; }
            set { _addressDetail = value; }
        }

        private string _postalCode;
        public string PostalCode
        {
            get { return _postalCode; }
            set { _postalCode = value; }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { _city = value; }
        }

        private string _region;
        public string Region
        {
            get { return _region; }
            set { _region = value; }
        }

        private string _country;
        public string Country
        {
            get { return _country; }
            set { _country = value; }
        }

        private Guid _countryGuid;
        public Guid CountryGuid
        {
            get { return _countryGuid; }
            set { _countryGuid = value; }
        }
    }

    public class ProcessFinanceDocumentParameter
    {
        //Required: DocumentType
        private Guid _documentType;
        public Guid DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        //Required: ArticleBag
        private ArticleBag _articleBag;
        public ArticleBag ArticleBag
        {
            get { return _articleBag; }
            set { _articleBag = value; }
        }

        //Required: Customer
        private Guid _customer;
        public Guid Customer
        {
            get { return _customer; }
            set { _customer = value; }
        }

        //Optional: DocumentDateTime
        private DateTime _documentDateTime;
        public DateTime DocumentDateTime
        {
            get { return _documentDateTime; }
            set { _documentDateTime = value; }
        }
        
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

        //Optional: ExchangeRate
        private decimal _exchangeRate;
        public decimal ExchangeRate
        {
            get { return _exchangeRate; }
            set { _exchangeRate = value; }
        }

        //Optional: SourceMode
        private PersistFinanceDocumentSourceMode _sourceMode;
        public PersistFinanceDocumentSourceMode SourceMode
        {
            get { return _sourceMode; }
            set { _sourceMode = value; }
        }

        //Optional: DocumentOrderMain
        private fin_documentordermain _sourceOrderMain;
        public fin_documentordermain SourceOrderMain
        {
            get { return _sourceOrderMain; }
            set { _sourceOrderMain = value; }
        }

        //Optional: DocumentParent
        private Guid _documentParent;
        public Guid DocumentParent
        {
            get { return _documentParent; }
            set { _documentParent = value; }
        }

        //Optional: FinanceDocuments
        private List<fin_documentfinancemaster> _financeDocuments;
        public List<fin_documentfinancemaster> FinanceDocuments
        {
            get { return _financeDocuments; }
            set { _financeDocuments = value; }
        }

        //Optional: TotalDelivery
        private decimal _totalDelivery;
        public decimal TotalDelivery
        {
            get { return _totalDelivery; }
            set { _totalDelivery = value; }
        }

        //Optional: TotalChange
        private decimal _totalChange;
        public decimal TotalChange
        {
            get { return _totalChange; }
            set { _totalChange = value; }
        }

        //Optional: OrderReferences
        private List<fin_documentfinancemaster> _orderReferences;
        public List<fin_documentfinancemaster> OrderReferences
        {
            get { return _orderReferences; }
            set { _orderReferences = value; }
        }

        //Optional: ShipTo
        private MovementOfGoodsProperties _shipTo;
        public MovementOfGoodsProperties ShipTo
        {
            get { return _shipTo; }
            set { _shipTo = value; }
        }

        //Optional: ShipFrom
        private MovementOfGoodsProperties _shipFrom;
        public MovementOfGoodsProperties ShipFrom
        {
            get { return _shipFrom; }
            set { _shipFrom = value; }
        }

        //Optional: Notes
        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set { _notes = value; }
        }

        public ProcessFinanceDocumentParameter(Guid pDocumentType, ArticleBag pArticleBag)
        {
            //Init Default Values
            _documentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
            _sourceMode = PersistFinanceDocumentSourceMode.CurrentOrderMain;
            _totalDelivery = 0.0m;
            _totalChange = 0.0m;
            _currency = SettingsApp.ConfigurationSystemCurrency.Oid;
            //_discount = 0.0m;
            _exchangeRate = 1.0m;

            //Init Parameters
            _documentType = pDocumentType;
            _articleBag = pArticleBag;

            //Validate();
        }
    }
}
