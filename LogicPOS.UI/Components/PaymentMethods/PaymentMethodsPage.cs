using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PaymentMethods.DeletePaymentMethod;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class PaymentMethodsPage : Page<PaymentMethod>
    {
        public PaymentMethodsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
        {
            Navigator.BtnInsert.Visible = false;
            Navigator.BtnDelete.Visible = false;
            Navigator.BtnUpdate.Visible = false;
        }

        protected override IRequest<ErrorOr<IEnumerable<PaymentMethod>>> GetAllQuery => new GetAllPaymentMethodsQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new PaymentMethodModal(mode, SelectedEntity);
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
                var paymentMethod = (PaymentMethod)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = paymentMethod.Acronym;
            }

            var title = GeneralUtils.GetResourceByName("global_ConfigurationPaymentMethod_Acronym");
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
                var leftPaymentMethod = (PaymentMethod)model.GetValue(left, 0);
                var rightPaymentMethod = (PaymentMethod)model.GetValue(right, 0);

                if (leftPaymentMethod == null || rightPaymentMethod == null)
                {
                    return 0;
                }

                return leftPaymentMethod.Acronym.CompareTo(rightPaymentMethod.Acronym);
            });
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeletePaymentMethodCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static PaymentMethodsPage _instance;
        public static PaymentMethodsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PaymentMethodsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }

        }
        #endregion
    }
}
