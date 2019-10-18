using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Documents
{
    // Note: Agregate Fields must have same name has in FRBODocumentFinanceMasterDetailView
    // This is a requirement for Shared Reports that use same Field Name

    //[DocumentFinanceDetail.ArticleCode]
    //[DocumentFinanceDetail.ArticleDesignation]
    //[FormatNumber([DocumentFinanceDetail.ArticlePriceWithDiscount],2)]
    //[FormatNumber([DocumentFinanceDetail.ArticleQuantity],2)][DocumentFinanceDetail.ArticleUnitMeasure]
    //[FormatNumber([DocumentFinanceDetail.ArticleTotalDiscount],2)]
    //[FormatNumber([DocumentFinanceDetail.ArticleTotalNet],2)]
    //[FormatNumber([DocumentFinanceDetail.ArticleTotalTax],2)]
    //[FormatNumber([DocumentFinanceDetail.ArticleTotalFinal],2)]

    [FRBO(Entity = "view_documentfinance")]
    public class FRBODocumentFinanceMasterDetailGroupView : FRBOBaseObject
    {
        // Group
        public string GroupOid { get; set; }
        public int GroupOrd { get; set; }
        public int GroupCode { get; set; }
        public string GroupDesignation { get; set; }
        //Article
        public string ArticleOid { get; set; }
        public string ArticleCode { get; set; }
        public string ArticleDesignation { get; set; }
        // Aggregate Fields
        public decimal ArticlePriceWithDiscount { get; set; }
        public decimal ArticleQuantity { get; set; }
        public string ArticleUnitMeasure { get; set; }
        public decimal ArticleTotalDiscount { get; set; }
        public decimal ArticleTotalNet { get; set; }
        public decimal ArticleTotalTax { get; set; }
        public decimal ArticleTotalFinal { get; set; }
        public int GroupCount { get; set; }

        /* IN009072 - begin */
        //[FRBO(Field = "fmEntityFiscalNumber")]                        //fmEntityFiscalNumber AS EntityFiscalNumber,
        public string EntityFiscalNumber { get; set; }
        //[FRBO(Field = "ftDocumentTypeAcronym")]                       //ftDocumentTypeAcronym AS DocumentTypeAcronym
        public string DocumentTypeAcronym { get; set; }
        //[FRBO(Field = "ftOid")]                                       //ftOid AS DocumentType
        public string DocumentType { get; set; }
        /* IN009072 - end */

        /* Old MySQL Implementation / Before Change code to Work With SqlServer

        // DocumentFinanceType
        [FRBO(Field = "ftOid")]                                             // ftOid AS DocumentType 
        public string DocumentType { get; set; }

        [FRBO(Field = "ftDocumentTypeOrd")]                                 // ftDocumentTypeOrd AS DocumentTypeOrd
        public uint DocumentTypeOrd { get; set; }

        [FRBO(Field = "ftDocumentTypeCode")]                                // ftDocumentTypeCode AS ftDocumentTypeCode
        public uint DocumentTypeCode { get; set; }

        [FRBO(Field = "ftDocumentTypeDesignation")]                         // ftDocumentTypeDesignation AS DocumentTypeDesignation
        public string DocumentTypeDesignation { get; set; }

        [FRBO(Field = "ftDocumentTypeAcronym")]                             // ftDocumentTypeAcronym AS DocumentTypeAcronym

        // DocumentFinanceMaster
        [FRBO(Field = "fmDocumentNumber")]                                  // fmDocumentNumber AS DocumentNumber,
        public string DocumentNumber { get; set; }                          

        [FRBO(Field = "fmDate")]                                            // fmDate AS Date,
        public DateTime Date { get; set; }                                  

        [FRBO(Field = "fmDocumentDate")]                                    // fmDocumentDate AS DocumentDate
        public string DocumentDate { get; set; }

        // Customer
        [FRBO(Field = "fmEntity")]                                          // fmEntity AS Entity
        public string Entity { get; set; }

        [FRBO(Field = "cuEntityOrd")]                                       // cuEntityOrd AS EntityOrd
        public uint EntityOrd { get; set; }

        [FRBO(Field = "cuEntityCode")]                                      // cuEntityCode AS EntityCode
        public uint EntityCode { get; set; }

        [FRBO(Field = "fmEntityName")]                                      // fmEntityName AS EntityName
        public string EntityName { get; set; }

        [FRBO(Field = "fmEntityFiscalNumber")]                              // fmEntityFiscalNumber AS EntityFiscalNumber
        public string EntityFiscalNumber { get; set; }

        [FRBO(Field = "fmEntityCountryCode2")]                              // fmEntityCountryCode2 AS EntityCountryCode2
        public string EntityCountryCode2 { get; set; }

        // PaymentMethod
        [FRBO(Field = "fmPaymentMethod")]                                   // fmPaymentMethod AS PaymentMethod
        public string PaymentMethod { get; set; }

        [FRBO(Field = "pmPaymentMethodOrd")]                                // pmPaymentMethodOrd AS PaymentMethodOrd
        public uint PaymentMethodOrd { get; set; }

        [FRBO(Field = "pmPaymentMethodCode")]                               // pmPaymentMethodCode AS PaymentMethodCode
        public uint PaymentMethodCode { get; set; }

        [FRBO(Field = "pmPaymentMethodDesignation")]                        // pmPaymentMethodDesignation AS PaymentMethodDesignation
        public string PaymentMethodDesignation { get; set; }

        [FRBO(Field = "pmPaymentMethodToken")]                              // pmPaymentMethodToken AS PaymentMethodToken
        public string PaymentMethodToken { get; set; }

        // PaymentCondition
        [FRBO(Field = "fmPaymentCondition")]                                // fmPaymentCondition AS PaymentCondition
        public string PaymentCondition { get; set; }

        [FRBO(Field = "pcPaymentConditionOrd")]                             // pcPaymentConditionOrd AS PaymentConditionOrd
        public uint PaymentConditionOrd { get; set; }

        [FRBO(Field = "pcPaymentConditionCode")]                            // pcPaymentConditionCode AS PaymentConditionCode
        public uint PaymentConditionCode { get; set; }

        [FRBO(Field = "pcPaymentConditionDesignation")]                     // pcPaymentConditionDesignation AS PaymentConditionDesignation
        public string PaymentConditionDesignation { get; set; }

        [FRBO(Field = "pcPaymentConditionAcronym")]                         // pcPaymentConditionAcronym AS PaymentConditionAcronym
        public string PaymentConditionAcronym { get; set; }

        // Country
        [FRBO(Field = "ccCountry")]                                         // ccCountry AS Country
        public string Country { get; set; }

        [FRBO(Field = "ccCountryOrd")]                                      // ccCountryOrd AS CountryOrd
        public uint CountryOrd { get; set; }

        [FRBO(Field = "ccCountryCode")]                                     // ccCountryCode AS CountryCode
        public uint CountryCode { get; set; }

        [FRBO(Field = "ccCountryDesignation")]                              // ccCountryDesignation AS CountryDesignation
        public string CountryDesignation { get; set; }

        // Currency
        [FRBO(Field = "fmCurrency")]                                        // fmCurrency AS Currency
        public string Currency { get; set; }

        [FRBO(Field = "crCurrencyOrd")]                                     // crCurrencyOrd AS CurrencyOrd
        public uint CurrencyOrd { get; set; }

        [FRBO(Field = "crCurrencyCode")]                                    // crCurrencyCode AS CurrencyCode
        public uint CurrencyCode { get; set; }

        [FRBO(Field = "crCurrencyDesignation")]                             // crCurrencyDesignation AS CurrencyDesignation
        public string CurrencyDesignation { get; set; }

        [FRBO(Field = "crCurrencyAcronym")]                                 // crCurrencyAcronym AS CurrencyAcronym
        public string CurrencyAcronym { get; set; }

        // ArticleFamily
        [FRBO(Field = "afFamily")]                                          // afFamily AS ArticleFamily
        public string ArticleFamily { get; set; }

        [FRBO(Field = "afFamilyOrd")]                                       // afFamilyOrd AS ArticleFamilyOrd
        public uint ArticleFamilyOrd { get; set; }

        [FRBO(Field = "afFamilyCode")]                                      // afFamilyCode AS ArticleFamilyCode
        public uint ArticleFamilyCode { get; set; }

        [FRBO(Field = "afFamilyDesignation")]                               // afFamilyDesignation AS ArticleFamilyDesignation
        public string ArticleFamilyDesignation { get; set; }

        // ArticleSubFamily
        [FRBO(Field = "sfSubFamily")]                                       // sfSubFamily AS ArticleSubFamily
        public string ArticleSubFamily { get; set; }

        [FRBO(Field = "sfSubFamilyOrd")]                                    // sfSubFamilyOrd AS ArticleSubFamilyOrd
        public uint ArticleSubFamilyOrd { get; set; }

        [FRBO(Field = "sfSubFamilyCode")]                                   // sfSubFamilyCode AS ArticleSubFamilyCode
        public uint ArticleSubFamilyCode { get; set; }

        [FRBO(Field = "sfSubFamilyDesignation")]                            // sfSubFamilyDesignation AS SubFamilyDesignation
        public string ArticleSubFamilyDesignation { get; set; }

        // UserDetail
        [FRBO(Field = "udUserDetail")]                                      // udUserDetail AS UserDetail
        public string UserDetail { get; set; }

        [FRBO(Field = "udUserDetailOrd")]                                   // udUserDetailOrd AS UserDetailOrd
        public uint UserDetailOrd { get; set; }

        [FRBO(Field = "udUserDetailCode")]                                  // udUserDetailCode AS UserDetailCode
        public uint UserDetailCode { get; set; }

        [FRBO(Field = "udUserDetailName")]                                  // udUserDetailName AS UserDetailName
        public string UserDetailName { get; set; }

        // Terminal
        [FRBO(Field = "trTerminal")]                                        // trTerminal AS Terminal
        public string Terminal { get; set; }

        [FRBO(Field = "trTerminalOrd")]                                     // trTerminalOrd AS TerminalOrd
        public uint TerminalOrd { get; set; }

        [FRBO(Field = "trTerminalCode")]                                    // trTerminalCode AS TerminalCode
        public uint TerminalCode { get; set; }

        [FRBO(Field = "trTerminalDesignation")]                             // trTerminalDesignation AS TerminalDesignation
        public string TerminalDesignation { get; set; }

        // Place
        [FRBO(Field = "cpPlace")]                                           // cpPlace AS Place
        public string Place { get; set; }

        [FRBO(Field = "cpPlaceOrd")]                                        // cpPlaceOrd AS PlaceOrd
        public uint PlaceOrd { get; set; }

        [FRBO(Field = "cpPlaceCode")]                                       // cpPlaceCode AS PlaceCode
        public uint PlaceCode { get; set; }

        [FRBO(Field = "cpPlaceDesignation")]                                // cpPlaceDesignation AS PlaceDesignation
        public string PlaceDesignation { get; set; }

        // PlaceTable
        [FRBO(Field = "dmPlaceTable")]                                      // ctPlaceTable AS PlaceTable
        public string PlaceTable { get; set; }

        [FRBO(Field = "ctPlaceTableOrd")]                                   // ctPlaceTableOrd AS PlaceTableOrd
        public uint PlaceTableOrd { get; set; }

        [FRBO(Field = "ctPlaceTableCode")]                                  // ctPlaceTableCode AS PlaceTableCode
        public uint PlaceTableCode { get; set; }

        [FRBO(Field = "ctPlaceTableDesignation")]                           // ctPlaceTableDesignation AS PlaceTableDesignation
        public string PlaceTableDesignation { get; set; }

        // Article
        [FRBO(Field = "fdArticle")]                                         // fdArticle AS Article
        override public string Oid { get; set; }

        [FRBO(Field = "fdOrd")]                                             // fdOrd AS ArticleOrd
        public uint ArticleOrd { get; set; }

        [FRBO(Field = "fdCode")]                                            // fdCode AS ArticleCode
        public uint ArticleCode { get; set; }

        [FRBO(Field = "fdDesignation")]                                     // fdDesignation AS ArticleDesignation
        public string ArticleDesignation { get; set; }

        [FRBO(Field = "SUM(fdQuantity)")]                                   // SUM(fdQuantity) AS ArticleQuantity
        public decimal ArticleQuantity { get; set; }
        
        [FRBO(Field = "fdUnitMeasure")]                                     // fdUnitMeasure AS ArticleUnitMeasure
        public string ArticleUnitMeasure { get; set; }
        
        [FRBO(Field = "AVG(fdPrice)")]                                      // fdAvgPrice AS ArticlePrice
        public decimal ArticlePrice { get; set; }
        
        [FRBO(Field = "AVG((fdPrice - ((fdPrice * fdDiscount) / 100)))")]   // AVG((fdPrice - ((fdPrice * fdDiscount) / 100))) AS ArticlePriceWithDiscount
        public decimal ArticlePriceWithDiscount { get; set; }

        [FRBO(Field = "AVG(fdVat)")]                                        // AVG(fdVat) AS ArticleVat
        public decimal ArticleVat { get; set; }
        
        [FRBO(Field = "AVG(fdDiscount)")]                                   // AVG(fdDiscount) AS ArticleDiscount
        public decimal ArticleDiscount { get; set; }

        [FRBO(Field = "SUM(fdTotalNet)")]                                   // SUM(fdTotalNet) AS ArticleTotalNet
        public decimal ArticleTotalNet { get; set; }

        [FRBO(Field = "SUM(fdTotalGross)")]                                 // SUM(fdTotalGross) AS ArticleTotalGross
        public decimal ArticleTotalGross { get; set; }

        [FRBO(Field = "SUM(fdTotalDiscount)")]                              // SUM(fdTotalDiscount) AS ArticleTotalDiscount
        public decimal ArticleTotalDiscount { get; set; }

        [FRBO(Field = "SUM(fdTotalTax)")]                                   // SUM(fdTotalTax) AS ArticleTotalTax
        public decimal ArticleTotalTax { get; set; }

        [FRBO(Field = "SUM(fdTotalFinal)")]                                 // SUM(fdTotalFinal) AS ArticleTotalFinal
        public decimal ArticleTotalFinal { get; set; }

        [FRBO(Field = "COUNT(*)")]                                          //COUNT(*) AS GroupCount
        public decimal GroupCount { get; set; }
        */
    }
}
