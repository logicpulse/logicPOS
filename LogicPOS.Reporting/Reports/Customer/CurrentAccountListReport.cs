using logicpos.shared.Enums;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using LogicPOS.Utility;

namespace LogicPOS.Reporting.Reports
{
    public class CurrentAccountListReport
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceCurrentAccount.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private readonly string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public CurrentAccountListReport(
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
            _reportToken = "REPORT_LIST_CURRENT_ACCOUNT";
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
            ReportDataList<CurrentAccountReportData> currentAccountReportDataList = new ReportDataList<CurrentAccountReportData>(_filter);

            if (PluginSettings.HasSoftwareVendorPlugin)
            {
                foreach (var item in currentAccountReportDataList)
                {
                    if (item.EntityName != null) item.EntityName = PluginSettings.SoftwareVendor.Decrypt(item.EntityName);
                    if (item.EntityFiscalNumber != null) item.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(item.EntityFiscalNumber);
                }
            }

            _report.RegisterData(currentAccountReportDataList, "CurrentAccount");
            if (_report.GetDataSource("CurrentAccount") != null) _report.GetDataSource("CurrentAccount").Enabled = true;

        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
