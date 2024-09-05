using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class VatRatesPage : Page<VatRate>
    {
        protected override IRequest<ErrorOr<IEnumerable<VatRate>>> GetAllQuery => new GetAllVatRatesQuery();
        public VatRatesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new VatRateModal(mode, SelectedEntity as VatRate);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateValueColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

      

        private TreeViewColumn CreateValueColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var vatRate = (VatRate)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = vatRate.Value.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_vat_rate");
            return Columns.CreateColumn(title, 2, RenderValue);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }
    }
}
