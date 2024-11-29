using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PaymentConditions.DeletePaymentCondition;
using LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition;
using LogicPOS.UI.Components.BackOffice.Windows;
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
        public PaymentConditionsPage(Window parent, Dictionary<string,string> options = null) : base(parent,options)
        {
        }


        protected override IRequest<ErrorOr<IEnumerable<PaymentCondition>>> GetAllQuery => new GetAllPaymentConditionsQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PaymentConditionModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
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

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeletePaymentConditionCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static PaymentConditionsPage _instance;
        public static PaymentConditionsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PaymentConditionsPage(BackOfficeWindow.Instance);
                }

                return _instance;
            }
        }
        #endregion
    }
}
