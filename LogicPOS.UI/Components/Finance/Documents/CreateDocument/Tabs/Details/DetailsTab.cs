using Gtk;
using LogicPOS.Globalization;
using LogicPOS.UI.Components.Finance.Documents.Sdr;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Finance.VatExemptionReasons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class DetailsTab : ModalTab
    {
        public DetailsPage Page { get; set; }
        public decimal TotalFinal => Page.TotalFinal;
        public decimal ServicesTotalFinal => Page.ServicesTotalFinal;
        public Func<string> GetDocumentType { get; set; }

        public DetailsTab(Window parent) : base(parent: parent,
                                                               name: LocalizedString.Instance["window_title_dialog_document_finance_page3"],
                                                               icon: AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            Page = new DetailsPage(SourceWindow);
            Page.GetDocumentType = () => GetDocumentType?.Invoke();
        }

        private void Design()
        {
            PackStart(Page);
        }

        public void ImportDataFromDocument(Guid documentId, decimal globalDiscount)
        {
            Page.Items.Clear();

            var details = DocumentsService.GetDocumentDetails(documentId);

            foreach (var detail in details)
            {
                Page.Items.Add(new DocumentDetail
                {
                    ArticleId = detail.ArticleId,
                    Designation = detail.Designation,
                    Code = detail.Code,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.Price,
                    VatRateId = detail.Tax.TaxId,
                    Vat = detail.Tax.Percentage,
                    VatDesignation = detail.Tax.Designation,
                    ExemptionReason = detail.VatExemptionReason,
                    VatExemptionReason = VatExemptionReasonsService.Reasons.Where(r => r.Acronym == detail.VatExemptionCode).FirstOrDefault(),
                    Discount = detail.Discount - globalDiscount,
                    Notes = detail.Notes,
                    SerialNumber = detail.SerialNumber
                });
            }

            Page.Refresh();
        }

        public List<Api.Features.Finance.Documents.Documents.IssueDocument.DocumentDetail> GetDocumentDetails()
        {
            var details = Page.Items.Select(detail => new Api.Features.Finance.Documents.Documents.IssueDocument.DocumentDetail
            {
                ArticleId = detail.ArticleId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                VatRateId = detail.VatRateId,
                VatExemptionId = detail.VatExemptionReason?.Id,
                Discount = detail.Discount,
                Notes = detail.Notes,
                SerialNumber = detail.SerialNumber,
            });

            var list = details.ToList();

            if (TrvDocumentUiRules.IsTrvDocument(GetDocumentType?.Invoke()))
            {
                return list;
            }

            return SdrDocumentDetailsService.Enrich(list);
        }

        public override bool IsValid()
        {
            if (Page.Items.Count == 0)
            {
                return false;
            }

            if (!TrvDocumentUiRules.IsTrvDocument(GetDocumentType?.Invoke()))
            {
                return true;
            }

            return Page.Items.All(item => TrvDocumentUiRules.IsAllowedArticle(item.Code));
        }
    }
}
