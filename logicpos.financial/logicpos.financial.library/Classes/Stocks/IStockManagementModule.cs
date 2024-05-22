using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.DataLayer.Xpo.Documents;
using logicpos.datalayer.Enums;
using LogicPOS.Plugin.Abstractions;
using System;
using System.Collections.Generic;

namespace logicpos.financial.library.Classes.Stocks
{
    public interface IStockManagementModule : IPlugin
    {
        bool Add(ProcessArticleStockMode pMode, ProcessArticleStockParameter pParameter);

        bool Add(ProcessArticleStockMode pMode, erp_customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, fin_article pArticle, decimal pQuantity, string pNotes, string pSerialNumber = "", decimal pPurchasePrice = 0, fin_warehouselocation pWarehouselocation = null, byte[] pAttachedFile = null, List<fin_articleserialnumber> pAssociatedArticles = null, bool pReverseStockMode = false, bool pChanged = false);

        bool Add(Session pSession, ProcessArticleStockMode pMode, erp_customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, fin_article pArticle, decimal pQuantity, string pNotes, string pSerialNumber = "", decimal pPurchasePrice = 0, fin_warehouselocation pWarehouselocation = null, byte[] pAttachedFile = null, List<fin_articleserialnumber> pAssociatedArticles = null, bool pReverseStockMode = false, bool pChanged = false);

        bool Add(Session pSession, ProcessArticleStockMode pMode, fin_documentfinancedetail pDocumentDetail, erp_customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, fin_article pArticle, decimal pQuantity, string pNotes, string pSerialNumber = "", decimal pPurchasePrice = 0, fin_warehouselocation pWarehouselocation = null, byte[] pAttachedFile = null, List<fin_articleserialnumber> pAssociatedArticles = null, bool pReverseStockMode = false, bool pChanged = false);

        bool Add(fin_documentfinancemaster pDocumentFinanceMaster, bool pReverseStockMode = false);
    }
}
