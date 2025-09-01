using Gtk;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.POS
{
    public partial class SaleItemsPage
    {
        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Article.Designation;
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_designation");
            var col = Columns.CreateColumn(title, 1, RenderDesignation, resizable: true, clickable: false);
            col.MinWidth = 30;
            return col;
        }

        private TreeViewColumn CreatePriceColumn()
        {
            void RenderPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.UnitPrice.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_price");
            return Columns.CreateColumn(title, 2, RenderPrice, resizable: false, clickable: false);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Quantity.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_quantity");
            return Columns.CreateColumn(title, 3, RenderQuantity, resizable: false, clickable: false);
        }

        private TreeViewColumn CreateDiscountColumn()
        {
            void RenderDiscount(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Discount.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_discount");
            return Columns.CreateColumn(title, 4, RenderDiscount, resizable: false, clickable: false);
        }

        private TreeViewColumn CreateVatColumn()
        {
            void RenderVat(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Vat.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_vat");
            return Columns.CreateColumn(title, 5, RenderVat, resizable: false, clickable: false);
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderTotal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.TotalFinal.ToString("0.00");
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_total");
            return Columns.CreateColumn(title, 6, RenderTotal, resizable: false, clickable: false);
        }
    }
}
