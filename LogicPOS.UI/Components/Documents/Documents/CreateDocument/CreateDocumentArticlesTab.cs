using ClosedXML.Excel;
using Gtk;
using LogicPOS.Api.Features.Documents.AddDocument;
using LogicPOS.Settings;
using LogicPOS.UI.Components.Modals.Common;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class CreateDocumentArticlesTab : ModalTab
    {
        public CreateDocumentItemsPage ItemsPage { get; set; }

        public CreateDocumentArticlesTab(Window parent) : base(parent: parent,
                                                               name: GeneralUtils.GetResourceByName("window_title_dialog_document_finance_page3"),
                                                               icon: PathsSettings.ImagesFolderLocation + @"Icons/Dialogs/DocumentFinanceDialog/icon_pos_dialog_toolbar_3_article.png")
        {
            Initialize();
            Design();
        }

        private void Initialize()
        {
            ItemsPage = new CreateDocumentItemsPage(SourceWindow);
            ItemsPage.Navigator.RightButtons.Remove(ItemsPage.Navigator.BtnRefresh);
            ItemsPage.Navigator.SearchBox.Bar.Remove(ItemsPage.Navigator.SearchBox.BtnFilter);
            ItemsPage.Navigator.SearchBox.Bar.Remove(ItemsPage.Navigator.SearchBox.BtnMore);
            ItemsPage.Navigator.ExtraButtonSpace.Remove(ItemsPage.Navigator.BtnApply);
        }

        private void Design()
        {
            PackStart(ItemsPage);
        }

        public void ImportDataFromDocument(Api.Entities.Document document)
        {
            ItemsPage.Items.Clear();

            foreach (var detail in document.Details)
            {
                ItemsPage.Items.Add(new Item
                {
                    ArticleId = detail.ArticleId,
                    Designation = detail.Designation,
                    Code = detail.Code,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.Price,
                    Vat = detail.VatRate,
                    VatDesignation = detail.VatDesignation,
                    ExemptionReason = detail.VatExemptionReason,
                    Discount = detail.Discount
                });
            }

            ItemsPage.Refresh();
        }

        public List<DocumentDetail> GetDocumentDetails(int? priceType)
        {
            var details = ItemsPage.Items.Select(item => new DocumentDetail
            {
                ArticleId = item.ArticleId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                VatRate = item.VatRate?.Value ?? item.Vat,
                VatDesignation = item.VatRate?.Designation ?? item.VatDesignation,
                VatExemptionReason = item.VatExemptionReason?.Designation ?? item.ExemptionReason,
                Discount = item.Discount,
                PriceType = priceType
            });

            return details.ToList();
        }

        public override bool IsValid()
        {
            return ItemsPage.Items.Count > 0;
        }
    }
}
