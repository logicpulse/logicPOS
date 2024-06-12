using LogicPOS.Reporting.Data.Common;
using System;

/* Report Fields
SELECT 
	Date, artCode, artDesignation, stkQuantity, aumAcronym
FROM 
	view_articlestock 
ORDER BY 
	stkDate
;
*/

namespace LogicPOS.Reporting.Reports.Data
{
    [ReportData(Entity = "view_articlestockwarehouse")]
    public class ArticleStockWareHouseViewReportData : ReportData
    {
        // ArticleFamily
        [ReportData(Field = "afaOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //afaOid AS Oid,  

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

        // ArticleStock
        [ReportData(Field = "stkOid")]
        public string ArticleStock { get; set; }                            //stkOid AS ArticleStock,

        [ReportData(Field = "Date")]
        public DateTime ArticleStockDate { get; set; }                      //stkDate AS ArticleStockDate,

        [ReportData(Field = "stkDateDay")]
        public string ArticleStockDateDay { get; set; }                            //stkDateDay AS ArticleStock,


        [ReportData(Field = "stkQuantity")]
        public decimal ArticleStockQuantity { get; set; }                   //stkQuantity AS ArticleStockQuantity,

        // ConfigurationUnitMeasure
        [ReportData(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [ReportData(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,

        //Warehouse
        [ReportData(Field = "wrhDesignation")]
        public string ArticleWareHouseDesignation { get; set; }                   //wrhDesignation AS ArticleWareHouseDesignation,

        [ReportData(Field = "whlDesignation")]
        public string ArticleWareHouseLocation { get; set; }                   //whlDesignation AS ArticleWareHouseLocation,

        //SerialNumber
        [ReportData(Field = "asnSerialNumber")]
        public string ArticleSerialNumber { get; set; }                   //asnSerialNumber AS ArticleSerialNumber,
    }
}
