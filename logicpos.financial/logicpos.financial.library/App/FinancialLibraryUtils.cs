using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.datalayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Reports;
using logicpos.shared.App;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using LogicPOS.Settings;
using System;
using System.Collections.Generic;

namespace logicpos.financial.library.App
{
    public class FinancialLibraryUtils 
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //FinanceDocument

        //Check if Document has DocumentFinanceType Parent (Recursively)
        public static bool IsDocumentMasterChildOfDocumentType(fin_documentfinancemaster pDocumentFinanceMaster, List<Guid> pDocumentFinanceTypeList)
        {
            bool debug = false;
            bool result = false;

            try
            {
                if (pDocumentFinanceMaster.DocumentParent != null)
                {
                    if (debug) _logger.Debug(string.Format("IsDocumentMasterChildOfDocumentType DocumentParent: [{0}]", pDocumentFinanceMaster.DocumentParent.DocumentNumber));

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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //Check if Document has SaftDocumentType Parent (Recursively)
        public static bool IsDocumentMasterChildOfDocumentType(fin_documentfinancemaster pDocumentFinanceMaster, SaftDocumentType pSaftDocumentType)
        {
            bool debug = false;
            bool result = false;
            List<Guid> documentFinanceTypeList = new List<Guid>();
            CriteriaOperator criteriaOperator = CriteriaOperator.Parse(string.Format("(Disabled = 0 OR Disabled is NULL) AND (SaftDocumentType = {0})", (int)pSaftDocumentType));
            SortingCollection sortingCollection = new SortingCollection
            {
                new SortProperty("Ord", SortingDirection.Ascending)
            };
            XPCollection xpcDocumentFinanceType = XPOHelper.GetXPCollectionFromCriteria(XPOSettings.Session, typeof(fin_documentfinancetype), criteriaOperator, sortingCollection);

            try
            {
                foreach (fin_documentfinancetype item in xpcDocumentFinanceType)
                {
                    if (debug) _logger.Debug(string.Format("Add DocumentFinanceType: [{0}]", item.Designation));
                    documentFinanceTypeList.Add(item.Oid);
                }

                if (documentFinanceTypeList.Count > 0)
                {
                    result = IsDocumentMasterChildOfDocumentType(pDocumentFinanceMaster, documentFinanceTypeList);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                    pDocumentFinanceType == InvoiceSettings.XpoOidDocumentFinanceTypeInvoice ||
                    //pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill ||
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice ||
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                    //SaftDocumentType = 0
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeCurrentAccountInput
                )
                {
                    //Moçambique - Pedidos da reunião 13/10/2020 [IN:014327]
                    //- Fatura simplificada em documentos de origem, para inserir nº contribuinte após emissão de fatura
                    if (CultureSettings.MozambiqueCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                    {      
                        result = new Guid[] {
                        //SaftDocumentType = 2
                        InvoiceSettings.XpoOidDocumentFinanceTypeInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment,
                        DocumentSettings.XpoOidDocumentFinanceTypeDeliveryNote,
                        DocumentSettings.XpoOidDocumentFinanceTypeCurrentAccountInput,
                        DocumentSettings.XpoOidDocumentFinanceTypeTransportationGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide,
                        //SaftDocumentType = 3 
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice,
                        //SaftDocumentType = 0 
                        DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice };
                    }
                    else
                    {
                        result = new Guid[] {
                        //SaftDocumentType = 2
                        DocumentSettings.XpoOidDocumentFinanceTypeDeliveryNote,
                        DocumentSettings.XpoOidDocumentFinanceTypeTransportationGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide,
                        //SaftDocumentType = 3 
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice,
                        //SaftDocumentType = 0 
                        DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice };
                    }
                   
                }
                //CreditNote
                else if (
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeCreditNote
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 1
                        InvoiceSettings.XpoOidDocumentFinanceTypeInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill,
                        DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment
                    };
                }
                //MovementOfGoods,WorkingDocuments
                else if (
                    //SaftDocumentType = 2
                    // pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeDeliveryNote || /* IN009175 */
                    // pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeTransportationGuide || /* IN009175 */
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide ||
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide ||
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide ||
                    //SaftDocumentType = 3
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument ||
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 1
                        InvoiceSettings.XpoOidDocumentFinanceTypeInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment,
                        //SaftDocumentType = 3 
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        //SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice,
                        //SaftDocumentType = 0
                        DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice
                    };
                }
                /* IN009175 - Transport Documents ("Guia de Transporte" and "Guia de Remessa") */
                else if (
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeTransportationGuide ||
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeDeliveryNote
                    )
                { /* #TODO check this list and all others here */
                    result = new Guid[] { 
                        InvoiceSettings.XpoOidDocumentFinanceTypeInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment,
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice
                    };
                }
                //DocumentFinanceTypeInvoiceWayBill
                else if (
                    pDocumentFinanceType == DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 3
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice,
                        DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice
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
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Get a Collection of UnCredited DocumentFinance Detail Items , ex Detail Lines that dont have been already Credited from a Invoice Type Document
        /// Method specific to CreditNotes
        /// </summary>
        public static XPCollection<fin_documentfinancedetail> GetUnCreditedItemsFromSourceDocument(fin_documentfinancemaster pSourceDocument, out string pCreditedDocuments)
        {
            XPCollection<fin_documentfinancedetail> result = new XPCollection<fin_documentfinancedetail>(pSourceDocument.Session, false);
            CriteriaOperator criteria = CriteriaOperator.Parse(string.Format("DocumentMaster = '{0}' AND Disabled = 'False'", pSourceDocument.Oid));
            XPCollection xpoCollectionReferences = new XPCollection(pSourceDocument.Session, typeof(fin_documentfinancedetailreference), criteria);
            List<string> listCreditedDocuments = new List<string>();
            pCreditedDocuments = string.Empty;

            try
            {
                //Loop SourceDocument Details
                foreach (fin_documentfinancedetail itemSource in pSourceDocument.DocumentDetail)
                {
                    //Reset
                    bool addToCollection = true;

                    //Force Reload : This Prevent cached Substracts 
                    itemSource.Reload();

                    //Loop SourceDocument Details
                    foreach (fin_documentfinancedetailreference itemReferences in xpoCollectionReferences)
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
                            /* IN009235 - Begin */
                            decimal itemDiscountValue = (itemReferences.DocumentDetail.TotalGross * itemReferences.DocumentDetail.Discount / 100);
                            decimal itemTotalNetValue = itemReferences.DocumentDetail.TotalGross - itemDiscountValue;
                            decimal itemTaxValue = (itemTotalNetValue * itemReferences.DocumentDetail.Vat / 100);
                            decimal itemTotalFinalValue = itemTotalNetValue + itemTaxValue;

                            decimal itemOldDiscountValue = (itemSource.TotalGross * itemSource.Discount / 100);
                            decimal itemOldTotalNetValue = itemSource.TotalGross - itemOldDiscountValue;
                            decimal itemOldTaxValue = (itemOldTotalNetValue * itemSource.Vat / 100);
                            decimal itemOldTotalFinalValue = itemOldTotalNetValue + itemOldTaxValue;

                            // Substract Credited Quantity from itemSource
                            itemSource.Quantity -= itemReferences.DocumentDetail.Quantity;
                            itemSource.TotalGross -= itemReferences.DocumentDetail.TotalGross;
                            itemSource.TotalNet = itemOldTotalNetValue - itemTotalNetValue;
                            itemSource.TotalFinal = itemOldTotalFinalValue - itemTotalFinalValue;
							/* IN009235 - End */

                            // Debug Helper
                            //_logger.Debug(string.Format("DocumentNumber: [{0}], Designation: [{1}], Quantity: [{2}], itemReferences.Quantity: [{3}]",
                            //    pSourceDocument.DocumentNumber, itemSource.Designation, itemSource.Quantity, itemReferences.DocumentDetail.Quantity)
                            //    );

                            // Add Document to listCreditedDocuments, this list will be shown in showMessage when all articles are credited for sourceDocument
                            if (!listCreditedDocuments.Contains(itemReferences.DocumentDetail.DocumentMaster.DocumentNumber)) {
                                listCreditedDocuments.Add(itemReferences.DocumentDetail.DocumentMaster.DocumentNumber);
                            }
                        }
                    }
                    //Add Line to result Collection if has Quantity Greater than Zero
                    if (addToCollection && itemSource.Quantity > 0) result.Add(itemSource);
                }

                //Generate CreditedDocuments outs Result, this will show NC for Current Source Document, used in ShowMessage
                listCreditedDocuments.Sort();
                if (result.Count == 0 && listCreditedDocuments.Count > 0)
                {
                    for (int i = 0; i < listCreditedDocuments.Count; i++)
                    {
                        // Out variable, to be used outside of this method
                        pCreditedDocuments += string.Format("{0} - {1}", Environment.NewLine, listCreditedDocuments[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        /// <summary>
        /// Method to check if Credit Note ArticleBag is Valid
        /// </summary>
        public static bool GetCreditNoteValidation(fin_documentfinancemaster pDocumentParent, ArticleBag pArticleBag)
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
                        resultParentDocument = XPOSettings.Session.ExecuteScalar(sql);
                        totalParentDocument = (resultParentDocument != null) ? Convert.ToDecimal(resultParentDocument) : 0.0m;

                        sql = string.Format("SELECT SUM(fdQuantity) AS Total FROM view_documentfinance WHERE ftOid = '{0}' AND fmDocumentParent = '{1}' AND fdArticle = '{2}';", DocumentSettings.XpoOidDocumentFinanceTypeCreditNote, pDocumentParent.Oid, item.Key.ArticleOid);
                        resultAlreadyCredited = XPOSettings.Session.ExecuteScalar(sql);
                        totalAlreadyCredited = (resultAlreadyCredited != null) ? Convert.ToDecimal(resultAlreadyCredited) : 0.0m;

                        if (debug) _logger.Debug(string.Format(
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
                _logger.Error(ex.Message, ex);
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
                    InvoiceSettings.XpoOidDocumentFinanceTypeInvoice,
                    DocumentSettings.XpoOidDocumentFinanceTypeCreditNote,
                    DocumentSettings.XpoOidDocumentFinanceTypeDebitNote
                );

                SelectedData selectedData = XPOSettings.Session.ExecuteQuery(sql);

                //Add to result from SelectedData
                foreach (var item in selectedData.ResultSet[0].Rows)
                {
                    result.Add(new Guid(Convert.ToString(item.Values[0])));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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

                SelectedData selectedData = XPOSettings.Session.ExecuteQuery(sql);

                //Add to result from SelectedData
                foreach (var item in selectedData.ResultSet[0].Rows)
                {
                    result.Add(new Guid(Convert.ToString(item.Values[0])));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //Detect if ArticleBag has a "Recharge Article" and Valid "Customer Card" customer
        //Returns true if is valid, or false if is invalidCustomerCardDetected
        public static bool IsCustomerCardValidForArticleBag(ArticleBag pArticleBag, erp_customer pCustomer)
        {
            //Default result is true
            bool result = true;

            try
            {
                fin_article article;
                foreach (var item in pArticleBag)
                {
                    //Get Article
                    article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), item.Key.ArticleOid);
                    //Assign Required if ArticleClassCustomerCard Detected
                    if (article.Type.Oid == XPOSettings.XpoOidArticleClassCustomerCard
                        && (
                            pCustomer.Oid == InvoiceSettings.FinalConsumerId
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
                _logger.Error(ex.Message, ex);
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
                    , InvoiceSettings.XpoOidDocumentFinanceTypeInvoice, DocumentSettings.XpoOidDocumentFinanceTypeCreditNote, DocumentSettings.XpoOidDocumentFinanceTypeDebitNote
                );
                //Add Customer Filter if Defined
                if (pCustomer != Guid.Empty)
                {
                    result = string.Format("{0} AND EntityOid = '{1}'", result, pCustomer);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
        public static fin_documentfinancemaster GetOrderMainLastDocumentConference(bool pGenerateNewIfDiferentFromArticleBag = false)
        {
            //Declare local Variables
            fin_documentfinancemaster lastDocument = null;
            fin_documentfinancemaster result = null;
            Guid currentOrderMainOid = SharedFramework.SessionApp.CurrentOrderMainOid;
            _logger.Debug("fin_documentfinancemaster GetOrderMainLastDocumentConference(bool pGenerateNewIfDiferentFromArticleBag = false) :: currentOrderMainOid: " + currentOrderMainOid);
            OrderMain currentOrderMain = null;
            
            try
            {
                /* IN009179 - System.Collections.Generic.KeyNotFoundException */
                if (SharedFramework.SessionApp.OrdersMain.Count > 0)
                {
                    currentOrderMain = SharedFramework.SessionApp.OrdersMain[currentOrderMainOid];
                }
                else
                {
                    currentOrderMain = new OrderMain();
                }

                string sql = string.Format(@"
                    SELECT 
	                    Oid
                    FROM 
	                    fin_documentfinancemaster 
                    WHERE 
	                    DocumentType = '{0}' AND 
	                    SourceOrderMain = '{1}'
                    ORDER BY 
	                    CreatedAt DESC;
                    "
                    , DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument
                    , currentOrderMain.PersistentOid
                );

                var sqlResult = XPOSettings.Session.ExecuteScalar(sql);

                //Get LastDocument Object
                if (sqlResult != null) lastDocument = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), new Guid(Convert.ToString(sqlResult)));

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
                        ProcessFinanceDocumentParameter processFinanceDocumentParameter = new ProcessFinanceDocumentParameter(DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument, articleBag)
                        {
                            Customer = InvoiceSettings.FinalConsumerId
                        };

                        fin_documentordermain orderMain = (fin_documentordermain)XPOSettings.Session.GetObjectByKey(typeof(fin_documentordermain), currentOrderMain.PersistentOid);
                        processFinanceDocumentParameter.SourceOrderMain = orderMain;
                        if (lastDocument != null)
                        {
                            processFinanceDocumentParameter.DocumentParent = lastDocument.Oid;
                            processFinanceDocumentParameter.OrderReferences = new List<fin_documentfinancemaster>
                            {
                                lastDocument
                            };
                        }

                        //Generate New Document
                        fin_documentfinancemaster newDocument = ProcessFinanceDocument.PersistFinanceDocument(processFinanceDocumentParameter, false);

                        //Assign DocumentStatus and OrderReferences
                        if (newDocument != null)
                        {
                            //Assign Result Document to New Document
                            //Get Object outside UOW else we have a problem with "A first chance exception of type 'System.ObjectDisposedException'"
                            result = (fin_documentfinancemaster)XPOSettings.Session.GetObjectByKey(typeof(fin_documentfinancemaster), newDocument.Oid);

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
                _logger.Error("fin_documentfinancemaster GetOrderMainLastDocumentConference(bool pGenerateNewIfDiferentFromArticleBag = false) :: currentOrderMain: " + currentOrderMain);
                // Send Exception to logicpos, must treat exception in ui, to Show Alert to User
                throw ex;
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Customer

        public static erp_customer GetFinalConsumerEntity()
        {
            erp_customer result = null;

            try
            {
                string filterCriteria = string.Format("Oid = '{0}'", InvoiceSettings.FinalConsumerId.ToString());
                result = (XPOHelper.GetXPGuidObjectFromCriteria(typeof(erp_customer), filterCriteria) as erp_customer);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
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
                _logger.Error(ex.Message, ex);
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
                    pTotalFinal > GeneralSettings.GetRequiredCustomerDetailsAboveValue(XPOSettings.ConfigurationSystemCountry.Oid) &&
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
                _logger.Error(ex.Message, ex);
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

        public static bool HasWritePermissionOnDir(string path)
        {
            var writeAllow = false;
            var writeDeny = false;
            var accessControlList = System.IO.Directory.GetAccessControl(path);
            if (accessControlList == null)
                return false;
            var accessRules = accessControlList.GetAccessRules(true, true,
                                        typeof(System.Security.Principal.SecurityIdentifier));
            if (accessRules == null)
                return false;

            foreach (System.Security.AccessControl.FileSystemAccessRule rule in accessRules)
            {
                if ((System.Security.AccessControl.FileSystemRights.Write & rule.FileSystemRights) != System.Security.AccessControl.FileSystemRights.Write)
                    continue;

                if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Allow)
                    writeAllow = true;
                else if (rule.AccessControlType == System.Security.AccessControl.AccessControlType.Deny)
                    writeDeny = true;
            }

            return writeAllow && !writeDeny;
        }
    }
}
