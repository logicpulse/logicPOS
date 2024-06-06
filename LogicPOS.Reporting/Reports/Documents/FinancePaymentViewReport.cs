using LogicPOS.Reporting.Common;
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

namespace LogicPOS.Reporting.Reports.Documents
{
    [Report(Entity = "view_documentfinancepayment", Limit = 1)]
    public class FinancePaymentViewReport : ReportData
    {
        [Report(Field = "fpaOid")]
        override public string Oid { get; set; }                                        //fpaOid AS Oid,  

        [Report(Field = "ftpDesignation")]
        public string DocumentTypeDesignation { get; set; }                             //ftpDesignation AS DocumentTypeDesignation,

        [Report(Field = "ftpResourceString")]
        public string DocumentTypeResourceString { get; set; }                          //ftpResourceString AS DocumentTypeResourceString,

        [Report(Field = "ftpResourceStringReport")]
        public string DocumentTypeResourceStringReport { get; set; }                    //ftpResourceStringReport AS DocumentTypeResourceStringReport,  

        [Report(Field = "fpaPaymentRefNo")]
        public string PaymentRefNo { get; set; }                                        //fpaPaymentRefNo AS PaymentRefNo,

        [Report(Field = "fpaPaymentStatus")]
        public string PaymentStatus { get; set; }                                       //fpaPaymentStatus AS PaymentStatus,

        [Report(Field = "fpaPaymentAmount")]
        public decimal PaymentAmount { get; set; }                                      //fpaPaymentAmount AS PaymentAmount,
                                                                                        //TK016319 - Certificação Angola - Alterações para teste da AGT
        [Report(Field = "fpaTaxPayable")]
        public decimal TaxPayable { get; set; }                                         //fmaTotalTax AS TaxPayable,

        [Report(Field = "fpaPaymentDate")]
        public string PaymentDate { get; set; }                                         //fpaPaymentDate AS PaymentDate,

        [Report(Field = "fpaDocumentDate")]
        public string DocumentDate { get; set; }                                        //fpaDocumentDate AS DocumentDate,

        [Report(Field = "fpaExtendedValue")]
        public string ExtendedValue { get; set; }                                       //fpaExtendedValue AS ExtendedValue,

        [Report(Field = "cusCode")]
        public uint EntityCode { get; set; }                                            //cusCode AS EntityCode, 

        [Report(Field = "cusName")]
        public string EntityName { get; set; }                                          //cusName AS EntityName, 

        [Report(Field = "cusAddress")]
        public string EntityAddress { get; set; }                                       //cusAddress AS EntityAddress, 

        [Report(Field = "cusZipCode")]
        public string EntityZipCode { get; set; }                                       //cusZipCode AS EntityZipCode, 

        [Report(Field = "cusCity")]
        public string EntityCity { get; set; }                                          //cusCity AS EntityCity,

        [Report(Field = "cusLocality")]
        public string EntityLocality { get; set; }                                          //cusCity AS EntityCity, 

        [Report(Field = "ccoDesignation")]
        public string EntityCountry { get; set; }                                       //ccoDesignation AS EntityCountry, 

        [Report(Field = "cusFiscalNumber")]
        public string EntityFiscalNumber { get; set; }                                  //cusFiscalNumber as EntityFiscalNumber,

        [Report(Field = "cpmCode")]
        public uint MethodCode { get; set; }                                            //cpmCode AS MethodCode,

        [Report(Field = "cpmDesignation")]
        public string PaymentMethodDesignation { get; set; }                            //cpmDesignation as MethodDesignation,

        [Report(Field = "curDesignation")]
        public string CurrencyDesignation { get; set; }                                 //curDesignation as CurrencyDesignation,

        [Report(Field = "curAcronym")]
        public string CurrencyAcronym { get; set; }                                     //curAcronym as CurrencyAcronym,

        [Report(Field = "curSymbol")]
        public string CurrencySymbol { get; set; }                                      //curSymbol as CurrencySymbol 

        [Report(Field = "fpaExchangeRate")]
        public decimal ExchangeRate { get; set; }                                       //fpaExchangeRate AS ExchangeRate,

        [Report(Field = "fpaNotes")]
        public string Notes { get; set; }                                               //fpaNotes AS Notes

        //Chield FRBOs Objects
        public List<FinancePaymentDocumentViewReport> DocumentFinancePaymentDocument { get; set; }
    }
}