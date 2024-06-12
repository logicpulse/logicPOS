using logicpos.shared.Enums;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Customers;
using LogicPOS.Settings;
using LogicPOS.Utility;

namespace LogicPOS.Reporting.Reports
{
    public class CustomersListReport
    {
        private const string REPORT_FILENAME = "ReportCustomerList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _reportToken;
        protected readonly CustomReportDisplayMode _viewMode;

        public CustomersListReport()
        {
            _report = new Common.FastReport(
                reportFileName: REPORT_FILENAME,
                templateBase: Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                numberOfCopies: 1);

            _reportToken = "REPORT_LIST_CUSTOMERS".ToLower();
            _viewMode = CustomReportDisplayMode.ExportPDF;

            Initialize();
        }

        public void Initialize()
        {
            SetParametersValues();
            PrepareDataSources();
        }

        private void SetParametersValues()
        {
            var reportTitle = GeneralUtils.GetResourceByName(_reportToken);

            _report.SetParameterValue("Report Title", reportTitle);
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);
        }

        private void PrepareDataSources()
        {
            ReportList<CustomerTypeReportData> customerTypeReportDataList = new ReportList<CustomerTypeReportData>();
            ReportList<CustomerReportData> customerReportDataList;

            foreach (CustomerTypeReportData customerType in customerTypeReportDataList)
            {
                customerReportDataList = new ReportList<CustomerReportData>(string.Format("CustomerType = '{0}'", customerType.Oid), "Ord");
                customerType.Customer = customerReportDataList.List;

                if (customerReportDataList != null && customerReportDataList.List.Count > 0)
                {
                    // Decrypt Phase
                    if (PluginSettings.HasSoftwareVendorPlugin)
                    {
                        foreach (var item in customerReportDataList.List)
                        {
                            item.Name = PluginSettings.SoftwareVendor.Decrypt(item.Name);
                            item.Address = PluginSettings.SoftwareVendor.Decrypt(item.Address);
                            item.Locality = PluginSettings.SoftwareVendor.Decrypt(item.Locality);
                            item.ZipCode = PluginSettings.SoftwareVendor.Decrypt(item.ZipCode);
                            item.City = PluginSettings.SoftwareVendor.Decrypt(item.City);
                            item.DateOfBirth = PluginSettings.SoftwareVendor.Decrypt(item.DateOfBirth);
                            item.Phone = PluginSettings.SoftwareVendor.Decrypt(item.Phone);
                            item.Fax = PluginSettings.SoftwareVendor.Decrypt(item.Fax);
                            item.MobilePhone = PluginSettings.SoftwareVendor.Decrypt(item.MobilePhone);
                            item.Email = PluginSettings.SoftwareVendor.Decrypt(item.Email);
                            item.WebSite = PluginSettings.SoftwareVendor.Decrypt(item.WebSite);
                            item.FiscalNumber = PluginSettings.SoftwareVendor.Decrypt(item.FiscalNumber);
                            item.CardNumber = PluginSettings.SoftwareVendor.Decrypt(item.CardNumber);
                        }
                    }
                }
            }

            _report.RegisterData(customerTypeReportDataList, "CustomerType");
            if (_report.GetDataSource("CustomerType") != null) _report.GetDataSource("CustomerType").Enabled = true;
            if (_report.GetDataSource("CustomerType.Customer") != null) _report.GetDataSource("CustomerType.Customer").Enabled = true;
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
