using Gtk;
using LogicPOS.Api.Features.Articles.StockManagement.GetArticlesHistories;
using LogicPOS.Api.Features.Articles.Stocks.Movements.GetStockMovementById;
using LogicPOS.Api.Features.Articles.Stocks.UniqueArticles.GenerateBarcodeLabelPdf;
using LogicPOS.Api.Features.Common.Responses;
using LogicPOS.Api.Features.Finance.Documents.Documents.Prints.GetDocumentPdf;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Components.Articles.Stocks.Modals.Filters;
using LogicPOS.UI.Errors;
using LogicPOS.UI.PDFViewer;
using System;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ArticleHistoryPage
    {
        private void AddEventHandlers()
        {
            Navigator.SearchBox.BtnMore.Clicked += BtnMore_Clicked;
            Navigator.SearchBox.BtnFilter.Clicked += BtnFilter_Clicked;
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

        private void BtnPrintBarcodeLabel_Clicked(object sender, EventArgs e)
        {
            if (SelectedHistories.Count == 0)
            {
                return;
            }

            var ids = SelectedHistories.Select(x => x.Id).ToList();
            var result = _mediator.Send(new GenerateBarcodeLabelPdfQuery(ids)).Result;

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

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var history = (ArticleHistory)GridView.Model.GetValue(iterator, 0);

                if (SelectedHistories.Contains(history))
                {
                    SelectedHistories.Remove(history);
                }
                else
                {
                    SelectedHistories.Add(history);
                }
            }
        }

        private void BtnFilter_Clicked(object sender, EventArgs e)
        {
            RunFilterModal();
        }

        public void RunFilterModal()
        {
            if (filterModal == null)
            {
                filterModal = new ArticleHistoryFilterModal(SourceWindow);
            }
            var response = (ResponseType)filterModal.Run();
            ArticleHistoryFilterModalData? filterModalData = filterModal.GetFilterData();
            filterModal.Hide();

            if (response != ResponseType.Ok)
            {
                return;
            }

            CurrentQuery.StartDate = filterModalData?.StartDate ?? CurrentQuery.StartDate;
            CurrentQuery.EndDate = filterModalData?.EndDate ?? CurrentQuery.EndDate;
            CurrentQuery.ArticleId = filterModalData?.ArticleId ?? CurrentQuery.ArticleId;
            CurrentQuery.Search = filterModalData?.SerialNumber ?? CurrentQuery.Search;

            Refresh();

            if (_entities.Count == 0)
            {
                CustomAlerts.Information()
                    .WithMessageResource("dialog_message_report_filter_no_records_with_criteria")
                    .ShowAlert();

                CurrentQuery = GetDefaultQuery();
                Refresh();
            }

            PageChanged?.Invoke(this, EventArgs.Empty);

        }
    }
}
