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
        public GetDocumentsQuery Query { get; private set; } = GetDefaultQuery();
        public PaginatedResult<Document> Documents { get; private set; }
        private List<DocumentTotals> _totals = new List<DocumentTotals>();
        private List<DocumentRelation> _relations = new List<DocumentRelation>();
        public List<Document> SelectedDocuments { get; private set; } = new List<Document>();
        public decimal SelectedDocumentsTotalFinal { get; private set; }
        public event EventHandler PageChanged;
        
        public DocumentsPage(Window parent,
                             Dictionary<string, string> options = null) : base(parent, options)
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

        private void LoadDocumentsTotals()
        {
            var query = new GetDocumentsTotalsQuery(_entities.Select(d => d.Id));
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result,
                                                    source: SourceWindow);
                return;
            }

            if (_totals.Count > 0)
            {
                _totals.Clear();
            }

            _totals.AddRange(result.Value);
        }

        private void LoadDocumentsRelations()
        {
            var query = new GetDocumentsRelationsQuery(_entities.Select(x => x.Id));
            var result = _mediator.Send(query).Result;

            if (result.IsError)
            {
                ErrorHandlingService.HandleApiError(result, source: SourceWindow);
                return;
            }

            if (_relations.Count > 0)
            {
                _relations.Clear();
            }

            _relations.AddRange(result.Value);
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var entity = model.GetValue(iterator, 0) as Document;

                if (entity != null && entity.Number.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
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

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            if (GridView.Model.GetIter(out TreeIter iterator, new TreePath(args.Path)))
            {
                var document = (Document)GridView.Model.GetValue(iterator, 0);

                if (SelectedDocuments.Contains(document))
                {
                    SelectedDocuments.Remove(document);
                    SelectedDocumentsTotalFinal -= document.TotalFinal;
                }
                else
                {
                    SelectedDocuments.Add(document);
                    SelectedDocumentsTotalFinal += document.TotalFinal;
                }

                PageChanged?.Invoke(this, EventArgs.Empty);
            }
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

        protected override void InitializeGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(Document), typeof(bool));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
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
