using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public class CreateDocumentItemsPage : Page<Item>
    {
        public List<Item> Items => _entities;
        public event Action<decimal> OnTotalChanged;
        public decimal TotalFinal => Math.Round(_entities.Sum(x => x.TotalFinal),2);

        public CreateDocumentItemsPage(Window parent) : base(parent)
        {
        }

        protected override void LoadEntities() { }

        public override bool DeleteEntity()
        {
            if (SelectedEntity == null)
            {
                return false;
            }

            var result = _entities.Remove(SelectedEntity);
            SelectedEntity = null;
            return result;
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            switch (mode)
            {
                case EntityEditionModalMode.Insert:
                    return RunInsertModal();
                case EntityEditionModalMode.Update:
                    return RunUpdateModal();
                default:
                    return RunViewModal();
            }
        }

        private int RunViewModal()
        {
            if (SelectedEntity == null)
            {
                return (int)ResponseType.Cancel;
            }

            var modal = new AddArticleModal(SourceWindow, EntityEditionModalMode.View, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        private int RunUpdateModal()
        {
            if (SelectedEntity == null)
            {
                return (int)ResponseType.Cancel;
            }

            var modal = new AddArticleModal(SourceWindow, EntityEditionModalMode.Update, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            OnTotalChanged?.Invoke(TotalFinal);
            return response;
        }

        private int RunInsertModal()
        {
            var modal = new AddArticleModal(SourceWindow, EntityEditionModalMode.Insert);
            var response = (ResponseType)modal.Run();

            if (response == ResponseType.Ok)
            {
                var newItem = modal.GetItem();
                if (ItemExists(newItem.ArticleId))
                {
                    UpdateItemData(newItem.ArticleId, newItem);
                }
                else
                {
                    _entities.Add(newItem);
                }
            }

            modal.Destroy();
            OnTotalChanged?.Invoke(TotalFinal);
            return (int)response;
        }

        private void UpdateItemData(Guid articleId, Item newData)
        {
            var existingItem = _entities.Find(x => x.ArticleId == articleId);

            existingItem.Order = newData.Order;
            existingItem.Code = newData.Code;
            existingItem.Designation = newData.Designation;
            existingItem.Article = newData.Article;
            existingItem.ArticleId = newData.ArticleId;
            existingItem.Quantity = newData.Quantity;
            existingItem.UnitPrice = newData.UnitPrice;
            existingItem.Discount = newData.Discount;
            existingItem.Vat  = newData.Vat;
            existingItem.VatRate = newData.VatRate;
            existingItem.VatRateId = newData.VatRateId;
            existingItem.VatDesignation = newData.VatDesignation;
            existingItem.ExemptionReason = newData.ExemptionReason;
            existingItem.VatExemptionReason = newData.VatExemptionReason;
        }

        private bool ItemExists(Guid articleId)
        {
            return _entities.Exists(x => x.ArticleId == articleId);
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

        protected override DeleteCommand GetDeleteCommand() => null;
    }
}
