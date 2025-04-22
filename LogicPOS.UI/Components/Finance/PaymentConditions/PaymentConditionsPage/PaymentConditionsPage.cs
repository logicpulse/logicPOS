using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.PaymentConditions.DeletePaymentCondition;
using LogicPOS.Api.Features.PaymentConditions.GetAllPaymentCondition;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PaymentConditionsPage : Page<PaymentCondition>
    {
        public PaymentConditionsPage(Window parent, Dictionary<string, string> options = null) : base(parent, options)
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
