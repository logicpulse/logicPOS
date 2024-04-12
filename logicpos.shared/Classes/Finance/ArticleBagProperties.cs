using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.shared.Classes.Finance
{
    public class ArticleBagProperties
    {
        private Guid _placeOid;
        public Guid PlaceOid
        {
            get { return _placeOid; }
            set { _placeOid = value; }
        }

        private Guid _tableOid;
        public Guid TableOid
        {
            get { return _tableOid; }
            set { _tableOid = value; }
        }

        private PriceType _priceType;
        public PriceType PriceType
        {
            get { return _priceType; }
            set { _priceType = value; }
        }

        private string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        private decimal _quantity;
        public decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        private string _unitMeasure;
        public string UnitMeasure
        {
            get { return _unitMeasure; }
            set { _unitMeasure = value; }
        }

        private decimal _priceWithDiscount = 0;
        public decimal PriceWithDiscount
        {
            get { return _priceWithDiscount; }
            set { _priceWithDiscount = value; }
        }

        private decimal _priceWithDiscountGlobal = 0;
        public decimal PriceWithDiscountGlobal
        {
            get { return _priceWithDiscountGlobal; }
            set { _priceWithDiscountGlobal = value; }
        }

        private decimal _totalGross = 0;
        public decimal TotalGross
        {
            get { return _totalGross; }
            set { _totalGross = value; }
        }

        private decimal _totalNet = 0;
        public decimal TotalNet
        {
            get { return _totalNet; }
            set { _totalNet = value; }
        }

        private decimal _totalDiscount = 0;
        public decimal TotalDiscount
        {
            get { return _totalDiscount; }
            set { _totalDiscount = value; }
        }

        private decimal _totalTax = 0;
        public decimal TotalTax
        {
            get { return _totalTax; }
            set { _totalTax = value; }
        }

        private decimal _totalFinal = 0;
        public decimal TotalFinal
        {
            get { return _totalFinal; }
            set { _totalFinal = value; }
        }

        private decimal _priceFinal = 0;
        public decimal PriceFinal
        {
            get { return _priceFinal; }
            set { _priceFinal = value; }
        }

        //Reference DocumentMaster usefull for <References> in CreditNotes, see SAF-T PT References
        private fin_documentfinancemaster fReference;
        public fin_documentfinancemaster Reference
        {
            get { return fReference; }
            set { fReference = value; }
        }

        //Reference Reason
        private string fReason;
        [Size(50)]
        public string Reason
        {
            get { return fReason; }
            set { fReason = value; }
        }

        private string fToken1;
        [Size(255)]
        public string Token1
        {
            get { return fToken1; }
            set { fToken1 = value; }
        }

        private string fToken2;
        [Size(255)]
        public string Token2
        {
            get { return fToken2; }
            set { fToken2 = value; }
        }

        private string fNotes;
        [Size(255)]
        public string Notes
        {
            get { return fNotes; }
            set { fNotes = value; }
        }

        private string fSerialNumber;
        [Size(255)]
        public string SerialNumber
        {
            get { return fSerialNumber; }
            set { fSerialNumber = value; }
        }

        private string fWarehouse;
        [Size(255)]
        public string Warehouse
        {
            get { return fWarehouse; }
            set { fWarehouse = value; }
        }

        //this TreeIter as object Type, to Remove Gtk from Framework, need to be cast as (TreeIter as TreeIter) when used
        private object _treeIter;
        public object TreeIter
        {
            get { return _treeIter; }
            set { _treeIter = value; }
        }

        //Used to store TreeView Index, Required to Get Current TreeView Position
        private int _listIndex;
        public int ListIndex
        {
            get { return _listIndex; }
            set { _listIndex = value; }
        }

        public ArticleBagProperties(Guid pPlaceOid, Guid pTableOid, PriceType pPriceType, string pCode, decimal pQuantity, string pUnitMeasure)
        {
            _placeOid = pPlaceOid;
            _tableOid = pTableOid;
            _priceType = pPriceType;
            _code = pCode;
            _quantity = pQuantity;
            _unitMeasure = pUnitMeasure;
        }
    }
}
