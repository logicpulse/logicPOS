using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Reports;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using System;
using System.Collections.Generic;

namespace logicpos.financial.library.App
{
    public class FrameworkUtils : logicpos.shared.App.FrameworkUtils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceDocument

        //Check if Document has DocumentFinanceType Parent (Recursively)
        public static bool IsDocumentMasterChildOfDocumentType(FIN_DocumentFinanceMaster pDocumentFinanceMaster, List<Guid> pDocumentFinanceTypeList)
        {
            bool debug = false;
            bool result = false;

            try
            {
                if (pDocumentFinanceMaster.DocumentParent != null)
                {
                    if (debug) _log.Debug(string.Format("IsDocumentMasterChildOfDocumentType DocumentParent: [{0}]", pDocumentFinanceMaster.DocumentParent.DocumentNumber));

                    if (pDocumentFinanceTypeList.Contains(pDocumentFinanceMaster.DocumentParent.DocumentType.Oid))
                    {
                        //DocumentType Detected
                        result = true;
                    }
                    else
                    {
                        //Recursive Call IsDocumentMasterChildOfDocumentType
                        result = IsDocumentMasterChildOfDocumentType(pDocumentFinanceMaster.DocumentParent, pDocumentFinanceTypeList);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //Check if Document has SaftDocumentType Parent (Recursively)
        public static bool IsDocumentMasterChildOfDocumentType(FIN_DocumentFinanceMaster pDocumentFinanceMaster, SaftDocumentType pSaftDocumentType)
        {
            bool debug = false;
            bool result = false;
            List<Guid> documentFinanceTypeList = new List<Guid>();
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled is NULL) AND (SaftDocumentType = {0})", (int)pSaftDocumentType));
            SortingCollection sortingCollection = new SortingCollection();
            sortingCollection.Add(new SortProperty("Ord", SortingDirection.Ascending));
            XPCollection xpcDocumentFinanceType = GetXPCollectionFromCriteria(GlobalFramework.SessionXpo, typeof(FIN_DocumentFinanceType), criteriaOperator, sortingCollection);

            try
            {
                foreach (FIN_DocumentFinanceType item in xpcDocumentFinanceType)
                {
                    if (debug) _log.Debug(string.Format("Add DocumentFinanceType: [{0}]", item.Designation));
                    documentFinanceTypeList.Add(item.Oid);
                }

                if (documentFinanceTypeList.Count > 0)
                {
                    result = IsDocumentMasterChildOfDocumentType(pDocumentFinanceMaster, documentFinanceTypeList);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        // Get and Array of Compatible Source Documents for DocumentType, used in Selection boxs and in Framework Validation to test if DocumentParent is Valid
        public static Guid[] GetDocumentTypeValidSourceDocuments(Guid pDocumentFinanceType)
        {
            Guid[] result = new Guid[] { };

            try
            {
                //Invoice,SimplifiedInvoice,InvoiceAndPayment
                if (
                    //SaftDocumentType = 1
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                    //pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                    //SaftDocumentType = 0
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput
                )
                {
                    result = new Guid[] {
                        //SaftDocumentType = 2
                        SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote,
                        SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide,
                        SettingsApp.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide,
                        SettingsApp.XpoOidDocumentFinanceTypeConsignmentGuide,
                        SettingsApp.XpoOidDocumentFinanceTypeReturnGuide,
                        //SaftDocumentType = 3 
                        SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument,
                        SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice,
                        //SaftDocumentType = 0 
                        SettingsApp.XpoOidDocumentFinanceTypeBudget,
                        SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice
                    };
                }
                //CreditNote
                else if (
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeCreditNote
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 1
                        SettingsApp.XpoOidDocumentFinanceTypeInvoice,
                        SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill,
                        SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                        SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment
                    };
                }
                //MovementOfGoods,WorkingDocuments
                else if (
                    //SaftDocumentType = 2
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeConsignmentGuide ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeReturnGuide ||
                    //SaftDocumentType = 3
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument ||
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 1
                        SettingsApp.XpoOidDocumentFinanceTypeInvoice,
                        SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                        SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment,
                        //SaftDocumentType = 3 
                        SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument,
                        //SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice,
                        //SaftDocumentType = 0
                        SettingsApp.XpoOidDocumentFinanceTypeBudget,
                        SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice
                    };
                }
                //DocumentFinanceTypeInvoiceWayBill
                else if (
                    pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 3
                        SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument,
                        SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice,
                        SettingsApp.XpoOidDocumentFinanceTypeBudget,
                        SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice
                    };
                }
                //All Other Documents
                else
                {
                    result = new Guid[] { };
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Get a Collection of UnCredited DocumentFinance Detail Items , ex Detail Lines that dont have been already Credited from a Invoice Type Document
        /// Method specific to CreditNotes
        /// </summary>
        public static XPCollection<FIN_DocumentFinanceDetail> GetUnCreditedItemsFromSourceDocument(FIN_DocumentFinanceMaster pSourceDocument, out string pCreditedDocuments)
        {
            XPCollection<FIN_DocumentFinanceDetail> result = new XPCollection<FIN_DocumentFinanceDetail>(pSourceDocument.Session, false);
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("DocumentMaster = '{0}'", pSourceDocument.Oid));
            XPCollection xpoCollectionReferences = new XPCollection(pSourceDocument.Session, typeof(FIN_DocumentFinanceDetailReference), criteria);
            bool addToCollection = true;
            List<string> listCreditedDocuments = new List<string>();
            pCreditedDocuments = string.Empty;

            try
            {
                //Loop SourceDocument Details
                foreach (FIN_DocumentFinanceDetail itemSource in pSourceDocument.DocumentDetail)
                {
                    //Reset
                    addToCollection = true;
                    //Loop SourceDocument Details
                    foreach (FIN_DocumentFinanceDetailReference itemReferences in xpoCollectionReferences)
                    {
                        if (
                            //Same props has ArticleBag Key
                            itemSource.Article.Oid == itemReferences.DocumentDetail.Article.Oid &&
                            itemSource.Designation == itemReferences.DocumentDetail.Designation &&
                            itemSource.Price == itemReferences.DocumentDetail.Price &&
                            itemSource.Discount == itemReferences.DocumentDetail.Discount &&
                            itemSource.Vat == itemReferences.DocumentDetail.Vat &&
                            itemSource.VatExemptionReason == itemReferences.DocumentDetail.VatExemptionReason
                        )
                        {
                            //_log.Debug(string.Format("Already Credited in DocumentNumber: [{0}]", itemReferences.DocumentDetail.DocumentMaster.DocumentNumber));
                            //Add to out pCreditedInDocumentNumber List, to Show Documents to User, if current Documents has been full Credited
                            if (!listCreditedDocuments.Contains(itemReferences.DocumentDetail.DocumentMaster.DocumentNumber)) listCreditedDocuments.Add(itemReferences.DocumentDetail.DocumentMaster.DocumentNumber);
                            //Dont add this detail line to Collection, item has been already processed
                            addToCollection = false;
                            break;
                        }
                    }
                    //Add Line to result Collection
                    if (addToCollection) result.Add(itemSource);
                }

                //Generate CreditedDocuments outs Result
                listCreditedDocuments.Sort();
                if (result.Count == 0 && listCreditedDocuments.Count > 0)
                {
                    for (int i = 0; i < listCreditedDocuments.Count; i++)
                    {
                        pCreditedDocuments += string.Format("{0} - {1}", Environment.NewLine, listCreditedDocuments[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Method to check if Credit Note ArticleBag is Valid
        /// </summary>
        public static bool GetCreditNoteValidation(FIN_DocumentFinanceMaster pDocumentParent, ArticleBag pArticleBag)
        {
            bool debug = false;
            bool result = false;

            try
            {
                if (pArticleBag != null)
                {
                    string sql = string.Empty;
                    object resultAlreadyCredited;
                    object resultParentDocument;
                    decimal totalAlreadyCredited;
                    decimal totalParentDocument;

                    foreach (var item in pArticleBag)
                    {
                        //Get Total Already Credit Items in this Document
                        sql = string.Format("SELECT Quantity AS Total FROM fin_documentfinancedetail WHERE DocumentMaster = '{0}' AND Article = '{1}';", pDocumentParent.Oid, item.Key.ArticleOid);
                        resultParentDocument = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                        totalParentDocument = (resultParentDocument != null) ? Convert.ToDecimal(resultParentDocument) : 0.0m;

                        sql = string.Format("SELECT SUM(fdQuantity) AS Total FROM view_documentfinance WHERE ftOid = '{0}' AND fmDocumentParent = '{1}' AND fdArticle = '{2}';", SettingsApp.XpoOidDocumentFinanceTypeCreditNote, pDocumentParent.Oid, item.Key.ArticleOid);
                        resultAlreadyCredited = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                        totalAlreadyCredited = (resultAlreadyCredited != null) ? Convert.ToDecimal(resultAlreadyCredited) : 0.0m;

                        if (debug) _log.Debug(String.Format(
                            "[{0}], Parent: [{1}], CanBeCredited: [{2}], TryToCredit: [{3}], Diference: [{4}]",
                            item.Key.Designation,
                            totalParentDocument,                                                //Total in Parent/SourceDocument
                            (totalParentDocument - totalAlreadyCredited),                       //Total that can be Credited
                            item.Value.Quantity,                                                //Total Trying to Credit
                            (totalParentDocument - totalAlreadyCredited) - item.Value.Quantity  //Diference
                            )
                        );

                        //Check if try to Credit more than UnCredit Articles
                        if (item.Value.Quantity > (totalParentDocument - totalAlreadyCredited))
                        {
                            result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        // Get and Array of Compatible Source Documents for DocumentType, used in Selection boxs and in Framework Validation to test if DocumentParent is Valid
        public static List<Guid> GetValidDocumentsForPayment(Guid pCustomer)
        {
            List<Guid> result = new List<Guid>();

            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    Oid
                    FROM 
	                    fin_documentfinancemaster 
                    WHERE
	                    (Disabled IS NULL OR Disabled <> 1) AND 
	                    (
		                    DocumentType = '{1}' OR
		                    DocumentType = '{2}' OR
		                    DocumentType = '{3}'
	                    ) AND
	                    (
		                    EntityOid = '{0}' AND 
		                    Payed = 0 AND DocumentStatusStatus <> 'A'
	                    )
                    ORDER BY
	                    CreatedAt
                    ;",
                    pCustomer,
                    SettingsApp.XpoOidDocumentFinanceTypeInvoice,
                    SettingsApp.XpoOidDocumentFinanceTypeCreditNote,
                    SettingsApp.XpoOidDocumentFinanceTypeDebitNote
                );

                SelectedData selectedData = GlobalFramework.SessionXpo.ExecuteQuery(sql);

                //Add to result from SelectedData
                foreach (var item in selectedData.ResultSet[0].Rows)
                {
                    result.Add(new Guid(Convert.ToString(item.Values[0])));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static List<Guid> GetValidDocumentsForPayment(Guid pCustomer, Guid pDocumentType)
        {
            List<Guid> result = new List<Guid>();

            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    Oid
                    FROM 
	                    fin_documentfinancemaster 
                    WHERE
	                    (Disabled IS NULL OR Disabled <> 1) AND 
	                    (
		                    DocumentType = '{1}'
	                    ) AND
	                    (
		                    EntityOid = '{0}' AND 
		                    Payed = 0 AND DocumentStatusStatus <> 'A'
	                    )
                    ORDER BY
	                    CreatedAt
                    ;",
                    pCustomer,
                    pDocumentType
                );

                SelectedData selectedData = GlobalFramework.SessionXpo.ExecuteQuery(sql);

                //Add to result from SelectedData
                foreach (var item in selectedData.ResultSet[0].Rows)
                {
                    result.Add(new Guid(Convert.ToString(item.Values[0])));
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //Detect if ArticleBag has a "Recharge Article" and Valid "Customer Card" customer
        //Returns true if is valid, or false if is invalidCustomerCardDetected
        public static bool IsCustomerCardValidForArticleBag(ArticleBag pArticleBag, ERP_Customer pCustomer)
        {
            //Default result is true
            bool result = true;

            try
            {
                FIN_Article article;
                foreach (var item in pArticleBag)
                {
                    //Get Article
                    article = (FIN_Article)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_Article), item.Key.ArticleOid);
                    //Assign Required if ArticleClassCustomerCard Detected
                    if (article.Type.Oid == SettingsApp.XpoOidArticleClassCustomerCard
                        && (
                            pCustomer.Oid == SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity
                            || string.IsNullOrEmpty(pCustomer.CardNumber)
                        )
                    )
                    {
                        //Override Default true Result when Invalid Customer is Detected
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //OrderMain/Document Conferences

        /// <summary>
        /// Get the list of Documents Ready to Pay for all Customers
        /// </summary>
        /// <returns></returns>
        public static string GetQueryFilterForPayableFinanceDocuments()
        {
            return GetQueryFilterForPayableFinanceDocuments(Guid.Empty);
        }

        /// <summary>
        /// Get the list of Documents Ready to Pay for Customer
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns>A Query ready for use in ERP</returns>
        public static string GetQueryFilterForPayableFinanceDocuments(Guid pCustomer)
        {
            string result = string.Empty;

            try
            {
                result = string.Format(
                    "(Disabled IS NULL OR Disabled  <> 1) AND (DocumentType = '{0}' OR DocumentType = '{1}' OR DocumentType = '{2}') AND Payed = 0 AND DocumentStatusStatus <> 'A'"
                    , SettingsApp.XpoOidDocumentFinanceTypeInvoice, SettingsApp.XpoOidDocumentFinanceTypeCreditNote, SettingsApp.XpoOidDocumentFinanceTypeDebitNote
                );
                //Add Customer Filter if Defined
                if (pCustomer != Guid.Empty)
                {
                    result = string.Format("{0} AND EntityOid = '{1}'", result, pCustomer);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //OrderMain/Document Conferences

        //Works in Dual Mode based on pGenerateNewIfDiferentFromArticleBag, sometimes passes 2 times, on for get last document and other to create new one
        //Mode1 : GenerateNewIfDiferentFromArticleBag = false : Returns Last Conference Document for Current Working Order Main
        //Mode2 : GenerateNewIfDiferentFromArticleBag = true : Returns Last or New Conference Document if Current Working Order Main is Diferent from working currente Working OrderMain ArticleBag
        //Returns Last|New DocumentConference, for table current OrderMain
        //GenerateNewIfDiferentFromArticleBag : Used to generate a new Document if latest Document has been changed (Compare it to current ArticleBag), 
        //else if false use the latest on Database ignoring Diferences, Used to Get latest DocumentConference to use in Generate DocumentConference PosOrdersDialog.buttonTableConsult_Clicked
        public static FIN_DocumentFinanceMaster GetOrderMainLastDocumentConference(bool pGenerateNewIfDiferentFromArticleBag = false)
        {
            //Declare local Variables
            FIN_DocumentFinanceMaster lastDocument = null;
            FIN_DocumentFinanceMaster newDocument = null;
            FIN_DocumentFinanceMaster result = null;
            FIN_DocumentOrderMain orderMain = null;
            Guid currentOrderMainOid = GlobalFramework.SessionApp.CurrentOrderMainOid;
            OrderMain currentOrderMain = GlobalFramework.SessionApp.OrdersMain[currentOrderMainOid];

            try
            {
                string sql = string.Format(@"
                    SELECT 
	                    Oid
                    FROM 
	                    FIN_documentfinancemaster 
                    WHERE 
	                    DocumentType = '{0}' AND 
	                    SourceOrderMain = '{1}'
                    ORDER BY 
	                    CreatedAt DESC;
                    "
                    , SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument
                    , currentOrderMain.PersistentOid
                );

                var sqlResult = GlobalFramework.SessionXpo.ExecuteScalar(sql);

                //Get LastDocument Object
                if (sqlResult != null) lastDocument = (FIN_DocumentFinanceMaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), new Guid(Convert.ToString(sqlResult)));

                //If GenerateNewIfDiferentFromArticleBag Enabled compare ArticleBag with Document and If is diferent Generate a New One
                if (pGenerateNewIfDiferentFromArticleBag)
                {
                    //Initialize ArticleBag to Compare with Order Detail and use in ProcessFinanceDocuments
                    ArticleBag articleBag = ArticleBag.TicketOrderToArticleBag(currentOrderMain);
                    //Check if Total is Not Equal and Generate New DocumentConference, This way it will be Equal to Invoice
                    if (
                        lastDocument == null ||
                            (!lastDocument.TotalFinal.Equals(articleBag.TotalFinal) || !lastDocument.DocumentDetail.Count.Equals(articleBag.Count))
                        )
                    {
                        //Prepare ProcessFinanceDocumentParameter
                        ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument, articleBag)
                        {
                            Customer = SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity
                        };

                        orderMain = (FIN_DocumentOrderMain)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentOrderMain), currentOrderMain.PersistentOid);
                        processFinanceDocumentParameter.SourceOrderMain = orderMain;
                        if (lastDocument != null)
                        {
                            processFinanceDocumentParameter.DocumentParent = lastDocument.Oid;
                            processFinanceDocumentParameter.OrderReferences = new List<FIN_DocumentFinanceMaster>();
                            processFinanceDocumentParameter.OrderReferences.Add(lastDocument);
                        }

                        //Generate New Document
                        newDocument = ProcessFinanceDocument.PersistFinanceDocument(processFinanceDocumentParameter, false);

                        //Assign DocumentStatus and OrderReferences
                        if (newDocument != null)
                        {
                            //Assign Result Document to New Document
                            //Get Object outside UOW else we have a problem with "A first chance exception of type 'System.ObjectDisposedException'"
                            result = (FIN_DocumentFinanceMaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), newDocument.Oid);

                            ////Old Code that changes last Conference Document to Status "A", it is not Required, Confirmed with Carlos Bento, we must Leave it without status changes
                            //if (lastDocument != null) 
                            //{
                            //    lastDocument.DocumentStatusStatus = "A";
                            //    lastDocument.DocumentStatusDate = newDocument.DocumentStatusDate;
                            //    lastDocument.DocumentStatusUser = newDocument.DocumentStatusUser;
                            //    lastDocument.SystemEntryDate = newDocument.SystemEntryDate;
                            //    lastDocument.Save();
                            //}
                        }
                    }
                }
                else
                {
                    result = lastDocument;
                }
            }
            catch (Exception ex)
            {
                // Send Exception to logicpos, must treat exception in ui, to Show Alert to User
                throw ex;
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Customer

        public static ERP_Customer GetFinalConsumerEntity()
        {
            ERP_Customer result = null;

            try
            {
                string filterCriteria = string.Format("Oid = '{0}'", SettingsApp.XpoOidDocumentFinanceMasterFinalConsumerEntity.ToString());
                result = (GetXPGuidObjectFromCriteria(typeof(ERP_Customer), filterCriteria) as ERP_Customer);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static bool IsFinalConsumerEntity(string pFiscalNumber)
        {
            bool result = false;

            try
            {
                var entity = GetFinalConsumerEntity();
                result = (entity != null && pFiscalNumber == entity.FiscalNumber);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //Check if customer is Invalid for Simplified Invoices
        public static bool IsInValidFinanceDocumentCustomer(decimal pTotalFinal, string pName, string pAddress, string pZipCode, string pCity, string pCountry, string pFiscalNumber)
        {
            bool result = false;

            try
            {
                if (
                    pTotalFinal > SettingsApp.FinanceRuleRequiredCustomerDetailsAboveValue &&
                    (
                        IsFinalConsumerEntity(pFiscalNumber) ||
                        (
                            string.IsNullOrEmpty(pName) ||
                            string.IsNullOrEmpty(pAddress) ||
                            string.IsNullOrEmpty(pZipCode) ||
                            string.IsNullOrEmpty(pCity) ||
                            string.IsNullOrEmpty(pCountry) ||
                            string.IsNullOrEmpty(pFiscalNumber)
                        )
                    )
                )
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //PreferenceParameters

        //Alias to Reports.CustomFunctions
        public static string GetPreferenceParameter(string pToken)
        {
            return CustomFunctions.Pref(pToken);
        }
    }
}
