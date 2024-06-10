using FastReport;
using logicpos.shared.Enums;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.Reporting.Common;
using LogicPOS.Reporting.Reports.Documents;
using LogicPOS.Settings;
using LogicPOS.Shared.CustomDocument;
using LogicPOS.Utility;
using System;

namespace LogicPOS.Reporting.Reports
{
    public abstract class SalesReport
    {
        private const string REPORT_FILENAME = "ReportDocumentFinanceMasterList.frx";
        protected readonly Common.FastReport _report;
        private readonly string _readableFilter;
        private readonly string _groupTitle;
        private readonly string _groupCondition;
        private readonly string _reportToken;
        private readonly string _filter;
        protected readonly CustomReportDisplayMode _viewMode;

        public SalesReport(
            string filter,
            string readableFilter,
            string groupTitle,
            string groupCondition,
            string reportToken,
            CustomReportDisplayMode viewMode
            )
        {
            _report = new Common.FastReport(
                reportFileName: REPORT_FILENAME,
                templateBase: Common.FastReport.FILENAME_TEMPLATE_BASE_SIMPLE,
                numberOfCopies: 1);

            _filter = filter;
            _readableFilter = readableFilter;
            _groupTitle = groupTitle;
            _groupCondition = groupCondition;
            _reportToken = reportToken.ToLower();
            _viewMode = viewMode;
        }

        public void Initialize()
        {
            SetParametersValues();
            PrepareDataSources();
        }

        private static void Decrypt(FinanceMasterReportData item)
        {
            item.EntityName = PluginSettings.SoftwareVendor.Decrypt(item.EntityName);
            item.EntityFiscalNumber = PluginSettings.SoftwareVendor.Decrypt(item.EntityFiscalNumber);
        }

        private void SetParametersValues()
        {
            var reportTitle = GeneralUtils.GetResourceByName(_reportToken);
            _report.SetParameterValue("Report Title", reportTitle);
            _report.SetParameterValue("Report_FileName_loggero", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO"]);
            _report.SetParameterValue("Report_FileName_loggero_Small", GeneralSettings.PreferenceParameters["REPORT_FILENAME_loggerO_SMALL"]);

            if (!string.IsNullOrEmpty(_readableFilter))
            {
                _report.SetParameterValue("Report Filter", _readableFilter);
            }


            GroupHeaderBand groupHeaderBand = (GroupHeaderBand)_report.FindObject("GroupHeader1");
            TextObject groupHeaderBandText = (TextObject)_report.FindObject("TextGroupHeader1");
            if (groupHeaderBand != null && groupHeaderBandText != null)
            {
                groupHeaderBand.Condition = _groupCondition;
                groupHeaderBandText.Text = _groupTitle;
            }
            else
            {
                throw new Exception("Error cant find Report Objects");
            }
        }

        protected virtual void PrepareDataSources()
        {
            ReportList<FinanceMasterReportData> financeMasterReportDataList = new ReportList<FinanceMasterReportData>(_filter);

            DevExpress.Xpo.UnitOfWork uowSession = new DevExpress.Xpo.UnitOfWork();
            foreach (var item in financeMasterReportDataList)
            {
                if (PluginSettings.HasSoftwareVendorPlugin)
                {
                    Decrypt(item);
                }

                if (CustomDocumentSettings.CreditNoteDocumentTypeId.Equals(item.DocumentType.Oid))
                {
                    item.PaymentMethod = new fin_configurationpaymentmethod { Designation = item.DocumentType.Designation, Ord = 999, Code = 999 }; /* Setting to 999 to avoid NC being grouped with other Payment Method created */
                    item.PaymentCondition = new fin_configurationpaymentcondition { Designation = item.DocumentType.Designation, Ord = 999, Code = 999 }; /* Sets the same as above in order to keep the pattern */

                    /* IN009084 - Make total values negative when NC (see IN009066) */
                    item.TotalFinalRound *= -1;
                    item.TotalFinal *= -1;
                    item.TotalDiscount *= -1;
                    item.TotalDelivery *= -1;
                    item.TotalTax *= -1;
                    item.TotalNet *= -1;
                    item.TotalGross *= -1;

                }
                else
                {
                    /* Case FS */
                    if (item.PaymentCondition == null)
                    {
                        item.PaymentCondition = (fin_configurationpaymentcondition)uowSession.GetObjectByKey(typeof(fin_configurationpaymentcondition), InvoiceSettings.InstantPaymentMethodId); /* Sets "Pronto Pagamento" to FS */
                    }
                    /* Case FT */
                    if (item.PaymentMethod == null)
                    {
                        if (!item.Payed)
                        {
                            item.PaymentMethod = new fin_configurationpaymentmethod();
                            item.PaymentMethod.Designation = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "report_detailed_grouped_pending_payment");

                        }
                        else
                        {
                            item.PaymentMethod = new fin_configurationpaymentmethod { Designation = item.DocumentType.Designation, Ord = 998, Code = 998 }; /* Setting to 998 to avoid Payed FT being grouped with other Payment Method created */
                        }
                    }
                }
            }

            _report.RegisterData(financeMasterReportDataList, "DocumentFinanceMaster");
            if (_report.GetDataSource("DocumentFinanceMaster") != null) _report.GetDataSource("DocumentFinanceMaster").Enabled = true;
        }

        public virtual void Present()
        {
            _report.Process(_viewMode);
            _report.Dispose();
        }

    }
}
