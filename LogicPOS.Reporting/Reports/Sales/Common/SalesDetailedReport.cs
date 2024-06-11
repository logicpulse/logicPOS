using DevExpress.Xpo;
using FastReport;
using logicpos.shared.Enums;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Data.Common;
using LogicPOS.Reporting.Reports.Data;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Utility;
using System;

namespace LogicPOS.Reporting.Reports
{
    public abstract class SalesDetailedReport
    {
        private const string REPORT_GROUPED_FILENAME = "ReportDocumentFinanceDetailGroupList.frx";
        private const string REPORT_FILENAME = "ReportDocumentFinanceDetailList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupField;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private string _filter;
        protected readonly CustomReportDisplayMode _viewMode;
        private readonly bool _isGrouped;
        private bool _decryptGroupField;
        private string _groupSelectFields;

        public SalesDetailedReport(
            string filter,
            string readableFilter,
            string groupTitle,
            string groupCondition,
            string reportToken,
            CustomReportDisplayMode viewMode,
            bool isGrouped = false,
            bool decryptGroupField = false,
            string groupField = null,
            string groupSelectFields = null
          )
        {
            string reportFileName = isGrouped ? REPORT_GROUPED_FILENAME : REPORT_FILENAME;

            _report = new Common.FastReport(
                reportFileName: reportFileName,
                templateBase: Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                numberOfCopies: 1);

            _filter = filter;
            _readableFilter = readableFilter;
            _groupTitle = groupTitle;
            _groupCondition = groupCondition;
            _reportToken = reportToken.ToLower();
            _viewMode = viewMode;
            _isGrouped = isGrouped;
            _groupField = groupField;
            _decryptGroupField = decryptGroupField;
            _groupSelectFields = groupSelectFields;
        }

        public void Initialize()
        {
            SetParametersValues();
            PrepareDataSources();
        }


        private void SetParametersValues()
        {
            Tuple<string, string> reportTitle = Common.FastReport.GetResourceString(_reportToken);
            string reportTitleString = reportTitle.Item1;
            string reportTitleStringPostfix = reportTitle.Item2;

            _report.SetParameterValue("Report Title", $"{reportTitleString}{reportTitleStringPostfix}");
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);
            if (!string.IsNullOrEmpty(_readableFilter)) _report.SetParameterValue("Report Filter", _readableFilter);

            // Get Objects
            GroupHeaderBand groupHeaderBand = (GroupHeaderBand)_report.FindObject("GroupHeader1");
            TextObject groupHeaderBandText = (TextObject)_report.FindObject("TextGroupHeader1");
            if (groupHeaderBand != null && groupHeaderBandText != null)
            {
                groupHeaderBand.Condition = _groupCondition;
                groupHeaderBandText.Text = _groupTitle;
                if (_groupTitle == "[DocumentFinanceDetail.ArticleVat]")
                {

                    groupHeaderBandText.Text = GeneralUtils.GetResourceByName("global_vat_rates") + ": " + _groupTitle + "%";
                    string documentNodeFilter = " AND (ftSaftDocumentType = 1 AND (fmDocumentStatusStatus = 'N' OR fmDocumentStatusStatus = 'F' OR fmDocumentStatusStatus = 'A'))";
                    _filter += documentNodeFilter;
                }
            }
            else
            {
                throw new Exception("Error cant find Report Objects");
            }
        }

        protected virtual void PrepareDataSources()
        {
            if (!_isGrouped)
            {
                ReportList<FinanceMasterDetailViewReportData> financeMasterDetailViewReportDataList =
                    new ReportList<FinanceMasterDetailViewReportData>(_filter);

                if (financeMasterDetailViewReportDataList.List.Count == 0)
                {
                    financeMasterDetailViewReportDataList = new ReportList<FinanceMasterDetailViewReportData>();
                }

                if (PluginSettings.HasSoftwareVendorPlugin)
                {
                    UnitOfWork unitOfWork = new UnitOfWork();

                    foreach (var item in financeMasterDetailViewReportDataList)
                    {
                        item.UserDetailName = PluginSettings.SoftwareVendor.Decrypt(item.UserDetailName);
                        item.EntityName = PluginSettings.SoftwareVendor.Decrypt(item.EntityName);
                        item.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(item.EntityFiscalNumber);

                        if (CustomDocumentSettings.CreditNoteDocumentTypeId.Equals(new Guid(item.DocumentType)))
                        {
                            item.ArticleQuantity *= -1;
                            item.ArticleTotalFinal *= -1;
                            item.ArticleTotalNet *= -1;
                            item.ArticleTotalTax *= -1;
                            item.PaymentMethod = item.DocumentTypeDesignation;
                            item.PaymentMethodOrd = 999;
                            item.PaymentMethodCode = 999;

                            item.PaymentCondition = item.DocumentTypeDesignation;
                            item.PaymentConditionOrd = 999;
                            item.PaymentConditionCode = 999;
                        }
                        else
                        {
                            if (item.PaymentCondition == null)
                            {
                                fin_configurationpaymentcondition paymentCondition = (fin_configurationpaymentcondition)unitOfWork.GetObjectByKey(typeof(fin_configurationpaymentcondition), InvoiceSettings.InstantPaymentMethodId);
                                item.PaymentCondition = paymentCondition.Designation;
                            }


                            if (item.PaymentMethod == null)
                            {
                                if (item.Payed == false)
                                {
                                    item.PaymentMethod = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_detailed_grouped_pending_payment");
                                }
                                else
                                {
                                    item.PaymentMethod = item.DocumentTypeDesignation;
                                    item.PaymentMethodOrd = 998;
                                    item.PaymentMethodCode = 998;
                                }
                            }
                        }

                        if (item.PlaceCode == 0)
                        {
                            item.PlaceOrd = 999;
                            item.PlaceCode = 999;
                            item.PlaceDesignation = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_others");
                        }

                        if (item.PlaceTableCode == 0)
                        {
                            item.PlaceTableOrd = 999;
                            item.PlaceTableCode = 999;
                            item.PlaceTableDesignation = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_others");
                        }
                    }
                }


                _report.RegisterData(financeMasterDetailViewReportDataList, "DocumentFinanceDetail");
            }
            else
            {
                string queryGroupFields = $"fdArticle, fdCode, fdDesignation, fdUnitMeasure, {_groupField}";

                string queryFields = string.Format("{0}, fdArticle AS ArticleOid, fdCode AS ArticleCode, fdDesignation AS ArticleDesignation, AVG((fdPrice - ((fdPrice * fdDiscount) / 100))) AS ArticlePriceWithDiscount, SUM(fdQuantity) AS ArticleQuantity, fdUnitMeasure AS ArticleUnitMeasure, SUM(fdTotalDiscount) AS ArticleTotalDiscount, SUM(fdTotalNet) AS ArticleTotalNet, SUM(fdTotalTax) AS ArticleTotalTax, SUM(fdTotalFinal) AS ArticleTotalFinal,COUNT(*) AS GroupCount", _groupSelectFields);

                ReportList<FinanceMasterDetailGroupViewReportData> gcDocumentFinanceMasterDetail = new
                    ReportList<FinanceMasterDetailGroupViewReportData>(_filter, queryGroupFields, string.Empty, queryFields);


                if (PluginSettings.HasSoftwareVendorPlugin)
                {
                    foreach (var item in gcDocumentFinanceMasterDetail)
                    {

                        if (_decryptGroupField)
                        {
                            item.GroupDesignation = PluginSettings.SoftwareVendor.Decrypt(item.GroupDesignation);
                        }

                        if (CustomDocumentSettings.CreditNoteDocumentTypeId.Equals(new Guid(item.DocumentType)))
                        {
                            item.ArticleQuantity *= -1;
                            item.ArticleTotalNet *= -1;
                            item.ArticleTotalTax *= -1;
                            item.ArticleTotalFinal *= -1;
                        }
                    }
                }

                _report.RegisterData(gcDocumentFinanceMasterDetail, "DocumentFinanceDetail");
            }

            if (_report.GetDataSource("DocumentFinanceDetail") != null) _report.GetDataSource("DocumentFinanceDetail").Enabled = true;
        }

        public virtual void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }
    }
}
