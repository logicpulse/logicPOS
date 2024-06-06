using Gtk;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared.Enums;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Reports.CustomerBalanceSummary;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    // Use PrintRouter for financialDocuments ex PrintFinanceDocument

    internal partial class PosReportsDialog
    {
        public static CustomReportDisplayMode exportType;

        private void buttonReportUnderConstruction_Clicked(object sender, EventArgs e)
        {
            logicpos.Utils.ShowMessageUnderConstruction();
        }

        private List<string> GetReportsQueryDialogFilter(ReportsQueryDialogMode pReportsQueryDialogMode, string pDatabaseSourceObject)
        {
            PosReportsQueryDialog dialog = new PosReportsQueryDialog(_sourceWindow, DialogFlags.DestroyWithParent, pReportsQueryDialogMode, pDatabaseSourceObject, _windowTitle);
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
{filterField} = '{InvoiceSettings.XpoOidDocumentFinanceTypeInvoice}' OR 
{filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypeSimplifiedInvoice}' OR 
{filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypeInvoiceAndPayment}' OR 
{filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypeConsignationInvoice}' OR 
{filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypeDebitNote}' OR 
{filterField} = '{CustomDocumentSettings.CreditNoteDocumentTypeId}' OR 
{filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypePayment}' 
OR 
{filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypeCurrentAccountInput}'
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

        public Type SenderType { get; set; }

        public void PrintReportRouter(object sender, EventArgs e)
        {

            // Override Default Development Mode
            // Local Variables
            string reportFilter = string.Empty;
            string reportFilterHumanReadable = string.Empty;
            string databaseSourceObject = string.Empty;

            dynamic button;

            SenderType = sender.GetType();

            if (SenderType.Name == "AccordionChildButton")
            {
                button = (sender as AccordionChildButton);
            }
            else
            {
                button = (sender as TouchButtonIconWithText);
            }
            //TouchButtonIconWithText buttonIcon = (sender as TouchButtonIconWithText);
            //AccordionChildButton button = (sender as AccordionChildButton);
            //_logger.Debug(String.Format("Button.Name: [{0}], Button.label: [{1}]", button.Name, button.Label));

            // Get Token From buttonName
            ReportsTypeToken reportToken = (ReportsTypeToken)Enum.Parse(typeof(ReportsTypeToken), button.Name, true);
            _logger.Debug("void PrintReportRouter(object sender, EventArgs e) :: ReportsTypeToken: " + reportToken.ToString());
            //TK016249 - Impressoras - Diferenciação entre Tipos
            PrintingSettings.ThermalPrinter.UsingThermalPrinter = true;
            // Prepare ReportsQueryDialogMode
            //Titulo nas janelas de filtro de relatório [IN:014328]
            ReportsQueryDialogMode reportsQueryDialogMode = ReportsQueryDialogMode.UNDEFINED;
            // Catch REPORT_SALES_DETAIL_* and REPORT_SALES_DETAIL_GROUP_* use same View
            if (reportToken.ToString().StartsWith("REPORT_SALES_DETAIL_"))
            {
                if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_finance_document") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_DATE")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_date") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_USER")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_user") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_TERMINAL")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_terminal") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_CUSTOMER")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_customer") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_PAYMENT_METHOD")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_payment_method") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_PAYMENT_CONDITION")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_payment_condition") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_CURRENCY")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_currency") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_PER_COUNTRY")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_country") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");
                }
                else if (reportToken.ToString() == "REPORT_SALES_DETAIL_GROUP_PER_VAT" || reportToken.ToString() == "REPORT_SALES_PER_VAT")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_vat");
                    reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL_VAT;
                }
                else
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_report_filter") + CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_detail_postfix");

                if (reportsQueryDialogMode == ReportsQueryDialogMode.UNDEFINED) reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL;
                databaseSourceObject = "view_documentfinance";
            }
            else if (reportToken.ToString() == "REPORT_SALES_PER_VAT" || reportToken.ToString() == "REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS")
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_vat");
                reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL_VAT;
                databaseSourceObject = "view_documentfinance";
            }

            else if (reportToken.ToString().StartsWith("REPORT_SALES_"))
            {
                if (reportToken.ToString() == "REPORT_SALES_PER_FINANCE_DOCUMENT")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_finance_document");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_DATE")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_date");
                }

                else if (reportToken.ToString() == "REPORT_SALES_PER_USER")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_user");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_TERMINAL")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_terminal");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_CUSTOMER")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_customer");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_PAYMENT_METHOD")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_payment_method");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_PAYMENT_CONDITION")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_payment_condition");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_CURRENCY")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_currency");
                }
                else if (reportToken.ToString() == "REPORT_SALES_PER_COUNTRY")
                {
                    this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_country");
                }
                else { this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_report_filter"); }

                reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL;
                databaseSourceObject = "fin_documentfinancemaster";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_AUDIT_TABLE"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_list_audit_table");
                reportsQueryDialogMode = ReportsQueryDialogMode.SYSTEM_AUDIT;
                databaseSourceObject = "view_systemaudit";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_CURRENT_ACCOUNT"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_customer_balance_details");
                reportsQueryDialogMode = ReportsQueryDialogMode.CURRENT_ACCOUNT;
                databaseSourceObject = "view_documentfinancecurrentaccount";
            }
            /* IN008018 */
            else if (reportToken.ToString().Equals("REPORT_CUSTOMER_BALANCE_DETAILS"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_customer_balance_summary");
                reportsQueryDialogMode = ReportsQueryDialogMode.CUSTOMER_BALANCE_DETAILS;
                databaseSourceObject = "view_documentfinancecustomerbalancedetails";
            }
            /* IN009010 */
            else if (reportToken.ToString().Equals("REPORT_CUSTOMER_BALANCE_SUMMARY"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_customer_balance_details");
                reportsQueryDialogMode = ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY;
                databaseSourceObject = "view_documentfinancecustomerbalancesummary";
            }
            /* IN009204 - based on CUSTOMER_BALANCE_DETAILS report */
            else if (reportToken.ToString().Equals("REPORT_COMPANY_BILLING"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_company_billing");
                reportsQueryDialogMode = ReportsQueryDialogMode.COMPANY_BILLING;
                databaseSourceObject = "view_documentfinancecustomerbalancedetails";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_USER_COMMISSION"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_list_user_commission");
                reportsQueryDialogMode = ReportsQueryDialogMode.USER_COMMISSION;
                databaseSourceObject = "view_usercommission";
            }
            //Stock Reports
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_MOVEMENTS"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_list_stock_movements");
                reportsQueryDialogMode = ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS;
                databaseSourceObject = "view_articlestockmovement";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_WAREHOUSE"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_list_stock_warehouse");
                reportsQueryDialogMode = ReportsQueryDialogMode.FILTER_ARTICLE_WAREHOUSE;
                databaseSourceObject = "view_articlestockwarehouse";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_ARTICLE"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_list_stock_article");
                reportsQueryDialogMode = ReportsQueryDialogMode.FILTER_ARTICLE_STOCK;
                databaseSourceObject = "view_articlestock";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_SUPPLIER"))
            {
                this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_list_stock_supplier");
                reportsQueryDialogMode = ReportsQueryDialogMode.FILTER_ARTICLE_STOCK_SUPPLIER;
                databaseSourceObject = "view_articlestocksupplier";
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
                //CustomReportDisplayMode displayMode = (Debugger.IsAttached)
                //    ? CustomReportDisplayMode.Design
                //    : CustomReportDisplayMode.ExportPDF;
                // Now we have two types of export according the Button response
                CustomReportDisplayMode displayMode = exportType;
                switch (reportToken)
                {
                    case ReportsTypeToken.REPORT_SALES_PER_FINANCE_DOCUMENT:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
                            , "[DocumentFinanceMaster.DocumentType.Ord]"
                            , "([DocumentFinanceMaster.DocumentType.Code]) [DocumentFinanceMaster.DocumentType.Designation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_DATE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
                            , "[DocumentFinanceMaster.DocumentDate]"
                            , "[DocumentFinanceMaster.DocumentDate]",
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        this._windowTitle = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "report_sales_per_date");
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_USER:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
                            , "[DocumentFinanceMaster.CreatedBy.Ord]"
                            , "([DocumentFinanceMaster.CreatedBy.Code]) [DocumentFinanceMaster.CreatedBy.Name]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_TERMINAL:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
                            , "[DocumentFinanceMaster.CreatedWhere.Ord]"
                            , "([DocumentFinanceMaster.CreatedWhere.Code]) [DocumentFinanceMaster.CreatedWhere.Designation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_CUSTOMER:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
                            , "[DocumentFinanceMaster.EntityFiscalNumber]"
                            , "[DocumentFinanceMaster.EntityFiscalNumber] / [DocumentFinanceMaster.EntityName]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_PAYMENT_METHOD:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentMasterList(displayMode
                            , CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, reportToken.ToString().ToLower())
                            , "[DocumentFinanceMaster.EntityCountry]"
                            , "[DocumentFinanceMaster.EntityCountry]",
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;

                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
                    // Detail

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.DocumentTypeOrd]"
                            , "([DocumentFinanceDetail.DocumentTypeCode]) [DocumentFinanceDetail.DocumentTypeDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_DATE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.DocumentDate]"
                            , "[DocumentFinanceDetail.DocumentDate]",
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_USER:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.UserDetailOrd]"
                            , "([DocumentFinanceDetail.UserDetailCode]) [DocumentFinanceDetail.UserDetailName]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_TERMINAL:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.TerminalOrd]"
                            , "([DocumentFinanceDetail.TerminalCode]) [DocumentFinanceDetail.TerminalDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_CUSTOMER:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.EntityFiscalNumber]"
                            , "[DocumentFinanceDetail.EntityFiscalNumber] / [DocumentFinanceDetail.EntityName]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PAYMENT_METHOD:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.CurrencyOrd]"
                            , "([DocumentFinanceDetail.CurrencyCode]) [DocumentFinanceDetail.CurrencyDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_COUNTRY:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.CountryOrd]"
                            , "([DocumentFinanceDetail.EntityCountryCode2]) [DocumentFinanceDetail.CountryDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FAMILY:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.ArticleFamilyOrd]"
                            , "([DocumentFinanceDetail.ArticleFamilyCode]) [DocumentFinanceDetail.ArticleFamilyDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FAMILY_AND_SUBFAMILY:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
                            , "[DocumentFinanceDetail.ArticleSubFamilyOrd]"
                            , "([DocumentFinanceDetail.ArticleFamilyCode]) [DocumentFinanceDetail.ArticleFamilyDesignation] / ([DocumentFinanceDetail.ArticleSubFamilyCode]) [DocumentFinanceDetail.ArticleSubFamilyDesignation]",/* IN009066 */
                            reportFilter,
                            reportFilterHumanReadable
                            );
                        break;
                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PLACE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                            , reportToken.ToString().ToLower()
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

                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_VAT:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentDetail(displayMode
                     , reportToken.ToString().ToLower()
                     , "[DocumentFinanceDetail.ArticleVat]"
                     , "[DocumentFinanceDetail.ArticleVat]",
                     reportFilter,
                     reportFilterHumanReadable
                     );
                        break;
                    //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

                    // Where it is Called?
                    /*
                    case ReportsTypeToken.REPORT_SALES_PER_FAMILY_AND_SUBFAMILY:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, token.ToString().ToLower())
                            , "[DocumentFinanceDetail.ArticleFamilyCode]"
                            , "[DocumentFinanceDetail.ArticleFamilyDesignation] ([DocumentFinanceDetail.ArticleFamilyCode])"
                            , false
                            );
                        break;
                    // Where it is Called?
                    case ReportsTypeToken.REPORT_SALES_PER_ZONE_TABLE:
                        CustomReport.ProcessReportDocumentDetail(displayMode
                            , CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, token.ToString().ToLower())
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
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticle(CustomReportDisplayMode.ExportPDF);
                        break;
                    case ReportsTypeToken.REPORT_LIST_CUSTOMERS:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportCustomer(CustomReportDisplayMode.ExportPDF);
                        break;

                    // Other Reports
                    case ReportsTypeToken.REPORT_LIST_AUDIT_TABLE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportSystemAudit(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_CURRENT_ACCOUNT:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentFinanceCurrentAccount(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    /* IN008018 */
                    case ReportsTypeToken.REPORT_CUSTOMER_BALANCE_DETAILS:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportCustomerBalanceDetails(displayMode, reportFilter, reportFilterHumanReadable);
                        break;


                    case ReportsTypeToken.REPORT_CUSTOMER_BALANCE_SUMMARY:

                        PresentCostumerBalanceSummaryReport(
                            reportFilter,
                            reportFilterHumanReadable,
                            displayMode);

                        break;


                    case ReportsTypeToken.REPORT_COMPANY_BILLING:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportCompanyBilling(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_USER_COMMISSION:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportUserCommission(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    // Stock Reports
                    case ReportsTypeToken.REPORT_LIST_STOCK_MOVEMENTS:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStockMovement(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_WAREHOUSE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStockWarehouse(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_ARTICLE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStock(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_SUPPLIER:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStockSupplier(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_VAT:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportVatSalesResumed(displayMode, reportFilter, reportFilterHumanReadable);
                        break;
                    case ReportsTypeToken.REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportVatSalesByClassResumed(displayMode, reportFilter, reportFilterHumanReadable);
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
                        _logger.Error(string.Format("Undetected Token: [{0}]", reportToken));
                        break;
                }
            }
        }

        private static void PresentCostumerBalanceSummaryReport(string reportFilter, string reportFilterHumanReadable, CustomReportDisplayMode displayMode)
        {
            var customerBalanceSummaryReport = new CustomerBalanceSummaryReport(
                                        displayMode,
                                        reportFilter,
                                        reportFilterHumanReadable);

            customerBalanceSummaryReport.Present();
        }
    }
}
