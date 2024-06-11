using logicpos.shared.Enums;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;

namespace LogicPOS.Reporting.Reports
{
    public class SalesByVatAndArticleClassResumedReport
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceVatSalesByClassSummary.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private readonly string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public SalesByVatAndArticleClassResumedReport(
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
            _reportToken = "REPORT_SALES_PER_VAT_BY_ARTICLE_CLASS";
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
            _report.SetParameterValue("Report Filter", _readableFilter);
        }

        private void PrepareDataSources()
        {
            string query = string.Format(@"SELECT cfOid as Oid, fdVat as Vat, acDesignation AS ArticleClassDesignation, SUM(fdTotalNet) AS TotalNet, SUM(fdTotalTax) AS TotalTax, SUM(fdTotalFinal) AS TotalFinal
                                FROM view_documentfinance
                                WHERE arClass IS NOT NULL AND cfOid IS NOT NULL
                                AND fmOid NOT IN (select DocumentParent From fin_documentfinancemaster where DocumentParent = fmOid AND DocumentStatusStatus <> 'A' AND (ftDocumentTypeAcronym = 'NC' OR ftDocumentTypeAcronym = 'ND' ))
                                AND fmOid IN (SELECT Oid FROM fin_documentfinancemaster WHERE Oid = fmOid and DocumentStatusStatus <> 'A' AND (ftDocumentTypeAcronym = 'FT' OR ftDocumentTypeAcronym = 'FS'  OR ftDocumentTypeAcronym = 'FR'))
                                AND {0}
                                GROUP BY cfOid, fdVat, acDesignation", _filter);

            ReportList<VatSalesByClassSummaryReportData> GCDocumentFinanceVatSalesByClassSummary = new ReportList<VatSalesByClassSummaryReportData>("", 0, query);

            foreach (var item in GCDocumentFinanceVatSalesByClassSummary)
            {
                item.Vat = Convert.ToDecimal(DataConversionUtils.DecimalToString(item.Vat, "0"));
            }

            cfg_configurationcurrency defaultCurrency = XPOSettings.ConfigurationSystemCurrency;

            cfg_configurationcurrency configurationCurrency;
            configurationCurrency = (cfg_configurationcurrency)XPOSettings.Session.GetObjectByKey(typeof(cfg_configurationcurrency), defaultCurrency.Oid);
            _report.SetParameterValue("Currency", configurationCurrency.Acronym);

            _report.RegisterData(GCDocumentFinanceVatSalesByClassSummary, "DocumentFinanceVatSalesByClassSummary");
            if (_report.GetDataSource("DocumentFinanceVatSalesByClassSummary") != null) _report.GetDataSource("DocumentFinanceVatSalesByClassSummary").Enabled = true;
        }

        public void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
