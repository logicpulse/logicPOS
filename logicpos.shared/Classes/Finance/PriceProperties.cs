using logicpos.datalayer.App;
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
        public PricePropertiesSourceMode SourceMode { get; set; }

        private decimal _quantity;
        public decimal Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                //If Change Quantity, change mode to PriceNet, Require Update()
                if (SourceMode != PricePropertiesSourceMode.FromPriceNet)
                {
                    SourceMode = PricePropertiesSourceMode.FromPriceNet;
                }
            }
        }

        private decimal _priceUser;
        public decimal PriceUser
        {
            get { return _priceUser; }
            set
            {
                _priceUser = value;
                //If Change PriceUser, change mode to PriceUser, Require Update()
                if (SourceMode != PricePropertiesSourceMode.FromPriceUser)
                {
                    SourceMode = PricePropertiesSourceMode.FromPriceUser;
                }
            }
        }

        public decimal DiscountArticle { get; set; }

        public decimal DiscountGlobal { get; set; }

        public bool PriceWithVat { get; set; }

        public decimal Vat { get; set; }

        private Guid _vatExemptionReason;
        public Guid VatExemptionReason
        {
            get { return _vatExemptionReason; }
            set { _vatExemptionReason = value; }
        }

        private decimal _priceNet;
        public decimal PriceNet
        {
            get { return _priceNet; }
            set
            {
                _priceNet = value;
                //If Change PriceNet, change mode to PriceNet, Require Update()
                if (SourceMode != PricePropertiesSourceMode.FromPriceNet)
                {
                    SourceMode = PricePropertiesSourceMode.FromPriceNet;
                }
            }
        }

        public decimal PriceWithDiscount { get; set; }

        /// <summary>
        /// Represents the total net after applying Customer Discount and before Global Discount (discount registered for customer).
        /// Used when showing price details from item level.
        /// See #IN009235 for further details.
        /// </summary>
        public decimal TotalNetBeforeDiscountGlobal { get; set; }

        /// <summary>
        /// Represents the total final after applying Customer Discount and before Global Discount (discount registered for customer).
        /// Used when showing price details from item level.
        /// See #IN009235 for further details.
        /// </summary>
        public decimal TotalFinalBeforeDiscountGlobal { get; set; }

        public decimal PriceWithDiscountGlobal { get; set; }

        public decimal TotalGross { get; set; }

        public decimal TotalNet { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal TotalTax { get; set; }

        private decimal _totalFinal;
        public decimal TotalFinal
        {
            get { return _totalFinal; }
            set
            {
                _totalFinal = value;
                //If Change TotalFinal, change mode to TotalFinal, Require Update()
                if (SourceMode != PricePropertiesSourceMode.FromTotalFinal)
                {
                    SourceMode = PricePropertiesSourceMode.FromTotalFinal;
                }
            }
        }

        public decimal PriceFinal { get; set; }

        public PriceProperties(PricePropertiesSourceMode pSourceMode, bool pPriceWithVat, decimal pSource, decimal pQuantity, decimal pDiscountArticle, decimal pDiscountGlobal, decimal pVat)
        {
            //Fixed, Never Change
            SourceMode = pSourceMode;
            PriceWithVat = pPriceWithVat;
            _quantity = pQuantity;
            DiscountArticle = pDiscountArticle;
            DiscountGlobal = pDiscountGlobal;
            Vat = pVat;

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
            switch (SourceMode)
            {
                case PricePropertiesSourceMode.FromPriceUser:
                case PricePropertiesSourceMode.FromPriceNet:
                    switch (SourceMode)
                    {
                        case PricePropertiesSourceMode.FromPriceUser:
                            //=IF(PriceWithVat=1;Price/(Vat/100+1);Price)
                            _priceNet = (PriceWithVat) ? _priceUser / (Vat / 100 + 1) : _priceUser;
                            break;
                        case PricePropertiesSourceMode.FromPriceNet:
                            //=IF(PriceWithVat=1;PriceNet*(Vat/100+1);PriceNet)
                            _priceUser = (PriceWithVat) ? _priceNet * (Vat / 100 + 1) : _priceNet;
                            break;
                    }
                    //=PriceNet - ((PriceNet * Discount) / 100)
                    PriceWithDiscount = _priceNet - (_priceNet * DiscountArticle) / 100;
                    //=PriceWithDiscount - ((PriceWithDiscount * GlobalDiscount) / 100)
                    PriceWithDiscountGlobal = PriceWithDiscount - ((PriceWithDiscount * DiscountGlobal) / 100);
                    //=Quantity*PriceNet
                    TotalGross = _quantity * _priceNet;
                    //=Quantity*PriceWithDiscountGlobal
                    TotalNet = _quantity * PriceWithDiscountGlobal;

                    /* IN009235 - Total Net before applying Discount Global (discount registered for customer) */
                    TotalNetBeforeDiscountGlobal = _quantity * PriceWithDiscount;

                    //=TotalGross-TotalNet
                    TotalDiscount = TotalGross - TotalNet;
                    //=(TotalNet*(Vat/100+1))-TotalNet
                    TotalTax = (TotalNet * (Vat / 100 + 1)) - TotalNet;
                    //=TotalGross-TotalDiscount+TotalTax
                    _totalFinal = TotalGross - TotalDiscount + TotalTax;

                    /* IN009235 - Total Final before applying Discount Global (discount registered for customer) */
                    decimal totalTaxBeforeDiscountGlobal = TotalNetBeforeDiscountGlobal * (Vat / 100);
                    decimal totalDiscountBeforeDiscountGlobal = TotalGross - TotalNetBeforeDiscountGlobal;
                    TotalFinalBeforeDiscountGlobal = TotalGross - totalDiscountBeforeDiscountGlobal + totalTaxBeforeDiscountGlobal;

                    break;
                case PricePropertiesSourceMode.FromTotalFinal:
                    //=TotalFinal-(TotalFinal/(Vat/100+1))
                    TotalTax = _totalFinal - (_totalFinal / (Vat / 100 + 1));
                    //=TotalFinal-TotalTax
                    TotalNet = _totalFinal - TotalTax;
                    //=TotalNet/Quantity
                    PriceWithDiscountGlobal = TotalNet / _quantity;
                    //=PriceWithDiscountGlobal/(-GlobalDiscount/100+1)
                    PriceWithDiscount = PriceWithDiscountGlobal / (-DiscountGlobal / 100 + 1);
                    //=PriceWithDiscount/(-Discount/100+1)
                    _priceNet = PriceWithDiscount / (-DiscountArticle / 100 + 1);
                    //=Quantity*PriceNet
                    TotalGross = _quantity * _priceNet;
                    //=TotalGross-TotalNet
                    TotalDiscount = TotalGross - TotalNet;
                    //=IF(PriceWithVat=1;PriceNet*(Vat/100+1);PriceNet)
                    _priceUser = (PriceWithVat) ? _priceNet * (Vat / 100 + 1) : _priceNet;
                    break;
            }
            //Shared
            //=IF(TotalFinal>0;TotalFinal/Quantity;0)
            PriceFinal = (_totalFinal > 0) ? _totalFinal / _quantity : 0.0m;
        }

        public void SendToLog(string pLabel)
        {
            PropertyInfo[] pis = this.GetType().GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);

            StreamWriter sw = new StreamWriter(string.Format("{0}output.txt", DataLayerFramework.Path["temp"]), true);
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
