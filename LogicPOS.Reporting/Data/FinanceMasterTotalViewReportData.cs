using LogicPOS.Domain.Enums;
using LogicPOS.Reporting.Data.Common;

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "view_documentfinancemastertotal")]
    public class FinanceMasterTotalViewReportData : ReportData
    {
        [ReportData(Field = "fmtOid", Hide = true)]
        override public string Oid { get; set; }                //fmtOid AS Oid,  

        [ReportData(Field = "fmtDocumentMaster")]
        public string DocumentMaster { get; set; }              //fmtDocumentMaster AS DocumentMaster,

        [ReportData(Field = "cvrDesignation")]
        public string Designation { get; set; }                 //cvrDesignation AS Designation,

        [ReportData(Field = "cvrTaxCode")]
        public string TaxCode { get; set; }                     //cvrTaxCode AS TaxCode,

        [ReportData(Field = "cvrTaxCountryRegion")]
        public string TaxCountryRegion { get; set; }            //cvrTaxCountryRegion AS TaxCountryRegion,

        [ReportData(Field = "fmtValue")]
        public decimal Value { get; set; }                      //fmtValue AS Value, 

        [ReportData(Field = "fmtTotal")]
        public decimal Total { get; set; }                      //fmtTotal AS Total,

        [ReportData(Field = "fmtTotalBase")]
        public decimal TotalBase { get; set; }                  //fmtTotalBase AS TotalBase,

        [ReportData(Field = "fmtTotalType")]
        public FinanceMasterTotalType TotalType { get; set; }   //fmtTotalType AS TotalType
    }
}
