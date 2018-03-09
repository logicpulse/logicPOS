using logicpos.datalayer.DataLayer.Xpo;

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    [FRBO(Entity = "view_documentfinancemastertotal")]
    public class FRBODocumentFinanceMasterTotalView : FRBOBaseObject
    {
        [FRBO(Field = "fmtOid", Hide = true)]
        override public string Oid { get; set; }                //fmtOid AS Oid,  

        [FRBO(Field = "fmtDocumentMaster")]
        public string DocumentMaster { get; set; }              //fmtDocumentMaster AS DocumentMaster,

        [FRBO(Field = "cvrDesignation")]
        public string Designation { get; set; }                 //cvrDesignation AS Designation,

        [FRBO(Field = "cvrTaxCode")]
        public string TaxCode { get; set; }                     //cvrTaxCode AS TaxCode,

        [FRBO(Field = "cvrTaxCountryRegion")]
        public string TaxCountryRegion { get; set; }            //cvrTaxCountryRegion AS TaxCountryRegion,

        [FRBO(Field = "fmtValue")]
        public decimal Value { get; set; }                      //fmtValue AS Value, 

        [FRBO(Field = "fmtTotal")]
        public decimal Total { get; set; }                      //fmtTotal AS Total,

        [FRBO(Field = "fmtTotalBase")]
        public decimal TotalBase { get; set; }                  //fmtTotalBase AS TotalBase,

        [FRBO(Field = "fmtTotalType")]
        public FinanceMasterTotalType TotalType { get; set; }   //fmtTotalType AS TotalType
    }
}
