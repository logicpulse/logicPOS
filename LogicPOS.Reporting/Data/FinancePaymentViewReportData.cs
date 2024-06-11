using LogicPOS.Reporting.Data.Common;
using System.Collections.Generic;

/* USED QUERY 
SELECT 
      fpaOid AS Oid,  
      ftpDesignation AS DocumentTypeDesignation,
      ftpResourceString AS DocumentTypeResourceString,
      ftpResourceStringReport AS DocumentTypeResourceStringReport,  
      fpaPaymentRefNo AS PaymentRefNo,
      fpaPaymentStatus AS PaymentStatus,
      fpaPaymentAmount AS PaymentAmount,
      fpaDocumentDate AS DocumentDate,
      fpaPaymentDate AS PaymentDate,
      fpaExtendedValue AS ExtendedValue,
      cusCode AS EntityCode, 
      cusName AS EntityName, 
      cusAddress AS EntityAddress, 
      cusZipCode AS EntityZipCode, 
      cusCity AS EntityCity, 
      ccoDesignation AS EntityCountry, 
      cusFiscalNumber as EntityFiscalNumber,
      cpmCode AS MethodCode,
      cpmDesignation as MethodDesignation,
      curDesignation as CurrencyDesignation,
      curAcronym as CurrencyAcronym,
      curSymbol as CurrencySymbol,
      fpaExchangeRate AS ExchangeRate,
      fpaNotes AS Notes
FROM 
	view_documentfinancepayment
WHERE 
    fpaOid = '71db09a8-a685-492a-a30c-d410599a0aa1'
;
*/

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "view_documentfinancepayment", Limit = 1)]
    public class FinancePaymentViewReportData : ReportData
    {
        [ReportData(Field = "fpaOid")]
        override public string Oid { get; set; }                                        //fpaOid AS Oid,  

        [ReportData(Field = "ftpDesignation")]
        public string DocumentTypeDesignation { get; set; }                             //ftpDesignation AS DocumentTypeDesignation,

        [ReportData(Field = "ftpResourceString")]
        public string DocumentTypeResourceString { get; set; }                          //ftpResourceString AS DocumentTypeResourceString,

        [ReportData(Field = "ftpResourceStringReport")]
        public string DocumentTypeResourceStringReport { get; set; }                    //ftpResourceStringReport AS DocumentTypeResourceStringReport,  

        [ReportData(Field = "fpaPaymentRefNo")]
        public string PaymentRefNo { get; set; }                                        //fpaPaymentRefNo AS PaymentRefNo,

        [ReportData(Field = "fpaPaymentStatus")]
        public string PaymentStatus { get; set; }                                       //fpaPaymentStatus AS PaymentStatus,

        [ReportData(Field = "fpaPaymentAmount")]
        public decimal PaymentAmount { get; set; }                                      //fpaPaymentAmount AS PaymentAmount,
                                                                                        //TK016319 - Certificação Angola - Alterações para teste da AGT
        [ReportData(Field = "fpaTaxPayable")]
        public decimal TaxPayable { get; set; }                                         //fmaTotalTax AS TaxPayable,

        [ReportData(Field = "fpaPaymentDate")]
        public string PaymentDate { get; set; }                                         //fpaPaymentDate AS PaymentDate,

        [ReportData(Field = "fpaDocumentDate")]
        public string DocumentDate { get; set; }                                        //fpaDocumentDate AS DocumentDate,

        [ReportData(Field = "fpaExtendedValue")]
        public string ExtendedValue { get; set; }                                       //fpaExtendedValue AS ExtendedValue,

        [ReportData(Field = "cusCode")]
        public uint EntityCode { get; set; }                                            //cusCode AS EntityCode, 

        [ReportData(Field = "cusName")]
        public string EntityName { get; set; }                                          //cusName AS EntityName, 

        [ReportData(Field = "cusAddress")]
        public string EntityAddress { get; set; }                                       //cusAddress AS EntityAddress, 

        [ReportData(Field = "cusZipCode")]
        public string EntityZipCode { get; set; }                                       //cusZipCode AS EntityZipCode, 

        [ReportData(Field = "cusCity")]
        public string EntityCity { get; set; }                                          //cusCity AS EntityCity,

        [ReportData(Field = "cusLocality")]
        public string EntityLocality { get; set; }                                          //cusCity AS EntityCity, 

        [ReportData(Field = "ccoDesignation")]
        public string EntityCountry { get; set; }                                       //ccoDesignation AS EntityCountry, 

        [ReportData(Field = "cusFiscalNumber")]
        public string EntityFiscalNumber { get; set; }                                  //cusFiscalNumber as EntityFiscalNumber,

        [ReportData(Field = "cpmCode")]
        public uint MethodCode { get; set; }                                            //cpmCode AS MethodCode,

        [ReportData(Field = "cpmDesignation")]
        public string PaymentMethodDesignation { get; set; }                            //cpmDesignation as MethodDesignation,

        [ReportData(Field = "curDesignation")]
        public string CurrencyDesignation { get; set; }                                 //curDesignation as CurrencyDesignation,

        [ReportData(Field = "curAcronym")]
        public string CurrencyAcronym { get; set; }                                     //curAcronym as CurrencyAcronym,

        [ReportData(Field = "curSymbol")]
        public string CurrencySymbol { get; set; }                                      //curSymbol as CurrencySymbol 

        [ReportData(Field = "fpaExchangeRate")]
        public decimal ExchangeRate { get; set; }                                       //fpaExchangeRate AS ExchangeRate,

        [ReportData(Field = "fpaNotes")]
        public string Notes { get; set; }                                               //fpaNotes AS Notes

        //Chield FRBOs Objects
        public List<FinancePaymentDocumentViewReportData> DocumentFinancePaymentDocument { get; set; }
    }
}