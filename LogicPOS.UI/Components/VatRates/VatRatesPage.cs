using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.VatRates.GetAllVatRate;
using LogicPOS.UI.Components.BackOffice.Windows;
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
        public VatRatesPage(Window parent, Dictionary<string,string> options = null) : base(parent,options)
        {
        }

        public override bool DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new VatRateModal(mode, SelectedEntity as VatRate);
            var response = modal.Run();
            modal.Destroy();
            return response;
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
            AddValueSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddValueSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftVatRate = (VatRate)model.GetValue(left, 0);
                var rightVatRate = (VatRate)model.GetValue(right, 0);

                if (leftVatRate == null || rightVatRate == null)
                {
                    return 0;
                }

                return leftVatRate.Value.CompareTo(rightVatRate.Value);
            });
        }

        #region Singleton
        private static VatRatesPage _instance;
        public static VatRatesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new VatRatesPage(BackOfficeWindow.Instance);
                }

                return _instance;
            }
        }
        #endregion
    }
}
