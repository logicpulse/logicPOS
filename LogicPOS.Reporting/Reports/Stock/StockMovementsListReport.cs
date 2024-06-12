using logicpos.shared.Enums;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using LogicPOS.Utility;

namespace LogicPOS.Reporting.Reports
{
    public class StockMovementsListReport
    {
        private const string REPORT_FILENAME = "ReportArticleStockMovementList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private readonly string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public StockMovementsListReport(
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
            _reportToken = "REPORT_LIST_STOCK_MOVEMENTS";
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
            ReportDataList<ArticleStockMovementViewReportData> articleStockMovementViewReportDataList = new ReportDataList<ArticleStockMovementViewReportData>(_filter);

            _report.RegisterData(articleStockMovementViewReportDataList, "ArticleStockMovement");

            if (_report.GetDataSource("ArticleStockMovement") != null)
            {
                _report.GetDataSource("ArticleStockMovement").Enabled = true;
            }
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
