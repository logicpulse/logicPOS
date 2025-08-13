using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Documents.GetDocumentsTotals;
using LogicPOS.UI.Components.Documents.Utilities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Errors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage : Page<Document>
    {
        public DocumentsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            AddEventHandlers();
        }

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
            var getDocumentsResult = _mediator.Send(CurrentQuery).Result;

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

            LoadDocumentsTotals();
            LoadDocumentsRelations();
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

        protected override void AddEntitiesToModel(IEnumerable<Document> documents)
        {
            var model = (ListStore)GridViewSettings.Model;
            foreach (var document in documents)
            {
                model.AppendValues(document, false);
            }
        }

        public IEnumerable<(Document, DocumentTotals)> GetSelectedDocumentsWithTotals()
        {
            return SelectedDocuments.Select(document => (document, _totals.FirstOrDefault(x => x.DocumentId == document.Id)));
        }

        protected override DeleteCommand GetDeleteCommand() => null;
    }
}
