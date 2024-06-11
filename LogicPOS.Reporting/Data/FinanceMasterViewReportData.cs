using LogicPOS.Reporting.Data.Common;
using System;
using System.Collections.Generic;

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "view_documentfinance"/*, Limit = 1*/)]
    public class FinanceMasterViewReportData : ReportData
    {
        //DocumentFinanceType
        [ReportData(Field = "ftOid")]                                                         //ftOid AS DocumentType
        public string DocumentType { get; set; }

        [ReportData(Field = "ftDocumentTypeOrd")]                                             //ftDocumentTypeOrd AS DocumentTypeOrd
        public uint DocumentTypeOrd { get; set; }

        [ReportData(Field = "ftDocumentTypeCode")]                                            //ftDocumentTypeCode AS DocumentTypeCode
        public uint DocumentTypeCode { get; set; }

        [ReportData(Field = "ftDocumentTypeDesignation")]                                     //ftDocumentTypeDesignation AS DocumentTypeDesignation
        public string DocumentTypeDesignation { get; set; }

        [ReportData(Field = "ftDocumentTypeAcronym")]                                         //ftDocumentTypeAcronym AS DocumentTypeAcronym
        public string DocumentTypeAcronym { get; set; }

        [ReportData(Field = "ftDocumentTypeResourceString")]                                  //ftDocumentTypeResourceString AS DocumentTypeResourceString
        public string DocumentTypeResourceString { get; set; }

        [ReportData(Field = "ftDocumentTypeResourceStringReport")]                            //ftDocumentTypeResourceStringReport AS DocumentTypeResourceStringReport
        public string DocumentTypeResourceStringReport { get; set; }

        [ReportData(Field = "ftWayBill")]
        public bool DocumentTypeWayBill { get; set; }                                //ftWayBill AS DocumentTypeWayBill,

        //DocumentFinanceMaster
        [ReportData(Field = "fmOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                                        //fmOid AS Oid,  

        [ReportData(Field = "fmDocumentNumber")]
        public string DocumentNumber { get; set; }                                      //fmDocumentNumber AS DocumentNumber,

        [ReportData(Field = "fmDate")]
        public DateTime Date { get; set; }                                              //fmDate AS Date,

        [ReportData(Field = "fmDocumentDate")]
        public string DocumentDate { get; set; }                                        //fmDocumentDate AS DocumentDate,

        [ReportData(Field = "fmSystemEntryDate")]
        public string SystemEntryDate { get; set; }                                     //fmSystemEntryDate AS SystemEntryDate,

        [ReportData(Field = "fmDocumentCreatorUser")]
        public string DocumentCreatorUser { get; set; }                                 //fmDocumentCreatorUser AS DocumentCreatorUser,

        [ReportData(Field = "fmTotalNet")]
        public decimal TotalNet { get; set; }                                           //fmTotalNet AS TotalNet,

        [ReportData(Field = "fmTotalGross")]
        public decimal TotalGross { get; set; }                                         //fmTotalGross AS TotalGross,

        [ReportData(Field = "fmTotalDiscount")]
        public decimal TotalDiscount { get; set; }                                      //fmTotalDiscount AS TotalDiscount,

        [ReportData(Field = "fmTotalTax")]
        public decimal TotalTax { get; set; }                                           //fmTotalTax AS TotalTax,

        [ReportData(Field = "fmTotalFinal")]
        public decimal TotalFinal { get; set; }                                         //fmTotalFinal AS TotalFinal,

        [ReportData(Field = "fmTotalFinalRound")]
        public decimal TotalFinalRound { get; set; }                                    //fmTotalFinalRound AS TotalFinalRound,

        [ReportData(Field = "fmTotalDelivery")]
        public decimal TotalDelivery { get; set; }                                      //fmTotalDelivery AS TotalDelivery,

        [ReportData(Field = "fmTotalChange")]
        public decimal TotalChange { get; set; }                                        //fmTotalChange AS TotalChange,

        [ReportData(Field = "fmDiscount")]
        public decimal Discount { get; set; }                                           //fmDiscount AS Discount,

        [ReportData(Field = "fmDiscountFinancial")]
        public decimal DiscountFinancial { get; set; }                                  //fmDiscountFinancial AS DiscountFinancial,

        [ReportData(Field = "fmExchangeRate")]
        public decimal ExchangeRate { get; set; }                                       //fmExchangeRate AS ExchangeRate,

        [ReportData(Field = "fmEntity")]
        public string EntityOid { get; set; }                                           //fmEntity AS EntityOid,

        [ReportData(Field = "cuEntityCode")]
        public uint EntityCode { get; set; }                                            //cuEntityCode AS EntityCode,

        [ReportData(Field = "cuEntityHidden")]
        public bool EntityHidden { get; set; }                                          //cuEntityHidden AS EntityHidden,

        [ReportData(Field = "fmEntityInternalCode")]
        public string EntityInternalCode { get; set; }                                  //fmEntityInternalCode AS EntityInternalCode,

        [ReportData(Field = "fmEntityName")]
        public string EntityName { get; set; }                                          //fmEntityName AS EntityName,

        [ReportData(Field = "fmEntityAddress")]
        public string EntityAddress { get; set; }                                       //fmEntityAddress AS EntityAddress,

        [ReportData(Field = "fmEntityZipCode")]
        public string EntityZipCode { get; set; }                                       //fmEntityZipCode AS EntityZipCode,

        [ReportData(Field = "fmEntityCity")]
        public string EntityCity { get; set; }                                          //fmEntityCity AS EntityCity,

        [ReportData(Field = "fmEntityLocality")]
        public string EntityLocality { get; set; }                                      //fmEntityLocality AS EntityCity,

        [ReportData(Field = "fmEntityCountryCode2")]
        public string EntityCountryCode2 { get; set; }                                  //fmEntityCountry EntityCountryCode2,

        [ReportData(Field = "ccCountryDesignation")]
        public string EntityCountry { get; set; }                                       //ccCountryDesignation AS EntityCountry,

        [ReportData(Field = "fmEntityFiscalNumber")]
        public string EntityFiscalNumber { get; set; }                                  //fmEntityFiscalNumber AS EntityFiscalNumber,

        [ReportData(Field = "fmDocumentStatusStatus")]
        public string DocumentStatusStatus { get; set; }                                //fmDocumentStatusStatus AS DocumentStatusStatus,

        [ReportData(Field = "fmTransactionID")]
        public string TransactionID { get; set; }                                       //fmTransactionID as TransactionID,

        [ReportData(Field = "fmShipToDeliveryID")]
        public string ShipToDeliveryID { get; set; }                                    //fmShipToDeliveryID as ShipToDeliveryID,

        [ReportData(Field = "fmShipToDeliveryDate")]
        public DateTime ShipToDeliveryDate { get; set; }                                //fmShipToDeliveryDate as ShipToDeliveryDate, 

        [ReportData(Field = "fmShipToWarehouseID")]
        public string ShipToWarehouseID { get; set; }                                   //fmShipToWarehouseID as ShipToWarehouseID,

        [ReportData(Field = "fmShipToLocationID")]
        public string ShipToLocationID { get; set; }                                    //fmShipToLocationID as ShipToLocationID,

        [ReportData(Field = "fmShipToAddressDetail")]
        public string ShipToAddressDetail { get; set; }                                 //fmShipToAddressDetail as ShipToAddressDetail,

        [ReportData(Field = "fmShipToCity")]
        public string ShipToCity { get; set; }                                          //fmShipToCity as ShipToCity,

        [ReportData(Field = "fmShipToPostalCode")]
        public string ShipToPostalCode { get; set; }                                    //fmShipToPostalCode as ShipToPostalCode,

        [ReportData(Field = "fmShipToRegion")]
        public string ShipToRegion { get; set; }                                        //fmShipToRegion as ShipToRegion,

        [ReportData(Field = "fmShipToCountry")]
        public string ShipToCountry { get; set; }                                       //fmShipToCountry as ShipToCountry,

        [ReportData(Field = "fmShipFromDeliveryID")]
        public string ShipFromDeliveryID { get; set; }                                  //fmShipFromDeliveryID as ShipFromDeliveryID,

        [ReportData(Field = "fmShipFromDeliveryDate")]
        public DateTime ShipFromDeliveryDate { get; set; }                              //fmShipFromDeliveryDate as ShipFromDeliveryDate,

        [ReportData(Field = "fmShipFromWarehouseID")]
        public string ShipFromWarehouseID { get; set; }                                 //fmShipFromWarehouseID as ShipFromWarehouseID,

        [ReportData(Field = "fmShipFromLocationID")]
        public string ShipFromLocationID { get; set; }                                  //fmShipFromLocationID as ShipFromLocationID,

        [ReportData(Field = "fmShipFromAddressDetail")]
        public string ShipFromAddressDetail { get; set; }                               //fmShipFromAddressDetail as ShipFromAddressDetail,

        [ReportData(Field = "fmShipFromCity")]
        public string ShipFromCity { get; set; }                                        //fmShipFromCity as ShipFromCity,

        [ReportData(Field = "fmShipFromPostalCode")]
        public string ShipFromPostalCode { get; set; }                                  //fmShipFromPostalCode as ShipFromPostalCode,

        [ReportData(Field = "fmShipFromRegion")]
        public string ShipFromRegion { get; set; }                                      //fmShipFromRegion as ShipFromRegion,

        [ReportData(Field = "fmShipFromCountry")]
        public string ShipFromCountry { get; set; }                                     //fmShipFromCountry as fmShipFromCountry,

        [ReportData(Field = "fmMovementStartTime")]
        public DateTime MovementStartTime { get; set; }                                 //fmMovementStartTime AS MovementStartTime,

        [ReportData(Field = "fmMovementEndTime")]
        public DateTime MovementEndTime { get; set; }                                   //fmMovementEndTime AS MovementEndTime,

        [ReportData(Field = "fmATDocCodeID")]
        public string ATDocCodeID { get; set; }                                         //fmATDocCodeID AS ATDocCodeID,

        [ReportData(Field = "fmPayed")]
        public bool Payed { get; set; }                                              //fmPayed AS Payed,

        [ReportData(Field = "fmPayedDate")]
        public DateTime PayedDate { get; set; }                                         //fmPayedDate AS PayedDate

        [ReportData(Field = "fmNotes")]
        public string Notes { get; set; }                                               //fmNotes AS Notes,

        //ConfigurationPaymentMethod
        [ReportData(Field = "fmPaymentMethod")]                                               //fmPaymentMethod AS PaymentMethod
        public string PaymentMethod { get; set; }

        [ReportData(Field = "pmPaymentMethodOrd")]                                            //pmPaymentMethodOrd AS PaymentMethodOrd
        public uint PaymentMethodOrd { get; set; }

        [ReportData(Field = "pmPaymentMethodCode")]
        public uint PaymentMethodCode { get; set; }                                     //pmPaymentMethodCode AS PaymentMethodCode,

        [ReportData(Field = "pmPaymentMethodDesignation")]
        public string PaymentMethodDesignation { get; set; }                            //pmPaymentMethodDesignation AS PaymentMethodDesignation,

        [ReportData(Field = "pmPaymentMethodToken")]
        public string PaymentMethodToken { get; set; }                                  //pmPaymentMethodToken AS PaymentMethodToken,

        //ConfigurationPaymentCondition
        [ReportData(Field = "fmPaymentCondition")]                                            //fmPaymentCondition AS PaymentCondition
        public string PaymentCondition { get; set; }

        [ReportData(Field = "pcPaymentConditionOrd")]                                         //pcPaymentConditionOrd AS PaymentConditionOrd
        public uint PaymentConditionOrd { get; set; }

        [ReportData(Field = "pcPaymentConditionCode")]
        public uint PaymentConditionCode { get; set; }                                  //pcPaymentConditionCode AS PaymentConditionCode,

        [ReportData(Field = "pcPaymentConditionDesignation")]
        public string PaymentConditionDesignation { get; set; }                         //pcPaymentConditionDesignation AS PaymentConditionDesignation,

        [ReportData(Field = "pcPaymentConditionAcronym")]
        public string PaymentConditionAcronym { get; set; }                             //pcPaymentConditionAcronym AS PaymentConditionAcronym

        //ConfigurationCurrency
        [ReportData(Field = "ccCountry")]                                                     //ccCountry AS Country
        public string Country { get; set; }

        [ReportData(Field = "ccCountryOrd")]                                                  //ccCountryOrd AS CountryOrd
        public uint CountryOrd { get; set; }

        [ReportData(Field = "ccCountryCode")]
        public uint CountryCode { get; set; }                                           //ccCountryCode AS CountryCode

        [ReportData(Field = "ccCountryDesignation")]
        public string CountryDesignation { get; set; }                                  //ccCountryDesignation AS CountryDesignation

        //ConfigurationCurrency
        [ReportData(Field = "fmCurrency")]                                                    //fmCurrency AS Currency
        public string Currency { get; set; }

        [ReportData(Field = "crCurrencyOrd")]                                                 //crCurrencyOrd AS CurrencyOrd
        public uint CurrencyOrd { get; set; }

        [ReportData(Field = "crCurrencyCode")]
        public uint CurrencyCode { get; set; }                                          //crCurrencyCode AS CurrencyCode

        [ReportData(Field = "crCurrencyDesignation")]
        public string CurrencyDesignation { get; set; }                                 //crCurrencyDesignation AS CurrencyDesignation

        [ReportData(Field = "crCurrencyAcronym")]
        public string CurrencyAcronym { get; set; }                                     //crCurrencyAcronym AS CurrencyAcronym,

        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        [ReportData(Field = "fmATDocQRCode")]
        public string ATDocQRCode { get; set; }                                     //fmATDocQRCode AS ATDocQRCode,

        //Chield FRBOs Objects
        public List<FinanceDetailReportData> DocumentFinanceDetail { get; set; }
        public List<FinanceMasterTotalViewReportData> DocumentFinanceMasterTotal { get; set; }
    }
}
