using FastReport;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using logicpos.shared.Enums;
using LogicPOS.Utility;
using System;
using LogicPOS.Reporting.Reports.Customers;

namespace LogicPOS.Reporting.Reports
{
    public class ArticlesByFamilyAndSubfamilyReport
    {
        private const string REPORT_FILENAME = "ReportArticleList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _reportToken;
        protected readonly CustomReportDisplayMode _viewMode;

        public ArticlesByFamilyAndSubfamilyReport( )
        {
            _report = new Common.FastReport(
                reportFileName: REPORT_FILENAME,
                templateBase: Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                numberOfCopies: 1);

            _reportToken = "REPORT_LIST_FAMILY_SUBFAMILY_ARTICLES".ToLower();
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
            ReportDataList<ArticleFamilyReportData> aarticleFamilyReportDataList = new ReportDataList<ArticleFamilyReportData>();
            ReportDataList<ArticleSubFamilyReportData> articleSubFamilyReportDataList;
            ReportDataList<ArticleReportData> articleReportDataList;

            foreach (ArticleFamilyReportData family in aarticleFamilyReportDataList)
            {
                articleSubFamilyReportDataList = new ReportDataList<ArticleSubFamilyReportData>(string.Format("Family = '{0}'", family.Oid), "Ord");
                family.ArticleSubFamily = articleSubFamilyReportDataList.List;

                foreach (ArticleSubFamilyReportData subFamily in family.ArticleSubFamily)
                {
                    articleReportDataList = new ReportDataList<ArticleReportData>(string.Format("SubFamily = '{0}'", subFamily.Oid), "Ord");
                    subFamily.Article = articleReportDataList.List;
                }
            }

            _report.RegisterData(aarticleFamilyReportDataList, "ArticleFamily");
            if (_report.GetDataSource("ArticleFamily") != null) _report.GetDataSource("ArticleFamily").Enabled = true;
            if (_report.GetDataSource("ArticleFamily.ArticleSubFamily") != null) _report.GetDataSource("ArticleFamily.ArticleSubFamily").Enabled = true;
            if (_report.GetDataSource("ArticleFamily.ArticleSubFamily.Article") != null) _report.GetDataSource("ArticleFamily.ArticleSubFamily.Article").Enabled = true;
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
