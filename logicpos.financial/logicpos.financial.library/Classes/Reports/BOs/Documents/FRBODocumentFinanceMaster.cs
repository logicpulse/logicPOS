using logicpos.datalayer.DataLayer.Xpo;
using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    //Now Entity is Required to be defined, since implementation of Table Prefix
    [FRBO(Entity = "fin_documentfinancemaster")]
    public class FRBODocumentFinanceMaster : FRBOBaseObject
    {
        public DateTime Date { get; set; }
        public string DocumentNumber { get; set; }
        public string DocumentStatusStatus { get; set; }
        public string DocumentStatusDate { get; set; }
        public string DocumentStatusReason { get; set; }
        public string DocumentStatusUser { get; set; }
        public string SourceBilling { get; set; }
        public string Hash { get; set; }
        public string HashControl { get; set; }
        public string DocumentDate { get; set; }
        public int SelfBillingIndicator { get; set; }
        public int CashVatSchemeIndicator { get; set; }
        public int ThirdPartiesBillingIndicator { get; set; }
        public string DocumentCreatorUser { get; set; }
        public string EACCode { get; set; }
        public string SystemEntryDate { get; set; }
        public string TransactionID { get; set; }
        public string ShipToDeliveryID { get; set; }
        public DateTime ShipToDeliveryDate { get; set; }
        public string ShipToWarehouseID { get; set; }
        public string ShipToLocationID { get; set; }
        public string ShipToBuildingNumber { get; set; }
        public string ShipToStreetName { get; set; }
        public string ShipToAddressDetail { get; set; }
        public string ShipToCity { get; set; }
        public string ShipToPostalCode { get; set; }
        public string ShipToRegion { get; set; }
        public string ShipToCountry { get; set; }
        public string ShipFromDeliveryID { get; set; }
        public DateTime ShipFromDeliveryDate { get; set; }
        public string ShipFromWarehouseID { get; set; }
        public string ShipFromLocationID { get; set; }
        public string ShipFromBuildingNumber { get; set; }
        public string ShipFromStreetName { get; set; }
        public string ShipFromAddressDetail { get; set; }
        public string ShipFromCity { get; set; }
        public string ShipFromPostalCode { get; set; }
        public string ShipFromRegion { get; set; }
        public string ShipFromCountry { get; set; }
        public DateTime MovementStartTime { get; set; }
        public DateTime MovementEndTime { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalDiscount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalFinal { get; set; }
        public decimal TotalFinalRound { get; set; }
        public decimal TotalDelivery { get; set; }
        public decimal TotalChange { get; set; }
        public string ExternalDocument { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountFinancial { get; set; }
        public decimal ExchangeRate { get; set; }
        public Guid EntityOid { get; set; }
        public string EntityInternalCode { get; set; }
        public string EntityName { get; set; }
        public string EntityAddress { get; set; }
        public string EntityLocality { get; set; }
        public string EntityZipCode { get; set; }
        public string EntityCity { get; set; }
        public string EntityCountry { get; set; }
        public Guid EntityCountryOid { get; set; }
        public string EntityFiscalNumber { get; set; }
        public Boolean Payed { get; set; }
        public DateTime PayedDate { get; set; }
        public Boolean Printed { get; set; }
        // Navigation Properties
        public fin_documentordermain SourceOrderMain { get; set; }
        public fin_documentfinancetype DocumentType { get; set; }
        public fin_configurationpaymentmethod PaymentMethod { get; set; }
        public fin_configurationpaymentcondition PaymentCondition { get; set; }
        public cfg_configurationcurrency Currency { get; set; }
        public sys_userdetail CreatedBy { get; set; }
        public pos_configurationplaceterminal CreatedWhere { get; set; }
    }
}
