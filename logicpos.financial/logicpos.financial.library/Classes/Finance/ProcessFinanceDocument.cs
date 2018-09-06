using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Enums;
using logicpos.financial.library.App;
using logicpos.financial.library.Classes.Reports;
using logicpos.financial.library.Classes.Stocks;
using logicpos.financial.library.Classes.WorkSession;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Classes.Finance;
using logicpos.shared.Classes.Orders;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.IO;

namespace logicpos.financial.library.Classes.Finance
{
    public class ProcessFinanceDocument
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static FIN_DocumentFinanceMaster PersistFinanceDocument(ProcessFinanceDocumentParameter pParameters, bool pIgnoreWarning = false)
        {
            Guid userDetailGuid = (GlobalFramework.LoggedUser != null) ? GlobalFramework.LoggedUser.Oid : Guid.Empty;
            Guid terminalGuid = (GlobalFramework.LoggedTerminal != null) ? GlobalFramework.LoggedTerminal.Oid : Guid.Empty;

            return (PersistFinanceDocument(pParameters, userDetailGuid, terminalGuid, pIgnoreWarning));
        }

        public static FIN_DocumentFinanceMaster PersistFinanceDocument(ProcessFinanceDocumentParameter pParameters, Guid pLoggedUser, Guid pTerminal, bool pIgnoreWarning = false)
        {
            FIN_DocumentFinanceMaster result = null;

            try
            {
                //Proccess Validation
                SortedDictionary<FinanceValidationError, object> errorsValidation = ProcessFinanceDocumentValidation.ValidatePersistFinanceDocument(pParameters, pLoggedUser, pIgnoreWarning);
                if (errorsValidation.Count > 0)
                {
                    String errors = String.Empty;
                    foreach (var item in errorsValidation)
                    {
                        errors += String.Format("{0}- {1}", Environment.NewLine, item.Key);
                    }

                    ProcessFinanceDocumentValidationException exception = new ProcessFinanceDocumentValidationException(new Exception(String.Format("ERROR_DETECTED{0}{1}", Environment.NewLine, errors)), errorsValidation);
                    //Throw without Errors only Exception for Muga Work, Return a simple String
                    //throw exception.Exception;
                    // Send with ExceptionErrors
                    throw exception;
                }

                //Settings
                //string dateTimeFormatDocumentDate = (GlobalFramework.PluginSoftwareVendor != null) ? GlobalFramework.PluginSoftwareVendor.GetDateTimeFormatDocumentDate() : null;//SettingsApp.DateTimeFormatDocumentDate;
                //string dateTimeFormatCombinedDateTime = (GlobalFramework.PluginSoftwareVendor != null) ? GlobalFramework.PluginSoftwareVendor.GetDateTimeFormatCombinedDateTime() : null;//SettingsApp.DateTimeFormatCombinedDateTime;

                //If has DocumentDateTime from Parameters use it, else use Current Atomic DateTime : This is Optional, Now DocumentDateTime is assigned on Parameter Constructor
                DateTime documentDateTime = (pParameters.DocumentDateTime != DateTime.MinValue) ? pParameters.DocumentDateTime : FrameworkUtils.CurrentDateTimeAtomic();
                //Init Local Vars
                OrderMain orderMain = null;

                //Start UnitOfWork
                using (UnitOfWork uowSession = new UnitOfWork())
                {
                    //Get Objects, To Prevent XPO Stress of Deleted Objects inside UOW
                    //WorkSessionPeriod workSessionPeriod = (WorkSessionPeriod)FrameworkUtils.GetXPGuidObjectFromSession(uowSession, typeof(WorkSessionPeriod), GlobalFramework.WorkSessionPeriodTerminal.Oid);
                    SYS_UserDetail userDetail = (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(uowSession, typeof(SYS_UserDetail), pLoggedUser);
                    POS_ConfigurationPlaceTerminal terminal = (POS_ConfigurationPlaceTerminal)FrameworkUtils.GetXPGuidObject(uowSession, typeof(POS_ConfigurationPlaceTerminal), pTerminal);

                    //Prepare Modes
                    FIN_DocumentOrderMain documentOrderMain = null;
                    if (pParameters.SourceMode == PersistFinanceDocumentSourceMode.CurrentOrderMain)
                    {
                        orderMain = GlobalFramework.SessionApp.OrdersMain[GlobalFramework.SessionApp.CurrentOrderMainOid];
                        //Get Persistent Oid from orderMain
                        documentOrderMain = (FIN_DocumentOrderMain)uowSession.GetObjectByKey(typeof(FIN_DocumentOrderMain), orderMain.PersistentOid);
                    }
                    else
                    {
                        if (pParameters.SourceOrderMain != null)
                        {
                            documentOrderMain = (FIN_DocumentOrderMain)uowSession.GetObjectByKey(typeof(FIN_DocumentOrderMain), pParameters.SourceOrderMain.Oid);
                        }
                    }

                    //Initialize DocumentFinance* Objects 
                    FIN_DocumentFinanceType documentFinanceType = (FIN_DocumentFinanceType)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceType), pParameters.DocumentType);
                    FIN_DocumentFinanceSeries documentFinanceSerie = null;
                    FIN_DocumentFinanceYearSerieTerminal documentFinanceYearSerieTerminal = null;

                    //Get Document Serie for current Terminal
                    documentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(uowSession, documentFinanceType.Oid);
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
                    FIN_DocumentFinanceMaster documentFinanceMaster = new FIN_DocumentFinanceMaster(uowSession)
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
                    FIN_DocumentFinanceMaster documentFinanceMasterParentDocument = null;
                    if (pParameters.DocumentParent != null && pParameters.DocumentParent != Guid.Empty)
                    {
                        documentFinanceMasterParentDocument = (FIN_DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), pParameters.DocumentParent);
                        documentFinanceMaster.DocumentParent = documentFinanceMasterParentDocument;

                        if (documentFinanceMasterParentDocument != null)
                        {
                            //Source Document Document Status: SourceDocument was Invoiced|InvoiceAndPayment|SimplifiedInvoice > change SourceDocument Status to F (Invoiced/Faturado)
                            if (
                                documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                                documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                                documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice
                            )
                            {
                                documentFinanceMasterParentDocument.DocumentStatusStatus = "F";
                            }

                            //Detected in Certification : Credit Notes dont Change Status Details like other Documents
                            if (documentFinanceType.Oid != SettingsApp.XpoOidDocumentFinanceTypeCreditNote)
                            {
                                //Assign Date and User for all Other
                                documentFinanceMasterParentDocument.DocumentStatusDate = documentDateTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime);
                                documentFinanceMasterParentDocument.DocumentStatusUser = userDetail.CodeInternal;
                            }
                            //_log.Debug(String.Format("DocumentNumber: [{0}], DocumentStatusStatus: [{1}], DocumentStatusDate: [{2}], DocumentStatusUser: [{3}]", pParameters.OrderReferences[0].DocumentNumber, pParameters.OrderReferences[0].DocumentStatusStatus, pParameters.OrderReferences[0].DocumentStatusDate, pParameters.OrderReferences[0].DocumentStatusUser));
                        }
                    }

                    //If Has a Valid Customer
                    ERP_Customer customer = (ERP_Customer)uowSession.GetObjectByKey(typeof(ERP_Customer), pParameters.Customer);
                    if (customer != null)
                    {
                        documentFinanceMaster.EntityOid = customer.Oid;
                        //Store CodeInternal to use in SAF-T
                        documentFinanceMaster.EntityInternalCode = customer.CodeInternal;
                        documentFinanceMaster.EntityFiscalNumber = customer.FiscalNumber;
                        //Always Update EntityCountryOid, usefull to AT WebServices to detect Country
                        if (customer.Country != null)
                        {
                            documentFinanceMaster.EntityCountry = customer.Country.Code2;
                            documentFinanceMaster.EntityCountryOid = customer.Country.Oid;
                        }

                        //Only persist Customer Details If is (! Simplified Invoice) AND (! FinalConsumer (in Invoices for ex))
                        if (
                            (
                                documentFinanceMaster.DocumentType.Oid != SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice &&
                                customer.Oid != FrameworkUtils.GetFinalConsumerEntity().Oid
                            )
                            ||
                            //Or If not in Portugal AND (! FinalConsumer (in Invoices for ex))
                            (
                                SettingsApp.XpoOidConfigurationCountryPortugal != SettingsApp.ConfigurationSystemCountry.Oid &&
                                customer.Oid != FrameworkUtils.GetFinalConsumerEntity().Oid
                            )
                            //Required Oids for Equallity Check
                            //Commented to save details if is a Hidden Customer, Diferent from FinalConsumerEntity
                            //&& !FrameworkUtils.IsFinalConsumerEntity(customer.FiscalNumber)
                            )
                        {
                            documentFinanceMaster.EntityName = customer.Name;
                            documentFinanceMaster.EntityAddress = customer.Address;
                            documentFinanceMaster.EntityLocality = customer.Locality;
                            documentFinanceMaster.EntityZipCode = customer.ZipCode;
                            documentFinanceMaster.EntityCity = customer.City;
                            //Deprecated Now Always assign Country, usefull to AT WebServices to detect Country 
                            //if (customer.Country != null)
                            //{
                            //    documentFinanceMaster.EntityCountry = customer.Country.Code2;
                            //    documentFinanceMaster.EntityCountryOid = customer.Country.Oid;
                            //}
                        }
                        //Persist Name if is has a FinalConsumer NIF and Name (Hidden Customer)
                        else if (FrameworkUtils.IsFinalConsumerEntity(customer.FiscalNumber) && customer.Name != string.Empty)
                        {
                            documentFinanceMaster.EntityName = customer.Name;
                        }
                    }

                    //Currency
                    CFG_ConfigurationCurrency configurationCurrency = null;
                    if (pParameters.Currency != null && pParameters.Currency != new Guid()) configurationCurrency = (CFG_ConfigurationCurrency)uowSession.GetObjectByKey(typeof(CFG_ConfigurationCurrency), pParameters.Currency);
                    //Assign Currency to Document
                    if (configurationCurrency != null) documentFinanceMaster.Currency = configurationCurrency;
                    //ExchangeRate
                    documentFinanceMaster.ExchangeRate = pParameters.ExchangeRate;

                    //PaymentMethod
                    FIN_ConfigurationPaymentMethod paymentMethod = null;
                    if (pParameters.PaymentMethod != new Guid())
                    {
                        paymentMethod = (FIN_ConfigurationPaymentMethod)uowSession.GetObjectByKey(typeof(FIN_ConfigurationPaymentMethod), pParameters.PaymentMethod);
                    }
                    //Assign Only it was not Null
                    if (paymentMethod != null)
                    {
                        documentFinanceMaster.PaymentMethod = paymentMethod;
                    }

                    //PaymentMethod
                    FIN_ConfigurationPaymentCondition paymentCondition = null;
                    if (pParameters.PaymentCondition != new Guid())
                    {
                        paymentCondition = (FIN_ConfigurationPaymentCondition)uowSession.GetObjectByKey(typeof(FIN_ConfigurationPaymentCondition), pParameters.PaymentCondition);
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
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoice ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoiceWayBill ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCreditNote ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeBudget ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeProformaInvoice ||
                        documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput ||
                        documentFinanceType.SaftDocumentType == SaftDocumentType.WorkingDocuments ||
                        documentFinanceType.SaftDocumentType == SaftDocumentType.MovementOfGoods
                    )
                    {
                        documentFinanceMaster.DocumentStatusStatus = "N";
                    }
                    //Always assign DocumentStatusDate
                    documentFinanceMaster.DocumentStatusDate = documentDateTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime);

                    //Notes
                    if (pParameters.Notes != String.Empty)
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
                    FIN_DocumentFinanceDetail documentFinanceDetail;
                    Guid vatRateOid;
                    FIN_ConfigurationVatRate vatRate;
                    FIN_ConfigurationVatExemptionReason vatExemptionReason;
                    FIN_Article article;
                    uint lineNumber = 1;

                    foreach (var item in pParameters.ArticleBag)
                    {
                        //Get Article
                        article = (FIN_Article)uowSession.GetObjectByKey(typeof(FIN_Article), item.Key.ArticleOid);
                        //Get VatRate formated for filter, in sql server gives error without this it filters 23,0000 and not 23.0000 resulting in null vatRate
                        string filterVat = FrameworkUtils.DecimalToString(item.Key.Vat, GlobalFramework.CurrentCultureNumberFormat);
                        string executeSql = string.Format(@"SELECT Oid FROM fin_configurationvatrate WHERE Value = '{0}';", filterVat);
                        vatRateOid = FrameworkUtils.GetGuidFromQuery(executeSql);
                        vatRate = (FIN_ConfigurationVatRate)uowSession.GetObjectByKey(typeof(FIN_ConfigurationVatRate), vatRateOid);

                        //If Type dont Have Price it Creates an Empty Details document, always use HavePrice, Only Type : Informative Created by Muga dont use HavePrice
                        if (article.Type.HavePrice)
                        {
                            documentFinanceDetail = new FIN_DocumentFinanceDetail(uowSession)
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
                                CreatedBy = (userDetail != null && userDetail.Oid != Guid.Empty) ? userDetail : null,
                                CreatedWhere = (terminal != null && terminal.Oid != Guid.Empty) ? terminal : null
                            };

                            if (vatRate != null) documentFinanceDetail.VatRate = vatRate;

                            //Assign VatExemptionReason if Set, else Leave Null
                            if (item.Key.VatExemptionReasonOid != new Guid())
                            {
                                //Get Fresh Object to Prevent Mixing Sessions
                                vatExemptionReason = (FIN_ConfigurationVatExemptionReason)uowSession.GetObjectByKey(typeof(FIN_ConfigurationVatExemptionReason), item.Key.VatExemptionReasonOid);
                                documentFinanceDetail.VatExemptionReason = vatExemptionReason;
                                documentFinanceDetail.VatExemptionReasonDesignation = vatExemptionReason.Designation;
                            }

                            //Notes
                            if (item.Value.Notes != null)
                            {
                                documentFinanceDetail.Notes = item.Value.Notes;
                            }

                            //Order References
                            //(4.1|2|3.3.20.2 Referência ao documento de origem)
                            if (pParameters.OrderReferences != null && pParameters.OrderReferences.Count > 0)
                            {
                                uint ord = 0;
                                foreach (FIN_DocumentFinanceMaster documentMaster in pParameters.OrderReferences)
                                {
                                    ord++;
                                    //Require Fresh Objects to Prevent Xpo Mixing Sessions
                                    documentFinanceDetail.OrderReferences.Add(
                                      new FIN_DocumentFinanceDetailOrderReference(uowSession)
                                      {
                                          Ord = ord,
                                          DocumentMaster = (FIN_DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), documentMaster.Oid),
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
                                    new FIN_DocumentFinanceDetailReference(uowSession)
                                    {
                                        Ord = 0,
                                        DocumentMaster = (FIN_DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), item.Value.Reference.Oid),
                                        Reference = item.Value.Reference.DocumentNumber,
                                        Reason = item.Value.Reason
                                    }
                                );
                            }

                            //Inc lineNumber for Ord
                            lineNumber++;

                            //Commissions Work
                            PersistFinanceDocumentCommission(uowSession, documentFinanceDetail, userDetail);
                        }
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Persist Totals

                    FIN_DocumentFinanceMasterTotal documentFinanceMasterTotal;
                    foreach (var item in pParameters.ArticleBag.TaxBag)
                    {
                        //Console.WriteLine(string.Format("{0}\t{1}", item.Key, item.Value));
                        documentFinanceMasterTotal = new FIN_DocumentFinanceMasterTotal(uowSession)
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
                    documentFinanceMaster.DocumentDate = documentDateTime.ToString(SettingsApp.DateTimeFormatDocumentDate);
                    documentFinanceMaster.SystemEntryDate = documentDateTime.ToString(SettingsApp.DateTimeFormatCombinedDateTime);
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
                    documentFinanceMaster.HashControl = SettingsApp.HashControl.ToString();
                    documentFinanceMaster.SourceBilling = "P";
                    documentFinanceMaster.SelfBillingIndicator = 0;
                    documentFinanceMaster.CashVatSchemeIndicator = 0;
                    documentFinanceMaster.ThirdPartiesBillingIndicator = 0;
                    //Store CodeInternals to use in SAF-T
                    documentFinanceMaster.DocumentStatusUser = userDetail.CodeInternal;
                    documentFinanceMaster.DocumentCreatorUser = userDetail.CodeInternal;
                    //CAE is Deprecated, this will prevent triggering Errors
                    if (GlobalFramework.PreferenceParameters.ContainsKey("COMPANY_CAE") && ! string.IsNullOrEmpty(GlobalFramework.PreferenceParameters["COMPANY_CAE"].ToString()))
                    {
                        documentFinanceMaster.EACCode = GlobalFramework.PreferenceParameters["COMPANY_CAE"];
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
                        article = (FIN_Article)uowSession.GetObjectByKey(typeof(FIN_Article), item.Key.ArticleOid);
                        if (article.Type.Oid == SettingsApp.XpoOidArticleClassCustomerCard)
                        {
                            customer.CardCredit = customer.CardCredit + item.Value.TotalFinal;
                        }
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Start Persist WorkSessions

                    //Assign null to Payment Method when DocumentType is a Not Payed Document, ex GR, OR etc
                    if (!documentFinanceType.Payed) paymentMethod = null;

                    //PersistFinanceDocumentWorkSession if document is Payed or if it is a CurrentAccount (Splited in Prints with SplitCurrentAccountMode Enum)
                    if (GlobalFramework.WorkSessionPeriodTerminal != null && (paymentMethod != null || documentFinanceType.Oid == SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput))
                    {
                        //Call PersistFinanceDocumentWorkSession to do WorkSession Job
                        PersistFinanceDocumentWorkSession(uowSession, documentFinanceMaster, pParameters, paymentMethod);
                    }

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Start Commit Changes in UOW

                    try
                    {
                        //Working on OrderMain Mode, if Not TableConsult or CreditNote
                        if (pParameters.SourceMode == PersistFinanceDocumentSourceMode.CurrentOrderMain && documentFinanceType.Oid != SettingsApp.XpoOidDocumentFinanceTypeConferenceDocument)
                        {
                            //Commit UOW Changes : Before get current OrderMain
                            uowSession.CommitChanges();

                            //Get current OrderMain Article Bag, After Process Payment/PartialPayment to check if current OrderMain has Items, or is Empty
                            pParameters.ArticleBag = ArticleBag.TicketOrderToArticleBag(orderMain);
                            if (pParameters.ArticleBag.Count <= 0)
                            {
                                // Warning required to check if (documentOrderMain != null), when we work with SplitPayments and work only one product, 
                                // the 2,3,4....orders are null, this is because first FinanceDocument Closes Order

                                //Close OrderMain
                                if (documentOrderMain != null) documentOrderMain.OrderStatus = OrderStatus.Close;

                                //Required to Update and Sync Terminals
                                if (documentOrderMain != null) documentOrderMain.UpdatedAt = documentDateTime;

                                //Change Table Status to Free
                                POS_ConfigurationPlaceTable placeTable;
                                placeTable = (POS_ConfigurationPlaceTable)FrameworkUtils.GetXPGuidObject(uowSession, typeof(POS_ConfigurationPlaceTable), orderMain.Table.Oid);
                                placeTable.TableStatus = TableStatus.Free;
                                FrameworkUtils.Audit("TABLE_OPEN", string.Format(Resx.audit_message_table_open, placeTable.Designation));
                                placeTable.DateTableClosed = documentDateTime;
                                placeTable.TotalOpen = 0;

                                //Required to Reload Objects after has been changed in Another Session(uowSession)
                                if (documentOrderMain != null) documentOrderMain = (FIN_DocumentOrderMain)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(FIN_DocumentOrderMain), orderMain.PersistentOid);
                                if (documentOrderMain != null) documentOrderMain.Reload();
                                placeTable = (POS_ConfigurationPlaceTable)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(POS_ConfigurationPlaceTable), orderMain.Table.Oid);
                                placeTable.Reload();

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
                            FIN_DocumentFinanceMaster currentAcountDocument;
                            foreach (FIN_DocumentFinanceMaster financeDocument in pParameters.FinanceDocuments)
                            {
                                currentAcountDocument = (FIN_DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceMaster), financeDocument.Oid);
                                currentAcountDocument.Payed = true;
                                currentAcountDocument.PayedDate = documentDateTime;
                                currentAcountDocument.DocumentChild = documentFinanceMaster;
                            }
                        }

                        //Finnaly Commit Changes
                        uowSession.CommitChanges();

                        //Always Assign DocumentChild Reference to DocumentParent
                        if (documentFinanceMasterParentDocument != null)
                        {
                            //documentFinanceMaster.DocumentParent = (DocumentFinanceMaster)uowSession.GetObjectByKey(typeof(DocumentFinanceMaster), pParameters.DocumentParent);
                            documentFinanceMasterParentDocument.DocumentChild = documentFinanceMaster;
                            uowSession.CommitChanges();
                        }

                        //Audit
                        FrameworkUtils.Audit("FINANCE_DOCUMENT_CREATED", string.Format("{0} {1}: {2}", documentFinanceMaster.DocumentType.Designation, Resx.global_document_created, documentFinanceMaster.DocumentNumber));

                        //Process Stock
                        ProcessArticleStock.Add(documentFinanceMaster);

                        //Assign Document to Result
                        result = documentFinanceMaster;

                        // Call Generate GenerateDocument
                        GenerateDocumentFinanceMasterPDFIfNotExists(documentFinanceMaster);
                    }
                    catch (Exception ex)
                    {
                        uowSession.RollbackTransaction();
                        _log.Error(ex.Message, ex);
                        throw new Exception("ERROR_COMMIT_FINANCE_DOCUMENT", ex.InnerException);
                    }
                }
            }
            catch (ProcessFinanceDocumentValidationException ex)
            {
                _log.Error(ex.Message, ex);
                throw new Exception(ex.Message, ex.InnerException);
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

        public static string GenDocumentHash(Session pSession, FIN_DocumentFinanceType pDocType, FIN_DocumentFinanceSeries pDocSerie, FIN_DocumentFinanceMaster pDocumentFinanceMaster)
        {
            bool debug = false;
            string resultSignedHash = "";
            FIN_DocumentFinanceMaster doc = pDocumentFinanceMaster;
            //Required to ALways use "." and not Culture Decimal separator, ex ","
            //string TotalFinalRound = FrameworkUtils.DecimalToString(doc.TotalFinalRound).Replace(',', '.');
            string TotalFinalRound = FrameworkUtils.DecimalToString(Convert.ToDecimal(doc.TotalFinalRound), GlobalFramework.CurrentCultureNumberFormat);
            //Hash must be the first, to use first field has value for ExecuteScalar
            string sql = string.Format(@"SELECT Hash, HashControl, Date AS InvoiceDate, SystemEntryDate, DocumentNumber AS InvoiceNo, TotalFinalRound FROM fin_documentfinancemaster WHERE DocumentType = '{0}' and  DocumentSerie = '{1}' ORDER BY Date DESC;", pDocType.Oid, pDocSerie.Oid);
            var olastDocumentHash = pSession.ExecuteScalar(sql);
            string lastDocumentHash = (olastDocumentHash != null) ? olastDocumentHash.ToString() : "";
            string signTargetString = string.Format("{0};{1};{2};{3};{4}", doc.DocumentDate, doc.SystemEntryDate, doc.DocumentNumber, TotalFinalRound, lastDocumentHash);
            
            // Old Method without Plugin
            //resultSignedHash = FrameworkUtils.SignDataToSHA1Base64(signTargetString, debug);
            // Sign Document if has a valid PluginSoftwareVendor 
            if (GlobalFramework.PluginSoftwareVendor != null)
            {
                resultSignedHash = GlobalFramework.PluginSoftwareVendor.SignDataToSHA1Base64(SettingsApp.SecretKey, signTargetString, debug);
            }
            else
            {
                // Dont Sign it without SoftwareVendor
                resultSignedHash = null;
            }

            //Debug
            if (debug) _log.Debug(string.Format("GenDocumentHash(): #{0}", doc.DocumentNumber));
            if (debug) _log.Debug(string.Format("GenDocumentHash(): lastDocumentHash [{0}]", lastDocumentHash));
            if (debug) _log.Debug(string.Format("GenDocumentHash(): signTargetString [{0}]", signTargetString));
            if (debug) _log.Debug(string.Format("GenDocumentHash(): resultSignedHash [{0}]", resultSignedHash));
            if (debug) _log.Debug(string.Format("GenDocumentHash(): sql [{0}]", sql));

            return resultSignedHash;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GenDocumentNumber

        public static string GenDocumentNumber(FIN_DocumentFinanceSeries pDocType)
        {
            string documentNumber = "INVALID_DOCUMENT_NUMBER";

            try
            {
                string formatNumber = new string('0', SettingsApp.DocumentsPadLength);
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
                _log.Error(ex.Message, ex);
                return documentNumber;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //GenDocumentHash4Chars

        //Get 1º, 11º, 21º, 31º From Hash, Required for Printed Versions (Reports and Tickets)
        public static string GenDocumentHash4Chars(string pHash)
        {
            // Protection In case of bad hash, ex when we dont have SoftwareVendorPlugin Registered
            if (string.IsNullOrEmpty(pHash))
            {
                throw new Exception(Resx.dialog_message_error_creating_financial_document_bad_hash_detected);
            }
            else
            {
                string a = pHash.Substring(1 - 1, 1);
                string b = pHash.Substring(11 - 1, 1);
                string c = pHash.Substring(21 - 1, 1);
                string d = pHash.Substring(31 - 1, 1);

                //_log.Debug(string.Format("pHash: [{0}] [{1}][{2}][{3}][{4}]", pHash, a, b, c, d));
                //Ex.: Result [wESm]
                //wQ5dp/AesYEgM9QFlh8aSyfIcpJIDnm+Z8cr4PNsmF7AoxIR9+EU8vIq2PDXE7aIMYH0j.....
                //[w]:w
                //[E]:wQ5dp/AesYE
                //[S]:wQ5dp/AesYEgM9QFlh8aS
                //[m]_wQ5dp/AesYEgM9QFlh8aSyfIcpJIDnm

                return string.Format("{0}{1}{2}{3}", a, b, c, d);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Process Payments/Recibos

        public static FIN_DocumentFinancePayment PersistFinanceDocumentPayment(List<FIN_DocumentFinanceMaster> pInvoices, List<FIN_DocumentFinanceMaster> pCreditNotes, Guid pCustomer, Guid pPaymentMethod, Guid pConfigurationCurrency, decimal pPaymentAmount, string pPaymentNotes = "")
        {
            //Proccess Validation
            SortedDictionary<FinanceValidationError, object> errorsValidation = ProcessFinanceDocumentValidation.ValidatePersistFinanceDocumentPayment(pInvoices, pCreditNotes, pCustomer, pPaymentMethod, pConfigurationCurrency, pPaymentAmount, pPaymentNotes);
            if (errorsValidation.Count > 0)
            {
                ProcessFinanceDocumentValidationException exception = new ProcessFinanceDocumentValidationException(new Exception("ERROR_DETECTED"), errorsValidation);
                //Throw without Errors only Exception for Muga Work, Return a simple String
                //throw exception.Exception;
                // Send with ExceptionErrors
                throw exception;
            }

            //Settings
            string dateTimeFormatDocumentDate = SettingsApp.DateTimeFormatDocumentDate;
            string dateTimeFormatCombinedDateTime = SettingsApp.DateTimeFormatCombinedDateTime;
            int decimalRoundTo = SettingsApp.DecimalRoundTo;

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
            string sql = String.Empty;
            string extended = String.Empty;
            //CurrentDateTime
            DateTime currentDateTime = FrameworkUtils.CurrentDateTimeAtomic();
            //XpoObjects
            FIN_DocumentFinanceMaster documentMaster = null;
            FIN_DocumentFinancePayment documentFinancePayment = null;
            FIN_DocumentFinanceMasterPayment documentFinanceMasterPayment = null;

            //Start UnitOfWork
            using (UnitOfWork uowSession = new UnitOfWork())
            {
                try
                {
                    //Get DocumentType
                    FIN_DocumentFinanceType documentFinanceType = (FIN_DocumentFinanceType)uowSession.GetObjectByKey(typeof(FIN_DocumentFinanceType), SettingsApp.XpoOidDocumentFinanceTypePayment);
                    if (documentFinanceType == null)
                    {
                        throw new Exception("ERROR_MISSING_DOCUMENT_TYPE");
                    }

                    //Get UserDetail
                    SYS_UserDetail userDetail = (SYS_UserDetail)uowSession.GetObjectByKey(typeof(SYS_UserDetail), GlobalFramework.LoggedUser.Oid);
                    //Get Document Serie
                    FIN_DocumentFinanceSeries documentFinanceSerie = null;
                    FIN_DocumentFinanceYearSerieTerminal documentFinanceYearSerieTerminal = null;
                    //Get Document Serie for current Terminal
                    documentFinanceYearSerieTerminal = ProcessFinanceDocumentSeries.GetDocumentFinanceYearSerieTerminal(uowSession, SettingsApp.XpoOidDocumentFinanceTypePayment);
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
                    if (debug) _log.Debug(string.Format("documentNumber: [{0}]", documentNumber));

                    //Get Fresh UOW Objects
                    ERP_Customer customer = (ERP_Customer)FrameworkUtils.GetXPGuidObject(uowSession, typeof(ERP_Customer), pCustomer);
                    FIN_ConfigurationPaymentMethod paymentMethod = (FIN_ConfigurationPaymentMethod)FrameworkUtils.GetXPGuidObject(uowSession, typeof(FIN_ConfigurationPaymentMethod), pPaymentMethod);

                    //Initialize Payment/Recibo (Many(Invoices&CreditNotes) 2 One(Payment))
                    if (pInvoices.Count > 0)
                    {
                        //DocumentFinancePayment: Payments
                        documentFinancePayment = new FIN_DocumentFinancePayment(uowSession);
                        documentFinancePayment.PaymentRefNo = documentNumber;
                        documentFinancePayment.TransactionDate = currentDateTime.ToString(dateTimeFormatDocumentDate);
                        documentFinancePayment.PaymentType = "RG"; //Outros recibos emitidos
                        documentFinancePayment.PaymentStatus = "N";//Recibo normal e vigente
                        documentFinancePayment.PaymentDate = currentDateTime.ToString(dateTimeFormatDocumentDate); ;
                        documentFinancePayment.DocumentDate = currentDateTime.ToString(dateTimeFormatDocumentDate);
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
                        if (pPaymentNotes != String.Empty) documentFinancePayment.Notes = pPaymentNotes;
                    }

                    //Get Default defaultCurrency
                    CFG_ConfigurationCurrency defaultCurrency = SettingsApp.ConfigurationSystemCurrency;
                    //Currency - If Diferent from Default System Currency, get Currency Object from Parameter
                    CFG_ConfigurationCurrency configurationCurrency;
                    if (SettingsApp.ConfigurationSystemCurrency.Oid != pConfigurationCurrency)
                    {
                        configurationCurrency = (CFG_ConfigurationCurrency)uowSession.GetObjectByKey(typeof(CFG_ConfigurationCurrency), pConfigurationCurrency);
                    }
                    //Default Currency
                    else
                    {
                        //configurationCurrency = defaultCurrency;
                        //Fix for LogicErp else diferent Sessions Occur : Get Object in this Session
                        configurationCurrency = (CFG_ConfigurationCurrency)uowSession.GetObjectByKey(typeof(CFG_ConfigurationCurrency), defaultCurrency.Oid); ;
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
                    foreach (FIN_DocumentFinanceMaster item in pCreditNotes)
                    {
                        //If TotalCredit Greater than Current Credit Note, we Use this Credit Note, Else we Skip it and remain Untoutched for future uses
                        //if (Math.Round(totalCredit, decimalRoundTo) > Math.Round(item.TotalFinal, decimalRoundTo))
                        //{
                        //Get Fresh Object in UOW
                        documentMaster = (FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(uowSession, typeof(FIN_DocumentFinanceMaster), item.Oid);
                        //Persist DocumentFinanceMaster Payment 
                        documentMaster.Payed = true;
                        documentMaster.PayedDate = currentDateTime;
                        totalCreditNotes += item.TotalFinal;
                        totalCredit += item.TotalFinal;
                        //DocumentFinanceMasterPayment: Many2Many Connection
                        documentFinanceMasterPaymentOrd++;
                        documentFinanceMasterPayment = new FIN_DocumentFinanceMasterPayment(uowSession);
                        documentFinanceMasterPayment.Ord = documentFinanceMasterPaymentOrd;
                        documentFinanceMasterPayment.DebitAmount = item.TotalFinal;
                        documentFinanceMasterPayment.DocumentFinanceMaster = documentMaster;
                        documentFinanceMasterPayment.DocumentFinancePayment = documentFinancePayment;
                        if (debug) _log.Debug(string.Format("DocumentAcronym:[{0}] DocumentNumber:[{1}] DocumentValue: [{2}], TotalCredit: [{3}]", item.DocumentType.Acronym, item.DocumentNumber, item.TotalFinal, totalCredit));
                        //}
                    }

                    //Process Invoices/FTs
                    foreach (FIN_DocumentFinanceMaster item in pInvoices)
                    {
                        if (Math.Round(totalCredit, decimalRoundTo) > 0)
                        {
                            //This Query Exists 3 Locations, Find it and change in all Locations - Required "GROUP BY fmaOid,fmaTotalFinal" to work with SQLServer
                            sql = string.Format("SELECT fmaTotalFinal - SUM(fmpCreditAmount) as Result FROM view_documentfinancepayment WHERE fmaOid = '{0}' AND fpaPaymentStatus <> 'A' GROUP BY fmaOid,fmaTotalFinal;", item.Oid);
                            documentDebit = Convert.ToDecimal(GlobalFramework.SessionXpo.ExecuteScalar(sql));
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

                            if (debug) _log.Debug(string.Format("[{0}] PayRemain: [{1}], PayInCurrent: [{2}], RemainToPay: [{3}], Credit: [{4}], totalPayed: [{5}], PartialPayed: [{6}], FullPayed: [{7}]", item.DocumentNumber, documentTotalPayRemain, totalToPayInCurrentInvoice, (documentTotalPayRemain - totalToPayInCurrentInvoice), totalCredit, totalPayed, documentPartialPayed, documentFullPayed));

                            //Always Get Fresh Object in UOW, used to Assign to Full and Partial Payments, need to be Outside FullPayed
                            documentMaster = (FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(uowSession, typeof(FIN_DocumentFinanceMaster), item.Oid);

                            //Persist DocumentFinanceMaster Payment if FullPayed
                            if (documentFullPayed)
                            {
                                documentMaster.Payed = true;
                                documentMaster.PayedDate = currentDateTime;

                                //On Full Invoice Payment Call ChangePayedInvoiceAndRelatedDocumentsStatus (Change status of Parent Document to F)
                                string statusReason = string.Format(Resx.global_documents_status_document_invoiced, documentMaster.DocumentNumber);
                                //Get Fresh Object in UOW
                                FIN_DocumentFinanceMaster documentParent = null;
                                //Send with UOW Objects
                                if (item.DocumentParent != null)
                                {
                                    documentParent = (FIN_DocumentFinanceMaster)FrameworkUtils.GetXPGuidObject(uowSession, typeof(FIN_DocumentFinanceMaster), item.DocumentParent.Oid);
                                    ChangePayedInvoiceRelatedDocumentsStatus(uowSession, documentParent, statusReason, documentMaster.DocumentStatusDate, userDetail);
                                }
                            }

                            //Persist if Partial Payed or FullPayed (If is Partial is Full Too)
                            if (documentPartialPayed)
                            {
                                //DocumentFinanceMasterPayment: Many2Many Connection
                                documentFinanceMasterPaymentOrd++;
                                documentFinanceMasterPayment = new FIN_DocumentFinanceMasterPayment(uowSession);
                                documentFinanceMasterPayment.Ord = documentFinanceMasterPaymentOrd;
                                documentFinanceMasterPayment.CreditAmount = totalToPayInCurrentInvoice;
                                documentFinanceMasterPayment.DocumentFinanceMaster = documentMaster;
                                documentFinanceMasterPayment.DocumentFinancePayment = documentFinancePayment;
                                //if(debug) _log.Debug(string.Format("  [{0}], Persisted PaymentAmount: [{1}]", documentMaster.DocumentNumber, totalToPayInCurrentInvoice));
                            }
                        }
                    }

                    //Assign Total Payed Document, the Diference from totalPayed and totalCreditNotes, the Amount the Client has Payed
                    totalPayedDocument = totalPayed - totalCreditNotes;

                    //Get ExtendedValue
                    ExtendValue extendValue = new ExtendValue();
                    extended = extendValue.GetExtendedValue(totalPayedDocument, defaultCurrency.Designation);
                    if (debug) _log.Debug(string.Format("extended: [{0}]", extended));

                    //Now we can Assign PaymentAmount (Credit-Debit Diference)
                    documentFinancePayment.PaymentAmount = totalPayedDocument;
                    documentFinancePayment.CurrencyAmount = totalPayedDocument * documentFinancePayment.ExchangeRate;
                    documentFinancePayment.ExtendedValue = extended;

                    //Call PersistFinanceDocumentWorkSession to do WorkSession Job
                    PersistFinanceDocumentWorkSession(uowSession, documentFinancePayment, paymentMethod);

                    //Commit UOW Changes : Before get current OrderMain
                    uowSession.CommitChanges();

                    // Call Generate GenerateDocument
                    GenerateDocumentFinancePaymentPDFIfNotExists(documentFinancePayment);
                }
                catch (Exception ex)
                {
                    uowSession.RollbackTransaction();
                    _log.Error(ex.Message, ex);
                    //2016-01-05 apmuga passar erro para cima e não mascarar com outro erro
                    //throw new Exception("ERROR_COMMIT_FINANCE_DOCUMENT_PAYMENT", ex.InnerException);
                    throw ex;

                }
            }

            return documentFinancePayment;
        }

        //Recursive Method : Change Document Status of related invoice Documents : Change to (“F” — Documento faturado)
        public static bool ChangePayedInvoiceRelatedDocumentsStatus(Session pSession, FIN_DocumentFinanceMaster pDocumentFinanceMaster, string pStatusReason, string pCombinedDateTime, SYS_UserDetail pUserDetail)
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
                if (debug) _log.Debug(string.Format("ChangePayedInvoiceStatus On: [{0}] to [{1}]", pDocumentFinanceMaster.DocumentNumber, pDocumentFinanceMaster.DocumentStatusStatus));

                //Call Recursive Method on Parent Again, until it is NULL (No Parent)
                if (pDocumentFinanceMaster.DocumentParent != null) ChangePayedInvoiceRelatedDocumentsStatus(pSession, pDocumentFinanceMaster.DocumentParent, pStatusReason, pCombinedDateTime, pUserDetail);

                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Process

        //Used with DocumentFinancePayment
        public static bool PersistFinanceDocumentWorkSession(Session pSession, FIN_DocumentFinancePayment pDocumentFinancePayment, FIN_ConfigurationPaymentMethod pPaymentMethod)
        {
            return PersistFinanceDocumentWorkSession(pSession, null, null, pDocumentFinancePayment, pPaymentMethod);
        }

        //Used With DocumentFinanceMaster
        public static bool PersistFinanceDocumentWorkSession(Session pSession, FIN_DocumentFinanceMaster pDocumentFinanceMaster, ProcessFinanceDocumentParameter pParameters, FIN_ConfigurationPaymentMethod pPaymentMethod)
        {
            return PersistFinanceDocumentWorkSession(pSession, pDocumentFinanceMaster, pParameters, null, pPaymentMethod);
        }

        //Main Method
        public static bool PersistFinanceDocumentWorkSession(Session pSession, FIN_DocumentFinanceMaster pDocumentFinanceMaster, ProcessFinanceDocumentParameter pParameters, FIN_DocumentFinancePayment pDocumentFinancePayment, FIN_ConfigurationPaymentMethod pPaymentMethod)
        {
            bool result = false;

            try
            {
                //Get Period WorkSessionPeriodTerminal, UserDetail and Terminal
                POS_WorkSessionPeriod workSessionPeriod = (POS_WorkSessionPeriod)FrameworkUtils.GetXPGuidObject(pSession, typeof(POS_WorkSessionPeriod), GlobalFramework.WorkSessionPeriodTerminal.Oid);
                SYS_UserDetail userDetail = (SYS_UserDetail)FrameworkUtils.GetXPGuidObject(pSession, typeof(SYS_UserDetail), GlobalFramework.LoggedUser.Oid);
                POS_ConfigurationPlaceTerminal configurationPlaceTerminal = (POS_ConfigurationPlaceTerminal)FrameworkUtils.GetXPGuidObject(pSession, typeof(POS_ConfigurationPlaceTerminal), GlobalFramework.LoggedTerminal.Oid);

                //Variables to diferent Document Types : DocumentFinanceMaster or DocumentFinancePayment
                DateTime documentDate = FrameworkUtils.CurrentDateTimeAtomic();
                decimal movementAmount = 0m;
                string movementDescriptionDocument = String.Empty;
                string movementDescriptionTotalDelivery = String.Empty;
                string movementDescriptionTotalChange = String.Empty;
                decimal totalDelivery = 0m;
                decimal totalChange = 0m;

                if (pDocumentFinanceMaster != null)
                {
                    documentDate = pDocumentFinanceMaster.Date;
                    movementAmount = pDocumentFinanceMaster.TotalFinal;
                    movementDescriptionDocument = string.Format("{0} : {1}", pDocumentFinanceMaster.DocumentNumber, Resx.global_finance_document);
                    movementDescriptionTotalDelivery = string.Format("{0} : {1}", pDocumentFinanceMaster.DocumentNumber, Resx.global_total_deliver);
                    movementDescriptionTotalChange = string.Format("{0} : {1}", pDocumentFinanceMaster.DocumentNumber, Resx.global_total_change);
                    if (pParameters.TotalDelivery > 0) totalDelivery = pParameters.TotalDelivery;
                    if (pParameters.TotalChange > 0) totalChange = pParameters.TotalChange;
                }
                else if (pDocumentFinancePayment != null)
                {
                    documentDate = pDocumentFinancePayment.CreatedAt;//.PaymentDate
                    movementAmount = pDocumentFinancePayment.PaymentAmount;
                    movementDescriptionDocument = string.Format("{0} : {1}", pDocumentFinancePayment.PaymentRefNo, Resx.global_payment_document);
                    movementDescriptionTotalDelivery = string.Format("{0} : {1}", pDocumentFinancePayment.PaymentRefNo, Resx.global_total_deliver);
                    movementDescriptionTotalChange = string.Format("{0} : {1}", pDocumentFinancePayment.PaymentRefNo, Resx.global_total_change);
                    //TODO: Improve with Payment TotalChange Functionality
                    totalDelivery = movementAmount;
                }
                else
                {
                    return false;
                }

                //Persist DocumentFinance Movement
                ProcessWorkSessionMovement.PersistWorkSessionMovement(
                    pSession,
                    workSessionPeriod,
                    (POS_WorkSessionMovementType)FrameworkUtils.GetXPGuidObjectFromField(pSession, typeof(POS_WorkSessionMovementType), "Token", "FINANCE_DOCUMENT"),
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
                    ProcessWorkSessionMovement.PersistWorkSessionMovement(
                        pSession,
                        workSessionPeriod,
                        (POS_WorkSessionMovementType)FrameworkUtils.GetXPGuidObjectFromField(pSession, typeof(POS_WorkSessionMovementType), "Token", "CASHDRAWER_IN"),
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
                        if (pParameters.TotalChange > 0) ProcessWorkSessionMovement.PersistWorkSessionMovement(
                            pSession,
                            workSessionPeriod,
                            (POS_WorkSessionMovementType)FrameworkUtils.GetXPGuidObjectFromField(pSession, typeof(POS_WorkSessionMovementType), "Token", "CASHDRAWER_OUT"),
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
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Get Last FinanceDocument DateTime

        public static DateTime GetLastDocumentDateTime()
        {
            return GetLastDocumentDateTime(GlobalFramework.SessionXpo, null);
        }

        public static DateTime GetLastDocumentDateTime(string pFilter)
        {
            return GetLastDocumentDateTime(GlobalFramework.SessionXpo, pFilter);
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
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //ProcessArticleStock Commisions

        private static void PersistFinanceDocumentCommission(Session pSession, FIN_DocumentFinanceDetail pDocumentFinanceDetail, SYS_UserDetail pUserDetail)
        {
            try
            {
                POS_UserCommissionGroup commissionGroup = null;

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
                    POS_ConfigurationPlaceTerminal configurationPlaceTerminal = (POS_ConfigurationPlaceTerminal)FrameworkUtils.GetXPGuidObject(pSession, typeof(POS_ConfigurationPlaceTerminal), GlobalFramework.LoggedTerminal.Oid);
                    decimal totalCommission = (pDocumentFinanceDetail.TotalNet * pUserDetail.CommissionGroup.Commission) / 100;

                    FIN_DocumentFinanceCommission documentFinanceCommission = new FIN_DocumentFinanceCommission(pSession)
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
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Documents

        public static string GenerateDocumentFinanceMasterPDFIfNotExists(FIN_DocumentFinanceMaster documentFinanceMaster)
        {
            string result = string.Empty;
            
            try
            {
                //Generate Documents Filename
                if (!string.IsNullOrEmpty(GlobalFramework.Settings["generatePdfDocuments"]))
                {
                    bool generatePdfDocuments = false;
                    try
                    {
                        generatePdfDocuments = Convert.ToBoolean(GlobalFramework.Settings["generatePdfDocuments"]);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }

                    if (generatePdfDocuments)
                    {
                        string entityName = (!string.IsNullOrEmpty(documentFinanceMaster.EntityName)) ? string.Format("_{0}", documentFinanceMaster.EntityName.ToLower().Replace(' ', '_')) : string.Empty;
                        string reportFilename = string.Format("{0}/{1}{2}.pdf",
                            GlobalFramework.Path["documents"],
                            documentFinanceMaster.DocumentNumber.Replace('/', '-').Replace(' ', '_'),
                            entityName
                        );
                        
                        if (! File.Exists(reportFilename))
                        {
                            result = CustomReport.DocumentMasterCreatePDF(CustomReportDisplayMode.ExportPDFSilent, documentFinanceMaster, reportFilename);
                        }
                        else
                        {
                            result = reportFilename;
                        }
                        
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }

        public static string GenerateDocumentFinancePaymentPDFIfNotExists(FIN_DocumentFinancePayment documentFinancePayment)
        {
            string result = string.Empty;
            
            try
            {
                //Generate Documents Filename
                if (!string.IsNullOrEmpty(GlobalFramework.Settings["generatePdfDocuments"]))
                {
                    bool generatePdfDocuments = false;
                    try
                    {
                        generatePdfDocuments = Convert.ToBoolean(GlobalFramework.Settings["generatePdfDocuments"]);
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                    }

                    if (generatePdfDocuments)
                    {
                        ERP_Customer customer = (ERP_Customer)FrameworkUtils.GetXPGuidObject(GlobalFramework.SessionXpo, typeof(ERP_Customer), documentFinancePayment.EntityOid);
                        string entityName = (customer != null && !string.IsNullOrEmpty(customer.Name)) ? string.Format("_{0}", customer.Name.ToLower().Replace(' ', '_')) : string.Empty;
                        string reportFilename = string.Format("{0}/{1}{2}.pdf",
                            GlobalFramework.Path["documents"],
                            documentFinancePayment.PaymentRefNo.Replace('/', '-').Replace(' ', '_'),
                            entityName
                        );
                        
                        if (! File.Exists(reportFilename))
                        {
                            result = CustomReport.DocumentPaymentCreatePDF(CustomReportDisplayMode.ExportPDFSilent, documentFinancePayment, reportFilename);
                        }
                        else
                        {
                            result = reportFilename;
                        }
                        
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
