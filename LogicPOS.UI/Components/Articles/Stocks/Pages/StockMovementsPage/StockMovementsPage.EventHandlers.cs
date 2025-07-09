using LogicPOS.Api.Features.Articles.Stocks.Movements.GetStockMovementById;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage
    {
        private void AddEventHandlers()
        {
            Navigator.SearchBox.BtnMore.Clicked += BtnMore_Clicked;
            Navigator.SearchBox.BtnFilter.Clicked += BtnFilter_Clicked;
        }

        private void BtnOpenDocument_Clicked(object sender, EventArgs e)
        {
            if (SelectedEntity == null)
            {
                return;
            }

            if (SelectedEntity.Quantity > 0 && SelectedEntity.HasExternalDocument)
            {
                var filePath = System.IO.Path.GetTempFileName();
                var result = _mediator.Send(new GetStockMovementByIdQuery(SelectedEntity.Id)).Result;

                if (result.IsError)
                {
                    ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                    return;
                }

                System.IO.File.WriteAllBytes(filePath, Convert.FromBase64String(result.Value.ExternalDocument));
                LogicPOSPDFViewer.ShowPDF(filePath);
                return;
            }

            if (SelectedEntity.Quantity < 0 && SelectedEntity.DocumentNumber != null)
            {
                var result = _mediator.Send(new GetDocumentPdfQuery(SelectedEntity.DocumentNumber)).Result;

                if (result.IsError)
                {
                    ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                    return;
                }

                if (result.Value == null)
                {
                    return;
                }

                LogicPOSPDFViewer.ShowPDF(result.Value);
                return;
            }
        }

        private void BtnFilter_Clicked(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void BtnMore_Clicked(object sender, EventArgs e)
        {
            if(CurrentQuery.Page >= Movements.TotalPages)
            {
                return;
            }

            var paginatedResult = ShowMore(CurrentQuery);

            if(paginatedResult == null)
            {
                return;
            }

            Movements = paginatedResult.Value;
            AddEntitiesToModel(Movements.Items);
        }

        public override void UpdateButtonPrevileges()
        {
            //these buttons are always enabled in this page
        }
    }
}
