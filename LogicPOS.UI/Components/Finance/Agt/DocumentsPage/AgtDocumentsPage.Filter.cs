using Gtk;
using LogicPOS.Api.Features.Common.Pagination;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineDocuments;
using LogicPOS.Api.Features.Finance.Documents.Documents.Common;
using LogicPOS.UI.Components.Modals;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtDocumentsPage
    {
        private AgtDocumentsFilterModal _filterModal = null;
        public event EventHandler PageChanged;
        public PaginatedResult<DocumentViewModel> Documents { get; private set; }

        private static ListOnlineDocumentsQuery GetDefaultQuery()
        {
            var query = new ListOnlineDocumentsQuery(DateTime.Today.AddDays(-90), DateTime.Today);
            return query;
        }

        private void OnPageChanged(object o, EventArgs e)
        {
            CurrentQuery = GetDefaultQuery();
        }

        public void RunFilter()
        {
            if(_filterModal == null)
            {
                _filterModal = new AgtDocumentsFilterModal(SourceWindow);
            }

            var response = (ResponseType)_filterModal.Run();
            var query = _filterModal.GetOnlineDocumentsQuery();

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
