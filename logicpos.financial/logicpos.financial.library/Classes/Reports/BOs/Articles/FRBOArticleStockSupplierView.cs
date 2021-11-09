using System;

namespace logicpos.financial.library.Classes.Reports.BOs.Articles
{
    [FRBO(Entity = "view_articlestocksupplier")]
    public class FRBOArticleStockSupplierView : FRBOBaseObject
    {
        
        [FRBO(Field = "stmOid")]
        //Primary Oid (Required) Stock Moviment
        override public string Oid { get; set; }                            //stmOid AS Oid,  

		// ArticleFamily
        [FRBO(Field = "afaOid")]
        public string ArticleFamily { get; set; }                        //afaOid AS ArticleFamily,

        [FRBO(Field = "afaOrd")]
        public uint ArticleFamilyOrd { get; set; }                          //afaOrd AS ArticleFamilyOrd,

        [FRBO(Field = "afaCode")]                                           
        public uint ArticleFamilyCode { get; set; }                         //afaCode AS ArticleFamilyCode,

        [FRBO(Field = "afaDesignation")]
        public string ArticleFamilyDesignation { get; set; }                //afaDesignation AS ArticleFamilyDesignation,

        // ArticleSubFamily
        [FRBO(Field = "asfOid")]
        public string ArticleSubFamily { get; set; }                        //asfOid AS ArticleSubFamily,

        [FRBO(Field = "asfOrd")]
        public uint ArticleSubFamilyOrd { get; set; }                       //asfOrd AS ArticleSubFamilyOrd,

        [FRBO(Field = "asfCode")]
        public uint ArticleSubFamilyCode { get; set; }                      //asfCode AS ArticleSubFamilyCode,

        [FRBO(Field = "asfDesignation")]
        public string ArticleSubFamilyDesignation { get; set; }             //asfDesignation AS ArticleSubFamilyDesignation,

        // Article
        [FRBO(Field = "Article")]
        public string Article { get; set; }                                 //artOid AS Article,

        [FRBO(Field = "artOrd")]
        public uint ArticleOrd { get; set; }                                //artOrd AS ArticleOrd,

        [FRBO(Field = "artCode")]
        public string ArticleCode { get; set; }                             //artCode AS ArticleCode,

        [FRBO(Field = "artCodeDealer")]
        public string ArticleCodeDealer { get; set; }                       //artCodeDealer AS ArticleCodeDealer,

        [FRBO(Field = "artDesignation")]
        public string ArticleDesignation { get; set; }                      //artDesignation AS ArticleDesignation,
		
		// ConfigurationUnitMeasure
        [FRBO(Field = "aumAcronym")]
        public string ConfigurationUnitMeasureaumAcronym { get; set; }      //aumAcronym AS ConfigurationUnitMeasureaumAcronym

        [FRBO(Field = "aumDesignation")]
        public string ConfigurationUnitMeasureDesignation { get; set; }     //aumDesignation AS ConfigurationUnitMeasureDesignation,

        // ArticleStock
        [FRBO(Field = "Date")]
        public DateTime ArticleStockDate { get; set; }                      //stkDate AS ArticleStockDate,

        [FRBO(Field = "stmDateDay")]
        public string ArticleStockDateDay { get; set; }                     //stmDateDay AS ArticleStockDateDay,

        [FRBO(Field = "EntityOid")]
        public string ArticleStockCostumer { get; set; }              //stmCostumer AS ArticleStockCostumer,
		
		[FRBO(Field = "stmPrice")]
        public decimal ArticleStockPurchasedPrice { get; set; }             //stmPrice AS ArticleStockPurchasedPrice,

        [FRBO(Field = "stmQtd")]
        public decimal ArticleStockQuantity { get; set; }                   //stmQtd AS ArticleStockQuantity,
		
		//Article Details
		 [FRBO(Field = "wrhDesignation")]
        public string ArticleStockWarehouse { get; set; }                     

        [FRBO(Field = "whlDesignation")]
        public string ArticleStockLocation { get; set; }     

        [FRBO(Field = "asnSerialNumber")]
        public string ArticleStockSerialNumber { get; set; } 

		[FRBO(Field = "ctmName")]
        public string ArticleStockCostumerName { get; set; } 
		
		//Document
		[FRBO(Field = "stmDocumentNumber")]
        public string ArticleStockDocumentNumber { get; set; } 
		
		[FRBO(Field = "Currency")]		
		public string ArticleStockCurrency { get; set; } 


    }
}
