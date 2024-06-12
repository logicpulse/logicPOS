using LogicPOS.Reporting.Data.Common;
using System;

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "view_articlestocksupplier")]
    public class ArticleStockSupplierViewReportData : ReportData
    {

        [ReportData(Field = "stmOid")]
        //Primary Oid (Required) Stock Moviment
        override public string Oid { get; set; }                            //stmOid AS Oid,  

        // ArticleFamily
        [ReportData(Field = "afaOid")]
        public string ArticleFamily { get; set; }                        //afaOid AS ArticleFamily,

        [ReportData(Field = "afaOrd")]
        public uint ArticleFamilyOrd { get; set; }                          //afaOrd AS ArticleFamilyOrd,

        [ReportData(Field = "afaCode")]
        public uint ArticleFamilyCode { get; set; }                         //afaCode AS ArticleFamilyCode,

        [ReportData(Field = "afaDesignation")]
        public string ArticleFamilyDesignation { get; set; }                //afaDesignation AS ArticleFamilyDesignation,

        // ArticleSubFamily
        [ReportData(Field = "asfOid")]
        public string ArticleSubFamily { get; set; }                        //asfOid AS ArticleSubFamily,

        [ReportData(Field = "asfOrd")]
        public uint ArticleSubFamilyOrd { get; set; }                       //asfOrd AS ArticleSubFamilyOrd,

        [ReportData(Field = "asfCode")]
        public uint ArticleSubFamilyCode { get; set; }                      //asfCode AS ArticleSubFamilyCode,

        [ReportData(Field = "asfDesignation")]
        public string ArticleSubFamilyDesignation { get; set; }             //asfDesignation AS ArticleSubFamilyDesignation,

        // Article
        [ReportData(Field = "Article")]
        public string Article { get; set; }                                 //artOid AS Article,

        [ReportData(Field = "artOrd")]
        public uint ArticleOrd { get; set; }                                //artOrd AS ArticleOrd,

        [ReportData(Field = "artCode")]
        public string ArticleCode { get; set; }                             //artCode AS ArticleCode,

        [ReportData(Field = "artCodeDealer")]
        public string ArticleCodeDealer { get; set; }                       //artCodeDealer AS ArticleCodeDealer,

        [ReportData(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,

        // ConfigurationUnitMeasure
        [ReportData(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [ReportData(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,

        // ArticleStock
        [ReportData(Field = "Date")]
        public DateTime ArticleStockDate { get; set; }                      //stkDate AS ArticleStockDate,

        [ReportData(Field = "stmDateDay")]
        public string ArticleStockDateDay { get; set; }                     //stmDateDay AS ArticleStockDateDay,

        [ReportData(Field = "EntityOid")]
        public string ArticleStockCostumer { get; set; }              //stmCostumer AS ArticleStockCostumer,

        [ReportData(Field = "stmPrice")]
        public decimal ArticleStockPurchasedPrice { get; set; }             //stmPrice AS ArticleStockPurchasedPrice,

        [ReportData(Field = "stmQtd")]
        public decimal ArticleStockQuantity { get; set; }                   //stmQtd AS ArticleStockQuantity,

        //Article Details
        [ReportData(Field = "wrhDesignation")]
        public string ArticleStockWarehouse { get; set; }

        [ReportData(Field = "whlDesignation")]
        public string ArticleStockLocation { get; set; }

        [ReportData(Field = "asnSerialNumber")]
        public string ArticleStockSerialNumber { get; set; }

        [ReportData(Field = "ctmName")]
        public string ArticleStockCostumerName { get; set; }

        //Document
        [ReportData(Field = "stmDocumentNumber")]
        public string ArticleStockDocumentNumber { get; set; }

        [ReportData(Field = "Currency")]
        public string ArticleStockCurrency { get; set; }


    }
}
