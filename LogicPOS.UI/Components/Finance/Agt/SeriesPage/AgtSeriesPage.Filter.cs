using Gtk;
using LogicPOS.Api.Features.Finance.Agt.ListOnlineSeries;
using LogicPOS.UI.Components.Modals;
using System;

namespace LogicPOS.UI.Components.Pages
{
    public partial class AgtSeriesPage
    {
        private AgtSeriesFilterModal _filterModal = null;
        public event EventHandler PageChanged;

        private static ListOnlineSeriesQuery GetDefaultQuery()
        {
            var query = new ListOnlineSeriesQuery();

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
                _filterModal = new AgtSeriesFilterModal(SourceWindow);
            }

            var response = (ResponseType)_filterModal.Run();
            var query = _filterModal.GetOnlineSeriesQuery();
            _filterModal.Clear();
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
