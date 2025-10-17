using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Components.Documents;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        private DocumentsFilterModal _filterModal = null;

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

        public GetDocumentsQuery CurrentQuery { get; private set; } = GetDefaultQuery();
        public PaginatedResult<DocumentViewModel> Documents { get; private set; }

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
            if(_filterModal == null)
            {
                _filterModal = new DocumentsFilterModal(SourceWindow);
            }

            var response = (ResponseType)_filterModal.Run();
            var query = _filterModal.GetDocumentsQuery();

            _filterModal.Hide();

            if (response != ResponseType.Ok)
            {
                return;
            }

            CurrentQuery = query;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
