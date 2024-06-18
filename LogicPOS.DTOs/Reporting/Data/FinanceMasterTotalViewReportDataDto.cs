namespace LogicPOS.DTOs.Reporting
{
    public class FinanceMasterTotalViewReportDataDto 
    {
        public string Oid { get; set; }            

        public string DocumentMaster { get; set; }            

        public string Designation { get; set; }               

        public string TaxCode { get; set; }                  

        public string TaxCountryRegion { get; set; }            

        public decimal Value { get; set; }                    

        public decimal Total { get; set; }                    

        public decimal TotalBase { get; set; }                 

        public int TotalType { get; set; } 
    }
}
