using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.Api.Features.Finance.Documents.Documents.GetUnsettledDocumentsForSettlement;
using ErrorOr;
using LogicPOS.UI.Components.Finance.Documents.Utilities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using LogicPOS.Api.Features.Common.Pagination;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage : Page<DocumentViewModel>
    {
        private const string UnpaidInvoicesOption = "unpaid-invoices-only";
        public static readonly Dictionary<string, string> UpaidInvoicesOptions = new Dictionary<string, string> { { "selection-page", "true" }, { UnpaidInvoicesOption, "true" } };
        public DocumentsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            AddEventHandlers();
        }

        private bool IsUnpaidInvoicesMode() => Options != null && Options.ContainsKey(UnpaidInvoicesOption);

        public void MoveToNextPage()
        {
            CurrentQuery.Page = Documents.Page + 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void MoveToPreviousPage()
        {
            CurrentQuery.Page = Documents.Page - 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public override void Search(string searchText)
        {
            CurrentQuery = new GetDocumentsQuery { Search = searchText };
            Refresh();
        }

        protected override void LoadEntities()
        {
            ErrorOr.ErrorOr<PaginatedResult<DocumentViewModel>> getDocumentsResult;

            if (IsUnpaidInvoicesMode())
            {
                var unsettledQuery = new GetUnsettledDocumentsForSettlementQuery
                {
                    Page = CurrentQuery.Page,
                    PageSize = CurrentQuery.PageSize,
                    StartDate = CurrentQuery.StartDate,
                    EndDate = CurrentQuery.EndDate,
                    Search = CurrentQuery.Search,
                    CustomerId = CurrentQuery.CustomerId
                };
                getDocumentsResult = _mediator.Send(unsettledQuery).Result;
            }
            else
            {
                getDocumentsResult = _mediator.Send(CurrentQuery).Result;
            }

            if (getDocumentsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getDocumentsResult,
                                                    source: SourceWindow);
                return;
            }

            Documents = getDocumentsResult.Value;

            _entities.Clear();
            _entities.AddRange(Documents.Items);

            if (_entities.Any() == false)
            {
                return;
            }
        }

        public override int RunModal(EntityEditionModalMode mode) => (int)ResponseType.None;

        protected override void AddEntitiesToModel(IEnumerable<DocumentViewModel> documents)
        {
            var model = (ListStore)GridViewSettings.Model;
            foreach (var document in documents)
            {
                model.AppendValues(document, false);
            }
        }

        protected override DeleteCommand GetDeleteCommand() => null;

        private decimal CalculateSelectedDocumentsTotalFinal() =>
            DocumentSettlementTotals.CalculateNetSettlementAmount(SelectedDocuments);
    }
}
