using Gtk;
using logicpos.App;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial.library.Classes.Finance;
using logicpos.financial.library.Classes.Reports;
using logicpos.resources.Resources.Localization;
using logicpos.shared.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    // Use PrintRouter for financialDocuments ex PrintFinanceDocument

    partial class PosReportsDialog
    {
        public static CustomReportDisplayMode exportType;
        // Test Document Report
        void TestDocumentReport()
        {
            Guid docOid = new Guid("6d44b0a8-6450-4245-b4ee-a6e971f4bcec");
            fin_documentfinancemaster documentFinanceMaster = (fin_documentfinancemaster)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_documentfinancemaster), docOid);
            if (documentFinanceMaster != null)
            {
                //Generate Default CopyNames from DocumentType
                List<int> copyNames = CustomReport.CopyNames(documentFinanceMaster.DocumentType.PrintCopies);
                string hash4Chars = ProcessFinanceDocument.GenDocumentHash4Chars(documentFinanceMaster.Hash);
                string destinationFileName = "";
                string result = CustomReport.ProcessReportFinanceDocument(CustomReportDisplayMode.Design, documentFinanceMaster.Oid, hash4Chars, copyNames, destinationFileName);
                _log.Debug(String.Format("Result: [{0}]", result));
            }
            else
            {
                _log.Debug(String.Format("Null Document Found for documentType: [{0}]", nameof(fin_documentfinancemaster), docOid.ToString()));
            }
        }

        void buttonReportUnderConstruction_Clicked(object sender, EventArgs e)
        {
            Utils.ShowMessageUnderConstruction();
        }

        private List<string> GetReportsQueryDialogFilter(ReportsQueryDialogMode pReportsQueryDialogMode, string pDatabaseSourceObject)
        {
            PosReportsQueryDialog dialog = new PosReportsQueryDialog(_sourceWindow, DialogFlags.DestroyWithParent, pReportsQueryDialogMode, pDatabaseSourceObject);
            ResponseType response = (ResponseType)dialog.Run();
            List<string> result = new List<string>();
            // Filter SellDocuments
            string filterField = string.Empty;
            string statusField = string.Empty;
            bool filterSellDocuments = false;
            string extraFilter = string.Empty;
            //Added Response for Export to excel, in this point both Ok and export to Excel are the same
            if (response == ResponseType.Ok || response == (ResponseType)DialogResponseType.ExportXls || response == (ResponseType)DialogResponseType.ExportPdf)
            {
                // Filter SellDocuments
                if (pReportsQueryDialogMode.Equals(ReportsQueryDialogMode.FINANCIAL))
                {
                    filterSellDocuments = true;
                    filterField = "DocumentType";
                    statusField = "DocumentStatusStatus";
                }
                else if (pReportsQueryDialogMode.Equals(ReportsQueryDialogMode.FINANCIAL_DETAIL) || pReportsQueryDialogMode.Equals(ReportsQueryDialogMode.FINANCIAL_DETAIL_GROUP))
                {
                    filterSellDocuments = true;
                    filterField = "ftOid";
                    statusField = "fmDocumentStatusStatus";
                }

                // Add extraFilter for SellDocuments
                if (filterSellDocuments == true)
                {
					/* IN009066 - FS and NC added to reports */
                    extraFilter = $@" AND ({statusField} <> 'A') AND (
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoice}' OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeConsignationInvoice}' OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeDebitNote}' OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCreditNote}' OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypePayment}' 
OR 
{filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}'
)".Replace(Environment.NewLine, string.Empty);
                    /* IN009089 - # TO DO: above, we need to check with business this condition:  {filterField} = '{SettingsApp.XpoOidDocumentFinanceTypeCurrentAccountInput}' */
                }

                // Assign Dialog FilterValue to Method Result Value
                result.Add($"{dialog.FilterValue}{extraFilter}");
                result.Add(dialog.FilterValueHumanReadble);
            }
            else
            {
                // Destroy Dialog on Cancel
                dialog.Destroy();
                // Assign Result
                result = null;
            }

            return result;
        }
        public Type senderType;
        public void PrintReportRouter(object sender, EventArgs e)
        {
            //CustomReportDisplayMode displayMode = (Debugger.IsAttached)
            //    ? CustomReportDisplayMode.Design
            //    : CustomReportDisplayMode.ExportPDF;
            CustomReportDisplayMode displayMode = exportType;

            // Override Default Development Mode
            // Local Variables
            string reportFilter = string.Empty;
            string reportFilterHumanReadable = string.Empty;
            string databaseSourceObject = string.Empty;

            dynamic button;

            senderType = sender.GetType();

            if(senderType.Name == "AccordionChildButton")
            {
                button = (sender as AccordionChildButton);
            }
            else
            {
                button = (sender as TouchButtonIconWithText);
            }
            //TouchButtonIconWithText buttonIcon = (sender as TouchButtonIconWithText);
            //AccordionChildButton button = (sender as AccordionChildButton);
            //_log.Debug(String.Format("Button.Name: [{0}], Button.label: [{1}]", button.Name, button.Label));

            // Get Token From buttonName
            ReportsTypeToken token = (ReportsTypeToken)Enum.Parse(typeof(ReportsTypeToken), button.Name, true);
            _log.Debug("void PrintReportRouter(object sender, EventArgs e) :: ReportsTypeToken: " + token.ToString());
			//TK016249 - Impressoras - Diferenciação entre Tipos
            GlobalFramework.UsingThermalPrinter = true;
            // Prepare ReportsQueryDialogMode
            ReportsQueryDialogMode reportsQueryDialogMode = ReportsQueryDialogMode.UNDEFINED;
            // Catch REPORT_SALES_DETAIL_* and REPORT_SALES_DETAIL_GROUP_* use same View
             if (token.ToString().StartsWith("REPORT_SALES_DETAIL_"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL;
                databaseSourceObject = "view_documentfinance";
            }
            else if (token.ToString().StartsWith("REPORT_SALES_"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL;
                databaseSourceObject = "fin_documentfinancemaster";
            }
            else if (token.ToString().Equals("REPORT_LIST_STOCK_MOVEMENTS"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS;
                databaseSourceObject = "view_articlestockmovement";
            }
            else if (token.ToString().Equals("REPORT_LIST_AUDIT_TABLE"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.SYSTEM_AUDIT;
                databaseSourceObject = "view_systemaudit";
            }
            else if (token.ToString().Equals("REPORT_LIST_CURRENT_ACCOUNT"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.CURRENT_ACCOUNT;
                databaseSourceObject = "view_documentfinancecurrentaccount";
            }
            /* IN008018 */
            else if (token.ToString().Equals("REPORT_CUSTOMER_BALANCE_DETAILS"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.CUSTOMER_BALANCE_DETAILS;
                databaseSourceObject = "view_documentfinancecustomerbalancedetails";
            }
            /* IN009010 */
            else if (token.ToString().Equals("REPORT_CUSTOMER_BALANCE_SUMMARY"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY;
                databaseSourceObject = "view_documentfinancecustomerbalancesummary";
            }
            /* IN009204 - based on CUSTOMER_BALANCE_DETAILS report */
            else if (token.ToString().Equals("REPORT_COMPANY_BILLING"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.COMPANY_BILLING;
                databaseSourceObject = "view_documentfinancecustomerbalancedetails";
            }
            else if (token.ToString().Equals("REPORT_LIST_USER_COMMISSION"))
            {
                reportsQueryDialogMode = ReportsQueryDialogMode.USER_COMMISSION;
                databaseSourceObject = "view_usercommission";
            }

            // Common GetReportsQueryDialogFilter for All Non Undefined ReportsQueryDialogMode 
            if (reportsQueryDialogMode != ReportsQueryDialogMode.UNDEFINED)
            {
                // Call PosReportsQueryDialog to get Filter
                List<string> dialogresultFilter = GetReportsQueryDialogFilter(reportsQueryDialogMode, databaseSourceObject);
                if (dialogresultFilter != null && dialogresultFilter.Count == 2)
                {
                    reportFilter = dialogresultFilter[0];
                    reportFilterHumanReadable = dialogresultFilter[1];
                }
                // ResponseType.Cancel
                else
                {
                    reportFilter = null;
                    reportFilterHumanReadable = null;
                }
            }

            // Proceed if we have a Filter != null (ResponseType.Ok), ex can be a string.Empty
            //if (reportFilter != null || !financialViewMode)
            if (reportFilter != null/* || reportsQueryDialogMode != ReportsQueryDialogMode.UNDEFINED*/)
            {
            	// Now we have two types of export according the Button response
                displayMode = exportType;
                switch (token)
                {
                    case ReportsTypeToken.REPORT_SALES_PER_FINANCE_DOCUMENT:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.DocumentType.Ord]"
                            , "([DocumentFinanceMaster.DocumentType.Code]) [DocumentFinanceMaster.DocumentType.Designation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_DATE:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.DocumentDate]"
                            , "[DocumentFinanceMaster.DocumentDate]",
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_USER:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.CreatedBy.Ord]"
                            , "([DocumentFinanceMaster.CreatedBy.Code]) [DocumentFinanceMaster.CreatedBy.Name]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_TERMINAL:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.CreatedWhere.Ord]"
                            , "([DocumentFinanceMaster.CreatedWhere.Code]) [DocumentFinanceMaster.CreatedWhere.Designation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_CUSTOMER:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.EntityFiscalNumber]"
                            , "[DocumentFinanceMaster.EntityFiscalNumber] / [DocumentFinanceMaster.EntityName]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_PAYMENT_METHOD:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.PaymentMethod.Ord]"
                            , "([DocumentFinanceMaster.PaymentMethod.Code]) [DocumentFinanceMaster.PaymentMethod.Designation]",/* IN009066 - Duplicate of REPORT_SALES_PER_PAYMENT_CONDITION */
                            /* IN009066 - Faturas and Notas de Crédito were not in this report, because they have no Payment Method. Now the issue is fixed */
                            // Required to Exclude Documents without PaymentMethod else Errors Occur
                            reportFilter,
                            //(string.IsNullOrEmpty(reportFilter)) ? "PaymentMethod IS NOT NULL" : string.Format("{0} AND PaymentMethod IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_PAYMENT_CONDITION:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.PaymentCondition.Ord]"
                            , "([DocumentFinanceMaster.PaymentCondition.Code]) [DocumentFinanceMaster.PaymentCondition.Designation]",/* IN009066 */
                            /* IN009066 - Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            // Required to Exclude Documents without PaymentCondition else Errors Occur
                            reportFilter,
                            //(string.IsNullOrEmpty(reportFilter)) ? "PaymentCondition IS NOT NULL" : string.Format("{0} AND PaymentCondition IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_CURRENCY:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.Currency.Ord]"
                            , "([DocumentFinanceMaster.Currency.Code]) [DocumentFinanceMaster.Currency.Designation]",/* IN009066 */
                            /* IN009066 - Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            reportFilter,
                            // Required to Exclude Documents without PaymentCondition else Errors Occur
                            //(string.IsNullOrEmpty(reportFilter)) ? "PaymentCondition IS NOT NULL" : string.Format("{0} AND PaymentCondition IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_COUNTRY:
                        CustomReport.ProcessReportDocumentMasterList(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceMaster.EntityCountry]"
                            , "[DocumentFinanceMaster.EntityCountry]",
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    // Detail

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.DocumentTypeOrd]"
                            , "([DocumentFinanceDetail.DocumentTypeCode]) [DocumentFinanceDetail.DocumentTypeDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_DATE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.DocumentDate]"
                            , "[DocumentFinanceDetail.DocumentDate]",
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_USER:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.UserDetailOrd]"
                            , "([DocumentFinanceDetail.UserDetailCode]) [DocumentFinanceDetail.UserDetailName]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_TERMINAL:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.TerminalOrd]"
                            , "([DocumentFinanceDetail.TerminalCode]) [DocumentFinanceDetail.TerminalDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_CUSTOMER:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.EntityFiscalNumber]"
                            , "[DocumentFinanceDetail.EntityFiscalNumber] / [DocumentFinanceDetail.EntityName]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PAYMENT_METHOD:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.PaymentMethodOrd]"
                            , "([DocumentFinanceDetail.PaymentMethodCode]) [DocumentFinanceDetail.PaymentMethodDesignation]",/* IN009066 */
                            /* IN009066 - Faturas and Notas de Crédito were not in this report, because they have no Payment Method. Now the issue is fixed */
                            reportFilter,
                            //(string.IsNullOrEmpty(reportFilter)) ? "fmPaymentMethod IS NOT NULL" : string.Format("{0} AND fmPaymentMethod IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PAYMENT_CONDITION:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.PaymentConditionOrd]"
                            /* IN009066 - Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            , "([DocumentFinanceDetail.PaymentConditionCode]) [DocumentFinanceDetail.PaymentConditionDesignation]",
                            reportFilter,
                            //(string.IsNullOrEmpty(reportFilter)) ? "fmPaymentCondition IS NOT NULL" : string.Format("{0} AND fmPaymentCondition IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_CURRENCY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.CurrencyOrd]"
                            , "([DocumentFinanceDetail.CurrencyCode]) [DocumentFinanceDetail.CurrencyDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_COUNTRY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.CountryOrd]"
                            , "([DocumentFinanceDetail.EntityCountryCode2]) [DocumentFinanceDetail.CountryDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FAMILY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.ArticleFamilyOrd]"
                            , "([DocumentFinanceDetail.ArticleFamilyCode]) [DocumentFinanceDetail.ArticleFamilyDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FAMILY_AND_SUBFAMILY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.ArticleSubFamilyOrd]"
                            , "([DocumentFinanceDetail.ArticleFamilyCode]) [DocumentFinanceDetail.ArticleFamilyDesignation] / ([DocumentFinanceDetail.ArticleSubFamilyCode]) [DocumentFinanceDetail.ArticleSubFamilyDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PLACE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.PlaceOrd]"
                            , "([DocumentFinanceDetail.PlaceCode]) [DocumentFinanceDetail.PlaceDesignation]",/* IN009066 */
                            /* IN009066 - Faturas, Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            reportFilter,
                            // Required to Exclude Documents without Place
                            //(string.IsNullOrEmpty(reportFilter)) ? "cpPlace IS NOT NULL" : string.Format("{0} AND cpPlace IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PLACE_TABLE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "[DocumentFinanceDetail.PlaceTableOrd]"
                            , "([DocumentFinanceDetail.PlaceCode]) [DocumentFinanceDetail.PlaceDesignation] / ([DocumentFinanceDetail.PlaceTableCode]) [DocumentFinanceDetail.PlaceTableDesignation]",/* IN009066 */
                            /* IN009066 - Faturas, Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            reportFilter,
                            // Required to Exclude Documents without PlaceTable
                            //(string.IsNullOrEmpty(reportFilter)) ? "dmPlaceTable IS NOT NULL" : string.Format("{0} AND dmPlaceTable IS NOT NULL", reportFilter),
                            /* IN009066 - end */
                            reportFilterHumanReadable
                            );
                        break;

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    // Detail/Group

                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_FINANCE_DOCUMENT:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "ftOid, ftDocumentTypeOrd, ftDocumentTypeCode, ftDocumentTypeDesignation"
                            , "ftOid AS GroupOid, ftDocumentTypeOrd AS GroupOrd, ftDocumentTypeCode AS GroupCode, ftDocumentTypeDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_DATE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "fmDocumentDate"
                            , "fmDocumentDate AS GroupOid, fmDocumentDate AS GroupOrd, fmDocumentDate AS GroupCode, fmDocumentDate AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "[DocumentFinanceDetail.GroupDesignation]"
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_USER:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "udUserDetail, udUserDetailOrd, udUserDetailCode, udUserDetailName"
                            , "udUserDetail AS GroupOid, udUserDetailOrd AS GroupOrd, udUserDetailCode AS GroupCode, udUserDetailName AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_TERMINAL:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "trTerminal, trTerminalOrd, trTerminalCode, trTerminalDesignation"
                            , "trTerminal AS GroupOid, trTerminalOrd AS GroupOrd, trTerminalCode AS GroupCode, trTerminalDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_CUSTOMER:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "fmEntity, cuEntityOrd, cuEntityCode, fmEntityName"
                            , "fmEntity AS GroupOid, cuEntityOrd AS GroupOrd, cuEntityCode AS GroupCode, fmEntityName AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_PAYMENT_METHOD:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "fmPaymentMethod, pmPaymentMethodOrd, pmPaymentMethodCode, pmPaymentMethodDesignation"
                            , "fmPaymentMethod AS GroupOid, pmPaymentMethodOrd AS GroupOrd, pmPaymentMethodCode AS GroupCode, pmPaymentMethodDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            /* IN009066 - Faturas and Notas de Crédito were not in this report, because they have no Payment Method. Now the issue is fixed */
                            , reportFilter
                            //, (string.IsNullOrEmpty(reportFilter)) ? "fmPaymentMethod IS NOT NULL" : string.Format("{0} AND fmPaymentMethod IS NOT NULL", reportFilter)
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_PAYMENT_CONDITION:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "fmPaymentCondition, pcPaymentConditionOrd, pcPaymentConditionCode, pcPaymentConditionDesignation"
                            , "fmPaymentCondition AS GroupOid, pcPaymentConditionOrd AS GroupOrd, pcPaymentConditionCode AS GroupCode, pcPaymentConditionDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            /* IN009066 - Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            , reportFilter
                            //, (string.IsNullOrEmpty(reportFilter)) ? "fmPaymentCondition IS NOT NULL" : string.Format("{0} AND fmPaymentCondition IS NOT NULL", reportFilter)
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_CURRENCY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "fmCurrency, crCurrencyOrd, crCurrencyCode, crCurrencyDesignation"
                            , "fmCurrency AS GroupOid, crCurrencyOrd AS GroupOrd, crCurrencyCode AS GroupCode, crCurrencyDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_COUNTRY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "ccCountry, ccCountryOrd, ccCountryCode, ccCountryDesignation"
                            , "ccCountry AS GroupOid, ccCountryOrd AS GroupOrd, ccCountryCode AS GroupCode, ccCountryDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_FAMILY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "afFamily, afFamilyOrd, afFamilyCode, afFamilyDesignation"
                            , "afFamily AS GroupOid, afFamilyOrd AS GroupOrd, afFamilyCode AS GroupCode, afFamilyDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_FAMILY_AND_SUBFAMILY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "sfSubFamily, sfSubFamilyOrd, sfSubFamilyCode, sfSubFamilyDesignation"
                            , "sfSubFamily AS GroupOid, sfSubFamilyOrd AS GroupOrd, sfSubFamilyCode AS GroupCode, sfSubFamilyDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]" /* IN009066 */
                            , reportFilter
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_PLACE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "cpPlace, cpPlaceOrd, cpPlaceCode, cpPlaceDesignation"
                            , "cpPlace AS GroupOid, cpPlaceOrd AS GroupOrd, cpPlaceCode AS GroupCode, cpPlaceDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]"
                            /* IN009066 - Faturas, Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            , reportFilter
                            //, (string.IsNullOrEmpty(reportFilter)) ? "cpPlace IS NOT NULL" : string.Format("{0} AND cpPlace IS NOT NULL", reportFilter)
                            , reportFilterHumanReadable
                            , true
                        );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_PLACE_TABLE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , token.ToString().ToLower()
                            , "dmPlaceTable, ctPlaceTableOrd, ctPlaceTableCode, ctPlaceTableDesignation"
                            , "dmPlaceTable AS GroupOid, ctPlaceTableOrd AS GroupOrd, ctPlaceTableCode AS GroupCode, ctPlaceTableDesignation AS GroupDesignation"
                            , "[DocumentFinanceDetail.GroupOid]"
                            , "([DocumentFinanceDetail.GroupCode]) [DocumentFinanceDetail.GroupDesignation]"
                            /* IN009066 - Faturas, Faturas Simplificadas and Notas de Crédito were not in this report, because they have no Payment Condition. Now the issue is fixed */
                            , reportFilter
                            //, (string.IsNullOrEmpty(reportFilter)) ? "dmPlaceTable IS NOT NULL" : string.Format("{0} AND dmPlaceTable IS NOT NULL", reportFilter)
                            , reportFilterHumanReadable
                            , true
                        );
                        break;

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    // Where it is Called?
                    /*
                    case ReportsTypeToken.REPORT_SALES_PER_FAMILY_AND_SUBFAMILY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceDetail.ArticleFamilyCode]"
                            , "[DocumentFinanceDetail.ArticleFamilyDesignation] ([DocumentFinanceDetail.ArticleFamilyCode])"
                            , false
                            );
                        break;
                    // Where it is Called?
                    case ReportsTypeToken.REPORT_SALES_PER_ZONE_TABLE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], token.ToString().ToLower())
                            , "[DocumentFinanceDetail.ArticleFamilyCode]"
                            , "[DocumentFinanceDetail.ArticleFamilyDesignation] ([DocumentFinanceDetail.ArticleFamilyCode])"
                            , true
                            );
                        break;
                    */

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    // Other Non REPORT_SALES_* Reports

                    // Auxiliar Tables
                    case ReportsTypeToken.REPORT_LIST_FAMILY_SUBFAMILY_ARTICLES:
                        // Where it is Called?
                        CustomReport.ProcessReportArticle(CustomReportDisplayMode.ExportPDF);
                        break;
                    case ReportsTypeToken.REPORT_LIST_CUSTOMERS:
                        CustomReport.ProcessReportCustomer(CustomReportDisplayMode.ExportPDF);
                        break;

                    // Other Reports
                    case ReportsTypeToken.REPORT_LIST_AUDIT_TABLE:
                        CustomReport.ProcessReportSystemAudit(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_MOVEMENTS:
                        CustomReport.ProcessReportArticleStockMovement(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_CURRENT_ACCOUNT:
                        CustomReport.ProcessReportDocumentFinanceCurrentAccount(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    /* IN008018 */
                    case ReportsTypeToken.REPORT_CUSTOMER_BALANCE_DETAILS:
                        CustomReport.ProcessReportCustomerBalanceDetails(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    /* IN009010 */
                    case ReportsTypeToken.REPORT_CUSTOMER_BALANCE_SUMMARY:
                        CustomReport.ProcessReportCustomerBalanceSummary(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    /* IN009204 */
                    case ReportsTypeToken.REPORT_COMPANY_BILLING:
                        CustomReport.ProcessReportCompanyBilling(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_USER_COMMISSION:
                        CustomReport.ProcessReportUserCommission(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    // ABove are not Implemented Yet
                    case ReportsTypeToken.REPORT_TOTAL_PER_FAMILY:
                        break;
                    case ReportsTypeToken.REPORT_TOP_CLOSE_EMPLOYEES:
                        break;
                    case ReportsTypeToken.REPORT_OCCUPATION_AVERAGE:
                        break;
                    case ReportsTypeToken.REPORT_ZONE_TOTAL:
                        break;
                    case ReportsTypeToken.REPORT_CLOSE_PEAK_HOUR:
                        break;
                    case ReportsTypeToken.REPORT_TOP_OFFERS:
                        break;
                    case ReportsTypeToken.REPORT_TOP_EMPLOYEE_RECORDS:
                        break;
                    case ReportsTypeToken.REPORT_RECORD_PEAK_HOUR:
                        break;
                    case ReportsTypeToken.REPORT_EMPLOYEE_MOVENTS:
                        break;
                    case ReportsTypeToken.REPORT_ACOUNT_BALANCE:
                        break;
                    case ReportsTypeToken.REPORT_WITHHOLDING_TAX:
                        break;
                    case ReportsTypeToken.REPORT_BALANCE_SHEET:
                        break;
                    case ReportsTypeToken.REPORT_SERVICE_HOURS:
                        break;
                    case ReportsTypeToken.REPORT_COURIER_DELIVER:
                        break;
                    case ReportsTypeToken.REPORT_CANCELED_ARTICLES_PER_EMPLOYEE:
                        break;
                    case ReportsTypeToken.REPORT_LIST_INVENTORY:
                        break;
                    case ReportsTypeToken.REPORT_DISCOUNTS_PER_USER:
                        break;
                    case ReportsTypeToken.REPORT_LIST_CONSUMPTION_PER_USER:
                        break;
                    case ReportsTypeToken.REPORT_CASH_TOTAL:
                        break;
                    case ReportsTypeToken.REPORT_LIST_WORKSESSION:
                        break;
                    case ReportsTypeToken.REPORT_LIST_CLOSE_WORKSESSION:
                        break;
                    default:
                        _log.Error(String.Format("Undetected Token: [{0}]", token));
                        break;
                }
            }
        }
    }
}
