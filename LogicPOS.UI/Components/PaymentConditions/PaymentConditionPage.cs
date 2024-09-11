using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class PaymentConditionsPage : Page<PaymentCondition>
    {
        public PaymentConditionsPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        protected override IRequest<ErrorOr<IEnumerable<PaymentCondition>>> GetAllQuery => new GetAllPaymentConditionsQuery();

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new PaymentConditionModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateAcronymColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateAcronymColumn()
        {
            void RenderMonth(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var paymentCondition = (PaymentCondition)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = paymentCondition.Acronym.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPaymentCondition_Acronym");
            return Columns.CreateColumn(title, 2, RenderMonth);
        }

        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddAcronymSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftPaymentCondition = (PaymentCondition)model.GetValue(left, 0);
                var rightPaymentCondition = (PaymentCondition)model.GetValue(right, 0);

                if (leftPaymentCondition == null || rightPaymentCondition == null)
                {
                    return 0;
                }

                return leftPaymentCondition.Acronym.CompareTo(rightPaymentCondition.Acronym);
            });
        }
    }
}
