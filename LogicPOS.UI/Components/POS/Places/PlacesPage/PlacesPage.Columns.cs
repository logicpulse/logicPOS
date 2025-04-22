using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PlacesPage
    {
        private TreeViewColumn CreatePriceTypeColumn()
        {
            void RenderPriceType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var place = (Place)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = place.PriceType.Designation.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPlace_PriceType");
            return Columns.CreateColumn(title, 2, RenderPriceType);
        }

        private TreeViewColumn CreateMovementTypeColumn()
        {
            void RenderMovementType(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var place = (Place)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = place.MovementType.Designation.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPlace_MovementType");
            return Columns.CreateColumn(title, 3, RenderMovementType);
        }
    }
}
