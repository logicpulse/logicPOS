using LogicPOS.Data.XPO.Settings;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using logicpos.shared.Enums;
using LogicPOS.Utility;
using System.Collections.Generic;
using System;

namespace LogicPOS.Reporting.Reports
{
    public class StockByArticleReport
    {
        private const string REPORT_FILENAME = "ReportArticleStockList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public StockByArticleReport(
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
            _reportToken = "REPORT_LIST_STOCK_ARTICLE";
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
            ReportDataList<ArticleStockViewReportData> gcArticleStock = new ReportDataList<ArticleStockViewReportData>(_filter);

            ReportDataList<ArticleStockViewReportData> gcArticleStockNew = new ReportDataList<ArticleStockViewReportData>("Date <= '0001-01-01 00:00:00");
            List<string> listArticles = new List<string>();

            foreach (ArticleStockViewReportData line in gcArticleStock)
            {
                string sqlCount = $"SELECT SUM(Quantity) as Result FROM fin_articlestock WHERE {_filter} AND Article = '{line.Article}' AND (Disabled = 0 OR Disabled is NULL)";
                line.ArticleStockQuantity = Convert.ToDecimal(XPOSettings.Session.ExecuteScalar(sqlCount));
                line.ArticleStockDateDay = _readableFilter;

                if (!listArticles.Contains(line.Article.ToString()))
                {
                    listArticles.Add(line.Article.ToString());
                    gcArticleStockNew.Add(line);
                }
            }

            _report.RegisterData(gcArticleStockNew, "ArticleStock");
            if (_report.GetDataSource("ArticleStock") != null) _report.GetDataSource("ArticleStock").Enabled = true;
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
