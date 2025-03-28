using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticleSerialNumberPdf;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.PDFViewer;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage : Page<ArticleHistory>
    {
        protected override IRequest<ErrorOr<IEnumerable<ArticleHistory>>> GetAllQuery => new GetArticlesHistoriesQuery();
        private IconButtonWithText BtnPrintSerialNumber { get; set; } = IconButtonWithText.Create("buttonUserId",
                                                                                                  "Cod.Barras",
                                                                                                  @"Icons/Dialogs/icon_pos_dialog_action_print.png");
        private IconButtonWithText BtnOpenExternalDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument);
        private IconButtonWithText BtnOpenSaleDocument { get; set; } = ActionAreaButton.FactoryGetDialogButtonTypeDocuments(DialogButtonType.OpenDocument);

        public ArticleHistoryPage(Window parent) : base(parent)
        {
            RemoveForbiddenButtons();
            AddPrintSerialNumberButton();
            AddOpenExternalDocumentButton();
            AddOpenSaleDocumentButton();
        }
        public ArticleHistoryPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }
        private void AddOpenSaleDocumentButton()
        {
            BtnOpenSaleDocument.ButtonLabel.Text = "Doc.Venda";
            BtnOpenSaleDocument.Clicked += BtnOpenSaleDocument_Clicked;
            Navigator.RightButtons.PackStart(BtnOpenSaleDocument, false, false, 0);
        }

        private void BtnOpenSaleDocument_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddOpenExternalDocumentButton()
        {
            BtnOpenExternalDocument.ButtonLabel.Text = "Doc.Origem";
            BtnOpenExternalDocument.Clicked += BtnOpenExternalDocument_Clicked;
            Navigator.RightButtons.PackStart(BtnOpenExternalDocument, false, false, 0);
        }

        private void BtnOpenExternalDocument_Clicked(object sender, EventArgs e)
        {
            if (SelectedEntity == null)
            {
                return;
            }

            if (SelectedEntity.InStockMovement.Quantity > 0 && SelectedEntity.InStockMovement.ExternalDocument != null)
            {
                var filePath = System.IO.Path.GetTempFileName();
                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(SelectedEntity.InStockMovement.ExternalDocument));
                LogicPOSPDFViewer.ShowPDF(filePath);
                return;
            }
        }

        private void AddPrintSerialNumberButton()
        {
            BtnPrintSerialNumber.Clicked += BtnPrintSerialNumber_Clicked;
            Navigator.RightButtons.Add(BtnPrintSerialNumber);
        }

        private void BtnPrintSerialNumber_Clicked(object sender, EventArgs e)
        {
            if (SelectedEntity == null || string.IsNullOrWhiteSpace(SelectedEntity.WarehouseArticle.SerialNumber))
            {
                return;
            }

            var result = _mediator.Send(new GetArticleSerialNumberPdfQuery(SelectedEntity.WarehouseArticle.Id)).Result;

            if (result.IsError)
            {
                HandleErrorResult(result);
                return;
            }

            LogicPOSPDFViewer.ShowPDF(result.Value);
        }

        private void RemoveForbiddenButtons()
        {
            Navigator.RightButtons.Remove(Navigator.BtnView);
            Navigator.RightButtons.Remove(Navigator.BtnDelete);
            Navigator.RightButtons.Remove(Navigator.BtnInsert);
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new UpdateUniqueArticleModal(SelectedEntity);
            int response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateDesignationColumn());
            GridView.AppendColumn(CreateSerialNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateIsComposedColumn());
            GridView.AppendColumn(CreatePurchaseDateColumn());
            GridView.AppendColumn(CreateProviderColumn());
            GridView.AppendColumn(CreatePurchasePriceColumn());
            GridView.AppendColumn(CreateOriginDocumentColumn());
            GridView.AppendColumn(CreateSaleDocumentColumn());
            GridView.AppendColumn(CreateWarehouseColumn());
            GridView.AppendColumn(CreateLocationColumn());
            GridView.AppendColumn(CreateUpdatedAtColumn());
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
        }
    }
}
