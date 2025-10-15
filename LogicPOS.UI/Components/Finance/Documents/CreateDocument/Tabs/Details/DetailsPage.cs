using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Documents.CreateDocument
{
    public partial class DetailsPage : Page<DocumentDetail>
    {
        public List<DocumentDetail> Items => _entities;
        public event Action<decimal> OnTotalChanged;
        public decimal TotalFinal => _entities.Sum(x => x.TotalFinal);
        public decimal ServicesTotalFinal => _entities.Where(detail => detail.Article.ClassAcronym == "S").Sum(detail => detail.TotalFinal);

        public DetailsPage(Window parent) : base(parent)
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
            OnTotalChanged?.Invoke(TotalFinal);
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
                var newItem = modal.GetDetail();
                 _entities.Add(newItem);
            }

            modal.Destroy();
            OnTotalChanged?.Invoke(TotalFinal);
            return (int)response;
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

        public override void UpdateButtonPrevileges() { }

    }
}
