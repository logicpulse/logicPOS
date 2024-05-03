using DevExpress.Xpo;
using logicpos.datalayer.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using logicpos.shared.App;
using System;

namespace logicpos.financial.library.Classes.Stocks
{
    public class ProcessArticleStock
    {
        public static bool Add(ProcessArticleStockMode pMode, ProcessArticleStockParameter pParameter)
        {
            return Add(pMode, pParameter.Customer, 10, pParameter.DocumentDate, pParameter.DocumentNumber, pParameter.Article, pParameter.Quantity, pParameter.Notes);
        }

        public static bool Add(ProcessArticleStockMode pMode, erp_customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, fin_article pArticle, decimal pQuantity, string pNotes)
        {
            return Add(XPOSettings.Session, pMode, pCustomer, 10, pDocumentDate, pDocumentNumber, pArticle, pQuantity, pNotes);
        }

        public static bool Add(Session pSession, ProcessArticleStockMode pMode, erp_customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, fin_article pArticle, decimal pQuantity, string pNotes)
        {
            return Add(XPOSettings.Session, pMode, null, pCustomer, 10, pDocumentDate, pDocumentNumber, pArticle, pQuantity, pNotes);
        }

        public static bool Add(Session pSession, ProcessArticleStockMode pMode, fin_documentfinancedetail pDocumentDetail, erp_customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, fin_article pArticle, decimal pQuantity, string pNotes)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool result = false;
            decimal quantity = 0.0m;

            try
            {
                switch (pMode)
                {
                    case ProcessArticleStockMode.Out:
                        quantity = -(pQuantity);
                        break;
                    case ProcessArticleStockMode.In:
                        quantity = pQuantity;
                        break;
                }

                //Get Objects in same Session
                //Gestão de Stocks - Ajuste de Stock diretamente no Artigo (BackOffice) [IN:016530]
                erp_customer customer = (erp_customer)pSession.GetObjectByKey(typeof(erp_customer), SharedSettings.XpoOidUserRecord);
                if (pCustomer != null)
                {
                    customer = (erp_customer)pSession.GetObjectByKey(typeof(erp_customer), pCustomer.Oid);
                }
                fin_article article = (fin_article)pSession.GetObjectByKey(typeof(fin_article), pArticle.Oid);
                pos_configurationplaceterminal terminal = (pos_configurationplaceterminal)pSession.GetObjectByKey(typeof(pos_configurationplaceterminal), DataLayerFramework.LoggedTerminal.Oid);
                sys_userdetail userDetail = (sys_userdetail)pSession.GetObjectByKey(typeof(sys_userdetail), DataLayerFramework.LoggedUser.Oid);

                fin_articlestock articleStock = new fin_articlestock(pSession)
                {
                    Customer = customer,
                    Date = pDocumentDate,
                    Article = article,
                    Quantity = quantity,
                    Notes = pNotes,
                    CreatedWhere = terminal,
                    CreatedBy = userDetail
                };
                if (pDocumentNumber != string.Empty) articleStock.DocumentNumber = pDocumentNumber;
                if (pDocumentDetail != null)
                {
                    articleStock.DocumentNumber = pDocumentDetail.DocumentMaster.DocumentNumber;
                    articleStock.DocumentMaster = pDocumentDetail.DocumentMaster;
                    articleStock.DocumentDetail = pDocumentDetail;
                }

                //Only saves if not Working on a Unit Of Work Transaction
                //Gestão de Stocks : Janela de Gestão de Stocks [IN:016534]
                if (pSession.GetType() != typeof(UnitOfWork))
                {
                    article.Accounting += quantity;
                    article.Save();
                    articleStock.Save();
                }

                //Audit
                switch (pMode)
                {
                    case ProcessArticleStockMode.Out:
                        SharedUtils.Audit("STOCK_MOVEMENT_OUT", string.Format(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "audit_message_stock_movement_out"), article.Designation, SharedUtils.DecimalToString(quantity, SharedSettings.DecimalFormatStockQuantity)));
                        break;
                    case ProcessArticleStockMode.In:
                        SharedUtils.Audit("STOCK_MOVEMENT_IN", string.Format(resources.CustomResources.GetCustomResource(LogicPOS.Settings.GeneralSettings.Settings["customCultureResourceDefinition"], "audit_message_stock_movement_in"), article.Designation, SharedUtils.DecimalToString(quantity, SharedSettings.DecimalFormatStockQuantity)));
                        break;
                }

                result = true;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// If ProcessArticleStockMode.None
        /// </summary>
        public static bool Add(
            fin_documentfinancemaster pDocumentFinanceMaster,
            // Used to force ReverseStockMode, used in cancel Documents to restore Stocks
            bool pReverseStockMode = false
            )
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool result = false;
            int ord = 0;

            // Check if Works with Stock
            ProcessArticleStockMode mode = (pDocumentFinanceMaster.DocumentType.StockMode > 0)
                ? (ProcessArticleStockMode)pDocumentFinanceMaster.DocumentType.StockMode
                : ProcessArticleStockMode.None
            ;

            try
            {
                //Only Process if has a Valid Mode and a valid article Class
                if (mode != ProcessArticleStockMode.None)
                {
                    // ReverseStockMode : Used for ex when we cancal Finance Documents to Restore/Revert Stock
                    if (pReverseStockMode && mode.Equals(ProcessArticleStockMode.Out))
                    {
                        mode = ProcessArticleStockMode.In;
                    }
                    else if (pReverseStockMode && mode.Equals(ProcessArticleStockMode.In))
                    {
                        mode = ProcessArticleStockMode.Out;
                    }

                    using (UnitOfWork uowSession = new UnitOfWork())
                    {
                        try
                        {
                            //Get Objects in same Session
                            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)uowSession.GetObjectByKey(typeof(fin_documentfinancemaster), pDocumentFinanceMaster.Oid);
                            erp_customer customer = (erp_customer)uowSession.GetObjectByKey(typeof(erp_customer), pDocumentFinanceMaster.EntityOid);

                            foreach (fin_documentfinancedetail item in documentFinanceMaster.DocumentDetail)
                            {
                                //Check if article works in Stock
                                if (item.Article.Class.WorkInStock)
                                {
                                    //Increment Order
                                    ord += 10;
                                    Add(
                                        uowSession,
                                        mode, item,
                                        customer, ord, documentFinanceMaster.Date, documentFinanceMaster.DocumentNumber,
                                        item.Article,
                                        // ReverseStock
                                        item.Quantity,
                                        item.Notes
                                    );

                                    //Artigos Compostos [IN:016522]
                                    if (item.Article.IsComposed)
                                    {
                                        foreach (fin_articlecomposition compositeArticle in item.Article.ArticleComposition)
                                        {
                                            fin_article articleChild = compositeArticle.ArticleChild;
                                            Add(
                                            uowSession,
                                            mode, item,
                                            customer, ord, documentFinanceMaster.Date, documentFinanceMaster.DocumentNumber,
                                            articleChild,
                                            // ReverseStock
                                            compositeArticle.Quantity * item.Quantity,
                                            item.Notes
                                        );
                                        }
                                    }
                                }
                            }
                            uowSession.CommitChanges();
                        }
                        catch (Exception ex)
                        {
                            uowSession.RollbackTransaction();
                            log.Error(ex.Message, ex);
                        }
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                log.Debug(ex.Message);
            }

            return result;
        }
    }
}
