using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using System;

namespace LogicPOS.Reporting.Reports.CustomerBalanceSummary
{
    public class CustomerBalanceSummaryReport 
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceCustomerBalanceSummary.frx";
        private readonly Common.FastReport _report;
        private readonly CustomReportDisplayMode _viewMode;
        private readonly string _filter;
        private readonly string _readableFilter;

        public CustomerBalanceSummaryReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            )
        {
            _viewMode = viewMode;

            _report =   new Common.FastReport(
                reportFileName: REPORT_FILENAME,
                templateBase: Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                numberOfCopies: 1);

            _filter = filter;
            _readableFilter = readableFilter;

            Initialize();
        }

        public void Initialize()
        {
            SetParametersValues();
            PrepareDataSources();
        }

        private void SetParametersValues()
        {
            _report.SetParameterValue("Report Title", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_customer_balance_summary"));
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);

            if (!string.IsNullOrEmpty(_readableFilter)) _report.SetParameterValue("Report Filter", _readableFilter);
        }

        private static void Decrypt(
            ReportDataList<CustomerBalanceSummaryReportData> gcCustomerBalanceSummary, 
            ReportDataList<CustomerBalanceDetailsReportData> gcCustomerBalanceDetails)
        {
            foreach (var customerBalance in gcCustomerBalanceDetails)
            {
                erp_customer customer = null;
                if (customerBalance.EntityName != null) customerBalance.EntityName = PluginSettings.SoftwareVendor.Decrypt(customerBalance.EntityName);
                if (customerBalance.EntityFiscalNumber != null) customerBalance.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(customerBalance.EntityFiscalNumber);

                foreach (var summary in gcCustomerBalanceSummary)
                {
                    if (summary.Oid != null && summary.Oid.Equals(customerBalance.EntityOid))
                    {
                        if (!string.IsNullOrEmpty(customerBalance.EntityOid))
                        {
                            customer = XPOUtility.GetEntityById<erp_customer>(new Guid(customerBalance.EntityOid));
                            summary.EntityName = customer.Name;
                            summary.EntityFiscalNumber = customer.FiscalNumber;
                        }
                        else
                        {
                            summary.EntityName = "No data";
                            summary.EntityFiscalNumber = "No data";
                        }
                    }
                }
            }
        }

        private void PrepareDataSources()
        {
            ReportDataList<CustomerBalanceSummaryReportData> gcCustomerBalanceSummary = new ReportDataList<CustomerBalanceSummaryReportData>(_filter);
            ReportDataList<CustomerBalanceDetailsReportData> gcCustomerBalanceDetails = new ReportDataList<CustomerBalanceDetailsReportData>(_filter.Replace("CustomerSinceDate", "Date"));

            // Decrypt Phase
            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                Decrypt(gcCustomerBalanceSummary, gcCustomerBalanceDetails);
            }

            _report.RegisterData(gcCustomerBalanceSummary, "CustomerBalanceSummary");

            if (_report.GetDataSource("CustomerBalanceSummary") != null)
            {
                _report.GetDataSource("CustomerBalanceSummary").Enabled = true;
            }
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
