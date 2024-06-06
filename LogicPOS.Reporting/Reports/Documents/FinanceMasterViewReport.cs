using LogicPOS.Reporting.Common;
using System;
using System.Collections.Generic;

namespace LogicPOS.Reporting.Reports.Documents
{
    [Report(Entity = "view_documentfinance"/*, Limit = 1*/)]
    public class FinanceMasterViewReport : ReportData
    {
        //DocumentFinanceType
        [Report(Field = "ftOid")]                                                         //ftOid AS DocumentType
        public string DocumentType { get; set; }

        [Report(Field = "ftDocumentTypeOrd")]                                             //ftDocumentTypeOrd AS DocumentTypeOrd
        public uint DocumentTypeOrd { get; set; }

        [Report(Field = "ftDocumentTypeCode")]                                            //ftDocumentTypeCode AS DocumentTypeCode
        public uint DocumentTypeCode { get; set; }

        [Report(Field = "ftDocumentTypeDesignation")]                                     //ftDocumentTypeDesignation AS DocumentTypeDesignation
        public string DocumentTypeDesignation { get; set; }

        [Report(Field = "ftDocumentTypeAcronym")]                                         //ftDocumentTypeAcronym AS DocumentTypeAcronym
        public string DocumentTypeAcronym { get; set; }

        [Report(Field = "ftDocumentTypeResourceString")]                                  //ftDocumentTypeResourceString AS DocumentTypeResourceString
        public string DocumentTypeResourceString { get; set; }

        [Report(Field = "ftDocumentTypeResourceStringReport")]                            //ftDocumentTypeResourceStringReport AS DocumentTypeResourceStringReport
        public string DocumentTypeResourceStringReport { get; set; }

        [Report(Field = "ftWayBill")]
        public bool DocumentTypeWayBill { get; set; }                                //ftWayBill AS DocumentTypeWayBill,

        //DocumentFinanceMaster
        [Report(Field = "fmOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                                        //fmOid AS Oid,  

        [Report(Field = "fmDocumentNumber")]
        public string DocumentNumber { get; set; }                                      //fmDocumentNumber AS DocumentNumber,

        [Report(Field = "fmDate")]
        public DateTime Date { get; set; }                                              //fmDate AS Date,

        [Report(Field = "fmDocumentDate")]
        public string DocumentDate { get; set; }                                        //fmDocumentDate AS DocumentDate,

        [Report(Field = "fmSystemEntryDate")]
        public string SystemEntryDate { get; set; }                                     //fmSystemEntryDate AS SystemEntryDate,

        [Report(Field = "fmDocumentCreatorUser")]
        public string DocumentCreatorUser { get; set; }                                 //fmDocumentCreatorUser AS DocumentCreatorUser,

        [Report(Field = "fmTotalNet")]
        public decimal TotalNet { get; set; }                                           //fmTotalNet AS TotalNet,

        [Report(Field = "fmTotalGross")]
        public decimal TotalGross { get; set; }                                         //fmTotalGross AS TotalGross,

        [Report(Field = "fmTotalDiscount")]
        public decimal TotalDiscount { get; set; }                                      //fmTotalDiscount AS TotalDiscount,

        [Report(Field = "fmTotalTax")]
        public decimal TotalTax { get; set; }                                           //fmTotalTax AS TotalTax,

        [Report(Field = "fmTotalFinal")]
        public decimal TotalFinal { get; set; }                                         //fmTotalFinal AS TotalFinal,

        [Report(Field = "fmTotalFinalRound")]
        public decimal TotalFinalRound { get; set; }                                    //fmTotalFinalRound AS TotalFinalRound,

        [Report(Field = "fmTotalDelivery")]
        public decimal TotalDelivery { get; set; }                                      //fmTotalDelivery AS TotalDelivery,

        [Report(Field = "fmTotalChange")]
        public decimal TotalChange { get; set; }                                        //fmTotalChange AS TotalChange,

        [Report(Field = "fmDiscount")]
        public decimal Discount { get; set; }                                           //fmDiscount AS Discount,

        [Report(Field = "fmDiscountFinancial")]
        public decimal DiscountFinancial { get; set; }                                  //fmDiscountFinancial AS DiscountFinancial,

        [Report(Field = "fmExchangeRate")]
        public decimal ExchangeRate { get; set; }                                       //fmExchangeRate AS ExchangeRate,

        [Report(Field = "fmEntity")]
        public string EntityOid { get; set; }                                           //fmEntity AS EntityOid,

        [Report(Field = "cuEntityCode")]
        public uint EntityCode { get; set; }                                            //cuEntityCode AS EntityCode,

        [Report(Field = "cuEntityHidden")]
        public bool EntityHidden { get; set; }                                          //cuEntityHidden AS EntityHidden,

        [Report(Field = "fmEntityInternalCode")]
        public string EntityInternalCode { get; set; }                                  //fmEntityInternalCode AS EntityInternalCode,

        [Report(Field = "fmEntityName")]
        public string EntityName { get; set; }                                          //fmEntityName AS EntityName,

        [Report(Field = "fmEntityAddress")]
        public string EntityAddress { get; set; }                                       //fmEntityAddress AS EntityAddress,

        [Report(Field = "fmEntityZipCode")]
        public string EntityZipCode { get; set; }                                       //fmEntityZipCode AS EntityZipCode,

        [Report(Field = "fmEntityCity")]
        public string EntityCity { get; set; }                                          //fmEntityCity AS EntityCity,

        [Report(Field = "fmEntityLocality")]
        public string EntityLocality { get; set; }                                      //fmEntityLocality AS EntityCity,

        [Report(Field = "fmEntityCountryCode2")]
        public string EntityCountryCode2 { get; set; }                                  //fmEntityCountry EntityCountryCode2,

        [Report(Field = "ccCountryDesignation")]
        public string EntityCountry { get; set; }                                       //ccCountryDesignation AS EntityCountry,

        [Report(Field = "fmEntityFiscalNumber")]
        public string EntityFiscalNumber { get; set; }                                  //fmEntityFiscalNumber AS EntityFiscalNumber,

        [Report(Field = "fmDocumentStatusStatus")]
        public string DocumentStatusStatus { get; set; }                                //fmDocumentStatusStatus AS DocumentStatusStatus,

        [Report(Field = "fmTransactionID")]
        public string TransactionID { get; set; }                                       //fmTransactionID as TransactionID,

        [Report(Field = "fmShipToDeliveryID")]
        public string ShipToDeliveryID { get; set; }                                    //fmShipToDeliveryID as ShipToDeliveryID,

        [Report(Field = "fmShipToDeliveryDate")]
        public DateTime ShipToDeliveryDate { get; set; }                                //fmShipToDeliveryDate as ShipToDeliveryDate, 

        [Report(Field = "fmShipToWarehouseID")]
        public string ShipToWarehouseID { get; set; }                                   //fmShipToWarehouseID as ShipToWarehouseID,

        [Report(Field = "fmShipToLocationID")]
        public string ShipToLocationID { get; set; }                                    //fmShipToLocationID as ShipToLocationID,

        [Report(Field = "fmShipToAddressDetail")]
        public string ShipToAddressDetail { get; set; }                                 //fmShipToAddressDetail as ShipToAddressDetail,

        [Report(Field = "fmShipToCity")]
        public string ShipToCity { get; set; }                                          //fmShipToCity as ShipToCity,

        [Report(Field = "fmShipToPostalCode")]
        public string ShipToPostalCode { get; set; }                                    //fmShipToPostalCode as ShipToPostalCode,

        [Report(Field = "fmShipToRegion")]
        public string ShipToRegion { get; set; }                                        //fmShipToRegion as ShipToRegion,

        [Report(Field = "fmShipToCountry")]
        public string ShipToCountry { get; set; }                                       //fmShipToCountry as ShipToCountry,

        [Report(Field = "fmShipFromDeliveryID")]
        public string ShipFromDeliveryID { get; set; }                                  //fmShipFromDeliveryID as ShipFromDeliveryID,

        [Report(Field = "fmShipFromDeliveryDate")]
        public DateTime ShipFromDeliveryDate { get; set; }                              //fmShipFromDeliveryDate as ShipFromDeliveryDate,

        [Report(Field = "fmShipFromWarehouseID")]
        public string ShipFromWarehouseID { get; set; }                                 //fmShipFromWarehouseID as ShipFromWarehouseID,

        [Report(Field = "fmShipFromLocationID")]
        public string ShipFromLocationID { get; set; }                                  //fmShipFromLocationID as ShipFromLocationID,

        [Report(Field = "fmShipFromAddressDetail")]
        public string ShipFromAddressDetail { get; set; }                               //fmShipFromAddressDetail as ShipFromAddressDetail,

        [Report(Field = "fmShipFromCity")]
        public string ShipFromCity { get; set; }                                        //fmShipFromCity as ShipFromCity,

        [Report(Field = "fmShipFromPostalCode")]
        public string ShipFromPostalCode { get; set; }                                  //fmShipFromPostalCode as ShipFromPostalCode,

        [Report(Field = "fmShipFromRegion")]
        public string ShipFromRegion { get; set; }                                      //fmShipFromRegion as ShipFromRegion,

        [Report(Field = "fmShipFromCountry")]
        public string ShipFromCountry { get; set; }                                     //fmShipFromCountry as fmShipFromCountry,

        [Report(Field = "fmMovementStartTime")]
        public DateTime MovementStartTime { get; set; }                                 //fmMovementStartTime AS MovementStartTime,

        [Report(Field = "fmMovementEndTime")]
        public DateTime MovementEndTime { get; set; }                                   //fmMovementEndTime AS MovementEndTime,

        [Report(Field = "fmATDocCodeID")]
        public string ATDocCodeID { get; set; }                                         //fmATDocCodeID AS ATDocCodeID,

        [Report(Field = "fmPayed")]
        public bool Payed { get; set; }                                              //fmPayed AS Payed,

        [Report(Field = "fmPayedDate")]
        public DateTime PayedDate { get; set; }                                         //fmPayedDate AS PayedDate

        [Report(Field = "fmNotes")]
        public string Notes { get; set; }                                               //fmNotes AS Notes,

        //ConfigurationPaymentMethod
        [Report(Field = "fmPaymentMethod")]                                               //fmPaymentMethod AS PaymentMethod
        public string PaymentMethod { get; set; }

        [Report(Field = "pmPaymentMethodOrd")]                                            //pmPaymentMethodOrd AS PaymentMethodOrd
        public uint PaymentMethodOrd { get; set; }

        [Report(Field = "pmPaymentMethodCode")]
        public uint PaymentMethodCode { get; set; }                                     //pmPaymentMethodCode AS PaymentMethodCode,

        [Report(Field = "pmPaymentMethodDesignation")]
        public string PaymentMethodDesignation { get; set; }                            //pmPaymentMethodDesignation AS PaymentMethodDesignation,

        [Report(Field = "pmPaymentMethodToken")]
        public string PaymentMethodToken { get; set; }                                  //pmPaymentMethodToken AS PaymentMethodToken,

        //ConfigurationPaymentCondition
        [Report(Field = "fmPaymentCondition")]                                            //fmPaymentCondition AS PaymentCondition
        public string PaymentCondition { get; set; }

        [Report(Field = "pcPaymentConditionOrd")]                                         //pcPaymentConditionOrd AS PaymentConditionOrd
        public uint PaymentConditionOrd { get; set; }

        [Report(Field = "pcPaymentConditionCode")]
        public uint PaymentConditionCode { get; set; }                                  //pcPaymentConditionCode AS PaymentConditionCode,

        [Report(Field = "pcPaymentConditionDesignation")]
        public string PaymentConditionDesignation { get; set; }                         //pcPaymentConditionDesignation AS PaymentConditionDesignation,

        [Report(Field = "pcPaymentConditionAcronym")]
        public string PaymentConditionAcronym { get; set; }                             //pcPaymentConditionAcronym AS PaymentConditionAcronym

        //ConfigurationCurrency
        [Report(Field = "ccCountry")]                                                     //ccCountry AS Country
        public string Country { get; set; }

        [Report(Field = "ccCountryOrd")]                                                  //ccCountryOrd AS CountryOrd
        public uint CountryOrd { get; set; }

        [Report(Field = "ccCountryCode")]
        public uint CountryCode { get; set; }                                           //ccCountryCode AS CountryCode

        [Report(Field = "ccCountryDesignation")]
        public string CountryDesignation { get; set; }                                  //ccCountryDesignation AS CountryDesignation

        //ConfigurationCurrency
        [Report(Field = "fmCurrency")]                                                    //fmCurrency AS Currency
        public string Currency { get; set; }

        [Report(Field = "crCurrencyOrd")]                                                 //crCurrencyOrd AS CurrencyOrd
        public uint CurrencyOrd { get; set; }

        [Report(Field = "crCurrencyCode")]
        public uint CurrencyCode { get; set; }                                          //crCurrencyCode AS CurrencyCode

        [Report(Field = "crCurrencyDesignation")]
        public string CurrencyDesignation { get; set; }                                 //crCurrencyDesignation AS CurrencyDesignation

        [Report(Field = "crCurrencyAcronym")]
        public string CurrencyAcronym { get; set; }                                     //crCurrencyAcronym AS CurrencyAcronym,

        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        [Report(Field = "fmATDocQRCode")]
        public string ATDocQRCode { get; set; }                                     //fmATDocQRCode AS ATDocQRCode,

        //Chield FRBOs Objects
        public List<FinanceDetailReport> DocumentFinanceDetail { get; set; }
        public List<FinanceMasterTotalViewReport> DocumentFinanceMasterTotal { get; set; }
    }
}
