using logicpos.datalayer.DataLayer.Xpo;
using System;

namespace logicpos.financial.library.Classes.Reports.BOs
{
    //Now Entity is Required to be defined, since implementation of Table Prefix
    [FRBO(Entity = "fin_documentfinancedetail")]
    public class FRBODocumentFinanceDetail : FRBOBaseObject
    {
        public string Code { get; set; }
        public string Designation { get; set; }
        public decimal Quantity { get; set; }
        public string UnitMeasure { get; set; }
        public decimal Price { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalFinal { get; set; }
        public string VatExemptionReasonDesignation { get; set; }
    }
}
