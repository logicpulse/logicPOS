using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using logicpos.shared.Enums;
using LogicPOS.Data.Services;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Modules;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Settings;
using LogicPOS.Settings.Enums;
using LogicPOS.Shared;
using LogicPOS.Shared.Article;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Shared.Orders;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.Finance.DocumentProcessing
{
    public class DocumentProcessingUtils
    {
        //Log4Net
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static fin_documentfinancemaster PersistFinanceDocument(DocumentProcessingParameters pParameters, bool pIgnoreWarning = false)
        {
            Guid userDetailGuid = (XPOSettings.LoggedUser != null) ? XPOSettings.LoggedUser.Oid : Guid.Empty;
            Guid terminalGuid = (TerminalSettings.LoggedTerminal != null) ? TerminalSettings.LoggedTerminal.Oid : Guid.Empty;

            return (PersistFinanceDocument(pParameters, userDetailGuid, terminalGuid, pIgnoreWarning));
        }

        public static fin_documentfinancemaster PersistFinanceDocument(DocumentProcessingParameters pParameters, Guid pLoggedUser, Guid pTerminal, bool pIgnoreWarning = false)
        {
            fin_documentfinancemaster result = null;

            try
            {
                //Proccess Validation
                SortedDictionary<DocumentValidationErrorType, object> errorsValidation = DocumentProcessingValidationUtils.ValidatePersistFinanceDocument(pParameters, pLoggedUser, pIgnoreWarning);
                if (errorsValidation.Count > 0)
                {
                    string errors = string.Empty;
                    foreach (var item in errorsValidation)
                    {
                        errors += string.Format("{0}- {1}", Environment.NewLine, item.Key);
                    }

                    DocumentProcessingValidationException exception = new DocumentProcessingValidationException(new Exception(string.Format("ERROR_DETECTED{0}{1}", Environment.NewLine, errors)), errorsValidation);
                    //Throw without Errors only Exception for Muga Work, Return a simple String
                    //throw exception.Exception;
                    // Send with ExceptionErrors
                    throw exception;
                }

                //Settings
                //string dateTimeFormatDocumentDate = (LogicPOS.Settings.PluginSettings.HasPlugin) ? LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.GetDateTimeFormatDocumentDate() : null;//SettingsApp.DateTimeFormatDocumentDate;
                //string dateTimeFormatCombinedDateTime = (LogicPOS.Settings.PluginSettings.HasPlugin) ? LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.GetDateTimeFormatCombinedDateTime() : null;//SettingsApp.DateTimeFormatCombinedDateTime;

                //If has DocumentDateTime from Parameters use it, else use Current Atomic DateTime : This is Optional, Now DocumentDateTime is assigned on Parameter Constructor
                DateTime documentDateTime = (pParameters.DocumentDateTime != DateTime.MinValue) ? pParameters.DocumentDateTime : XPOUtility.CurrentDateTimeAtomic();
                //Init Local Vars
                OrderMain orderMain = null;

                //Start UnitOfWork
                using (UnitOfWork uowSession = new UnitOfWork())
                {
                    //Get Objects, To Prevent XPO Stress of Deleted Objects inside UOW
                    //WorkSessionPeriod workSessionPeriod = (WorkSessionPeriod)XPOUtility.GetXPGuidObjectFromSession(uowSession, typeof(WorkSessionPeriod), GlobalFramework.WorkSessionPeriodTerminal.Oid);
                    sys_userdetail userDetail = (sys_userdetail)XPOUtility.GetXPGuidObject(uowSession, typeof(sys_userdetail), pLoggedUser);
                    pos_configurationplaceterminal terminal = (pos_configurationplaceterminal)XPOUtility.GetXPGuidObject(uowSession, typeof(pos_configurationplaceterminal), pTerminal);

                    //Prepare Modes
                    fin_documentordermain documentOrderMain = null;
                    if (pParameters.SourceMode == PersistFinanceDocumentSourceMode.CurrentOrderMain)
                    {
                        orderMain = POSSession.CurrentSession.OrderMains[POSSession.CurrentSession.CurrentOrderMainId];
                        //Get Persistent Oid from orderMain
                        documentOrderMain = (fin_documentordermain)uowSession.GetObjectByKey(typeof(fin_documentordermain), orderMain.PersistentOid);
                    }
                    else
                    {
                        if (pParameters.SourceOrderMain != null)
                        {
                            documentOrderMain = (fin_documentordermain)uowSession.GetObjectByKey(typeof(fin_documentordermain), pParameters.SourceOrderMain.Oid);
                        }
                    }

                    //Initialize DocumentFinance* Objects 
                    fin_documentfinancetype documentFinanceType = (fin_documentfinancetype)uowSession.GetObjectByKey(typeof(fin_documentfinancetype), pParameters.DocumentType);
                    fin_documentfinanceseries documentFinanceSerie = null;
                    fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = null;

                    //Get Document Serie for current Terminal
                    documentFinanceYearSerieTerminal = DocumentProcessingSeriesUtils.GetDocumentFinanceYearSerieTerminal(uowSession, documentFinanceType.Oid);
                    if (documentFinanceYearSerieTerminal != null)
                    {
                        documentFinanceSerie = documentFinanceYearSerieTerminal.Serie;
                    }

                    //If has Invalid Session Show Message and Return
                    if (documentFinanceSerie == null)
                    {
                        uowSession.RollbackTransaction();
                        throw new Exception("ERROR_MISSING_SERIE");
                    }

                    //Get Document Number
                    string documentNumber = GenDocumentNumber(documentFinanceSerie);
                    //Check if Valid DocumentNumber
                    if (documentNumber == "INVALID_DOCUMENT_NUMBER")
                    {
                        uowSession.RollbackTransaction();
                        throw new Exception("ERROR_INVALID_DOCUMENT_NUMBER");
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //DocumentFinanceMaster

                    //Increase NextDocumentNumber
                    documentFinanceSerie.NextDocumentNumber++;

                    //Start Persist Objects
                    fin_documentfinancemaster documentFinanceMaster = new fin_documentfinancemaster(uowSession)
                    {
                        DocumentType = documentFinanceType,
                        DocumentSerie = documentFinanceSerie,
                        Payed = documentFinanceType.Payed,
                        TotalDelivery = pParameters.TotalDelivery,
                        TotalChange = pParameters.TotalChange
                    };
                    if (documentFinanceType.Payed) documentFinanceMaster.PayedDate = documentDateTime;

                    //SourceOrderMain
                    if (documentOrderMain != null)
                    {
                        documentFinanceMaster.SourceOrderMain = documentOrderMain;
                    }

                    //DocumentParent/SourceDocumentID(SAF-T PT)
                    fin_documentfinancemaster documentFinanceMasterParentDocument = null;
                    if (pParameters.DocumentParent != null && pParameters.DocumentParent != Guid.Empty)
                    {
                        documentFinanceMasterParentDocument = (fin_documentfinancemaster)uowSession.GetObjectByKey(typeof(fin_documentfinancemaster), pParameters.DocumentParent);
                        documentFinanceMaster.DocumentParent = documentFinanceMasterParentDocument;

                        if (documentFinanceMasterParentDocument != null)
                        {
                            //Source Document Document Status: SourceDocument was Invoiced|InvoiceAndPayment|SimplifiedInvoice > change SourceDocument Status to F (Invoiced/Faturado)
                            if (
                                documentFinanceType.Oid == InvoiceSettings.InvoiceId ||
                                documentFinanceType.Oid == DocumentSettings.InvoiceAndPaymentId ||
                                documentFinanceType.Oid == DocumentSettings.SimplifiedInvoiceId
                            )
                            {
                                documentFinanceMasterParentDocument.DocumentStatusStatus = "F";
                            }

                            //Detected in Certification : Credit Notes dont Change Status Details like other Documents
                            if (documentFinanceType.Oid != CustomDocumentSettings.CreditNoteId)
                            {
                                //Assign Date and User for all Other
                                documentFinanceMasterParentDocument.DocumentStatusDate = documentDateTime.ToString(CultureSettings.DateTimeFormatCombinedDateTime);
                                documentFinanceMasterParentDocument.DocumentStatusUser = userDetail.CodeInternal;
                            }
                            //_logger.Debug(String.Format("DocumentNumber: [{0}], DocumentStatusStatus: [{1}], DocumentStatusDate: [{2}], DocumentStatusUser: [{3}]", pParameters.OrderReferences[0].DocumentNumber, pParameters.OrderReferences[0].DocumentStatusStatus, pParameters.OrderReferences[0].DocumentStatusDate, pParameters.OrderReferences[0].DocumentStatusUser));
                        }
                    }

                    //If Has a Valid Customer
                    erp_customer customer = (erp_customer)uowSession.GetObjectByKey(typeof(erp_customer), pParameters.Customer);
                    if (customer != null)
                    {
                        documentFinanceMaster.EntityOid = customer.Oid;
                        //Store CodeInternal to use in SAF-T
                        documentFinanceMaster.EntityInternalCode = customer.CodeInternal;
                        documentFinanceMaster.EntityFiscalNumber = PluginSettings.SoftwareVendor.Encrypt(customer.FiscalNumber); /* IN009075 */
                        //Always Update EntityCountryOid, usefull to AT WebServices to detect Country
                        if (customer.Country != null)
                        {
                            documentFinanceMaster.EntityCountry = customer.Country.Code2;
                            documentFinanceMaster.EntityCountryOid = customer.Country.Oid;
                        }

                        //Only persist Customer Details If is (! Simplified Invoice) AND (! FinalConsumer (in Invoices for ex))
                        if (
                            (
                                // documentFinanceMaster.DocumentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice && /* IN009076 - FS is not saving customer data */
                                customer.Oid != XPOUtility.GetFinalConsumerEntity().Oid
                            )
                            ||
                            //Or If not in Portugal AND (! FinalConsumer (in Invoices for ex))
                            (
                                CultureSettings.PortugalCountryId != XPOSettings.ConfigurationSystemCountry.Oid &&
                                customer.Oid != XPOUtility.GetFinalConsumerEntity().Oid
                            )
                            //Required Oids for Equallity Check
                            //Commented to save details if is a Hidden Customer, Diferent from FinalConsumerEntity
                            //&& !FrameworkUtils.IsFinalConsumerEntity(customer.FiscalNumber)
                            )
                        {
                            /* IN009075 - encrypting customer datum when persisting finance document */
                            documentFinanceMaster.EntityName = PluginSettings.SoftwareVendor.Encrypt(customer.Name);
                            documentFinanceMaster.EntityAddress = PluginSettings.SoftwareVendor.Encrypt(customer.Address);
                            documentFinanceMaster.EntityLocality = PluginSettings.SoftwareVendor.Encrypt(customer.Locality);
                            documentFinanceMaster.EntityZipCode = PluginSettings.SoftwareVendor.Encrypt(customer.ZipCode);
                            documentFinanceMaster.EntityCity = PluginSettings.SoftwareVendor.Encrypt(customer.City);
                            //Deprecated Now Always assign Country, usefull to AT WebServices to detect Country 
                            //if (customer.Country != null)
                            //{
                            //    documentFinanceMaster.EntityCountry = customer.Country.Code2;
                            //    documentFinanceMaster.EntityCountryOid = customer.Country.Oid;
                            //}
                        }
                        //Persist Name if is has a FinalConsumer NIF and Name (Hidden Customer)
                        else if (XPOUtility.IsFinalConsumerEntity(customer.FiscalNumber) && customer.Name != string.Empty)
                        {
                            documentFinanceMaster.EntityName = PluginSettings.SoftwareVendor.Encrypt(customer.Name); /* IN009075 */
                        }
                    }

                    //Currency
                    cfg_configurationcurrency configurationCurrency = null;
                    if (pParameters.Currency != null && pParameters.Currency != new Guid()) configurationCurrency = (cfg_configurationcurrency)uowSession.GetObjectByKey(typeof(cfg_configurationcurrency), pParameters.Currency);
                    //Assign Currency to Document
                    if (configurationCurrency != null) documentFinanceMaster.Currency = configurationCurrency;
                    //ExchangeRate
                    documentFinanceMaster.ExchangeRate = pParameters.ExchangeRate;

                    //Currency Congo K
                    if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "fr-CF")
                    {
                        documentFinanceMaster.Currency = (cfg_configurationcurrency)uowSession.GetObjectByKey(typeof(cfg_configurationcurrency), Guid.Parse("c96b971b-5d6a-40dd-be1a-8d1ef408a25c"));
                        //ExchangeRate
                        documentFinanceMaster.ExchangeRate = documentFinanceMaster.Currency.ExchangeRate;
                    }

                    //PaymentMethod
                    fin_configurationpaymentmethod paymentMethod = null;
                    if (pParameters.PaymentMethod != new Guid())
                    {
                        paymentMethod = (fin_configurationpaymentmethod)uowSession.GetObjectByKey(typeof(fin_configurationpaymentmethod), pParameters.PaymentMethod);
                    }
                    //Assign Only it was not Null
                    // Moçambique - Pedidos da reunião 13/10/2020 + Faturas no Front-Office [IN:014327]
                    if (paymentMethod != null && paymentMethod.Oid != InvoiceSettings.XpoOidConfigurationPaymentMethodCurrentAccount)
                    {
                        documentFinanceMaster.PaymentMethod = paymentMethod;
                    }

                    //PaymentMethod
                    fin_configurationpaymentcondition paymentCondition = null;
                    if (pParameters.PaymentCondition != new Guid())
                    {
                        paymentCondition = (fin_configurationpaymentcondition)uowSession.GetObjectByKey(typeof(fin_configurationpaymentcondition), pParameters.PaymentCondition);
                    }
                    if (paymentCondition != null)
                    {
                        documentFinanceMaster.PaymentCondition = paymentCondition;
                    }

                    // UserDetail
                    if (userDetail != null && userDetail.Oid != new Guid())
                    {
                        documentFinanceMaster.CreatedBy = userDetail;
                    }

                    // Terminal
                    if (terminal != null && terminal.Oid != new Guid())
                    {
                        documentFinanceMaster.CreatedWhere = terminal;
                    }

                    //Assign Document Status for FT | FS | FR | DC | NC | CC | WorkingDocuments, else Ignore DocumentStatusStatus
                    if (
                        documentFinanceType.Oid == InvoiceSettings.InvoiceId ||
                        documentFinanceType.Oid == DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill ||
                        documentFinanceType.Oid == DocumentSettings.SimplifiedInvoiceId ||
                        documentFinanceType.Oid == DocumentSettings.InvoiceAndPaymentId ||
                        documentFinanceType.Oid == CustomDocumentSettings.CreditNoteId ||
                        documentFinanceType.Oid == DocumentSettings.XpoOidDocumentFinanceTypeBudget ||
                        documentFinanceType.Oid == DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice ||
                        documentFinanceType.Oid == DocumentSettings.CurrentAccountInputId ||
                        documentFinanceType.SaftDocumentType == SaftDocumentType.WorkingDocuments ||
                        documentFinanceType.SaftDocumentType == SaftDocumentType.MovementOfGoods
                    )
                    {
                        documentFinanceMaster.DocumentStatusStatus = "N";
                    }
                    //Always assign DocumentStatusDate
                    documentFinanceMaster.DocumentStatusDate = documentDateTime.ToString(CultureSettings.DateTimeFormatCombinedDateTime);

                    //Notes
                    if (pParameters.Notes != string.Empty)
                    {
                        documentFinanceMaster.Notes = pParameters.Notes;
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //WayBill : MovementOfGoods

                    //ShipTo
                    if (pParameters.ShipTo != null)
                    {
                        if (pParameters.ShipTo.DeliveryID != null) { documentFinanceMaster.ShipToDeliveryID = pParameters.ShipTo.DeliveryID; };
                        if (pParameters.ShipTo.DeliveryDate != null)
                        {
                            documentFinanceMaster.ShipToDeliveryDate = pParameters.ShipTo.DeliveryDate;
                            documentFinanceMaster.MovementEndTime = pParameters.ShipTo.DeliveryDate;
                        };
                        if (pParameters.ShipTo.WarehouseID != null) { documentFinanceMaster.ShipToWarehouseID = pParameters.ShipTo.WarehouseID; };
                        if (pParameters.ShipTo.LocationID != null) { documentFinanceMaster.ShipToLocationID = pParameters.ShipTo.LocationID; };
                        if (pParameters.ShipTo.BuildingNumber != null) { documentFinanceMaster.ShipToBuildingNumber = pParameters.ShipTo.BuildingNumber; };
                        if (pParameters.ShipTo.StreetName != null) { documentFinanceMaster.ShipToStreetName = pParameters.ShipTo.StreetName; };
                        if (pParameters.ShipTo.AddressDetail != null) { documentFinanceMaster.ShipToAddressDetail = pParameters.ShipTo.AddressDetail; };
                        if (pParameters.ShipTo.PostalCode != null) { documentFinanceMaster.ShipToPostalCode = pParameters.ShipTo.PostalCode; };
                        if (pParameters.ShipTo.City != null) { documentFinanceMaster.ShipToCity = pParameters.ShipTo.City; };
                        if (pParameters.ShipTo.Region != null) { documentFinanceMaster.ShipToRegion = pParameters.ShipTo.Region; };
                        if (pParameters.ShipTo.Country != null) { documentFinanceMaster.ShipToCountry = pParameters.ShipTo.Country; };
                    }
                    //ShipFrom
                    if (pParameters.ShipFrom != null)
                    {
                        if (pParameters.ShipFrom.DeliveryID != null) { documentFinanceMaster.ShipFromDeliveryID = pParameters.ShipFrom.DeliveryID; };
                        if (pParameters.ShipFrom.DeliveryDate != null)
                        {
                            documentFinanceMaster.ShipFromDeliveryDate = pParameters.ShipFrom.DeliveryDate;
                            documentFinanceMaster.MovementStartTime = pParameters.ShipFrom.DeliveryDate;
                        };
                        if (pParameters.ShipFrom.WarehouseID != null) { documentFinanceMaster.ShipFromWarehouseID = pParameters.ShipFrom.WarehouseID; };
                        if (pParameters.ShipFrom.LocationID != null) { documentFinanceMaster.ShipFromLocationID = pParameters.ShipFrom.LocationID; };
                        if (pParameters.ShipFrom.BuildingNumber != null) { documentFinanceMaster.ShipFromBuildingNumber = pParameters.ShipFrom.BuildingNumber; };
                        if (pParameters.ShipFrom.StreetName != null) { documentFinanceMaster.ShipFromStreetName = pParameters.ShipFrom.StreetName; };
                        if (pParameters.ShipFrom.AddressDetail != null) { documentFinanceMaster.ShipFromAddressDetail = pParameters.ShipFrom.AddressDetail; };
                        if (pParameters.ShipFrom.PostalCode != null) { documentFinanceMaster.ShipFromPostalCode = pParameters.ShipFrom.PostalCode; };
                        if (pParameters.ShipFrom.City != null) { documentFinanceMaster.ShipFromCity = pParameters.ShipFrom.City; };
                        if (pParameters.ShipFrom.Region != null) { documentFinanceMaster.ShipFromRegion = pParameters.ShipFrom.Region; };
                        if (pParameters.ShipFrom.Country != null) { documentFinanceMaster.ShipFromCountry = pParameters.ShipFrom.Country; };
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //DocumentFinanceDetail

                    //Cant Work Here, Because in UnitOfWork we cant get the "last inserted oid..."
                    fin_documentfinancedetail documentFinanceDetail;
                    Guid vatRateOid;
                    fin_configurationvatrate vatRate;
                    fin_configurationvatexemptionreason vatExemptionReason;
                    fin_article article;
                    uint lineNumber = 1;

                    foreach (var item in pParameters.ArticleBag)
                    {
                        if (item.Value.Quantity > 0)
                        {
                            //Get Article
                            article = (fin_article)uowSession.GetObjectByKey(typeof(fin_article), item.Key.ArticleId);
                            //Get VatRate formated for filter, in sql server gives error without this it filters 23,0000 and not 23.0000 resulting in null vatRate
                            string filterVat = DataConversionUtils.DecimalToString(item.Key.Vat);
                            string executeSql = string.Format(@"SELECT Oid FROM fin_configurationvatrate WHERE Value = '{0}';", filterVat);
                            vatRateOid = XPOUtility.GetGuidFromQuery(executeSql);
                            vatRate = (fin_configurationvatrate)uowSession.GetObjectByKey(typeof(fin_configurationvatrate), vatRateOid);

                            //If Type dont Have Price it Creates an Empty Details document, always use HavePrice, Only Type : Informative Created by Muga dont use HavePrice
                            if (article.Type.HavePrice)
                            {
                                documentFinanceDetail = new fin_documentfinancedetail(uowSession)
                                {
                                    Ord = lineNumber,
                                    Code = item.Value.Code,
                                    Designation = item.Key.Designation,
                                    Quantity = item.Value.Quantity,
                                    UnitMeasure = item.Value.UnitMeasure,
                                    Price = item.Key.Price,
                                    Vat = item.Key.Vat,
                                    Discount = item.Key.Discount,
                                    TotalNet = item.Value.TotalNet,
                                    TotalGross = item.Value.TotalGross,
                                    TotalDiscount = item.Value.TotalDiscount,
                                    TotalTax = item.Value.TotalTax,
                                    TotalFinal = item.Value.TotalFinal,
                                    DocumentMaster = documentFinanceMaster,
                                    Article = article,
                                    PriceFinal = item.Value.PriceFinal,
                                    PriceType = item.Value.PriceType,
                                    SerialNumber = item.Value.SerialNumber,
                                    Warehouse = item.Value.Warehouse,
                                    CreatedBy = (userDetail != null && userDetail.Oid != Guid.Empty) ? userDetail : null,
                                    CreatedWhere = (terminal != null && terminal.Oid != Guid.Empty) ? terminal : null
                                };

                                if (vatRate != null) documentFinanceDetail.VatRate = vatRate;

                                //Assign VatExemptionReason if Set, else Leave Null
                                if (item.Key.VatExemptionReasonOid != new Guid())
                                {
                                    //Get Fresh Object to Prevent Mixing Sessions
                                    vatExemptionReason = (fin_configurationvatexemptionreason)uowSession.GetObjectByKey(typeof(fin_configurationvatexemptionreason), item.Key.VatExemptionReasonOid);
                                    documentFinanceDetail.VatExemptionReason = vatExemptionReason;
                                    documentFinanceDetail.VatExemptionReasonDesignation = vatExemptionReason.Designation;
                                }

                                //Notes
                                if (item.Value.Notes != null)
                                {
                                    documentFinanceDetail.Notes = item.Value.Notes;
                                }

                                // SerialNumber
                                if (!string.IsNullOrEmpty(item.Value.SerialNumber))
                                {
                                    documentFinanceDetail.SerialNumber = item.Value.SerialNumber;
                                }

                                // Warehouse
                                if (!string.IsNullOrEmpty(item.Value.SerialNumber))
                                {
                                    documentFinanceDetail.SerialNumber = item.Value.SerialNumber;
                                }

                                //Order References
                                //(4.1|2|3.3.20.2 Referência ao documento de origem)
                                if (pParameters.OrderReferences != null && pParameters.OrderReferences.Count > 0)
                                {
                                    uint ord = 0;
                                    foreach (fin_documentfinancemaster documentMaster in pParameters.OrderReferences)
                                    {
                                        ord++;
                                        //Require Fresh Objects to Prevent Xpo Mixing Sessions
                                        documentFinanceDetail.OrderReferences.Add(
                                          new fin_documentfinancedetailorderreference(uowSession)
                                          {
                                              Ord = ord,
                                              DocumentMaster = (fin_documentfinancemaster)uowSession.GetObjectByKey(typeof(fin_documentfinancemaster), documentMaster.Oid),
                                              OriginatingON = documentMaster.DocumentNumber,
                                              OrderDate = documentMaster.Date
                                          }
                                        );
                                    }
                                }

                                //References: From ArticleBag 
                                //(4.1.4.18.9 Referências a faturas nos documentos retificativos destas.)
                                if (item.Value.Reference != null)
                                {
                                    documentFinanceDetail.References.Add(
                                        new fin_documentfinancedetailreference(uowSession)
                                        {
                                            Ord = 0,
                                            DocumentMaster = (fin_documentfinancemaster)uowSession.GetObjectByKey(typeof(fin_documentfinancemaster), item.Value.Reference.Oid),
                                            Reference = item.Value.Reference.DocumentNumber,
                                            Reason = item.Value.Reason
                                        }
                                    );
                                }

                                /* IN009252 - This represents one of article Reason value. 
                                 * Because we have the same value included on Document when 
                                 * issuing it for its whole article list, therefore overwritting */
                                if (documentFinanceDetail.References.Count > 0)
                                {
                                    //Bug nas notas de credito para varios artigos ( "Motivo" repetido)
                                    documentFinanceMaster.Notes = documentFinanceDetail.References[0].Reason;
                                }

                                //Inc lineNumber for Ord
                                lineNumber++;

                                //Commissions Work
                                PersistFinanceDocumentCommission(uowSession, documentFinanceDetail, userDetail);
                            }

                            // TK013134
                            if (GeneralSettings.AppUseParkingTicketModule)
                            {
                                //Add to PendentPayedParkingTickets
                                //if (item.Key.ArticleOid.Equals(SettingsApp.XpoOidArticleParkingTicket))
                                //Get Original Designation from Fresh Object
                                fin_article articleForDesignation = (fin_article)XPOUtility.GetXPGuidObject(uowSession, typeof(fin_article), item.Key.ArticleId);
                                // Extract Ean from Designation 
                                string ticketEan = item.Key.Designation
                                    .Replace($"{articleForDesignation.Designation} [", string.Empty)
                                    .Replace("]", string.Empty);
                                // Extract Ean from designation after n~hours showed on ticket
                                string output = ticketEan.Split('[').Last();
                                /* 
                                 * TK013134
                                 * Defined on 2018-12-12 that Designation must be kept the same as it is when billing.
                                 */
                                /*articleForDesignation.Reload();
                                item.Key.Designation = articleForDesignation.Designation;
                                item.Value.Notes = $"[{ticketEan}]";*/

                                //Add to PendentPayedParkingCards
                                //if (item.Key.ArticleOid.Equals(SettingsApp.XpoOidArticleParkingCard))

                                //IN009279 If ticket (EAN 13)
                                if (output.Length != 13)
                                {
                                    int quantity = Convert.ToInt32(item.Value.Quantity);
                                    documentFinanceMaster.Notes += string.Format("{0} ", quantity.ToString());
                                    //documentOrderMain.Notes = item.Value.Quantity.ToString();
                                    GeneralSettings.PendentPaidParkingCards.Add(output, documentOrderMain.Oid);
                                }
                                //IN009279 If Card
                                else GeneralSettings.PendentPaidParkingTickets.Add(output, documentOrderMain.Oid);
                            }
                        }
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Persist Totals

                    fin_documentfinancemastertotal documentFinanceMasterTotal;
                    foreach (var item in pParameters.ArticleBag.TaxBag)
                    {
                        //Console.WriteLine(string.Format("{0}\t{1}", item.Key, item.Value));
                        documentFinanceMasterTotal = new fin_documentfinancemastertotal(uowSession)
                        {
                            Value = item.Key,
                            Total = item.Value.Total,
                            TotalBase = item.Value.TotalBase,
                            TotalType = FinanceMasterTotalType.Tax,
                            DocumentMaster = documentFinanceMaster
                        };
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Final DocumentFinanceMaster Updates

                    //Global Document Date
                    documentFinanceMaster.Date = documentDateTime;
                    //SAF-T(PT)
                    documentFinanceMaster.DocumentDate = documentDateTime.ToString(CultureSettings.DateTimeFormatDocumentDate);
                    documentFinanceMaster.SystemEntryDate = documentDateTime.ToString(CultureSettings.DateTimeFormatCombinedDateTime);
                    documentFinanceMaster.DocumentNumber = documentNumber;
                    documentFinanceMaster.TotalNet = pParameters.ArticleBag.TotalNet;
                    documentFinanceMaster.TotalGross = pParameters.ArticleBag.TotalGross;
                    documentFinanceMaster.TotalDiscount = pParameters.ArticleBag.TotalDiscount;
                    documentFinanceMaster.TotalTax = pParameters.ArticleBag.TotalTax;
                    documentFinanceMaster.TotalFinal = pParameters.ArticleBag.TotalFinal;
                    documentFinanceMaster.TotalFinalRound = Math.Round(pParameters.ArticleBag.TotalFinal, 2);
                    //Discount
                    if (pParameters.ArticleBag.DiscountGlobal > 0) documentFinanceMaster.Discount = pParameters.ArticleBag.DiscountGlobal;
                    //HASH
                    documentFinanceMaster.Hash = GenDocumentHash(uowSession, documentFinanceType, documentFinanceSerie, documentFinanceMaster);
                    documentFinanceMaster.HashControl = DocumentSettings.HashControl.ToString();
                    documentFinanceMaster.SourceBilling = "P";
                    documentFinanceMaster.SelfBillingIndicator = 0;
                    documentFinanceMaster.CashVatSchemeIndicator = 0;
                    documentFinanceMaster.ThirdPartiesBillingIndicator = 0;
                    //Store CodeInternals to use in SAF-T
                    documentFinanceMaster.DocumentStatusUser = userDetail.CodeInternal;
                    documentFinanceMaster.DocumentCreatorUser = userDetail.CodeInternal;
                    //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
                    documentFinanceMaster.ATCUD = "0"; //A preencher pelo WS da AT Julho 2021?
                    documentFinanceMaster.ATDocQRCode = GenDocumentQRCode(uowSession, documentFinanceType, documentFinanceSerie, documentFinanceMaster, true);
                    //CAE is Deprecated, this will prevent triggering Errors
                    if (GeneralSettings.PreferenceParameters.ContainsKey("COMPANY_CAE") && !string.IsNullOrEmpty(GeneralSettings.PreferenceParameters["COMPANY_CAE"].ToString()))
                    {
                        documentFinanceMaster.EACCode = GeneralSettings.PreferenceParameters["COMPANY_CAE"];
                    }

                    //Currency Congo K
                    if (System.Configuration.ConfigurationManager.AppSettings["cultureFinancialRules"] == "fr-CF")
                    {
                        var usCurrency = (cfg_configurationcurrency)uowSession.GetObjectByKey(typeof(cfg_configurationcurrency), Guid.Parse("28d692ad-0083-11e4-96ce-00ff2353398c"));
                        //ExchangeRate
                        var usExchangeRate = usCurrency.ExchangeRate;

                        documentFinanceMaster.Notes += string.Format("Valeur totale en dollars: " + documentFinanceMaster.TotalFinal * usExchangeRate) + "USD";
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Customer Card

                    //Discount Customer Card Credit
                    if (paymentMethod != null && paymentMethod.Token == "CUSTOMER_CARD")
                    {
                        customer.CardCredit = customer.CardCredit - documentFinanceMaster.TotalFinal;
                    }

                    //Add to Customer Card Credit
                    foreach (var item in pParameters.ArticleBag)
                    {
                        //Get Article
                        article = (fin_article)uowSession.GetObjectByKey(typeof(fin_article), item.Key.ArticleId);
                        if (article.Type.Oid == XPOSettings.XpoOidArticleClassCustomerCard)
                        {
                            customer.CardCredit = customer.CardCredit + item.Value.TotalFinal;
                        }
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Start Persist WorkSessions

                    //Assign null to Payment Method when DocumentType is a Not Payed Document, ex GR, OR etc
                    if (!documentFinanceType.Payed) paymentMethod = null;

                    //PersistFinanceDocumentWorkSession if document is Payed or if it is a CurrentAccount (Splited in Prints with SplitCurrentAccountMode Enum)
                    if (XPOSettings.WorkSessionPeriodTerminal != null && (paymentMethod != null || documentFinanceType.Oid == DocumentSettings.CurrentAccountInputId))
                    {
                        //Call PersistFinanceDocumentWorkSession to do WorkSession Job
                        PersistFinanceDocumentWorkSession(uowSession, documentFinanceMaster, pParameters, paymentMethod);
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Start Commit Changes in UOW

                    try
                    {
                        //Working on OrderMain Mode, if Not TableConsult or CreditNote
                        if (pParameters.SourceMode == PersistFinanceDocumentSourceMode.CurrentOrderMain && documentFinanceType.Oid != DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument)
                        {
                            //Commit UOW Changes : Before get current OrderMain
                            uowSession.CommitChanges();

                            //Get current OrderMain Article Bag, After Process Payment/PartialPayment to check if current OrderMain has Items, or is Empty
                            pParameters.ArticleBag = ArticleBag.TicketOrderToArticleBag(orderMain);
                            //Proteção para artigos do tipo "Sem Preço" [IN:013329]
                            //Check if Article bag contains Products with no price type and clean it
                            var toRemove = pParameters.ArticleBag.Where(pair => pair.Key.Price == 0.00m)
                             .Select(pair => pair.Key)
                             .ToList();
                            foreach (var key in toRemove)
                            {
                                pParameters.ArticleBag.Remove(key);
                            }

                            if (pParameters.ArticleBag.Count <= 0)
                            {
                                // Warning required to check if (documentOrderMain != null), when we work with SplitPayments and work only one product, 
                                // the 2,3,4....orders are null, this is because first FinanceDocument Closes Order

                                //Close OrderMain
                                if (documentOrderMain != null) documentOrderMain.OrderStatus = OrderStatus.Close;

                                //Required to Update and Sync Terminals
                                if (documentOrderMain != null) documentOrderMain.UpdatedAt = documentDateTime;

                                //Change Table Status to Free
                                pos_configurationplacetable placeTable;
                                placeTable = (pos_configurationplacetable)XPOUtility.GetXPGuidObject(uowSession, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                                if (placeTable != null)
                                {
                                    placeTable.TableStatus = TableStatus.Free;
                                    XPOUtility.Audit("TABLE_OPEN", string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "audit_message_table_open"), placeTable.Designation));
                                    placeTable.DateTableClosed = documentDateTime;
                                    placeTable.TotalOpen = 0;
                                    //Required to Reload Objects after has been changed in Another Session(uowSession)
                                    if (documentOrderMain != null) documentOrderMain = (fin_documentordermain)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(fin_documentordermain), orderMain.PersistentOid);
                                    if (documentOrderMain != null) documentOrderMain.Reload();
                                    placeTable = (pos_configurationplacetable)XPOUtility.GetXPGuidObject(XPOSettings.Session, typeof(pos_configurationplacetable), orderMain.Table.Oid);
                                    placeTable.Reload();
                                }
                                //Clean Session if Commited without problems
                                orderMain.CleanSessionOrder();
                            }
                            //PartialPayment Detected
                            else
                            {
                                //Required to Update and Sync Terminals
                                if (documentOrderMain != null) documentOrderMain.UpdatedAt = documentDateTime;
                            }
                        }
                        //Update CurrentAccount Documents
                        else if (pParameters.SourceMode == PersistFinanceDocumentSourceMode.CurrentAcountDocuments)
                        {
                            fin_documentfinancemaster currentAcountDocument;
                            foreach (fin_documentfinancemaster financeDocument in pParameters.FinanceDocuments)
                            {
                                currentAcountDocument = (fin_documentfinancemaster)uowSession.GetObjectByKey(typeof(fin_documentfinancemaster), financeDocument.Oid);
                                currentAcountDocument.Payed = true;
                                currentAcountDocument.PayedDate = documentDateTime;
                                currentAcountDocument.DocumentChild = documentFinanceMaster;
                            }
                        }

                        //Finnaly Commit Changes
                        uowSession.CommitChanges();

                        // TK013134
                        if (GeneralSettings.AppUseParkingTicketModule)
                            //{
                            //    foreach (var item in GlobalFramework.PendentPayedParkingTickets)
                            //    {
                            //        // Call #Ws Part 2 : Send Payed Cached Tickets
                            //        _logger.Debug($"TicketId: [{item.Key}], documentOrderMain.Oid: [{item.Value}]");
                            //    }

                            //    foreach (var item in GlobalFramework.PendentPayedParkingCards)
                            //    {
                            //        // Call #Ws Part 3 : Send Payed Cached Cards
                            //        _logger.Debug($"CardId: [{item.Key}], documentOrderMain.Oid: [{item.Value}]");
                            //    }
                            //}

                            //Always Assign DocumentChild Reference to DocumentParent
                            if (documentFinanceMasterParentDocument != null)
                            {
                                //documentFinanceMaster.DocumentParent = (DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(DocumentFinanceMaster), pParameters.DocumentParent);
                                documentFinanceMasterParentDocument.DocumentChild = documentFinanceMaster;
                                uowSession.CommitChanges();
                            }

                        //Audit
                        XPOUtility.Audit("FINANCE_DOCUMENT_CREATED", string.Format("{0} {1}: {2}", documentFinanceMaster.DocumentType.Designation, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_document_created"), documentFinanceMaster.DocumentNumber));

                        //Process Stock
                        try
                        {
                            ModulesSettings.StockManagementModule = (PluginSettings.PluginContainer.GetFirstPluginOfType<IStockManagementModule>());

                            if (
                                LicenseSettings.LicenseModuleStocks &&
                                ModulesSettings.HasStockManagementModule)
                            {
                                ModulesSettings.StockManagementModule.Add(documentFinanceMaster);
                            }
                            else
                            {
                                ProcessArticleStock.Add(documentFinanceMaster);
                            }

                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error processing stocks :: " + ex.Message, ex);
                        }

                        //Assign Document to Result
                        result = documentFinanceMaster;

                        // Call Generate GenerateDocument
                        //POS front-end - Consulta Mesa + Impressão Ticket's + Gerar PDF em modo Thermal Printer [IN009344]
                        // If is Thermal Print doc don't create PDF + Lindote(06/02/2020)
                        if (!PrintingSettings.ThermalPrinter.UsingThermalPrinter)
                        {
                            Reporting.Common.FastReport.GenerateDocumentFinanceMasterPDFIfNotExists(documentFinanceMaster);
                        }

                    }
                    catch (Exception ex)
                    {
                        uowSession.RollbackTransaction();
                        _logger.Error(ex.Message, ex);
                        throw new Exception("ERROR_COMMIT_FINANCE_DOCUMENT", ex.InnerException);
                    }
                }
            }
            catch (DocumentProcessingValidationException ex)
            {
                _logger.Error(ex.Message, ex);
                throw new Exception(ex.Exception.Message, ex.InnerException);
            }

            return (result);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GenDocumentHash

        //Despacho_n_8632_2014_03_07.pdf 
        //2.1.5 - Em regra, os documentos são assinados tendo em consideração o Hash do último documento emitido da mesma série/tipo. 
        //No caso da gravação de um primeiro documento de uma série/tipo de documento de faturação, 
        //o campo aplicável Chave do documento (Hash) das tabelas 4.1 a 4.3, deve ser assumido como não preenchido. 
        //No caso de utilização de séries plurianuais, no início de cada exercício, o primeiro documento poderá ser assinado tendo em consideração o Hash do 
        //último documento emitido da mesma série/tipo, no exercício fiscal anterior.

        public static string GenDocumentHash(Session pSession, fin_documentfinancetype pDocType, fin_documentfinanceseries pDocSerie, fin_documentfinancemaster pDocumentFinanceMaster)
        {
            bool debug = false;
            fin_documentfinancemaster doc = pDocumentFinanceMaster;
            //Required to ALways use "." and not Culture Decimal separator, ex ","
            //string TotalFinalRound = LogicPOS.Utility.DataConversionUtils.DecimalToString(doc.TotalFinalRound).Replace(',', '.');
            string TotalFinalRound = DataConversionUtils.DecimalToString(Convert.ToDecimal(doc.TotalFinalRound));
            //Hash must be the first, to use first field has value for ExecuteScalar
            string sql = string.Format(@"SELECT Hash, HashControl, Date AS InvoiceDate, SystemEntryDate, DocumentNumber AS InvoiceNo, TotalFinalRound FROM fin_documentfinancemaster WHERE DocumentType = '{0}' and  DocumentSerie = '{1}' ORDER BY Date DESC;", pDocType.Oid, pDocSerie.Oid);
            var olastDocumentHash = pSession.ExecuteScalar(sql);
            string lastDocumentHash = (olastDocumentHash != null) ? olastDocumentHash.ToString() : "";
            string signTargetString = string.Format("{0};{1};{2};{3};{4}", doc.DocumentDate, doc.SystemEntryDate, doc.DocumentNumber, TotalFinalRound, lastDocumentHash);

            string resultSignedHash;
            // Old Method without Plugin
            //resultSignedHash = FrameworkUtils.SignDataToSHA1Base64(signTargetString, debug);
            // Sign Document if has a valid PluginSoftwareVendor 
            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                resultSignedHash = PluginSettings.SoftwareVendor.SignDataToSHA1Base64(PluginSettings.SecretKey, signTargetString, debug);
            }
            else
            {
                // Dont Sign it without SoftwareVendor
                resultSignedHash = null;
            }

            //Debug
            if (debug) _logger.Debug(string.Format("GenDocumentHash(): #{0}", doc.DocumentNumber));
            if (debug) _logger.Debug(string.Format("GenDocumentHash(): lastDocumentHash [{0}]", lastDocumentHash));
            if (debug) _logger.Debug(string.Format("GenDocumentHash(): signTargetString [{0}]", signTargetString));
            if (debug) _logger.Debug(string.Format("GenDocumentHash(): resultSignedHash [{0}]", resultSignedHash));
            if (debug) _logger.Debug(string.Format("GenDocumentHash(): sql [{0}]", sql));

            return resultSignedHash;
        }

        //ATCUD Documentos - Criação do QRCode e ATCUD IN016508
        //Na criação da mensagem a incorporar no código QR devem ser observadas as
        //seguintes regras:
        //a) Cada campo será formado pela concatenação do valor da coluna “Código”,
        //constante na tabela do ponto 4, «:» (dois pontos) e o respetivo valor da coluna
        //“Descrição”, sem espaços;
        //b) Os campos assim criados, e sempre pela ordem indicada na tabela do ponto 4,
        //deverão ser concatenados com o separador «*» (asterisco);
        //c) Os campos monetários deverão ser representados em euros, com «.» (ponto)
        //como separador decimal e sempre com duas(2) casas decimais,
        //independentemente do número de casas decimais(inferior ou superior)
        //apresentado na base de dados e na exportação para o ficheiro SAF-T(PT). Nos
        //documentos emitidos em moeda diferente de Euro, os montantes apresentados
        //deverão ser previamente convertidos em euros;
        //d) Os campos assinalados com «+» são de criação obrigatória;
        //e) Os campos assinalados com «++», são opcionais, mas deverão ser criados

        //f) Nos campos opcionais, na ausência de informação não deverá ser criado o
        //respetivo campo;
        //g) Na criação dos campos com os códigos I1 a I8, J1 a J8 e K1 a K8,
        //representativos dos espaços fiscais para efeitos de IVA(por exemplo, PT, PTAC e PT-MA), terá sempre de existir, pelo menos, um espaço fiscal, até ao
        //máximo de três espaços fiscais em simultâneo, nacionais ou estrangeiros;
        //h) Na composição do campo com o código I1, no caso de documento emitido sem
        //indicação da taxa de IVA, que deva constar na tabela 4.2, 4.3 ou 4.4 do SAF-T
        //(PT), deverá ser preenchido com «I1»«:»«0»;
        //i) Nenhum valor da coluna “Descrição” poderá ultrapassar o tamanho máximo
        //definido na tabela do ponto 4;
        //j) Na composição do campo com o código “S”, sempre que necessário, os
        //elementos que o compõem serão concatenados com «;» (ponto e virgula) sem
        //espaços;
        //k) Para as diferentes taxas de IVA existentes no documento(isento, reduzida,
        //intermédia e normal) devem constar, nos respetivos campos, os totais
        //acumulados de base tributável e IVA.

        public static string GenDocumentQRCode(Session pSession, fin_documentfinancetype pDocType, fin_documentfinanceseries pDocSerie, fin_documentfinancemaster pDocumentFinanceMaster, bool _debug = false)
        {
            bool debug = _debug;
            fin_documentfinancemaster doc = pDocumentFinanceMaster;

            // Old Method without Plugin
            //resultSignedHash = FrameworkUtils.SignDataToSHA1Base64(signTargetString, debug);
            // Sign Document if has a valid PluginSoftwareVendor 

            //A NIF do emitente Exemplo A:123456789 +
            //B NIF do adquirente Exemplo B:999999990 +
            //C País do adquirente Exemplo C:PT +
            //D Tipo de documento Exemplo D:FS +
            //E Estado do documento Exemplo E:N +
            //F Data do documento Exemplo F:20190812 +
            //G Identificação única do documento Exemplo G:FS CDVF/ 12345 +
            //H ATCUD Exemplo H:CDF7T5HD-12345 +
            //I1 Espaço fiscal Exemplo I1:PT +
            //I7 Base tributável de IVA à taxa normal Exemplo I7:0.65 ++
            //I8 Total de IVA à taxa normal Exemplo I8:0.15++
            //N Total de impostos Exemplo N: 0.15 +
            //O Total do documento com impostos Exemplo O: 0.80 +
            //Q 4 carateres do Hash Exemplo Q:YhGV +
            //R Nº do certificado Exemplo R:9999 +
            //S Outras informações Exemplo S: NU; 0.80 ++

            string A = "A:" + GeneralSettings.PreferenceParameters["COMPANY_FISCALNUMBER"] + "*";
            string B = "B:" + PluginSettings.SoftwareVendor.Decrypt(doc.EntityFiscalNumber) + "*";
            string C = "C:" + doc.EntityCountry + "*";
            string D = "D:" + pDocType.Acronym + "*";
            string E = "E:" + doc.DocumentStatusStatus + "*";
            string F = "F:" + doc.DocumentDate.Replace("-", "") + "*";
            string G = "G:" + doc.DocumentNumber + "*";
            string H = "H:" + doc.ATCUD + "*";
            string I1 = "I1:PT*";
            string I7 = "";
            string I8 = "";
            string N = "N:" + DataConversionUtils.DecimalToString(doc.TotalTax).Replace(",", ".") + "*";
            string O = "O:" + DataConversionUtils.DecimalToString(doc.TotalFinal).Replace(",", ".") + "*";
            string Q = "Q:" + CryptographyUtils.GetDocumentHash4Chars(doc.Hash) + "*";
            string R = "R:" + SaftSettings.SaftSoftwareCertificateNumber + "*";
            string S = "";
            //Debug
            if (debug) _logger.Debug(string.Format("GenDocumentQRCode(): " + A + B + C + D + E + F + G + H + I1 + I7 + I8 + N + O + Q + R + S));

            //byte[] resultQRCode = new byte[64]; 
            string resultQRCode;
            if (PluginSettings.HasSoftwareVendorPlugin && (pDocType.SaftDocumentType == SaftDocumentType.SalesInvoices || pDocType.SaftDocumentType == SaftDocumentType.Payments))
            {
                //resultSignedHash = LogicPOS.Settings.PluginSettings.PluginSoftwareVendor.SignDataToSHA1Base64(SettingsApp.SecretKey, signTargetString, debug);

                //A elaboração do código de barras bidimensional (código QR) deve obedecer àsseguintes especificações:
                //a) Taxa de Recuperação de Erro(ECC): “M”;
                //b) Tipo: Byte;
                //c) Pontos por módulo(Size): 2;
                //d) Versão: v = 9(valor mínimo);
                //e) Dimensões de imagem: mínimo 30x30 milímetros;
                //f) Margem de Segurança(Margin): 0,25 cm.     
                //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                //QRCodeData qrCodeData = qrGenerator.CreateQrCode(A+B+C+D+E+F+G+H+I1+I7+I8+N+O+Q+R+S, QRCodeGenerator.ECCLevel.M, false,false, QRCodeGenerator.EciMode.Default, 9);
                //QRCode qrCode = new QRCode(qrCodeData);
                //Bitmap qrCodeImage = qrCode.GetGraphic(17, Color.Black, Color.White, null, 15, 2, true);

                //return FrameworkUtils.BitmapToByteArray(qrCodeImage);
                resultQRCode = A + B + C + D + E + F + G + H + I1 + I7 + I8 + N + O + Q + R + S;
                return resultQRCode;
            }
            else
            {
                // Dont Sign it without SoftwareVendor
                resultQRCode = null;
            }



            return resultQRCode;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GenDocumentNumber

        public static string GenDocumentNumber(fin_documentfinanceseries pDocType)
        {
            string documentNumber = "INVALID_DOCUMENT_NUMBER";

            try
            {
                string formatNumber = new string('0', SaftSettings.DocumentsPadLength);
                string publicDocID = pDocType.DocumentType.Acronym; ;
                string serieID = pDocType.Acronym; ;
                string seqNumber = "" + pDocType.NextDocumentNumber;


                //2018-05-08 : Old Format : [FT005012018S1] : Search GenDocumentNumber in ProcessFinanceDocument
                //string tmpInvoiceFormat = "{0} {1}/{2}";
                //documentNumber = string.Format(tmpInvoiceFormat, publicDocID, serieID, seqNumber);
                //2018-05-08 : New Format : [   05012018S1]
                string tmpInvoiceFormat = "{0}/{1}";
                documentNumber = string.Format(tmpInvoiceFormat, serieID, seqNumber);

                return documentNumber;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return documentNumber;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Process Payments/Recibos

        public static fin_documentfinancepayment PersistFinanceDocumentPayment(List<fin_documentfinancemaster> pInvoices, List<fin_documentfinancemaster> pCreditNotes, Guid pCustomer, Guid pPaymentMethod, Guid pConfigurationCurrency, decimal pPaymentAmount, string pPaymentNotes = "")
        {
            //Proccess Validation
            SortedDictionary<DocumentValidationErrorType, object> errorsValidation = DocumentProcessingValidationUtils.ValidatePersistFinanceDocumentPayment(pInvoices, pCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
            if (errorsValidation.Count > 0)
            {
                DocumentProcessingValidationException exception = new DocumentProcessingValidationException(new Exception("ERROR_DETECTED"), errorsValidation);
                //Throw without Errors only Exception for Muga Work, Return a simple String
                //throw exception.Exception;
                // Send with ExceptionErrors
                throw exception;
            }

            //Settings
            string dateTimeFormatDocumentDate = CultureSettings.DateTimeFormatDocumentDate;
            string dateTimeFormatCombinedDateTime = CultureSettings.DateTimeFormatCombinedDateTime;
            int decimalRoundTo = CultureSettings.DecimalRoundTo;

            //Init Local Vars  
            bool debug = false;
            decimal documentTotalPayRemain = 0.0m;
            decimal documentDebit = 0.0m;
            //Used to have Start Credit and Keep Discount Until equal to Zero, Start with pPaymentAmount and Add CreditNotes Total
            decimal totalCredit = pPaymentAmount;
            decimal totalCreditNotes = 0.0m;
            decimal totalPayed = 0.0m;
            //Stores the Final TotalDocument, sames has totalPayed - totalCreditNotes, The Amount the Client has Payed, Exposed in Extended and Total 
            decimal totalPayedDocument = 0.0m;
            //Total to Pay in Current Document (Invoices/Debit Notes)
            decimal totalToPayInCurrentInvoice = 0.0m;
            //Counter for DocumentFinanceMasterPayment Ord, Required to Order in SAF-T by same Order has Printed in Payment Document
            uint documentFinanceMasterPaymentOrd = 0;
            bool documentFullPayed = false;
            bool documentPartialPayed = false;
            string sql = string.Empty;
            string extended = string.Empty;
            //CurrentDateTime
            DateTime currentDateTime = XPOUtility.CurrentDateTimeAtomic();
            //XpoObjects
            fin_documentfinancemaster documentMaster = null;
            fin_documentfinancepayment documentFinancePayment = null;
            fin_documentfinancemasterpayment documentFinanceMasterPayment = null;

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                try
                {
                    //Get DocumentType
                    fin_documentfinancetype documentFinanceType = (fin_documentfinancetype)uowSession.GetObjectByKey(typeof(fin_documentfinancetype), DocumentSettings.PaymentDocumentTypeId);
                    if (documentFinanceType == null)
                    {
                        throw new Exception("ERROR_MISSING_DOCUMENT_TYPE");
                    }

                    //Get UserDetail
                    sys_userdetail userDetail = (sys_userdetail)uowSession.GetObjectByKey(typeof(sys_userdetail), XPOSettings.LoggedUser.Oid);
                    //Get Document Serie
                    fin_documentfinanceseries documentFinanceSerie = null;
                    fin_documentfinanceyearserieterminal documentFinanceYearSerieTerminal = null;
                    //Get Document Serie for current Terminal
                    documentFinanceYearSerieTerminal = DocumentProcessingSeriesUtils.GetDocumentFinanceYearSerieTerminal(uowSession, DocumentSettings.PaymentDocumentTypeId);
                    if (documentFinanceYearSerieTerminal != null)
                    {
                        documentFinanceSerie = documentFinanceYearSerieTerminal.Serie;
                    }
                    //If has Invalid Session Show Message and Return
                    if (documentFinanceSerie == null)
                    {
                        uowSession.RollbackTransaction();
                        throw new Exception("ERROR_MISSING_SERIE");
                    }

                    //Get Document Number
                    string documentNumber = GenDocumentNumber(documentFinanceSerie);
                    //Check if Valid DocumentNumber
                    if (documentNumber == "INVALID_DOCUMENT_NUMBER")
                    {
                        uowSession.RollbackTransaction();
                        throw new Exception("ERROR_INVALID_DOCUMENT_NUMBER");
                    }

                    //Increase NextDocumentNumber
                    documentFinanceSerie.NextDocumentNumber++;
                    if (debug) _logger.Debug(string.Format("documentNumber: [{0}]", documentNumber));

                    //Get Fresh UOW Objects
                    erp_customer customer = (erp_customer)XPOUtility.GetXPGuidObject(uowSession, typeof(erp_customer), pCustomer);
                    fin_configurationpaymentmethod paymentMethod = (fin_configurationpaymentmethod)XPOUtility.GetXPGuidObject(uowSession, typeof(fin_configurationpaymentmethod), pPaymentMethod);

                    //Initialize Payment/Recibo (Many(Invoices&CreditNotes) 2 One(Payment))
                    if (pInvoices.Count > 0)
                    {
                        /* IN009246 */
                        pInvoices.Sort((documentMasterBase, documentMasterToCompare) => documentMasterBase.Date.CompareTo(documentMasterToCompare.Date));

                        //DocumentFinancePayment: Payments
                        documentFinancePayment = new fin_documentfinancepayment(uowSession);
                        documentFinancePayment.PaymentRefNo = documentNumber;
                        documentFinancePayment.TransactionDate = currentDateTime.ToString(dateTimeFormatDocumentDate);
                        documentFinancePayment.PaymentType = "RG"; //Outros recibos emitidos
                        documentFinancePayment.PaymentStatus = "N";//Recibo normal e vigente
                        documentFinancePayment.PaymentDate = currentDateTime.ToString(dateTimeFormatDocumentDate); ;
                        //IN009294 Recibos - Data do documento = Data do recibo 
                        documentFinancePayment.DocumentDate = currentDateTime.ToString(dateTimeFormatDocumentDate);
                        //documentFinancePayment.DocumentDate = documentFinancePayment.PaymentDate;
                        documentFinancePayment.PaymentStatusDate = currentDateTime.ToString(dateTimeFormatCombinedDateTime);
                        documentFinancePayment.SourceID = userDetail.CodeInternal;
                        documentFinancePayment.SourcePayment = "P"; //Recibo produzido na aplicação
                        documentFinancePayment.EntityOid = customer.Oid;
                        documentFinancePayment.EntityInternalCode = customer.CodeInternal;
                        documentFinancePayment.PaymentMechanism = paymentMethod.Acronym;
                        documentFinancePayment.SystemEntryDate = currentDateTime.ToString(dateTimeFormatCombinedDateTime);
                        documentFinancePayment.PaymentMethod = paymentMethod;
                        documentFinancePayment.DocumentType = documentFinanceType;
                        documentFinancePayment.DocumentSerie = documentFinanceSerie;

                        //Only Assign if Not Empty
                        if (pPaymentNotes != string.Empty) documentFinancePayment.Notes = pPaymentNotes;
                    }

                    //Get Default defaultCurrency
                    cfg_configurationcurrency defaultCurrency = XPOSettings.ConfigurationSystemCurrency;
                    //Currency - If Diferent from Default System Currency, get Currency Object from Parameter
                    cfg_configurationcurrency configurationCurrency;
                    if (XPOSettings.ConfigurationSystemCurrency.Oid != pConfigurationCurrency)
                    {
                        configurationCurrency = (cfg_configurationcurrency)uowSession.GetObjectByKey(typeof(cfg_configurationcurrency), pConfigurationCurrency);
                    }
                    //Default Currency
                    else
                    {
                        //configurationCurrency = defaultCurrency;
                        //Fix for LogicErp else diferent Sessions Occur : Get Object in this Session
                        configurationCurrency = (cfg_configurationcurrency)uowSession.GetObjectByKey(typeof(cfg_configurationcurrency), defaultCurrency.Oid); ;
                    }
                    //Always Assign Currency,ExchangeRate and CurrencyCode
                    if (configurationCurrency != null)
                    {
                        //Currency
                        documentFinancePayment.Currency = configurationCurrency;
                        //ExchangeRate
                        documentFinancePayment.ExchangeRate = configurationCurrency.ExchangeRate;
                        //CurrencyCode
                        documentFinancePayment.CurrencyCode = configurationCurrency.Acronym;
                    }

                    //Process CreditNotes/NC, and get totalCreditNotes
                    foreach (fin_documentfinancemaster item in pCreditNotes)
                    {
                        //If TotalCredit Greater than Current Credit Note, we Use this Credit Note, Else we Skip it and remain Untoutched for future uses
                        //if (Math.Round(totalCredit, decimalRoundTo) > Math.Round(item.TotalFinal, decimalRoundTo))
                        //{
                        //Get Fresh Object in UOW
                        documentMaster = (fin_documentfinancemaster)XPOUtility.GetXPGuidObject(uowSession, typeof(fin_documentfinancemaster), item.Oid);
                        //Persist DocumentFinanceMaster Payment 
                        documentMaster.Payed = true;
                        documentMaster.PayedDate = currentDateTime;
                        totalCreditNotes += item.TotalFinal;
                        totalCredit += item.TotalFinal;
                        //DocumentFinanceMasterPayment: Many2Many Connection
                        documentFinanceMasterPaymentOrd++;
                        documentFinanceMasterPayment = new fin_documentfinancemasterpayment(uowSession);
                        documentFinanceMasterPayment.Ord = documentFinanceMasterPaymentOrd;
                        documentFinanceMasterPayment.DebitAmount = item.TotalFinal;
                        documentFinanceMasterPayment.DocumentFinanceMaster = documentMaster;
                        documentFinanceMasterPayment.DocumentFinancePayment = documentFinancePayment;
                        if (debug) _logger.Debug(string.Format("DocumentAcronym:[{0}] DocumentNumber:[{1}] DocumentValue: [{2}], TotalCredit: [{3}]", item.DocumentType.Acronym, item.DocumentNumber, item.TotalFinal, totalCredit));
                        //}
                    }

                    //Process Invoices/FTs
                    foreach (fin_documentfinancemaster item in pInvoices)
                    {
                        if (Math.Round(totalCredit, decimalRoundTo) > 0)
                        {
                            /* IN009182 - making this flow the same as Document presentation */
                            string stringFormatIndexZero = "{0}";
                            /* The following document types has no debit amount:
                             * Orçamento
                             * Guia ou Nota de Devolução
                             * Guia de Transporte
                             * Fatura Simplificada
                             * Documento de Conferência
                             * Fatura Pró-Forma
                             * Fatura-Recibo
                             * Nota de Crédito
                             * Guia de Consignação
                             * Guia de Remessa
                             * Guia de Movimentação de Ativos Fixos Próprios
                             */
                            string queryForTotalDebit = $@"
SELECT
(
	CASE  
		WHEN DFM.DocumentType IN (
            '{DocumentSettings.XpoOidDocumentFinanceTypeBudget}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide}', 
            '{CustomDocumentSettings.CreditNoteId}', 
            '{CustomDocumentSettings.DeliveryNoteDocumentTypeId}', 
            '{DocumentSettings.InvoiceAndPaymentId}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice}', 
            '{DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide}', 
            '{DocumentSettings.SimplifiedInvoiceId}', 
            '{CustomDocumentSettings.TransportDocumentTypeId}'
        ) THEN NULL 
		ELSE (
			DFM.TotalFinal - (COALESCE(
				(
					SELECT 
						SUM(DocFinMaster.TotalFinal) AS TotalFinal
					FROM
						fin_documentfinancemaster AS DocFinMaster
					WHERE
						DocumentType = 'fa924162-beed-4f2f-938d-919deafb7d47'
						AND 
							DocFinMaster.DocumentParent = DFM.Oid
						AND
							( DocFinMaster.DocumentStatusStatus <> 'A' AND DocFinMaster.Disabled <> 1)
					GROUP BY
						DocFinMaster.DocumentParent
					
				),0) + 
				(
					SELECT 
						 SUM(DocFinMasterPay.CreditAmount) AS CreditAmount
					FROM
						fin_documentfinancemasterpayment AS DocFinMasterPay
					LEFT JOIN 
						fin_documentfinancepayment AS DocFinPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
					WHERE
						DocFinMasterPay.DocumentFinanceMaster = DFM.Oid
						AND
							(DocFinPay.PaymentStatus <> 'A' AND DocFinPay.Disabled <> 1)
					GROUP BY
						DocFinMasterPay.DocumentFinanceMaster
					
				) 
			)
		)
	END
)
	 AS Balance
FROM
	fin_documentfinancemaster DFM
WHERE DFM.Oid =  '{stringFormatIndexZero}';
";

                            //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                            /* IN009182 - option #1 */
                            //sql = string.Format("SELECT fmaTotalFinal - SUM(fmpCreditAmount) as Result FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' GROUP BY fmaOid,fmaTotalFinal;", item.Oid);
                            /* IN009182 - option #2 */
                            sql = string.Format(queryForTotalDebit, item.Oid);
                            documentDebit = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(sql));
                            //Get current Document remain value to Pay
                            documentTotalPayRemain = (documentDebit > 0) ? documentDebit : item.TotalFinal;

                            //If the TotalCredit is Greater than or Equal
                            if (Math.Round(totalCredit, decimalRoundTo) >= Math.Round(documentTotalPayRemain, decimalRoundTo))
                            {
                                totalToPayInCurrentInvoice = documentTotalPayRemain;
                                documentFullPayed = true;
                            }
                            else
                            {
                                totalToPayInCurrentInvoice = totalCredit;
                                documentFullPayed = false;
                            }
                            documentPartialPayed = (totalToPayInCurrentInvoice > 0);

                            //Update totalCredit
                            totalCredit += -totalToPayInCurrentInvoice;
                            //Update Total Payed
                            totalPayed += totalToPayInCurrentInvoice;

                            if (debug) _logger.Debug(string.Format("[{0}] PayRemain: [{1}], PayInCurrent: [{2}], RemainToPay: [{3}], Credit: [{4}], totalPayed: [{5}], PartialPayed: [{6}], FullPayed: [{7}]", item.DocumentNumber, documentTotalPayRemain, totalToPayInCurrentInvoice, (documentTotalPayRemain - totalToPayInCurrentInvoice), totalCredit, totalPayed, documentPartialPayed, documentFullPayed));

                            //Always Get Fresh Object in UOW, used to Assign to Full and Partial Payments, need to be Outside FullPayed
                            documentMaster = (fin_documentfinancemaster)XPOUtility.GetXPGuidObject(uowSession, typeof(fin_documentfinancemaster), item.Oid);

                            //Persist DocumentFinanceMaster Payment if FullPayed
                            if (documentFullPayed)
                            {
                                documentMaster.Payed = true;
                                documentMaster.PayedDate = currentDateTime;

                                //On Full Invoice Payment Call ChangePayedInvoiceAndRelatedDocumentsStatus (Change status of Parent Document to F)
                                string statusReason = string.Format(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_documents_status_document_invoiced"), documentMaster.DocumentNumber);
                                //Get Fresh Object in UOW
                                fin_documentfinancemaster documentParent = null;
                                //Send with UOW Objects
                                if (item.DocumentParent != null)
                                {
                                    documentParent = (fin_documentfinancemaster)XPOUtility.GetXPGuidObject(uowSession, typeof(fin_documentfinancemaster), item.DocumentParent.Oid);
                                    ChangePayedInvoiceRelatedDocumentsStatus(uowSession, documentParent, statusReason, documentMaster.DocumentStatusDate, userDetail);
                                }
                            }

                            //Persist if Partial Payed or FullPayed (If is Partial is Full Too)
                            if (documentPartialPayed)
                            {
                                //DocumentFinanceMasterPayment: Many2Many Connection
                                documentFinanceMasterPaymentOrd++;
                                documentFinanceMasterPayment = new fin_documentfinancemasterpayment(uowSession);
                                documentFinanceMasterPayment.Ord = documentFinanceMasterPaymentOrd;
                                documentFinanceMasterPayment.CreditAmount = totalToPayInCurrentInvoice;
                                documentFinanceMasterPayment.DocumentFinanceMaster = documentMaster;
                                documentFinanceMasterPayment.DocumentFinancePayment = documentFinancePayment;
                                //if(debug) _logger.Debug(string.Format("  [{0}], Persisted PaymentAmount: [{1}]", documentMaster.DocumentNumber, totalToPayInCurrentInvoice));
                            }
                            /* IN009182 - adding related documents for reference */
                            string relatedDocumentsQuery = GenerateRelatedDocumentsQueryByDocumentType(documentMaster.Oid.ToString());
                            string relatedDocuments = Convert.ToString(XPOSettings.Session.ExecuteScalar(relatedDocumentsQuery));
                            if (!string.IsNullOrEmpty(relatedDocuments))
                            {
                                if (!string.IsNullOrEmpty(documentFinancePayment.Notes))
                                {
                                    if (documentFinancePayment.Notes.Contains(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_column_related_doc")))
                                    {
                                        documentFinancePayment.Notes += "; [" + documentMaster.DocumentNumber + "] " + relatedDocuments;
                                    }
                                    else
                                    {
                                        documentFinancePayment.Notes += Environment.NewLine + CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_column_related_doc") + ": [" + documentMaster.DocumentNumber + "] " + relatedDocuments;
                                    }
                                }
                                else
                                {
                                    documentFinancePayment.Notes += CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "window_title_dialog_document_finance_column_related_doc") + ": [" + documentMaster.DocumentNumber + "] " + relatedDocuments;
                                }
                            }
                        }
                    }

                    //Assign Total Payed Document, the Diference from totalPayed and totalCreditNotes, the Amount the Client has Payed
                    totalPayedDocument = totalPayed - totalCreditNotes;

                    //Get ExtendedValue
                    NumberToWordsUtility extendValue = new NumberToWordsUtility();
                    extended = extendValue.GetExtendedValue(totalPayedDocument, defaultCurrency.Designation);
                    if (debug) _logger.Debug(string.Format("extended: [{0}]", extended));

                    //Now we can Assign PaymentAmount (Credit-Debit Diference)
                    documentFinancePayment.PaymentAmount = totalPayedDocument;
                    documentFinancePayment.CurrencyAmount = totalPayedDocument * documentFinancePayment.ExchangeRate;
                    documentFinancePayment.ExtendedValue = extended;

                    //TK016319 - Certificação Angola - Alterações para teste da AGT 
                    //Calculo da taxa de imposto para recibos
                    decimal calcTax = ((documentFinancePayment.PaymentAmount * documentMaster.TotalTax) / documentMaster.TotalFinal);
                    documentFinancePayment.TaxPayable = Math.Round(calcTax, decimalRoundTo);

                    //Call PersistFinanceDocumentWorkSession to do WorkSession Job
                    PersistFinanceDocumentWorkSession(uowSession, documentFinancePayment, paymentMethod);

                    //Commit UOW Changes : Before get current OrderMain
                    uowSession.CommitChanges();

                    // Call Generate GenerateDocument
                    Reporting.Common.FastReport.GenerateDocumentFinancePaymentPDFIfNotExists(documentFinancePayment);
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                    _logger.Error(ex.Message, ex);
                    //2016-01-05 apmuga passar erro para cima e não mascarar com outro erro
                    //throw new Exception("ERROR_COMMIT_FINANCE_DOCUMENT_PAYMENT", ex.InnerException);
                    // throw ex;

                }
            }

            return documentFinancePayment;
        }

        //Recursive Method : Change Document Status of related invoice Documents : Change to (“F” — Documento faturado)
        public static bool ChangePayedInvoiceRelatedDocumentsStatus(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster, string pStatusReason, string pCombinedDateTime, sys_userdetail pUserDetail)
        {
            bool debug = true;
            bool result = false;

            try
            {
                //F: Document was Invoiced (“F” — Documento faturado.)
                pDocumentFinanceMaster.DocumentStatusStatus = "F";
                pDocumentFinanceMaster.DocumentStatusDate = pCombinedDateTime;
                pDocumentFinanceMaster.DocumentStatusReason = pStatusReason;
                pDocumentFinanceMaster.DocumentStatusUser = pUserDetail.CodeInternal;
                pDocumentFinanceMaster.SystemEntryDate = pCombinedDateTime;
                if (debug) _logger.Debug(string.Format("ChangePayedInvoiceStatus On: [{0}] to [{1}]", pDocumentFinanceMaster.DocumentNumber, pDocumentFinanceMaster.DocumentStatusStatus));

                //Call Recursive Method on Parent Again, until it is NULL (No Parent)
                if (pDocumentFinanceMaster.DocumentParent != null) ChangePayedInvoiceRelatedDocumentsStatus(pSession, pDocumentFinanceMaster.DocumentParent, pStatusReason, pCombinedDateTime, pUserDetail);

                result = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Process

        //Used with DocumentFinancePayment
        public static bool PersistFinanceDocumentWorkSession(Session pSession, fin_documentfinancepayment pDocumentFinancePayment, fin_configurationpaymentmethod pPaymentMethod)
        {
            return PersistFinanceDocumentWorkSession(pSession, null, null, pDocumentFinancePayment, pPaymentMethod);
        }

        //Used With DocumentFinanceMaster
        public static bool PersistFinanceDocumentWorkSession(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster, DocumentProcessingParameters pParameters, fin_configurationpaymentmethod pPaymentMethod)
        {
            return PersistFinanceDocumentWorkSession(pSession, pDocumentFinanceMaster, pParameters, null, pPaymentMethod);
        }

        //Main Method
        public static bool PersistFinanceDocumentWorkSession(Session pSession, fin_documentfinancemaster pDocumentFinanceMaster, DocumentProcessingParameters pParameters, fin_documentfinancepayment pDocumentFinancePayment, fin_configurationpaymentmethod pPaymentMethod)
        {
            bool result;
            try
            {
                //Get Period WorkSessionPeriodTerminal, UserDetail and Terminal
                pos_worksessionperiod workSessionPeriod = (pos_worksessionperiod)XPOUtility.GetXPGuidObject(pSession, typeof(pos_worksessionperiod), XPOSettings.WorkSessionPeriodTerminal.Oid);
                sys_userdetail userDetail = (sys_userdetail)XPOUtility.GetXPGuidObject(pSession, typeof(sys_userdetail), XPOSettings.LoggedUser.Oid);
                pos_configurationplaceterminal configurationPlaceTerminal = (pos_configurationplaceterminal)XPOUtility.GetXPGuidObject(pSession, typeof(pos_configurationplaceterminal), TerminalSettings.LoggedTerminal.Oid);

                //Variables to diferent Document Types : DocumentFinanceMaster or DocumentFinancePayment
                DateTime documentDate = XPOUtility.CurrentDateTimeAtomic();
                decimal movementAmount = 0m;
                string movementDescriptionDocument = string.Empty;
                string movementDescriptionTotalDelivery = string.Empty;
                string movementDescriptionTotalChange = string.Empty;
                decimal totalDelivery = 0m;
                decimal totalChange = 0m;

                if (pDocumentFinanceMaster != null)
                {
                    documentDate = pDocumentFinanceMaster.Date;
                    movementAmount = pDocumentFinanceMaster.TotalFinal;
                    movementDescriptionDocument = string.Format("{0} : {1}", pDocumentFinanceMaster.DocumentNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_finance_document"));
                    movementDescriptionTotalDelivery = string.Format("{0} : {1}", pDocumentFinanceMaster.DocumentNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_deliver"));
                    movementDescriptionTotalChange = string.Format("{0} : {1}", pDocumentFinanceMaster.DocumentNumber, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_change"));
                    if (pParameters.TotalDelivery > 0) totalDelivery = pParameters.TotalDelivery;
                    if (pParameters.TotalChange > 0) totalChange = pParameters.TotalChange;
                }
                else if (pDocumentFinancePayment != null)
                {
                    documentDate = pDocumentFinancePayment.CreatedAt;//.PaymentDate
                    movementAmount = pDocumentFinancePayment.PaymentAmount;
                    movementDescriptionDocument = string.Format("{0} : {1}", pDocumentFinancePayment.PaymentRefNo, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_payment_document"));
                    movementDescriptionTotalDelivery = string.Format("{0} : {1}", pDocumentFinancePayment.PaymentRefNo, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_deliver"));
                    movementDescriptionTotalChange = string.Format("{0} : {1}", pDocumentFinancePayment.PaymentRefNo, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_total_change"));
                    //TODO: Improve with Payment TotalChange Functionality
                    totalDelivery = movementAmount;
                }
                else
                {
                    return false;
                }

                //Persist DocumentFinance Movement
                WorkSessionProcessor.PersistWorkSessionMovement(
                    pSession,
                    workSessionPeriod,
                    (pos_worksessionmovementtype)XPOUtility.GetXPGuidObjectFromField(pSession, typeof(pos_worksessionmovementtype), "Token", "FINANCE_DOCUMENT"),
                    pDocumentFinanceMaster,
                    pDocumentFinancePayment,
                    userDetail,
                    configurationPlaceTerminal,
                    documentDate,
                    movementAmount,
                    movementDescriptionDocument,
                    1 //< Ord
                );

                //Persist CashDrawer Movements IN and OUT (Delivery and Charge)
                if (pPaymentMethod != null && pPaymentMethod.Token == "MONEY")
                {
                    //Process CASHDRAWER_IN Movement (Delivery)
                    WorkSessionProcessor.PersistWorkSessionMovement(
                        pSession,
                        workSessionPeriod,
                        (pos_worksessionmovementtype)XPOUtility.GetXPGuidObjectFromField(pSession, typeof(pos_worksessionmovementtype), "Token", "CASHDRAWER_IN"),
                        pDocumentFinanceMaster,
                        pDocumentFinancePayment,
                        userDetail,
                        configurationPlaceTerminal,
                        documentDate,
                        totalDelivery,
                        movementDescriptionTotalDelivery,
                        2 //< Ord
                    );
                    //Process CASHDRAWER_OUT Movement (Charge)
                    if (pParameters != null)
                    {
                        if (pParameters.TotalChange > 0) WorkSessionProcessor.PersistWorkSessionMovement(
                            pSession,
                            workSessionPeriod,
                            (pos_worksessionmovementtype)XPOUtility.GetXPGuidObjectFromField(pSession, typeof(pos_worksessionmovementtype), "Token", "CASHDRAWER_OUT"),
                            pDocumentFinanceMaster,
                            pDocumentFinancePayment,
                            userDetail,
                            configurationPlaceTerminal,
                            documentDate,
                            -totalChange,
                            movementDescriptionTotalChange,
                            3 //< Ord
                        );
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Get Last FinanceDocument DateTime

        public static DateTime GetLastDocumentDateTime()
        {
            return GetLastDocumentDateTime(XPOSettings.Session, null);
        }

        public static DateTime GetLastDocumentDateTime(string pFilter)
        {
            return GetLastDocumentDateTime(XPOSettings.Session, pFilter);
        }

        public static DateTime GetLastDocumentDateTime(Session pSesssion)
        {
            return GetLastDocumentDateTime(pSesssion, string.Empty);
        }

        public static DateTime GetLastDocumentDateTime(Session pSesssion, string pFilter)
        {
            DateTime result = DateTime.MinValue;

            try
            {
                if (!string.IsNullOrEmpty(pFilter)) pFilter = string.Format(" WHERE ({0})", pFilter);
                string sql = string.Format("SELECT MAX(SystemEntryDate) AS Max FROM fin_documentfinancemaster{0};", pFilter);
                var dateTime = pSesssion.ExecuteScalar(sql);
                if (dateTime != null) result = Convert.ToDateTime(dateTime);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProcessArticleStock Commisions

        private static void PersistFinanceDocumentCommission(Session pSession, fin_documentfinancedetail pDocumentFinanceDetail, sys_userdetail pUserDetail)
        {
            try
            {
                pos_usercommissiongroup commissionGroup = null;

                if (pDocumentFinanceDetail.Article.CommissionGroup != null)
                {
                    commissionGroup = pDocumentFinanceDetail.Article.CommissionGroup;
                }
                else if (pDocumentFinanceDetail.Article.SubFamily.CommissionGroup != null)
                {
                    commissionGroup = pDocumentFinanceDetail.Article.SubFamily.CommissionGroup;
                }
                else if (pDocumentFinanceDetail.Article.Family.CommissionGroup != null)
                {
                    commissionGroup = pDocumentFinanceDetail.Article.Family.CommissionGroup;
                }

                //Check if CommissionGroup has Commission
                if (commissionGroup != null && commissionGroup.Commission > 0 && commissionGroup == pUserDetail.CommissionGroup)
                {
                    pos_configurationplaceterminal configurationPlaceTerminal = (pos_configurationplaceterminal)XPOUtility.GetXPGuidObject(pSession, typeof(pos_configurationplaceterminal), TerminalSettings.LoggedTerminal.Oid);
                    decimal totalCommission = (pDocumentFinanceDetail.TotalNet * pUserDetail.CommissionGroup.Commission) / 100;

                    fin_documentfinancecommission documentFinanceCommission = new fin_documentfinancecommission(pSession)
                    {
                        Ord = pDocumentFinanceDetail.Ord,
                        Date = pDocumentFinanceDetail.DocumentMaster.Date,
                        Commission = pUserDetail.CommissionGroup.Commission,
                        Total = totalCommission,
                        CommissionGroup = pUserDetail.CommissionGroup,
                        FinanceMaster = pDocumentFinanceDetail.DocumentMaster,
                        FinanceDetail = pDocumentFinanceDetail,
                        UserDetail = pUserDetail,
                        Terminal = configurationPlaceTerminal,
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// /// Creates the SQL query for when retrieving linked documents based on DocumentType parameter.
        /// </summary>
        /// <param name="Oid"></param>
        /// <param name="documentType"></param>
        /// <returns></returns>
        private static string GenerateRelatedDocumentsQueryByDocumentType(string Oid, string documentType = "fa924162-beed-4f2f-938d-919deafb7d47")
        {
            string relatedDocumentsQuery = string.Empty;
            string andDocumentType = string.Empty;

            if (!string.IsNullOrEmpty(documentType))
            {
                andDocumentType = $" AND DocFinMaster.DocumentType = '{documentType}'";
            }

            switch (DatabaseSettings.DatabaseType)
            {
                case DatabaseType.MySql:
                case DatabaseType.SQLite:
                case DatabaseType.MonoLite:

                    {
                        relatedDocumentsQuery = @"
SELECT 
    GROUP_CONCAT(DISTINCT RelatedDocument.DocumentNumber) AS ResultConcat
FROM(
	SELECT
		DocFinMaster.DocumentNumber AS DocumentNumber,
		DocFinMaster.Date AS Date,
		DocFinMaster.DocumentParent AS DocumentParent,
		DocFinMaster.DocumentChild AS DocumentChild
	FROM
		fin_documentfinancemaster AS DocFinMaster
	WHERE
		(
            DocFinMaster.DocumentStatusStatus <> 'A' AND DocFinMaster.Disabled <> 1
        )
		AND (
			DocFinMaster.DocumentParent = '{0}'
			OR
				DocFinMaster.DocumentChild = '{0}'
				OR 
					DocFinMaster.Oid IN (
						SELECT 
							(SELECT B.Oid FROM fin_documentfinancemaster B WHERE B.Oid =  A.DocumentParent) AS DocumentParent
						FROM 
							fin_documentfinancemaster AS A
						WHERE
							A.Oid = '{0}'
					)
		)
        {1}
	UNION
	SELECT
		DocFinPay.PaymentRefNo AS DocumentNumber,
		DocFinPay.DocumentDate AS Date,
		DocFinMasterPay.DocumentFinanceMaster AS DocumentParent,
		NULL AS DocumentChild
	FROM
		fin_documentfinancepayment AS DocFinPay
	LEFT JOIN fin_documentfinancemasterpayment DocFinMasterPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
	WHERE
		(
            DocFinPay.PaymentStatus <> 'A' AND DocFinPay.Disabled <> 1
        )
		AND
			DocFinMasterPay.DocumentFinanceMaster = '{0}'
) AS RelatedDocument;
";
                    }
                    break;
                case DatabaseType.MSSqlServer:
                    {
                        relatedDocumentsQuery = @"
DECLARE @RelatedDocuments VARCHAR(MAX);
SELECT
    @RelatedDocuments = COALESCE(@RelatedDocuments + ', ', '') + RelatedDocument.DocumentNumber
FROM(
	SELECT
		DocFinMaster.DocumentNumber AS DocumentNumber,
		DocFinMaster.Date AS Date,
		DocFinMaster.DocumentParent AS DocumentParent,
		DocFinMaster.DocumentChild AS DocumentChild
	FROM
		fin_documentfinancemaster AS DocFinMaster
	WHERE
		(
            DocFinMaster.DocumentStatusStatus <> 'A' AND DocFinMaster.Disabled <> 1
        )
		AND (
			DocFinMaster.DocumentParent = '{0}'
			OR
				DocFinMaster.DocumentChild = '{0}'
				OR 
					DocFinMaster.Oid IN (
						SELECT 
							(SELECT B.Oid FROM fin_documentfinancemaster B WHERE B.Oid =  A.DocumentParent) AS DocumentParent
						FROM 
							fin_documentfinancemaster AS A
						WHERE
							A.Oid = '{0}'
					)
		)
        {1}
	UNION
	SELECT
		DocFinPay.PaymentRefNo AS DocumentNumber,
		DocFinPay.DocumentDate AS Date,
		DocFinMasterPay.DocumentFinanceMaster AS DocumentParent,
		NULL AS DocumentChild
	FROM
		fin_documentfinancepayment AS DocFinPay
	LEFT JOIN fin_documentfinancemasterpayment DocFinMasterPay ON (DocFinPay.Oid = DocFinMasterPay.DocumentFinancePayment)
	WHERE
		(
            DocFinPay.PaymentStatus <> 'A' AND DocFinPay.Disabled <> 1
        )
		AND
			DocFinMasterPay.DocumentFinanceMaster = '{0}'
) AS RelatedDocument

ORDER BY 
    RelatedDocument.Date ASC;
SELECT 
    @RelatedDocuments;
";
                    }
                    break;
                default:
                    break;
            }

            relatedDocumentsQuery = string.Format(relatedDocumentsQuery, Oid, andDocumentType);

            return relatedDocumentsQuery;
        }

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
            XPCollection xpcDocumentFinanceType = XPOUtility.GetXPCollectionFromCriteria(XPOSettings.Session, typeof(fin_documentfinancetype), criteriaOperator, sortingCollection);

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
                    pDocumentFinanceType == InvoiceSettings.InvoiceId ||
                    //pDocumentFinanceType == SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill ||
                    pDocumentFinanceType == DocumentSettings.SimplifiedInvoiceId ||
                    pDocumentFinanceType == DocumentSettings.InvoiceAndPaymentId ||
                    //SaftDocumentType = 0
                    pDocumentFinanceType == DocumentSettings.CurrentAccountInputId
                )
                {
                    //Moçambique - Pedidos da reunião 13/10/2020 [IN:014327]
                    //- Fatura simplificada em documentos de origem, para inserir nº contribuinte após emissão de fatura
                    if (CultureSettings.MozambiqueCountryId.Equals(XPOSettings.ConfigurationSystemCountry.Oid))
                    {
                        result = new Guid[] {
                        //SaftDocumentType = 2
                        InvoiceSettings.InvoiceId,
                        DocumentSettings.SimplifiedInvoiceId,
                        DocumentSettings.InvoiceAndPaymentId,
                        CustomDocumentSettings.DeliveryNoteDocumentTypeId,
                        DocumentSettings.CurrentAccountInputId,
                        CustomDocumentSettings.TransportDocumentTypeId,
                        DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide,
                        //SaftDocumentType = 3 
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        DocumentSettings.ConsignationInvoiceId,
                        //SaftDocumentType = 0 
                        DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice };
                    }
                    else
                    {
                        result = new Guid[] {
                        //SaftDocumentType = 2
                        CustomDocumentSettings.DeliveryNoteDocumentTypeId,
                        CustomDocumentSettings.TransportDocumentTypeId,
                        DocumentSettings.XpoOidDocumentFinanceTypeOwnAssetsDriveGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeConsignmentGuide,
                        DocumentSettings.XpoOidDocumentFinanceTypeReturnGuide,
                        //SaftDocumentType = 3 
                        DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument,
                        DocumentSettings.ConsignationInvoiceId,
                        //SaftDocumentType = 0 
                        DocumentSettings.XpoOidDocumentFinanceTypeBudget,
                        DocumentSettings.XpoOidDocumentFinanceTypeProformaInvoice };
                    }

                }
                //CreditNote
                else if (
                    pDocumentFinanceType == CustomDocumentSettings.CreditNoteId
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 1
                        InvoiceSettings.InvoiceId,
                        DocumentSettings.XpoOidDocumentFinanceTypeInvoiceWayBill,
                        DocumentSettings.SimplifiedInvoiceId,
                        DocumentSettings.InvoiceAndPaymentId
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
                    pDocumentFinanceType == DocumentSettings.ConsignationInvoiceId
                )
                {
                    result = new Guid[] { 
                        //SaftDocumentType = 1
                        InvoiceSettings.InvoiceId,
                        DocumentSettings.SimplifiedInvoiceId,
                        DocumentSettings.InvoiceAndPaymentId,
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
                    pDocumentFinanceType == CustomDocumentSettings.TransportDocumentTypeId ||
                    pDocumentFinanceType == CustomDocumentSettings.DeliveryNoteDocumentTypeId
                    )
                { /* #TODO check this list and all others here */
                    result = new Guid[] {
                        InvoiceSettings.InvoiceId,
                        DocumentSettings.SimplifiedInvoiceId,
                        DocumentSettings.InvoiceAndPaymentId,
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
                        DocumentSettings.ConsignationInvoiceId,
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
                            if (!listCreditedDocuments.Contains(itemReferences.DocumentDetail.DocumentMaster.DocumentNumber))
                            {
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
                        sql = string.Format("SELECT Quantity AS Total FROM fin_documentfinancedetail WHERE DocumentMaster = '{0}' AND Article = '{1}';", pDocumentParent.Oid, item.Key.ArticleId);
                        resultParentDocument = XPOSettings.Session.ExecuteScalar(sql);
                        totalParentDocument = (resultParentDocument != null) ? Convert.ToDecimal(resultParentDocument) : 0.0m;

                        sql = string.Format("SELECT SUM(fdQuantity) AS Total FROM view_documentfinance WHERE ftOid = '{0}' AND fmDocumentParent = '{1}' AND fdArticle = '{2}';", CustomDocumentSettings.CreditNoteId, pDocumentParent.Oid, item.Key.ArticleId);
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
                    InvoiceSettings.InvoiceId,
                    CustomDocumentSettings.CreditNoteId,
                    DocumentSettings.DebitNoteId
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
                    article = (fin_article)XPOSettings.Session.GetObjectByKey(typeof(fin_article), item.Key.ArticleId);
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
                    , InvoiceSettings.InvoiceId, CustomDocumentSettings.CreditNoteId, DocumentSettings.DebitNoteId
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
            Guid currentOrderMainOid = POSSession.CurrentSession.CurrentOrderMainId;
            _logger.Debug("fin_documentfinancemaster GetOrderMainLastDocumentConference(bool pGenerateNewIfDiferentFromArticleBag = false) :: currentOrderMainOid: " + currentOrderMainOid);
            OrderMain currentOrderMain = null;

            try
            {
                /* IN009179 - System.Collections.Generic.KeyNotFoundException */
                if (POSSession.CurrentSession.OrderMains.Count > 0)
                {
                    currentOrderMain = POSSession.CurrentSession.OrderMains[currentOrderMainOid];
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
                        DocumentProcessingParameters processFinanceDocumentParameter = new DocumentProcessingParameters(DocumentSettings.XpoOidDocumentFinanceTypeConferenceDocument, articleBag)
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
                        fin_documentfinancemaster newDocument = PersistFinanceDocument(processFinanceDocumentParameter, false);

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

        //Check if customer is Invalid for Simplified Invoices
        public static bool IsInValidFinanceDocumentCustomer(decimal pTotalFinal, string pName, string pAddress, string pZipCode, string pCity, string pCountry, string pFiscalNumber)
        {
            bool result = false;

            try
            {
                if (
                    pTotalFinal > GeneralSettings.GetRequiredCustomerDetailsAboveValue(XPOSettings.ConfigurationSystemCountry.Oid) &&
                    (
                        XPOUtility.IsFinalConsumerEntity(pFiscalNumber) ||
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
    }
}
