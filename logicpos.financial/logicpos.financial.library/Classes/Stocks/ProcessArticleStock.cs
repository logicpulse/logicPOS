using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.resources.Resources.Localization;
using System;

namespace logicpos.financial.library.Classes.Stocks
{
    public class ProcessArticleStock
    {
        public static bool Add(ProcessArticleStockMode pMode, ProcessArticleStockParameter pParameter)
        {
            return Add(pMode, pParameter.Customer, 10, pParameter.DocumentDate, pParameter.DocumentNumber, pParameter.Article, pParameter.Quantity, pParameter.Notes);
        }

        public static bool Add(ProcessArticleStockMode pMode, ERP_Customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, FIN_Article pArticle, decimal pQuantity, string pNotes)
        {
            return Add(GlobalFramework.SessionXpo, pMode, pCustomer, 10, pDocumentDate, pDocumentNumber, pArticle, pQuantity, pNotes);
        }

        public static bool Add(Session pSession, ProcessArticleStockMode pMode, ERP_Customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, FIN_Article pArticle, decimal pQuantity, string pNotes)
        {
            return Add(GlobalFramework.SessionXpo, pMode, null, pCustomer, 10, pDocumentDate, pDocumentNumber, pArticle, pQuantity, pNotes);
        }

        public static bool Add(Session pSession, ProcessArticleStockMode pMode, FIN_DocumentFinanceDetail pDocumentDetail, ERP_Customer pCustomer, int pOrd, DateTime pDocumentDate, string pDocumentNumber, FIN_Article pArticle, decimal pQuantity, string pNotes)
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
                ERP_Customer customer = (ERP_Customer)pSession.GetObjectByKey(typeof(ERP_Customer), pCustomer.Oid);
                FIN_Article article = (FIN_Article)pSession.GetObjectByKey(typeof(FIN_Article), pArticle.Oid);
                POS_ConfigurationPlaceTerminal terminal = (POS_ConfigurationPlaceTerminal)pSession.GetObjectByKey(typeof(POS_ConfigurationPlaceTerminal), GlobalFramework.LoggedTerminal.Oid);
                SYS_UserDetail userDetail = (SYS_UserDetail)pSession.GetObjectByKey(typeof(SYS_UserDetail), GlobalFramework.LoggedUser.Oid);


                FIN_ArticleStock articleStock = new FIN_ArticleStock(pSession)
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
                if (pSession.GetType() != typeof(UnitOfWork)) articleStock.Save();

                //Audit
                switch (pMode)
                {
                    case ProcessArticleStockMode.Out:
                        FrameworkUtils.Audit("STOCK_MOVEMENT_OUT", string.Format(Resx.audit_message_stock_movement_out, article.Designation, quantity));
                        break;
                    case ProcessArticleStockMode.In:
                        FrameworkUtils.Audit("STOCK_MOVEMENT_IN", string.Format(Resx.audit_message_stock_movement_in, article.Designation, quantity));
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

        public static bool Add(FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            bool result = false;
            int ord = 0;
            ProcessArticleStockMode mode = (pDocumentFinanceMaster.DocumentType.StockMode > 0) 
                ? (ProcessArticleStockMode) pDocumentFinanceMaster.DocumentType.StockMode 
                : ProcessArticleStockMode.None
            ;

            try
            {
                //Only Process if has a Valid Mode and a valid article Class
                if (mode != ProcessArticleStockMode.None)
                {
                    using (UnitOfWork uowSession = new UnitOfWork())
                    {
                        try
                        {
                            //Get Objects in same Session
                            FIN_DocumentFinanceMaster documentFinanceMaster = (FIN_DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), pDocumentFinanceMaster.Oid);
                            ERP_Customer customer = (ERP_Customer)uowSession.GetObjectByKey(typeof(ERP_Customer), pDocumentFinanceMaster.EntityOid);

                            foreach (FIN_DocumentFinanceDetail item in documentFinanceMaster.DocumentDetail)
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
                                        item.Article, item.Quantity, item.Notes
                                    );
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
