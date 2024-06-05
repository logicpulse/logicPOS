using System.Collections.Generic;

namespace LogicPOS.DTOs.Reporting
{
    public class FinancePaymentViewReportDto 
    {
        public string Oid { get; set; }                                        
        public string DocumentTypeDesignation { get; set; }                            
        public string DocumentTypeResourceString { get; set; }                          
        public string DocumentTypeResourceStringReport { get; set; }                    
        public string PaymentRefNo { get; set; }                                        
        public string PaymentStatus { get; set; }                                       
        public decimal PaymentAmount { get; set; }                                         
        public decimal TaxPayable { get; set; }                                         
        public string PaymentDate { get; set; }                                         
        public string DocumentDate { get; set; }                                        
        public string ExtendedValue { get; set; }                                       
        public uint EntityCode { get; set; }                                            
        public string EntityName { get; set; }                                          
        public string EntityAddress { get; set; }                                       
        public string EntityZipCode { get; set; }                                       
        public string EntityCity { get; set; }                                          
        public string EntityLocality { get; set; }                                      
        public string EntityCountry { get; set; }                                       
        public string EntityFiscalNumber { get; set; }                                  
        public uint MethodCode { get; set; }                                            
        public string PaymentMethodDesignation { get; set; }                            
        public string CurrencyDesignation { get; set; }                 
        public string CurrencyAcronym { get; set; }                     
        public string CurrencySymbol { get; set; }                      
        public decimal ExchangeRate { get; set; }                       
        public string Notes { get; set; }                               
        public List<FinancePaymentDocumentViewReportDto> DocumentFinancePaymentDocument { get; set; }
    }
}