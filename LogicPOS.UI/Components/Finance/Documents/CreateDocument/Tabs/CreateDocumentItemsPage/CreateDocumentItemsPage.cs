using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using LogicPOS.UI.Components.Pages.GridViews;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class CreateDocumentItemsPage : Page<Item>
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
        protected override DeleteCommand GetDeleteCommand() => null;

        public override void UpdateButtonPrevileges()
        {
            //these buttons are not used in this page

        }
    }
}
