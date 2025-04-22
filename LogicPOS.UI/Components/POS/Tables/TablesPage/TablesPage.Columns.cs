
using Gtk;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage
    {
        private TreeViewColumn CreatePlaceColumn()
        {
            void RenderPlace(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var table = (Table)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = table.Place.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_places");
            return Columns.CreateColumn(title, 3, RenderPlace);
        }
    }
}
