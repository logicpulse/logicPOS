using Gtk;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System.Data.Common;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentItemsPage
    {

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            var designationColumn = Columns.CreateDesignationColumn(1);
            designationColumn.MaxWidth = 250;
            GridView.AppendColumn(designationColumn);
            GridView.AppendColumn(CreateQuantityColumn());
            GridView.AppendColumn(CreatePriceColumn());
            GridView.AppendColumn(CreateDiscountColumn());
            GridView.AppendColumn(CreateTaxColumn());
            GridView.AppendColumn(CreateTotalColumn());
            GridView.AppendColumn(CreateTotalWithTaxColumn());
        }

        private TreeViewColumn CreateTotalWithTaxColumn()
        {
            void RenderTotalWithTax(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.TotalFinal.ToString("F2");
            }

            var title = GeneralUtils.GetResourceByName("global_total_per_item_vat");
            return Columns.CreateColumn(title, 7, RenderTotalWithTax);
        }

        private TreeViewColumn CreatePriceColumn()
        {
            void RenderPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.UnitPrice.ToString("F2");
            }

            var title = GeneralUtils.GetResourceByName("global_price");
            return Columns.CreateColumn(title, 3, RenderPrice);
        }

        private TreeViewColumn CreateDiscountColumn()
        {
            void RenderDiscount(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Discount.ToString("F2");
            }

            var title = GeneralUtils.GetResourceByName("global_discount");
            return Columns.CreateColumn(title, 4, RenderDiscount);
        }

        private TreeViewColumn CreateTaxColumn()
        {
            void RenderTax(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.VatRate?.Designation ?? item.VatDesignation;
            }

            var title = GeneralUtils.GetResourceByName("global_vat_rate");
            return Columns.CreateColumn(title, 5, RenderTax);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Quantity.ToString("F2");
            }

            var title = GeneralUtils.GetResourceByName("global_quantity_acronym");
            return Columns.CreateColumn(title, 2, RenderQuantity);
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderTotal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.TotalNet.ToString("F2");
            }

            var title = GeneralUtils.GetResourceByName("global_total_article_tab");
            return Columns.CreateColumn(title, 6, RenderTotal);
        }
    }
}
