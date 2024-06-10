using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Common;
using LogicPOS.Reporting.Reports.CustomerBalanceSummary;
using LogicPOS.Reporting.Reports.Customers;
using LogicPOS.Reporting.Reports.Documents;
using LogicPOS.Settings;
using System;

namespace LogicPOS.Reporting.Reports
{
    public class CompanyBillingReport
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceCompanyBilling.frx";
        private readonly Common.FastReport _report;
        private readonly CustomReportDisplayMode _viewMode;
        private readonly string _filter;
        private readonly string _readableFilter;

        public CompanyBillingReport(
            CustomReportDisplayMode viewMode,
            string filter,
            string readableFilter
            )
        {
            _viewMode = viewMode;

            _report = new Common.FastReport(
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
            _report.SetParameterValue("Report Title", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_company_billing"));
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);

            if (!string.IsNullOrEmpty(_readableFilter))
            {
                _report.SetParameterValue("Report Filter", _readableFilter);
            }
        }

        private static void Decrypt(
            ReportList<CustomerBalanceDetailsReportData> gcCustomerBalanceDetails)
        {
            foreach (var item in gcCustomerBalanceDetails)
            {
                if (item.EntityName != null) item.EntityName = PluginSettings.SoftwareVendor.Decrypt(item.EntityName);
                if (item.EntityFiscalNumber != null) item.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(item.EntityFiscalNumber);
            }
        }

        private void PrepareDataSources()
        {
            ReportList<CustomerBalanceDetailsReportData> gcCustomerBalanceDetails = new ReportList<CustomerBalanceDetailsReportData>(_filter);

            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                Decrypt(gcCustomerBalanceDetails);
            }

            _report.RegisterData(gcCustomerBalanceDetails, "CustomerBalanceDetails");

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
