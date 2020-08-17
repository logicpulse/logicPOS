using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    [FRBO(Entity = "view_documentfinance"/*, Limit = 1*/)]
    public class FRBODocumentFinanceMasterView : FRBOBaseObject
    {
        //DocumentFinanceType
        [FRBO(Field = "ftOid")]                                                         //ftOid AS DocumentType
        public string DocumentType { get; set; }

        [FRBO(Field = "ftDocumentTypeOrd")]                                             //ftDocumentTypeOrd AS DocumentTypeOrd
        public uint DocumentTypeOrd { get; set; }

        [FRBO(Field = "ftDocumentTypeCode")]                                            //ftDocumentTypeCode AS DocumentTypeCode
        public uint DocumentTypeCode { get; set; }

        [FRBO(Field = "ftDocumentTypeDesignation")]                                     //ftDocumentTypeDesignation AS DocumentTypeDesignation
        public string DocumentTypeDesignation { get; set; }

        [FRBO(Field = "ftDocumentTypeAcronym")]                                         //ftDocumentTypeAcronym AS DocumentTypeAcronym
        public string DocumentTypeAcronym { get; set; }

        [FRBO(Field = "ftDocumentTypeResourceString")]                                  //ftDocumentTypeResourceString AS DocumentTypeResourceString
        public string DocumentTypeResourceString { get; set; }

        [FRBO(Field = "ftDocumentTypeResourceStringReport")]                            //ftDocumentTypeResourceStringReport AS DocumentTypeResourceStringReport
        public string DocumentTypeResourceStringReport { get; set; }

        [FRBO(Field = "ftWayBill")]
        public Boolean DocumentTypeWayBill { get; set; }                                //ftWayBill AS DocumentTypeWayBill,

        //DocumentFinanceMaster
        [FRBO(Field = "fmOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                                        //fmOid AS Oid,  

        [FRBO(Field = "fmDocumentNumber")]
        public string DocumentNumber { get; set; }                                      //fmDocumentNumber AS DocumentNumber,

        [FRBO(Field = "fmDate")]
        public DateTime Date { get; set; }                                              //fmDate AS Date,

        [FRBO(Field = "fmDocumentDate")]
        public string DocumentDate { get; set; }                                        //fmDocumentDate AS DocumentDate,

        [FRBO(Field = "fmSystemEntryDate")]
        public string SystemEntryDate { get; set; }                                     //fmSystemEntryDate AS SystemEntryDate,

        [FRBO(Field = "fmDocumentCreatorUser")]
        public string DocumentCreatorUser { get; set; }                                 //fmDocumentCreatorUser AS DocumentCreatorUser,

        [FRBO(Field = "fmTotalNet")]
        public decimal TotalNet { get; set; }                                           //fmTotalNet AS TotalNet,

        [FRBO(Field = "fmTotalGross")]
        public decimal TotalGross { get; set; }                                         //fmTotalGross AS TotalGross,

        [FRBO(Field = "fmTotalDiscount")]
        public decimal TotalDiscount { get; set; }                                      //fmTotalDiscount AS TotalDiscount,

        [FRBO(Field = "fmTotalTax")]
        public decimal TotalTax { get; set; }                                           //fmTotalTax AS TotalTax,

        [FRBO(Field = "fmTotalFinal")]
        public decimal TotalFinal { get; set; }                                         //fmTotalFinal AS TotalFinal,

        [FRBO(Field = "fmTotalFinalRound")]
        public decimal TotalFinalRound { get; set; }                                    //fmTotalFinalRound AS TotalFinalRound,

        [FRBO(Field = "fmTotalDelivery")]
        public decimal TotalDelivery { get; set; }                                      //fmTotalDelivery AS TotalDelivery,

        [FRBO(Field = "fmTotalChange")]
        public decimal TotalChange { get; set; }                                        //fmTotalChange AS TotalChange,

        [FRBO(Field = "fmDiscount")]
        public decimal Discount { get; set; }                                           //fmDiscount AS Discount,

        [FRBO(Field = "fmDiscountFinancial")]
        public decimal DiscountFinancial { get; set; }                                  //fmDiscountFinancial AS DiscountFinancial,

        [FRBO(Field = "fmExchangeRate")]
        public decimal ExchangeRate { get; set; }                                       //fmExchangeRate AS ExchangeRate,

        [FRBO(Field = "fmEntity")]
        public string EntityOid { get; set; }                                           //fmEntity AS EntityOid,

        [FRBO(Field = "cuEntityCode")]
        public uint EntityCode { get; set; }                                            //cuEntityCode AS EntityCode,

        [FRBO(Field = "cuEntityHidden")]
        public bool EntityHidden { get; set; }                                          //cuEntityHidden AS EntityHidden,

        [FRBO(Field = "fmEntityInternalCode")]
        public string EntityInternalCode { get; set; }                                  //fmEntityInternalCode AS EntityInternalCode,

        [FRBO(Field = "fmEntityName")]
        public string EntityName { get; set; }                                          //fmEntityName AS EntityName,

        [FRBO(Field = "fmEntityAddress")]
        public string EntityAddress { get; set; }                                       //fmEntityAddress AS EntityAddress,

        [FRBO(Field = "fmEntityZipCode")]
        public string EntityZipCode { get; set; }                                       //fmEntityZipCode AS EntityZipCode,

        [FRBO(Field = "fmEntityCity")]
        public string EntityCity { get; set; }                                          //fmEntityCity AS EntityCity,

        [FRBO(Field = "fmEntityLocality")]
        public string EntityLocality { get; set; }                                      //fmEntityLocality AS EntityCity,

        [FRBO(Field = "fmEntityCountryCode2")]
        public string EntityCountryCode2 { get; set; }                                  //fmEntityCountry EntityCountryCode2,

        [FRBO(Field = "ccCountryDesignation")]
        public string EntityCountry { get; set; }                                       //ccCountryDesignation AS EntityCountry,

        [FRBO(Field = "fmEntityFiscalNumber")]
        public string EntityFiscalNumber { get; set; }                                  //fmEntityFiscalNumber AS EntityFiscalNumber,

        [FRBO(Field = "fmDocumentStatusStatus")]
        public string DocumentStatusStatus { get; set; }                                //fmDocumentStatusStatus AS DocumentStatusStatus,

        [FRBO(Field = "fmTransactionID")]
        public string TransactionID { get; set; }                                       //fmTransactionID as TransactionID,

        [FRBO(Field = "fmShipToDeliveryID")]
        public string ShipToDeliveryID { get; set; }                                    //fmShipToDeliveryID as ShipToDeliveryID,

        [FRBO(Field = "fmShipToDeliveryDate")]
        public DateTime ShipToDeliveryDate { get; set; }                                //fmShipToDeliveryDate as ShipToDeliveryDate, 

        [FRBO(Field = "fmShipToWarehouseID")]
        public string ShipToWarehouseID { get; set; }                                   //fmShipToWarehouseID as ShipToWarehouseID,

        [FRBO(Field = "fmShipToLocationID")]
        public string ShipToLocationID { get; set; }                                    //fmShipToLocationID as ShipToLocationID,

        [FRBO(Field = "fmShipToAddressDetail")]
        public string ShipToAddressDetail { get; set; }                                 //fmShipToAddressDetail as ShipToAddressDetail,

        [FRBO(Field = "fmShipToCity")]
        public string ShipToCity { get; set; }                                          //fmShipToCity as ShipToCity,

        [FRBO(Field = "fmShipToPostalCode")]
        public string ShipToPostalCode { get; set; }                                    //fmShipToPostalCode as ShipToPostalCode,

        [FRBO(Field = "fmShipToRegion")]
        public string ShipToRegion { get; set; }                                        //fmShipToRegion as ShipToRegion,

        [FRBO(Field = "fmShipToCountry")]
        public string ShipToCountry { get; set; }                                       //fmShipToCountry as ShipToCountry,

        [FRBO(Field = "fmShipFromDeliveryID")]
        public string ShipFromDeliveryID { get; set; }                                  //fmShipFromDeliveryID as ShipFromDeliveryID,

        [FRBO(Field = "fmShipFromDeliveryDate")]
        public DateTime ShipFromDeliveryDate { get; set; }                              //fmShipFromDeliveryDate as ShipFromDeliveryDate,

        [FRBO(Field = "fmShipFromWarehouseID")]
        public string ShipFromWarehouseID { get; set; }                                 //fmShipFromWarehouseID as ShipFromWarehouseID,

        [FRBO(Field = "fmShipFromLocationID")]
        public string ShipFromLocationID { get; set; }                                  //fmShipFromLocationID as ShipFromLocationID,

        [FRBO(Field = "fmShipFromAddressDetail")]
        public string ShipFromAddressDetail { get; set; }                               //fmShipFromAddressDetail as ShipFromAddressDetail,

        [FRBO(Field = "fmShipFromCity")]
        public string ShipFromCity { get; set; }                                        //fmShipFromCity as ShipFromCity,

        [FRBO(Field = "fmShipFromPostalCode")]
        public string ShipFromPostalCode { get; set; }                                  //fmShipFromPostalCode as ShipFromPostalCode,

        [FRBO(Field = "fmShipFromRegion")]
        public string ShipFromRegion { get; set; }                                      //fmShipFromRegion as ShipFromRegion,

        [FRBO(Field = "fmShipFromCountry")]
        public string ShipFromCountry { get; set; }                                     //fmShipFromCountry as fmShipFromCountry,

        [FRBO(Field = "fmMovementStartTime")]
        public DateTime MovementStartTime { get; set; }                                 //fmMovementStartTime AS MovementStartTime,

        [FRBO(Field = "fmMovementEndTime")]
        public DateTime MovementEndTime { get; set; }                                   //fmMovementEndTime AS MovementEndTime,

        [FRBO(Field = "fmATDocCodeID")]
        public string ATDocCodeID { get; set; }                                         //fmATDocCodeID AS ATDocCodeID,

        [FRBO(Field = "fmPayed")]
        public Boolean Payed { get; set; }                                              //fmPayed AS Payed,

        [FRBO(Field = "fmPayedDate")]
        public DateTime PayedDate { get; set; }                                         //fmPayedDate AS PayedDate

        [FRBO(Field = "fmNotes")]
        public string Notes { get; set; }                                               //fmNotes AS Notes,

        //ConfigurationPaymentMethod
        [FRBO(Field = "fmPaymentMethod")]                                               //fmPaymentMethod AS PaymentMethod
        public string PaymentMethod { get; set; }

        [FRBO(Field = "pmPaymentMethodOrd")]                                            //pmPaymentMethodOrd AS PaymentMethodOrd
        public uint PaymentMethodOrd { get; set; }

        [FRBO(Field = "pmPaymentMethodCode")]
        public uint PaymentMethodCode { get; set; }                                     //pmPaymentMethodCode AS PaymentMethodCode,

        [FRBO(Field = "pmPaymentMethodDesignation")]
        public string PaymentMethodDesignation { get; set; }                            //pmPaymentMethodDesignation AS PaymentMethodDesignation,

        [FRBO(Field = "pmPaymentMethodToken")]
        public string PaymentMethodToken { get; set; }                                  //pmPaymentMethodToken AS PaymentMethodToken,

        //ConfigurationPaymentCondition
        [FRBO(Field = "fmPaymentCondition")]                                            //fmPaymentCondition AS PaymentCondition
        public string PaymentCondition { get; set; }

        [FRBO(Field = "pcPaymentConditionOrd")]                                         //pcPaymentConditionOrd AS PaymentConditionOrd
        public uint PaymentConditionOrd { get; set; }

        [FRBO(Field = "pcPaymentConditionCode")]
        public uint PaymentConditionCode { get; set; }                                  //pcPaymentConditionCode AS PaymentConditionCode,

        [FRBO(Field = "pcPaymentConditionDesignation")]
        public string PaymentConditionDesignation { get; set; }                         //pcPaymentConditionDesignation AS PaymentConditionDesignation,

        [FRBO(Field = "pcPaymentConditionAcronym")]
        public string PaymentConditionAcronym { get; set; }                             //pcPaymentConditionAcronym AS PaymentConditionAcronym

        //ConfigurationCurrency
        [FRBO(Field = "ccCountry")]                                                     //ccCountry AS Country
        public string Country { get; set; }

        [FRBO(Field = "ccCountryOrd")]                                                  //ccCountryOrd AS CountryOrd
        public uint CountryOrd { get; set; }

        [FRBO(Field = "ccCountryCode")]
        public uint CountryCode { get; set; }                                           //ccCountryCode AS CountryCode

        [FRBO(Field = "ccCountryDesignation")]
        public string CountryDesignation { get; set; }                                  //ccCountryDesignation AS CountryDesignation

        //ConfigurationCurrency
        [FRBO(Field = "fmCurrency")]                                                    //fmCurrency AS Currency
        public string Currency { get; set; }

        [FRBO(Field = "crCurrencyOrd")]                                                 //crCurrencyOrd AS CurrencyOrd
        public uint CurrencyOrd { get; set; }

        [FRBO(Field = "crCurrencyCode")]
        public uint CurrencyCode { get; set; }                                          //crCurrencyCode AS CurrencyCode

        [FRBO(Field = "crCurrencyDesignation")]
        public string CurrencyDesignation { get; set; }                                 //crCurrencyDesignation AS CurrencyDesignation

        [FRBO(Field = "crCurrencyAcronym")]
        public string CurrencyAcronym { get; set; }                                     //crCurrencyAcronym AS CurrencyAcronym,

        //Chield FRBOs Objects
        public List<FRBODocumentFinanceDetail> DocumentFinanceDetail { get; set; }
        public List<FRBODocumentFinanceMasterTotalView> DocumentFinanceMasterTotal { get; set; }
    }
}
