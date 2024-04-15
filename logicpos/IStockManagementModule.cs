using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Stocks;
using System;
using System.Collections.Generic;

namespace logicpos
{
    internal interface IStockManagementModule
    {
        bool Add(
            Session session, 
            ProcessArticleStockMode processArticleStockMode, 
            fin_documentfinancedetail documentDetail, 
            erp_customer own_customer, 
            int v1, 
            DateTime now, 
            string v2, 
            fin_article article, 
            int v3, 
            string v4, 
            string serialNumber, 
            decimal purchasePrice, 
            fin_warehouselocation location, 
            object p, 
            List<fin_articleserialnumber> selectedAssocietedArticles, 
            bool v5, 
            bool v6);

        bool Add(
            Session session, 
            ProcessArticleStockMode processArticleStockMode, 
            erp_customer own_customer, 
            int v1, 
            DateTime now, 
            string v2, 
            fin_article article, 
            int v3, 
            string v4, 
            string serialNumber, 
            decimal purchasePrice, 
            fin_warehouselocation location, 
            object p, 
            List<fin_articleserialnumber> selectedAssocietedArticles, 
            bool v5, 
            bool v6);

        void Add(
            ProcessArticleStockMode processArticleStockMode, 
            ProcessArticleStockParameter res);

        bool Add(
            fin_documentfinancemaster processArticleStockMode, 
            bool p);
    }
}