using LogicPOS.Reporting.Common;
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

namespace LogicPOS.Reporting.Reports.Articles
{
    [Report(Entity = "view_articlestock")]
    public class ArticleStockViewReport : ReportBase
    {
        // ArticleFamily
        [Report(Field = "afaOid")]
        //Primary Oid (Required)
        override public string Oid { get; set; }                            //afaOid AS Oid,  

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

        [Report(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,

        // ArticleStock
        [Report(Field = "artStk")]
        public decimal ArticleStockQuantity { get; set; }                   //artStk AS ArticleStockQuantity,

        [Report(Field = "artMinStock")]
        public decimal ArticleStockMinimum { get; set; }                   //artMinStock AS ArticleStockMinimum,

        // ConfigurationUnitMeasure
        [Report(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [Report(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,

        // Date
        [Report(Field = "stmDateDay")]
        public string ArticleStockDateDay { get; set; }

        [Report(Field = "Date")]
        public DateTime ArticleStockDate { get; set; }

    }
}
