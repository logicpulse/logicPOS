using Gtk;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using OxyPlot.Series;
using System;
using System.Windows.Forms;
using Label = Gtk.Label;

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
            var col = CreateColumn(title, RenderDesignation);
            col.MaxWidth = Convert.ToInt16(Theme.Columns.DesignationWidth) - 10;
            col.MinWidth = col.MaxWidth;
            return col;
        }

        private TreeViewColumn CreatePriceColumn()
        {
            void RenderPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.UnitPrice.ToString("0.00");
                (cell as CellRendererText).Xalign = 1.0F;
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_price");
            var col = CreateColumn(title, RenderPrice);
            col.MaxWidth = 65;
            col.MinWidth = col.MaxWidth;
            col.Alignment = 1.0F;
            return col;
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Quantity.ToString("0.00");
                (cell as CellRendererText).Xalign = 1.0F;
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_quantity");
            var col = CreateColumn(title, RenderQuantity);
            col.MaxWidth = 55;
            col.MinWidth = col.MaxWidth;
            col.Alignment = 1.0F;
            return col;
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderTotal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (SaleItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.TotalFinal.ToString("0.00");
                (cell as CellRendererText).Xalign = 1.0F;
            }

            var title = GeneralUtils.GetResourceByName("pos_ticketlist_label_total");
            var col =  CreateColumn(title, RenderTotal);
            col.MaxWidth = 75;
            col.MinWidth = col.MaxWidth;
            col.Alignment = 1.0F;
            return col;
        }


        public TreeViewColumn CreateColumn(string title,
                                                 TreeCellDataFunc renderFunction)
        {
            TreeViewColumn column = new TreeViewColumn();
            column.Title = title;
            var label = new Label(column.Title);
            label.ModifyFont(Pango.FontDescription.FromString(Theme.Columns.FontTitle));
            label.Show();
            column.Widget = label;

            var cellRenderer = CellRenderers.Text();
            column.PackStart(cellRenderer, true);

            column.SetCellDataFunc(cellRenderer, renderFunction);

            return column;
        }
    }
}
