using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Documents.GetDocumentsRelations;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Errors;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage : Page<Document>
    {

        public DocumentsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
        }

        public void MoveToNextPage()
        {
            Query.Page = Documents.Page + 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public void MoveToPreviousPage()
        {
            Query.Page = Documents.Page - 1;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        private static GetDocumentsQuery GetDefaultQuery()
        {
            var query = new GetDocumentsQuery
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now,
            };

            return query;
        }

        protected override void LoadEntities()
        {
            var getDocumentsResult = _mediator.Send(Query).Result;

            if (getDocumentsResult.IsError)
            {
                ErrorHandlingService.HandleApiError(getDocumentsResult,
                                                    source: SourceWindow);
                return;
            }

            Documents = getDocumentsResult.Value;

            _entities.Clear();
            _entities.AddRange(Documents.Items);

            LoadDocumentsTotals();
            LoadDocumentsRelations();
        }

        public void RunFilter()
        {
            var filterModal = new DocumentsFilterModal(SourceWindow);
            var response = (ResponseType)filterModal.Run();
            var query = filterModal.GetDocumentsQuery();
            filterModal.Destroy();

            if (response != ResponseType.Ok)
            {
                return;
            }

            Query = query;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }

        public override int RunModal(EntityEditionModalMode mode) => (int)ResponseType.None;

        protected override void AddColumns()
        {
            GridView.AppendColumn(CreateSelectColumn());
            GridView.AppendColumn(CreateDateColumn());
            GridView.AppendColumn(CreateNumberColumn());
            GridView.AppendColumn(CreateStatusColumn());
            GridView.AppendColumn(CreateEntityColumn());
            GridView.AppendColumn(CreateFiscalNumberColumn());
            GridView.AppendColumn(CreateTotalFinalColumn());
            GridView.AppendColumn(CreateTotalPaidColumn());
            GridView.AppendColumn(CreateTotalToPayColumn());
            GridView.AppendColumn(CreateRelatedDocumentsColumn());
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddDateSorting();
            AddNumberSorting();
            AddStatusSorting();
            AddEntitySorting();
            AddFiscalNumberSorting();
            AddTotalFinalSorting();
        }

        protected override void AddEntitiesToModel()
        {
            var model = (ListStore)GridViewSettings.Model;
            _entities.ForEach(entity => model.AppendValues(entity, false));
        }

        public IEnumerable<(Document, DocumentTotals)> GetSelectedDocumentsWithTotals()
        {
            return SelectedDocuments.Select(document => (document, _totals.FirstOrDefault(x => x.DocumentId == document.Id)));
        }

        protected override DeleteCommand GetDeleteCommand() => null;
    }
}
