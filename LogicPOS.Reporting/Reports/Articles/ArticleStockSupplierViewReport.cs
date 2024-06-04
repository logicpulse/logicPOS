using LogicPOS.Reporting.Common;
using System;

namespace LogicPOS.Reporting.Reports.Articles
{
    [Report(Entity = "view_articlestocksupplier")]
    public class ArticleStockSupplierViewReport : ReportBase
    {

        [Report(Field = "stmOid")]
        //Primary Oid (Required) Stock Moviment
        override public string Oid { get; set; }                            //stmOid AS Oid,  

        // ArticleFamily
        [Report(Field = "afaOid")]
        public string ArticleFamily { get; set; }                        //afaOid AS ArticleFamily,

        [Report(Field = "afaOrd")]
        public uint ArticleFamilyOrd { get; set; }                          //afaOrd AS ArticleFamilyOrd,

        [Report(Field = "afaCode")]
        public uint ArticleFamilyCode { get; set; }                         //afaCode AS ArticleFamilyCode,

        [Report(Field = "afaDesignation")]
        public string ArticleFamilyDesignation { get; set; }                //afaDesignation AS ArticleFamilyDesignation,

        // ArticleSubFamily
        [Report(Field = "asfOid")]
        public string ArticleSubFamily { get; set; }                        //asfOid AS ArticleSubFamily,

        [Report(Field = "asfOrd")]
        public uint ArticleSubFamilyOrd { get; set; }                       //asfOrd AS ArticleSubFamilyOrd,

        [Report(Field = "asfCode")]
        public uint ArticleSubFamilyCode { get; set; }                      //asfCode AS ArticleSubFamilyCode,

        [Report(Field = "asfDesignation")]
        public string ArticleSubFamilyDesignation { get; set; }             //asfDesignation AS ArticleSubFamilyDesignation,

        // Article
        [Report(Field = "Article")]
        public string Article { get; set; }                                 //artOid AS Article,

        [Report(Field = "artOrd")]
        public uint ArticleOrd { get; set; }                                //artOrd AS ArticleOrd,

        [Report(Field = "artCode")]
        public string ArticleCode { get; set; }                             //artCode AS ArticleCode,

        [Report(Field = "artCodeDealer")]
        public string ArticleCodeDealer { get; set; }                       //artCodeDealer AS ArticleCodeDealer,

        [Report(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,

        // ConfigurationUnitMeasure
        [Report(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [Report(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,

        // ArticleStock
        [Report(Field = "Date")]
        public DateTime ArticleStockDate { get; set; }                      //stkDate AS ArticleStockDate,

        [Report(Field = "stmDateDay")]
        public string ArticleStockDateDay { get; set; }                     //stmDateDay AS ArticleStockDateDay,

        [Report(Field = "EntityOid")]
        public string ArticleStockCostumer { get; set; }              //stmCostumer AS ArticleStockCostumer,

        [Report(Field = "stmPrice")]
        public decimal ArticleStockPurchasedPrice { get; set; }             //stmPrice AS ArticleStockPurchasedPrice,

        [Report(Field = "stmQtd")]
        public decimal ArticleStockQuantity { get; set; }                   //stmQtd AS ArticleStockQuantity,

        //Article Details
        [Report(Field = "wrhDesignation")]
        public string ArticleStockWarehouse { get; set; }

        [Report(Field = "whlDesignation")]
        public string ArticleStockLocation { get; set; }

        [Report(Field = "asnSerialNumber")]
        public string ArticleStockSerialNumber { get; set; }

        [Report(Field = "ctmName")]
        public string ArticleStockCostumerName { get; set; }

        //Document
        [Report(Field = "stmDocumentNumber")]
        public string ArticleStockDocumentNumber { get; set; }

        [Report(Field = "Currency")]
        public string ArticleStockCurrency { get; set; }


    }
}
