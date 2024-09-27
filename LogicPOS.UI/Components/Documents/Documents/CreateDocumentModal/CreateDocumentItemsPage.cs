using Gtk;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Documents.CreateDocumentModal
{
    public class CreateDocumentItemsPage : Page<Item>
    {
        public List<Item> Items => _entities;
        public CreateDocumentItemsPage(Window parent) : base(parent)
        {
        }

        protected override void LoadEntities() { }

        public override void DeleteEntity()
        {
            if (SelectedEntity == null)
            {
                return;
            }

            _entities.Remove(SelectedEntity);
            SelectedEntity = null;

            Refresh();
        }

        public override void RunModal(EntityEditionModalMode mode)
        {
            switch (mode)
            {
                case EntityEditionModalMode.Insert:
                    RunInsertModal();
                    break;
                case EntityEditionModalMode.Update:
                    RunUpdateModal();
                    break;
                default:
                    RunViewModal();
                    break;
            }

            Refresh();
        }

        private void RunViewModal()
        {
            if (SelectedEntity == null)
            {
                return;
            }

            var modal = new AddItemModal(SourceWindow, EntityEditionModalMode.View, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        private void RunUpdateModal()
        {
            if (SelectedEntity == null)
            {
                return;
            }

            var modal = new AddItemModal(SourceWindow, EntityEditionModalMode.Update, SelectedEntity);
            var response = (ResponseType)modal.Run();

            modal.Destroy();
        }

        private void RunInsertModal()
        {
            var modal = new AddItemModal(SourceWindow, EntityEditionModalMode.Insert);
            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                _entities.Add(modal.GetItem());
            }

            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
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
                (cell as CellRendererText).Text = item.TotalFinal.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_total_per_item_vat");
            return Columns.CreateColumn(title, 7, RenderTotalWithTax);
        }

        private TreeViewColumn CreatePriceColumn()
        {
            void RenderPrice(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.UnitPrice.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_price");
            return Columns.CreateColumn(title, 3, RenderPrice);
        }

        private TreeViewColumn CreateDiscountColumn()
        {
            void RenderDiscount(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Discount.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_discount");
            return Columns.CreateColumn(title, 4, RenderDiscount);
        }

        private TreeViewColumn CreateTaxColumn()
        {
            void RenderTax(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.VatRate.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_vat_rate");
            return Columns.CreateColumn(title, 5, RenderTax);
        }

        private TreeViewColumn CreateQuantityColumn()
        {
            void RenderQuantity(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.Quantity.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_quantity_acronym");
            return Columns.CreateColumn(title, 2, RenderQuantity);
        }

        private TreeViewColumn CreateTotalColumn()
        {
            void RenderTotal(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var item = (Item)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = item.TotalNet.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_total_article_tab");
            return Columns.CreateColumn(title, 6, RenderTotal);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddQuantitySorting();
            AddPriceSorting();
            AddDiscountSorting();
            AddTaxSorting();
            AddTotalSorting();
            AddTotalWithTaxSorting();
        }

        private void AddTotalWithTaxSorting()
        {
            GridViewSettings.Sort.SetSortFunc(7, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.TotalFinal.CompareTo(rightItem.TotalFinal);
            });
        }

        private void AddQuantitySorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.Quantity.CompareTo(rightItem.Quantity);
            });
        }

        private void AddPriceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.UnitPrice.CompareTo(rightItem.UnitPrice);
            });
        }

        private void AddDiscountSorting()
        {
            GridViewSettings.Sort.SetSortFunc(4, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.Discount.CompareTo(rightItem.Discount);
            });
        }

        private void AddTaxSorting()
        {
            GridViewSettings.Sort.SetSortFunc(5, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.VatRate.Designation.CompareTo(rightItem.VatRate.Designation);
            });
        }

        private void AddTotalSorting()
        {
            GridViewSettings.Sort.SetSortFunc(6, (model, left, right) =>
            {
                var leftItem = (Item)model.GetValue(left, 0);
                var rightItem = (Item)model.GetValue(right, 0);

                if (leftItem == null || rightItem == null)
                {
                    return 0;
                }

                return leftItem.TotalNet.CompareTo(rightItem.TotalNet);
            });
        }

    }
}
