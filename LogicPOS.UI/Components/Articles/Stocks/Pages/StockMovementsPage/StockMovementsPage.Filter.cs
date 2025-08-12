using Gtk;
using LogicPOS.Api.Features.Articles.StockManagement.GetStockMovements;
using LogicPOS.UI.Components.Articles.Stocks.Movements;
using LogicPOS.UI.Components.Articles.Stocks.Pages.StockMovementsPage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Pages
{
    public partial class StockMovementsPage
    {
        protected StockMovementsFilterModal _filterModal=null;
        private static GetStockMovementsQuery GetDefaultQuery()
        {
            var query = new GetStockMovementsQuery
            {
                StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                EndDate = DateTime.Now,
            };

            return query;
        }
        public void RunFilterModal()
        {
            if(_filterModal == null)
            {
            _filterModal = new StockMovementsFilterModal(SourceWindow);
            }
            var response = (ResponseType)_filterModal.Run();
            StockMovementsFilterModalData? filterModalData = _filterModal.GetFilterData();
            _filterModal.Hide();

            if (response != ResponseType.Ok)
            {
                return;
            }

            CurrentQuery.StartDate = filterModalData?.StartDate ?? CurrentQuery.StartDate;
            CurrentQuery.EndDate = filterModalData?.EndDate ?? CurrentQuery.EndDate;
            CurrentQuery.ArticleId = filterModalData?.ArticleId ?? CurrentQuery.ArticleId;
            CurrentQuery.CustomerId = filterModalData?.CustomerId ?? CurrentQuery.CustomerId;

            Refresh();
            PageChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
