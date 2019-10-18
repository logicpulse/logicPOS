using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using System;

namespace logicpos.shared.Classes.Finance
{
    public class ArticleBagProperties
    {
        Guid _placeOid;
        public Guid PlaceOid
        {
            get { return _placeOid; }
            set { _placeOid = value; }
        }

        Guid _tableOid;
        public Guid TableOid
        {
            get { return _tableOid; }
            set { _tableOid = value; }
        }

        PriceType _priceType;
        public PriceType PriceType
        {
            get { return _priceType; }
            set { _priceType = value; }
        }

        string _code;
        public string Code
        {
            get { return _code; }
            set { _code = value; }
        }

        decimal _quantity;
        public decimal Quantity
        {
            get { return _quantity; }
            set { _quantity = value; }
        }

        string _unitMeasure;
        public string UnitMeasure
        {
            get { return _unitMeasure; }
            set { _unitMeasure = value; }
        }

        decimal _priceWithDiscount = 0;
        public decimal PriceWithDiscount
        {
            get { return _priceWithDiscount; }
            set { _priceWithDiscount = value; }
        }

        decimal _priceWithDiscountGlobal = 0;
        public decimal PriceWithDiscountGlobal
        {
            get { return _priceWithDiscountGlobal; }
            set { _priceWithDiscountGlobal = value; }
        }

        decimal _totalGross = 0;
        public decimal TotalGross
        {
            get { return _totalGross; }
            set { _totalGross = value; }
        }

        decimal _totalNet = 0;
        public decimal TotalNet
        {
            get { return _totalNet; }
            set { _totalNet = value; }
        }

        decimal _totalDiscount = 0;
        public decimal TotalDiscount
        {
            get { return _totalDiscount; }
            set { _totalDiscount = value; }
        }

        decimal _totalTax = 0;
        public decimal TotalTax
        {
            get { return _totalTax; }
            set { _totalTax = value; }
        }

        decimal _totalFinal = 0;
        public decimal TotalFinal
        {
            get { return _totalFinal; }
            set { _totalFinal = value; }
        }

        decimal _priceFinal = 0;
        public decimal PriceFinal
        {
            get { return _priceFinal; }
            set { _priceFinal = value; }
        }

        //Reference DocumentMaster usefull for <References> in CreditNotes, see SAF-T PT References
        fin_documentfinancemaster fReference;
        public fin_documentfinancemaster Reference
        {
            get { return fReference; }
            set { fReference = value; }
        }

        //Reference Reason
        string fReason;
        [Size(50)]
        public string Reason
        {
            get { return fReason; }
            set { fReason = value; }
        }

        string fToken1;
        [Size(255)]
        public string Token1
        {
            get { return fToken1; }
            set { fToken1 = value; }
        }

        string fToken2;
        [Size(255)]
        public string Token2
        {
            get { return fToken2; }
            set { fToken2 = value; }
        }

        string fNotes;
        [Size(255)]
        public string Notes
        {
            get { return fNotes; }
            set { fNotes = value; }
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
