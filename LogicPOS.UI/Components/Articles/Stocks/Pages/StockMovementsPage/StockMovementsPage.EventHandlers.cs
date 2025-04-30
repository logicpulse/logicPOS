using Gtk;
using LogicPOS.Api.Features.Articles.Stocks.Movements.GetStockMovementById;
using LogicPOS.Api.Features.Documents.Documents.GetDocumentPdf;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage
    {
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
     
            var nextPageQuery = CurrentQuery.GetNextPageQuery();
            var result = _mediator.Send(nextPageQuery).Result;

            if (result.IsError)
            {
                HandleErrorResult(result);
                return;
            }

            var movements = result.Value;
            _entities.AddRange(movements.Items);
            CurrentQuery.Page = nextPageQuery.Page;

            var model = (ListStore)GridViewSettings.Model;

            foreach (var entity in movements.Items)
            {
                model.AppendValues(entity);
            }
        }
    }
}
