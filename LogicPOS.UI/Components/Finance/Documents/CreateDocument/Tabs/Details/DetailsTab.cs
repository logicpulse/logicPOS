using Gtk;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.UI.Components.Articles;
using LogicPOS.UI.Components.Finance.Documents.Services;
using LogicPOS.UI.Components.Finance.VatExemptionReasons;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
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

        public DetailsTab(Window parent) : base(parent: parent,
                                                               name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page3"),
                                                               icon: AppSettings.Paths.Images + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            Page = new DetailsPage(SourceWindow);
            Page.Navigator.RightButtons.Remove(Page.Navigator.BtnRefresh);
            Page.Navigator.SearchBox.Bar.Remove(Page.Navigator.SearchBox.BtnFilter);
            Page.Navigator.SearchBox.Bar.Remove(Page.Navigator.SearchBox.BtnMore);
            Page.Navigator.ExtraButtonSpace.Remove(Page.Navigator.BtnApply);
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
                });
            }

            Page.Refresh();
        }

        public List<Api.Features.Documents.AddDocument.DocumentDetail> GetDocumentDetails(int? priceType = null)
        {
            var details = Page.Items.Select(detail => new Api.Features.Documents.AddDocument.DocumentDetail
            {
                ArticleId = detail.ArticleId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                VatRateId = detail.VatRateId,
                VatExemptionId = detail.VatExemptionReason?.Id,
                Discount = detail.Discount,
                PriceType = priceType,
                Notes = detail.Notes
            });

            return details.ToList();
        }

        public override bool IsValid()
        {
            return Page.Items.Count > 0;
        }
    }
}
