
using Gtk;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage
    {
        private TreeViewColumn CreatePlaceColumn()
        {
            void RenderPlace(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var table = (TableViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = table.Place;
            }

            var title = GeneralUtils.GetResourceByName("global_places");
            return Columns.CreateColumn(title, 3, RenderPlace);
        }
    }
}
