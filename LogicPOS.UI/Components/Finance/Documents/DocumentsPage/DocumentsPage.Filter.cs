using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Documents.GetDocuments;
using LogicPOS.UI.Components.Documents;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class DocumentsPage
    {
        private DocumentsFilterModal _filterModal = null;
        public GetDocumentsQuery Query { get; private set; } = GetDefaultQuery();
        public PaginatedResult<Document> Documents { get; private set; }

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

            Query = query;
            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
