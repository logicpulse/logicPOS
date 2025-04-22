using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PaymentMethods.DeletePaymentMethod;
using LogicPOS.Api.Features.PaymentMethods.GetAllPaymentMethods;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PaymentMethodsPage : Page<PaymentMethod>
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
        protected override void InitializeSort()
        {

            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddAcronymSorting();
            AddUpdatedAtSorting(3);
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
