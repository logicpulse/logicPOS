using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using logicpos.shared.Enums;
using LogicPOS.Utility;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;

namespace LogicPOS.Reporting.Reports
{
    public class StockBySupplierReport
    {
        private const string REPORT_FILENAME = "ReportArticleStockSupplierList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private  string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public StockBySupplierReport(
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
            _reportToken = "REPORT_LIST_STOCK_SUPPLIER";
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
            if (_filter.Contains("stmOid"))
            {
                var result1 = _readableFilter.Split(',');
                var result2 = result1[result1.Length - 1].Substring(result1[result1.Length - 1].IndexOf('\''),
                                                                    result1[result1.Length - 1].LastIndexOf('\''));

                var result3 = _filter.Substring(0, _filter.IndexOf(')') + 1);

                _filter = result3 + " AND (stmDocumentNumber = " + result2 + ")";

            }

            ReportDataList<ArticleStockSupplierViewReportData> articleStockSupplierViewReportDataList = new ReportDataList<ArticleStockSupplierViewReportData>(_filter);

            cfg_configurationcurrency defaultCurrency = XPOSettings.ConfigurationSystemCurrency;

            cfg_configurationcurrency configurationCurrency;
            configurationCurrency = (cfg_configurationcurrency)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcurrency), defaultCurrency.Oid);

            foreach (ArticleStockSupplierViewReportData line in articleStockSupplierViewReportDataList)
            {
                line.ArticleStockCostumerName = PluginSettings.SoftwareVendor.Decrypt(line.ArticleStockCostumerName);
                if (string.IsNullOrEmpty(line.ArticleStockCurrency)) line.ArticleStockCurrency = configurationCurrency.Acronym;
            }

            _report.RegisterData(articleStockSupplierViewReportDataList, "ArticleStockSupplier");
            if (_report.GetDataSource("ArticleStockSupplier") != null) _report.GetDataSource("ArticleStockSupplier").Enabled = true;
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
