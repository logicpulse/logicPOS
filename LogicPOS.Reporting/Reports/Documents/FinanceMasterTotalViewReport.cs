using LogicPOS.Domain.Enums;
using LogicPOS.Reporting.Common;

namespace LogicPOS.Reporting.Reports.Documents
{
    [Report(Entity = "view_documentfinancemastertotal")]
    public class FinanceMasterTotalViewReport : ReportData
    {
        [Report(Field = "fmtOid", Hide = true)]
        override public string Oid { get; set; }                //fmtOid AS Oid,  

        [Report(Field = "fmtDocumentMaster")]
        public string DocumentMaster { get; set; }              //fmtDocumentMaster AS DocumentMaster,

        [Report(Field = "cvrDesignation")]
        public string Designation { get; set; }                 //cvrDesignation AS Designation,

        [Report(Field = "cvrTaxCode")]
        public string TaxCode { get; set; }                     //cvrTaxCode AS TaxCode,

        [Report(Field = "cvrTaxCountryRegion")]
        public string TaxCountryRegion { get; set; }            //cvrTaxCountryRegion AS TaxCountryRegion,

        [Report(Field = "fmtValue")]
        public decimal Value { get; set; }                      //fmtValue AS Value, 

        [Report(Field = "fmtTotal")]
        public decimal Total { get; set; }                      //fmtTotal AS Total,

        [Report(Field = "fmtTotalBase")]
        public decimal TotalBase { get; set; }                  //fmtTotalBase AS TotalBase,

        [Report(Field = "fmtTotalType")]
        public FinanceMasterTotalType TotalType { get; set; }   //fmtTotalType AS TotalType
    }
}
