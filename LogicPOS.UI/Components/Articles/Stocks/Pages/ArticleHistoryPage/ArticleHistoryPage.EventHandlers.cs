using LogicPOS.Api.Features.Articles.StockManagement.GetArticleSerialNumberPdf;
using LogicPOS.Api.Features.Articles.Stocks.Movements.GetStockMovementById;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage
    {
        private void AddEventHandlers()
        {
            Navigator.SearchBox.BtnMore.Clicked += BtnMore_Clicked;
        }

        public void BtnMore_Clicked(object sender, EventArgs e)
        {
            if (CurrentQuery.Page >= Histories.TotalPages)
            {
                return;
            }

            var paginatedResult = ShowMore(CurrentQuery);

            if (paginatedResult == null)
            {
                return;
            }

            Histories = paginatedResult.Value;
            AddEntitiesToModel(Histories.Items);
        }
        private void BtnOpenSaleDocument_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedEntity.SaleDocument))
            {
                return;
            }

            var result = _mediator.Send(new GetDocumentPdfQuery(SelectedEntity.SaleDocument, false)).Result;
            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                return;
            }

            TempFile tempFile = result.Value;

            LogicPOSPDFViewer.ShowPDF(tempFile.Path, tempFile.Name);
            return;
        }

        private void BtnOpenExternalDocument_Clicked(object sender, EventArgs e)
        {
            if (SelectedEntity == null)
            {
                return;
            }

            if (SelectedEntity.HasExternalDocument)
            {
                var filePath = global::System.IO.Path.GetTempFileName();
                var result = _mediator.Send(new GetStockMovementByIdQuery(SelectedEntity.InMovementId)).Result;

                if (result.IsError)
                {
                    ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                    return;
                }

                global::System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(result.Value.ExternalDocument));
                LogicPOSPDFViewer.ShowPDF(filePath, $"Document_Externo_{result.Value.DocumentNumber}");
                return;
            }
        }

        private void BtnPrintSerialNumber_Clicked(object sender, EventArgs e)
        {
            if (SelectedEntity == null || string.IsNullOrWhiteSpace(SelectedEntity.SerialNumber))
            {
                return;
            }

            var result = _mediator.Send(new GetArticleSerialNumberPdfQuery(SelectedEntity.Id)).Result;

            if (result.IsError)
            {
                HandleErrorResult(result);
                return;
            }

            LogicPOSPDFViewer.ShowPDF(result.Value.Path, result.Value.Name);
        }

        public override void UpdateButtonPrevileges()
        {
            this.Navigator.BtnInsert.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEWAREHOUSE_CREATE");
            this.Navigator.BtnUpdate.Sensitive = Users.AuthenticationService.UserHasPermission("BACKOFFICE_MAN_ARTICLEWAREHOUSE_EDIT");
        }

    }
}
