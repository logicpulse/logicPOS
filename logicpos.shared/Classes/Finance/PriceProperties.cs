using logicpos.datalayer.App;
using logicpos.shared;
using logicpos.shared.Enums;
using System;
using System.IO;
using System.Reflection;

// Use c:\Users\mario.monteiro\Desktop\Development\logicpulseProjects\logic.pos\Other\Sheets\invoicediscounts.ods 
// To Get Helper Data

//Use Prices in logicpos
// PriceUser   - Price input by user, only used to cal PriceNet
// PriceNet    - Price to used in Everywhere/Persistance etc, the Final Price, with With priceWithVat False
// PriceFinal  - Price to Show to User, Includes Discount, Discount Global and Vat

namespace logicpos.shared.Classes.Finance
{
    public class PriceProperties
    {
        PricePropertiesSourceMode _sourceMode;
        public PricePropertiesSourceMode SourceMode
        {
            get { return _sourceMode; }
            set { _sourceMode = value; }
        }

        decimal _quantity;
        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                //If Change Quantity, change mode to PriceNet, Require Update()
                if (_sourceMode != PricePropertiesSourceMode.FromPriceNet)
                {
                    _sourceMode = PricePropertiesSourceMode.FromPriceNet;
                }
            }
        }

        decimal _priceUser;
        public decimal PriceUser
        {
            get { return _priceUser; }
            set
            {
                _priceUser = value;
                //If Change PriceUser, change mode to PriceUser, Require Update()
                if (_sourceMode != PricePropertiesSourceMode.FromPriceUser)
                {
                    _sourceMode = PricePropertiesSourceMode.FromPriceUser;
                }
            }
        }

        decimal _discountArticle;
        public decimal DiscountArticle
        {
            get { return _discountArticle; }
            set { _discountArticle = value; }
        }

        decimal _discountGlobal;
        public decimal DiscountGlobal
        {
            get { return _discountGlobal; }
            set { _discountGlobal = value; }
        }

        bool _priceWithVat;
        public bool PriceWithVat
        {
            get { return _priceWithVat; }
            set { _priceWithVat = value; }
        }

        decimal _vat;
        public decimal Vat
        {
            get { return _vat; }
            set { _vat = value; }
        }

        Guid _vatExemptionReason;
        public Guid VatExemptionReason
        {
            get { return _vatExemptionReason; }
            set { _vatExemptionReason = value; }
        }

        decimal _priceNet;
        public decimal PriceNet
        {
            get { return _priceNet; }
            set
            {
                _priceNet = value;
                //If Change PriceNet, change mode to PriceNet, Require Update()
                if (_sourceMode != PricePropertiesSourceMode.FromPriceNet)
                {
                    _sourceMode = PricePropertiesSourceMode.FromPriceNet;
                }
            }
        }

        decimal _priceWithDiscount;
        public decimal PriceWithDiscount
        {
            get { return _priceWithDiscount; }
            set { _priceWithDiscount = value; }
        }

        decimal _totalNetBeforeDiscountGlobal;
        /// <summary>
        /// Represents the total net after applying Customer Discount and before Global Discount (discount registered for customer).
        /// Used when showing price details from item level.
		/// See #IN009235 for further details.
        /// </summary>
        public decimal TotalNetBeforeDiscountGlobal
        {
            get { return _totalNetBeforeDiscountGlobal; }
            set { _totalNetBeforeDiscountGlobal = value; }
        }

        decimal _totalFinalBeforeDiscountGlobal;
        /// <summary>
        /// Represents the total final after applying Customer Discount and before Global Discount (discount registered for customer).
        /// Used when showing price details from item level.
		/// See #IN009235 for further details.
        /// </summary>
        public decimal TotalFinalBeforeDiscountGlobal
        {
            get { return _totalFinalBeforeDiscountGlobal; }
            set { _totalFinalBeforeDiscountGlobal = value; }
        }

        decimal _priceWithDiscountGlobal;
        public decimal PriceWithDiscountGlobal
        {
            get { return _priceWithDiscountGlobal; }
            set { _priceWithDiscountGlobal = value; }
        }

        decimal _totalGross;
        public decimal TotalGross
        {
            get { return _totalGross; }
            set { _totalGross = value; }
        }

        decimal _totalNet;
        public decimal TotalNet
        {
            get { return _totalNet; }
            set { _totalNet = value; }
        }

        decimal _totalDiscount;
        public decimal TotalDiscount
        {
            get { return _totalDiscount; }
            set { _totalDiscount = value; }
        }

        decimal _totalTax;
        public decimal TotalTax
        {
            get { return _totalTax; }
            set { _totalTax = value; }
        }

        decimal _totalFinal;
        public decimal TotalFinal
        {
            get { return _totalFinal; }
            set
            {
                _totalFinal = value;
                //If Change TotalFinal, change mode to TotalFinal, Require Update()
                if (_sourceMode != PricePropertiesSourceMode.FromTotalFinal)
                {
                    _sourceMode = PricePropertiesSourceMode.FromTotalFinal;
                }
            }
        }

        decimal _priceFinal;
        public decimal PriceFinal
        {
            get { return _priceFinal; }
            set { _priceFinal = value; }
        }

        public PriceProperties(PricePropertiesSourceMode pSourceMode, bool pPriceWithVat, decimal pSource, decimal pQuantity, decimal pDiscountArticle, decimal pDiscountGlobal, decimal pVat)
        {
            //Fixed, Never Change
            _sourceMode = pSourceMode;
            _priceWithVat = pPriceWithVat;
            _quantity = pQuantity;
            _discountArticle = pDiscountArticle;
            _discountGlobal = pDiscountGlobal;
            _vat = pVat;

            switch (pSourceMode)
            {
                case PricePropertiesSourceMode.FromPriceUser:
                    _priceUser = pSource;
                    break;
                case PricePropertiesSourceMode.FromPriceNet:
                    _priceNet = pSource;
                    break;
                case PricePropertiesSourceMode.FromTotalFinal:
                    _totalFinal = pSource;
                    break;
            }
            Update();
        }

        public void Update()
        {
            switch (_sourceMode)
            {
                case PricePropertiesSourceMode.FromPriceUser:
                case PricePropertiesSourceMode.FromPriceNet:
                    switch (_sourceMode)
                    {
                        case PricePropertiesSourceMode.FromPriceUser:
                            //=IF(PriceWithVat=1;Price/(Vat/100+1);Price)
                            _priceNet = (_priceWithVat) ? _priceUser / (_vat / 100 + 1) : _priceUser;
                            break;
                        case PricePropertiesSourceMode.FromPriceNet:
                            //=IF(PriceWithVat=1;PriceNet*(Vat/100+1);PriceNet)
                            _priceUser = (_priceWithVat) ? _priceNet * (_vat / 100 + 1) : _priceNet;
                            break;
                    }
                    //=PriceNet - ((PriceNet * Discount) / 100)
                    _priceWithDiscount = _priceNet - (_priceNet * _discountArticle) / 100;
                    //=PriceWithDiscount - ((PriceWithDiscount * GlobalDiscount) / 100)
                    _priceWithDiscountGlobal = _priceWithDiscount - ((_priceWithDiscount * _discountGlobal) / 100);
                    //=Quantity*PriceNet
                    _totalGross = _quantity * _priceNet;
                    //=Quantity*PriceWithDiscountGlobal
                    _totalNet = _quantity * _priceWithDiscountGlobal;

                    /* IN009235 - Total Net before applying Discount Global (discount registered for customer) */
                    _totalNetBeforeDiscountGlobal = _quantity * _priceWithDiscount;

                    //=TotalGross-TotalNet
                    _totalDiscount = _totalGross - _totalNet;
                    //=(TotalNet*(Vat/100+1))-TotalNet
                    _totalTax = (_totalNet * (_vat / 100 + 1)) - _totalNet;
                    //=TotalGross-TotalDiscount+TotalTax
                    _totalFinal = _totalGross - _totalDiscount + _totalTax;

                    /* IN009235 - Total Final before applying Discount Global (discount registered for customer) */
                    decimal totalTaxBeforeDiscountGlobal = _totalNetBeforeDiscountGlobal * (_vat / 100);
                    decimal totalDiscountBeforeDiscountGlobal = _totalGross - _totalNetBeforeDiscountGlobal;
                    _totalFinalBeforeDiscountGlobal = _totalGross - totalDiscountBeforeDiscountGlobal + totalTaxBeforeDiscountGlobal;

                    break;
                case PricePropertiesSourceMode.FromTotalFinal:
                    //=TotalFinal-(TotalFinal/(Vat/100+1))
                    _totalTax = _totalFinal - (_totalFinal / (_vat / 100 + 1));
                    //=TotalFinal-TotalTax
                    _totalNet = _totalFinal - _totalTax;
                    //=TotalNet/Quantity
                    _priceWithDiscountGlobal = _totalNet / _quantity;
                    //=PriceWithDiscountGlobal/(-GlobalDiscount/100+1)
                    _priceWithDiscount = _priceWithDiscountGlobal / (-_discountGlobal / 100 + 1);
                    //=PriceWithDiscount/(-Discount/100+1)
                    _priceNet = _priceWithDiscount / (-_discountArticle / 100 + 1);
                    //=Quantity*PriceNet
                    _totalGross = _quantity * _priceNet;
                    //=TotalGross-TotalNet
                    _totalDiscount = _totalGross - _totalNet;
                    //=IF(PriceWithVat=1;PriceNet*(Vat/100+1);PriceNet)
                    _priceUser = (_priceWithVat) ? _priceNet * (_vat / 100 + 1) : _priceNet;
                    break;
            }
            //Shared
            //=IF(TotalFinal>0;TotalFinal/Quantity;0)
            _priceFinal = (_totalFinal > 0) ? _totalFinal / _quantity : 0.0m;
        }

        public void SendToLog(String pLabel)
        {
            PropertyInfo[] pis = this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

            StreamWriter sw = new StreamWriter(string.Format("{0}output.txt", GlobalFramework.Path["temp"]), true);
            sw.WriteLine(pLabel);

            foreach (PropertyInfo p in pis)
            {
                sw.Write(string.Format("{0}\t", p.Name));
            }

            sw.WriteLine("");
            foreach (PropertyInfo p in pis)
            {
                sw.Write(string.Format("{0}\t", p.GetValue(this)));
            }

            sw.WriteLine(Environment.NewLine);
            sw.Close();
        }

        public static PriceProperties GetPriceProperties(PricePropertiesSourceMode pSourceMode, bool pPriceWithVat, decimal pSource, decimal pQuantity, decimal pDiscountArticle, decimal pDiscountGlobal, decimal pVat)
        {
            PriceProperties result = new PriceProperties(pSourceMode, pPriceWithVat, pSource, pQuantity, pDiscountArticle, pDiscountGlobal, pVat);
            return result;
        }
    }
}
