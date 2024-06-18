using System;
using System.Collections.Generic;

namespace LogicPOS.DTOs.Reporting
{

    public class FinanceMasterViewReportDataDto 
    {
        public string Oid { get; set; }
                                                
        public string DocumentType { get; set; }

        public uint DocumentTypeOrd { get; set; }

        public uint DocumentTypeCode { get; set; }

        public string DocumentTypeDesignation { get; set; }

        public string DocumentTypeAcronym { get; set; }

        public string DocumentTypeResourceString { get; set; }

        public string DocumentTypeResourceStringReport { get; set; }

        public bool DocumentTypeWayBill { get; set; }                 

        public string DocumentNumber { get; set; }                                    

        public DateTime Date { get; set; }                                             

        public string DocumentDate { get; set; }                                       

        public string SystemEntryDate { get; set; }                                    

        public string DocumentCreatorUser { get; set; }                                

        public decimal TotalNet { get; set; }                                          

        public decimal TotalGross { get; set; }                                        

        public decimal TotalDiscount { get; set; }                                     

        public decimal TotalTax { get; set; }                                          

        public decimal TotalFinal { get; set; }                                        

        public decimal TotalFinalRound { get; set; }                                   

        public decimal TotalDelivery { get; set; }                                     

        public decimal TotalChange { get; set; }                                       

        public decimal Discount { get; set; }                                          

        public decimal DiscountFinancial { get; set; }                                 

        public decimal ExchangeRate { get; set; }                                      

        public string EntityOid { get; set; }                                          

        public uint EntityCode { get; set; }                                           

        public bool EntityHidden { get; set; }                                         

        public string EntityInternalCode { get; set; }                                 

        public string EntityName { get; set; }                                         

        public string EntityAddress { get; set; }                                      

        public string EntityZipCode { get; set; }                                      

        public string EntityCity { get; set; }                                         

        public string EntityLocality { get; set; }                                     

        public string EntityCountryCode2 { get; set; }                                 

        public string EntityCountry { get; set; }                                      

        public string EntityFiscalNumber { get; set; }                                 

        public string DocumentStatusStatus { get; set; }                               

        public string TransactionID { get; set; }                                      

        public string ShipToDeliveryID { get; set; }                                   

        public DateTime ShipToDeliveryDate { get; set; }                               

        public string ShipToWarehouseID { get; set; }                                  

        public string ShipToLocationID { get; set; }                                   

        public string ShipToAddressDetail { get; set; }                                

        public string ShipToCity { get; set; }                                         

        public string ShipToPostalCode { get; set; }                                   

        public string ShipToRegion { get; set; }                                       

        public string ShipToCountry { get; set; }                                      

        public string ShipFromDeliveryID { get; set; }                                 

        public DateTime ShipFromDeliveryDate { get; set; }                             

        public string ShipFromWarehouseID { get; set; }                                

        public string ShipFromLocationID { get; set; }                                 

        public string ShipFromAddressDetail { get; set; }                              

        public string ShipFromCity { get; set; }                                       
        
        public string ShipFromPostalCode { get; set; }                                 

        public string ShipFromRegion { get; set; }                                     

        public string ShipFromCountry { get; set; }                                    

        public DateTime MovementStartTime { get; set; }                                

        public DateTime MovementEndTime { get; set; }                                  
        
        public string ATDocCodeID { get; set; }                                        

        public bool Payed { get; set; }                                              

        public DateTime PayedDate { get; set; }                                        

        public string Notes { get; set; }                                              

        public string PaymentMethod { get; set; }

        public uint PaymentMethodOrd { get; set; }

        public uint PaymentMethodCode { get; set; }                                    

        public string PaymentMethodDesignation { get; set; }                           

        public string PaymentMethodToken { get; set; }                                 

        public string PaymentCondition { get; set; }

        public uint PaymentConditionOrd { get; set; }

        public uint PaymentConditionCode { get; set; }                                 

        public string PaymentConditionDesignation { get; set; }                        

        public string PaymentConditionAcronym { get; set; }                            

        public string Country { get; set; }

        public uint CountryOrd { get; set; }

        public uint CountryCode { get; set; }                                          

        public string CountryDesignation { get; set; }                                 

        public string Currency { get; set; }

        public uint CurrencyOrd { get; set; }

        public uint CurrencyCode { get; set; }                                         

        public string CurrencyDesignation { get; set; }                                

        public string CurrencyAcronym { get; set; }                                    

        public string ATDocQRCode { get; set; }                                    

        public List<FinanceDetailReportDataDto> DocumentFinanceDetail { get; set; }
        public List<FinanceMasterTotalViewReportDataDto> DocumentFinanceMasterTotal { get; set; }
    }
}
