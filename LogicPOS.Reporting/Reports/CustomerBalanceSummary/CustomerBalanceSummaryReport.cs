using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Common;
using LogicPOS.Reporting.Reports.Documents;
using LogicPOS.Reporting.Utility;
using LogicPOS.Settings;
using System;

namespace LogicPOS.Reporting.Reports.CustomerBalanceSummary
{
    public class CustomerBalanceSummaryReport 
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceCustomerBalanceSummary.frx";
        private readonly Common.FastReport _report;
        private readonly CustomReportDisplayMode _viewMode;

        public CustomerBalanceSummaryReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            )
        {
            _viewMode = viewMode;

            string reportFileLocation = ReportingUtils.GetReportFilePath(REPORT_FILENAME);

            _report = new Common.FastReport(
                reportFileLocation,
                Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                1);

            _report.SetParameterValue("Report Title", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_customer_balance_summary"));
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);

            if (!string.IsNullOrEmpty(readableFilter)) _report.SetParameterValue("Report Filter", readableFilter);

            ReportList<CustomerBalanceSummaryReportData> gcCustomerBalanceSummary = new ReportList<CustomerBalanceSummaryReportData>(filter);
            ReportList<CustomerBalanceDetailsReport> gcCustomerBalanceDetails = new ReportList<CustomerBalanceDetailsReport>(filter.Replace("CustomerSinceDate", "Date"));

            // Decrypt Phase
            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                Decrypt(gcCustomerBalanceSummary, gcCustomerBalanceDetails);
            }

            PrepareDataSources(gcCustomerBalanceSummary);

           
        }

        private static void Decrypt(
            ReportList<CustomerBalanceSummaryReportData> gcCustomerBalanceSummary, 
            ReportList<CustomerBalanceDetailsReport> gcCustomerBalanceDetails)
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

        private void PrepareDataSources(ReportList<CustomerBalanceSummaryReportData> reportData)
        {
            _report.RegisterData(reportData, "CustomerBalanceSummary");
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
