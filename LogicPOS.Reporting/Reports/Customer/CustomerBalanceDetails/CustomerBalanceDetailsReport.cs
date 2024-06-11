using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Reporting.Reports.CustomerBalanceSummary;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System.Collections.Generic;
using System;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Reporting.Data.Common;

namespace LogicPOS.Reporting.Reports
{
    public class CustomerBalanceDetailsReport
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceCustomerBalanceDetails.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private readonly string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public CustomerBalanceDetailsReport(
            string filter,
            string readableFilter,
            CustomReportDisplayMode viewMode
            )
        {
            _report = new Common.FastReport(
                reportFileName: REPORT_FILENAME,
                templateBase: Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                numberOfCopies: 1);

            _filter = filter;
            _readableFilter = readableFilter;
            _reportToken = "REPORT_CUSTOMER_BALANCE_DETAILS";
            _viewMode = viewMode;

            Initialize();
        }

        public void Initialize()
        {
            SetParametersValues();
            PrepareDataSources();
        }

        private void SetParametersValues()
        {
            var reportTitle = GeneralUtils.GetResourceByName(_reportToken.ToLower());
            _report.SetParameterValue("Report Title", reportTitle);
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);

            if (!string.IsNullOrEmpty(_readableFilter))
            {
                _report.SetParameterValue("Report Filter", _readableFilter);
            }
        }

        private void PrepareDataSources()
        {
            ReportList<CustomerBalanceDetailsReportData> customerBalanceDetailsReportDataList = new ReportList<CustomerBalanceDetailsReportData>(_filter);
            ReportList<CustomerBalanceSummaryReportData> customerBalanceSummaryReportDataList = new ReportList<CustomerBalanceSummaryReportData>();

            erp_customer customer = null;
            List<erp_customer> customersList = new List<erp_customer>();
            bool printTotalBalance = true;


            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                foreach (var item in customerBalanceDetailsReportDataList)
                {
                    if (item.EntityName != null)
                    {
                        item.EntityName = PluginSettings.SoftwareVendor.Decrypt(item.EntityName);
                    }

                    if (item.EntityFiscalNumber != null)
                    {
                        item.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(item.EntityFiscalNumber);
                    }

                    if (item.EntityOid != null)
                    {
                        customer = (erp_customer)XPOSettings.Session.GetObjectByKey(typeof(erp_customer), Guid.Parse(item.EntityOid));
                    }

                    if (!customersList.Contains(customer))
                    {
                        customersList.Add(customer);
                    }

                    foreach (var summary in customerBalanceSummaryReportDataList)
                    {
                        if (summary.Oid != null && summary.Oid.Equals(item.EntityOid))
                        {
                            item.Balance = summary.Balance;
                            item.CustomerSinceDate = summary.CustomerSinceDate;
                            break;
                        }
                    }
                }
            }

            if (customersList.Count > 1) printTotalBalance = false;

            ReportList<CustomerBalanceSummaryReportData> gcCustomerBalanceSummaryTotal = new ReportList<CustomerBalanceSummaryReportData>(string.Format("(EntityOid = '{0}')", customer.Oid));
            _report.SetParameterValue("PrintTotalBalance", printTotalBalance);
            _report.SetParameterValue("TotalCreditFinal", gcCustomerBalanceSummaryTotal.List[0].TotalCredit);
            _report.SetParameterValue("TotalDebitFinal", gcCustomerBalanceSummaryTotal.List[0].TotalDebit);
            _report.SetParameterValue("TotalBalanceFinal", gcCustomerBalanceSummaryTotal.List[0].Balance);

            _report.RegisterData(customerBalanceDetailsReportDataList, "CustomerBalanceDetails");

            if (_report.GetDataSource("CustomerBalanceDetails") != null)
            {
                _report.GetDataSource("CustomerBalanceDetails").Enabled = true;
            }
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }

    }
}
