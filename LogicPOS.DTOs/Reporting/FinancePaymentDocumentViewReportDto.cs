namespace LogicPOS.DTOs.Reporting
{
    public class FinancePaymentDocumentViewReportDto 
    {
        public string Oid { get; set; }                                      
        public string DocumentTypeDesignation { get; set; }                           
        public string DocumentNumber { get; set; }                       
        public string DocumentDate { get; set; }                                
        public decimal DocumentTotal { get; set; }                                                                                       
        public decimal TotalTax { get; set; }   
        public decimal CreditAmount { get; set; }   
        public decimal DebitAmount { get; set; }
        public bool Payed { get; set; }
    }
}
