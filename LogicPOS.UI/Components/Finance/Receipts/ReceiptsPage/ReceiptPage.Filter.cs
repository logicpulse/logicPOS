using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Receipts.GetReceipts;
using LogicPOS.UI.Components.Documents;
using LogicPOS.UI.Components.GridViews;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class ReceiptsPage
    {
        DocumentsFilterModal _filterModal = null;
        private static GetReceiptsQuery GetDefaultQuery()
        {
            var query = new GetReceiptsQuery
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now,
            };

            return query;
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

                var receipt = model.GetValue(iterator, 0) as ReceiptViewModel;

                if (receipt != null && receipt.RefNo.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        public void RunFilter()
        {
            if (_filterModal == null)
            {
                _filterModal=new DocumentsFilterModal(SourceWindow);
            }
            var response = (ResponseType)_filterModal.Run();
            var query = _filterModal.GetReceiptsQuery();
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
