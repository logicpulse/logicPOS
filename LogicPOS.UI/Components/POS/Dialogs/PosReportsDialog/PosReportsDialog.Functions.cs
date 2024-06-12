using Gtk;
using logicpos.Classes.Enums;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.Reports;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.shared.Enums;
using LogicPOS.Reporting.Reports;
using LogicPOS.Reporting.Reports.CustomerBalanceSummary;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    internal partial class PosReportsDialog
    {
        public static CustomReportDisplayMode ReportDisplayMode { get; set; }

        private void buttonReportUnderConstruction_Clicked(object sender, EventArgs e)
        {
            logicpos.Utils.ShowMessageUnderConstruction();
        }

        private List<string> GetReportsQueryDialogFilter(
            ReportsQueryDialogMode reportsQueryDialogMode, 
            string databaseSourceObject)
        {
            PosReportsQueryDialog reportsQueryDialog = new PosReportsQueryDialog(
                _sourceWindow, 
                DialogFlags.DestroyWithParent, 
                reportsQueryDialogMode, 
                databaseSourceObject, 
                _windowTitle);

            ResponseType response = (ResponseType)reportsQueryDialog.Run();
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
                if (reportsQueryDialogMode.Equals(ReportsQueryDialogMode.FINANCIAL))
                {
                    filterSellDocuments = true;
                    filterField = "DocumentType";
                    statusField = "DocumentStatusStatus";
                }
                else if (reportsQueryDialogMode.Equals(ReportsQueryDialogMode.FINANCIAL_DETAIL) || reportsQueryDialogMode.Equals(ReportsQueryDialogMode.FINANCIAL_DETAIL_GROUP))
                {
                    filterSellDocuments = true;
                    filterField = "ftOid";
                    statusField = "fmDocumentStatusStatus";
                }

                if (filterSellDocuments == true)
                {
                    extraFilter = $@" AND ({statusField} <> 'A') AND (
                        {filterField} = '{InvoiceSettings.InvoiceId}' OR 
                        {filterField} = '{DocumentSettings.SimplifiedInvoiceId}' OR 
                        {filterField} = '{DocumentSettings.InvoiceAndPaymentId}' OR 
                        {filterField} = '{DocumentSettings.ConsignationInvoiceId}' OR 
                        {filterField} = '{DocumentSettings.DebitNoteId}' OR 
                        {filterField} = '{CustomDocumentSettings.CreditNoteId}' OR 
                        {filterField} = '{DocumentSettings.PaymentDocumentTypeId}' 
                        OR 
                        {filterField} = '{DocumentSettings.XpoOidDocumentFinanceTypeCurrentAccountInput}'
                        )".Replace(Environment.NewLine, string.Empty);
                }

                result.Add($"{reportsQueryDialog.FilterValue}{extraFilter}");
                result.Add(reportsQueryDialog.FilterValueHumanReadble);
            }
            else
            {
                reportsQueryDialog.Destroy();
                result = null;
            }

            return result;
        }

        public Type SenderType { get; set; }

        public void PrintReportRouter(object sender, EventArgs e)
        {
            string reportFilter = string.Empty;
            string reportReadableFilter = string.Empty;
            string databaseSourceObject = string.Empty;

            Button button;

            SenderType = sender.GetType();

            if (SenderType.Name == "AccordionChildButton")
            {
                button = (sender as AccordionChildButton);
            }
            else
            {
                button = (sender as TouchButtonIconWithText);
            }

            ReportsTypeToken reportToken = GetReportTokenByName(button.Name);


            PrintingSettings.ThermalPrinter.UsingThermalPrinter = true;

            ReportsQueryDialogMode reportsQueryDialogMode = ReportsQueryDialogMode.UNDEFINED;

            if (reportToken.ToString().StartsWith("REPORT_SALES_DETAIL_"))
            {
                switch (reportToken.ToString())
                {
                    case "REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_finance_document") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_DATE":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_date") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_USER":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_user") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_TERMINAL":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_terminal") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_CUSTOMER":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_customer") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_PAYMENT_METHOD":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_payment_method") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_PAYMENT_CONDITION":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_payment_condition") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_CURRENCY":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_currency") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_PER_COUNTRY":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_country") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                    case "REPORT_SALES_DETAIL_GROUP_PER_VAT":
                    case "REPORT_SALES_PER_VAT":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_vat");
                        reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL_VAT;
                        break;
                    default:
                        this._windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_report_filter") + 
                                            GeneralUtils.GetResourceByName("report_sales_detail_postfix");
                        break;
                }

                if (reportsQueryDialogMode == ReportsQueryDialogMode.UNDEFINED)
                {
                    reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL;
                }

                databaseSourceObject = "view_documentfinance";
            }
            else if (reportToken.ToString() == "REPORT_SALES_PER_VAT" || 
                     reportToken.ToString() == "REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS")
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_vat");
                reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL_DETAIL_VAT;
                databaseSourceObject = "view_documentfinance";
            }

            else if (reportToken.ToString().StartsWith("REPORT_SALES_"))
            {
                switch (reportToken.ToString())
                {
                    case "REPORT_SALES_PER_FINANCE_DOCUMENT":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_finance_document");
                        break;
                    case "REPORT_SALES_PER_DATE":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_date");
                        break;
                    case "REPORT_SALES_PER_USER":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_user");
                        break;
                    case "REPORT_SALES_PER_TERMINAL":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_terminal");
                        break;
                    case "REPORT_SALES_PER_CUSTOMER":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_customer");
                        break;
                    case "REPORT_SALES_PER_PAYMENT_METHOD":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_payment_method");
                        break;
                    case "REPORT_SALES_PER_PAYMENT_CONDITION":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_payment_condition");
                        break;
                    case "REPORT_SALES_PER_CURRENCY":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_currency");
                        break;
                    case "REPORT_SALES_PER_COUNTRY":
                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_country");
                        break;
                    default:
                        this._windowTitle = GeneralUtils.GetResourceByName("window_title_dialog_report_filter"); 
                        break;
                }

                reportsQueryDialogMode = ReportsQueryDialogMode.FINANCIAL;
                databaseSourceObject = "fin_documentfinancemaster";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_AUDIT_TABLE"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_list_audit_table");
                reportsQueryDialogMode = ReportsQueryDialogMode.SYSTEM_AUDIT;
                databaseSourceObject = "view_systemaudit";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_CURRENT_ACCOUNT"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_customer_balance_details");
                reportsQueryDialogMode = ReportsQueryDialogMode.CURRENT_ACCOUNT;
                databaseSourceObject = "view_documentfinancecurrentaccount";
            }
            /* IN008018 */
            else if (reportToken.ToString().Equals("REPORT_CUSTOMER_BALANCE_DETAILS"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_customer_balance_summary");
                reportsQueryDialogMode = ReportsQueryDialogMode.CUSTOMER_BALANCE_DETAILS;
                databaseSourceObject = "view_documentfinancecustomerbalancedetails";
            }
            /* IN009010 */
            else if (reportToken.ToString().Equals("REPORT_CUSTOMER_BALANCE_SUMMARY"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_customer_balance_details");
                reportsQueryDialogMode = ReportsQueryDialogMode.CUSTOMER_BALANCE_SUMMARY;
                databaseSourceObject = "view_documentfinancecustomerbalancesummary";
            }
            /* IN009204 - based on CUSTOMER_BALANCE_DETAILS report */
            else if (reportToken.ToString().Equals("REPORT_COMPANY_BILLING"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_company_billing");
                reportsQueryDialogMode = ReportsQueryDialogMode.COMPANY_BILLING;
                databaseSourceObject = "view_documentfinancecustomerbalancedetails";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_USER_COMMISSION"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_list_user_commission");
                reportsQueryDialogMode = ReportsQueryDialogMode.USER_COMMISSION;
                databaseSourceObject = "view_usercommission";
            }
            //Stock Reports
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_MOVEMENTS"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_list_stock_movements");
                reportsQueryDialogMode = ReportsQueryDialogMode.ARTICLE_STOCK_MOVEMENTS;
                databaseSourceObject = "view_articlestockmovement";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_WAREHOUSE"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_list_stock_warehouse");
                reportsQueryDialogMode = ReportsQueryDialogMode.FILTER_ARTICLE_WAREHOUSE;
                databaseSourceObject = "view_articlestockwarehouse";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_ARTICLE"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_list_stock_article");
                reportsQueryDialogMode = ReportsQueryDialogMode.FILTER_ARTICLE_STOCK;
                databaseSourceObject = "view_articlestock";
            }
            else if (reportToken.ToString().Equals("REPORT_LIST_STOCK_SUPPLIER"))
            {
                this._windowTitle = GeneralUtils.GetResourceByName("report_list_stock_supplier");
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
                    reportReadableFilter = dialogresultFilter[1];
                }
                // ResponseType.Cancel
                else
                {
                    reportFilter = null;
                    reportReadableFilter = null;
                }
            }


            if (reportFilter != null)
            {
                CustomReportDisplayMode displayMode = ReportDisplayMode;
                switch (reportToken)
                {
                    case ReportsTypeToken.REPORT_SALES_PER_FINANCE_DOCUMENT:

                        PresentSalesByFinanceDocumentReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_DATE:

                        PresentSalesByDateReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        this._windowTitle = GeneralUtils.GetResourceByName("report_sales_per_date");
                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_USER:

                        PresentSalesByDateReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_TERMINAL:

                        PresentSalesByTerminalReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_CUSTOMER:

                        PresentSalesByCustomerReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_PAYMENT_METHOD:

                        PresentSalesByPaymentMethodReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_PAYMENT_CONDITION:

                        PresentSalesByPaymentConditionReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_CURRENCY:

                        PresentSalesByCurrencyReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_COUNTRY:

                        PresentSalesByCountryReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FINANCE_DOCUMENT:
                       
                        PresentSalesByFinanceDocumentDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_DATE:
                      
                        PresentSalesByDateDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_USER:
  
                        PresentSalesByUserDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_TERMINAL:
                                           
                        PresentSalesByTerminalDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_CUSTOMER:
                        
                        PresentSalesByCustomerDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PAYMENT_METHOD:
                        
                        PresentSalesByPaymentMethodDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;


                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PAYMENT_CONDITION:
                        
                        PresentSalesByPaymentConditionDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_CURRENCY:
                        
                        PresentSalesByCurrencyDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_COUNTRY:
                       
                        PresentSalesByCountryDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);
                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FAMILY:
                        
                        PresentSalesByFamilyDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_FAMILY_AND_SUBFAMILY:
                        
                        PresentSalesByFamilyAndSubfamilyDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PLACE:

                        PresentSalesByPlaceDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);
                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_PER_PLACE_TABLE:
                        
                        PresentSalesByPlaceTableDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
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
                            , reportReadableFilter
                            , true
                        );
                        break;

                    case ReportsTypeToken.REPORT_SALES_DETAIL_GROUP_PER_VAT:

                        PresentSalesByVatGroupDetailedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

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

                    case ReportsTypeToken.REPORT_LIST_FAMILY_SUBFAMILY_ARTICLES:

                        new ArticlesByFamilyAndSubfamilyReport().Present();

                        break;

                    case ReportsTypeToken.REPORT_LIST_CUSTOMERS:

                        new CustomersListReport().Present();

                        break;

                    case ReportsTypeToken.REPORT_LIST_AUDIT_TABLE:

                        PresentSystemAuditListReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_LIST_CURRENT_ACCOUNT:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportDocumentFinanceCurrentAccount(displayMode, reportFilter, reportReadableFilter);
                        break;


                    case ReportsTypeToken.REPORT_CUSTOMER_BALANCE_DETAILS:
                        
                        PresentCustomerBalanceDetailsReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);
                        break;


                    case ReportsTypeToken.REPORT_CUSTOMER_BALANCE_SUMMARY:

                        PresentCostumerBalanceSummaryReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;


                    case ReportsTypeToken.REPORT_COMPANY_BILLING:

                        PresentCompanyBillingReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_LIST_USER_COMMISSION:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportUserCommission(displayMode, reportFilter, reportReadableFilter);
                        break;
                    // Stock Reports
                    case ReportsTypeToken.REPORT_LIST_STOCK_MOVEMENTS:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStockMovement(displayMode, reportFilter, reportReadableFilter);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_WAREHOUSE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStockWarehouse(displayMode, reportFilter, reportReadableFilter);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_ARTICLE:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStock(displayMode, reportFilter, reportReadableFilter);
                        break;
                    case ReportsTypeToken.REPORT_LIST_STOCK_SUPPLIER:
                        LogicPOS.Reporting.Common.FastReport.ProcessReportArticleStockSupplier(displayMode, reportFilter, reportReadableFilter);
                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_VAT:
                        
                        PresentSalesByVatResumedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    case ReportsTypeToken.REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS:

                        PresentSalesByVatAndArticleClassResumedReport(
                            reportFilter,
                            reportReadableFilter,
                            displayMode);

                        break;

                    default:
                        throw new NotImplementedException("Report not implemented: " + reportToken.ToString());
                }
            }
        }

        private void PresentSystemAuditListReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SystemAuditListReport(
                reportFilter,
                reportReadableFilter,
                displayMode);

            report.Present();
        }

        private void PresentSalesByVatGroupDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByVatGroupDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentCustomerBalanceDetailsReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new CustomerBalanceDetailsReport(
                reportFilter, 
                reportReadableFilter, 
                displayMode);

            report.Present();
        }

        private void PresentSalesByVatAndArticleClassResumedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByVatAndArticleClassResumedReport(
               reportFilter,
               reportReadableFilter,
               displayMode);

            report.Present();
        }

        private void PresentSalesByVatResumedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByVatResumedReport(
               reportFilter,
               reportReadableFilter,
               displayMode);

            report.Present();
        }

        private void PresentSalesByPlaceTableDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByPlaceTableDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByPlaceDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByPlaceDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByFamilyAndSubfamilyDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByFamilyAndSubfamilyDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByFamilyDetailedReport(
            string reportFilter, 
            string reportReadableFilter,
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByFamilyDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByCountryDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByCountryDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByCurrencyDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByCurrencyDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByPaymentConditionDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByPaymentConditionDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByPaymentMethodDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByPaymentMethodDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByCustomerDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByCustomerDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByTerminalDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByTerminalDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByUserDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByUserDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByDateDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByDateDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private void PresentSalesByFinanceDocumentDetailedReport(
            string reportFilter, 
            string reportReadableFilter, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByFinanceDocumentDetailedReport(
                displayMode, 
                reportFilter, 
                reportReadableFilter);

            report.Present();
        }

        private static ReportsTypeToken GetReportTokenByName(string name)
        {
            return (ReportsTypeToken)Enum.Parse(
                typeof(ReportsTypeToken),
                name,
                true);
        }

        private void PresentSalesByCountryReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByCountryReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);

            report.Present();
        }

        private void PresentSalesByCurrencyReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByCurrencyReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);

            report.Present();
        }

        private void PresentSalesByPaymentConditionReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByPaymentConditionReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);

            report.Present();
        }

        private void PresentSalesByPaymentMethodReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByPaymentMethodReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);

            report.Present();
        }

        private void PresentSalesByCustomerReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByCustomerReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);

            report.Present();
        }

        private void PresentSalesByTerminalReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByTerminalReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);

            report.Present();
        }

        private void PresentSalesByDateReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByDateReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);
            report.Present();
        }

        private void PresentSalesByFinanceDocumentReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var report = new SalesByFinanceDocumentReport(
                displayMode, 
                reportFilter, 
                reportFilterHumanReadable);
            report.Present();
        }

        private void PresentCompanyBillingReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var companBillingReport = new CompanyBillingReport(
                displayMode,
                reportFilter,
                reportFilterHumanReadable);

            companBillingReport.Present();
        }

        private void PresentCostumerBalanceSummaryReport(
            string reportFilter, 
            string reportFilterHumanReadable, 
            CustomReportDisplayMode displayMode)
        {
            var customerBalanceSummaryReport = new CustomerBalanceSummaryReport(
                displayMode,
                reportFilter,
                reportFilterHumanReadable);

            customerBalanceSummaryReport.Present();
        }
    }
}
