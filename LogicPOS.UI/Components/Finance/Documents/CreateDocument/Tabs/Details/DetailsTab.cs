using Gtk;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class DetailsTab : ModalTab
    {
        public DetailsPage Page { get; set; }

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

        public void ImportDataFromDocument(Api.Entities.Document document)
        {
            Page.Items.Clear();

            foreach (var detail in document.Details)
            {
                Page.Items.Add(new Item
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
                    Discount = detail.Discount,
                    Notes = detail.Notes,
                });
            }

            Page.Refresh();
        }

        public List<DocumentDetail> GetDocumentDetails(int? priceType)
        {
            var details = Page.Items.Select(item => new DocumentDetail
            {
                ArticleId = item.ArticleId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRateId = item.VatRateId,
                VatExemptionId = item.VatExemptionReason?.Id,
                Discount = item.Discount,
                PriceType = priceType,
                Notes = item.Notes
            });

            return details.ToList();
        }

        public override bool IsValid()
        {
            return Page.Items.Count > 0;
        }
    }
}
