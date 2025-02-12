using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Api.Features.DiscountGroups.DeleteDiscountGroup;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class DiscountGroupsPage : Page<DiscountGroup>
    {

        protected override IRequest<ErrorOr<IEnumerable<DiscountGroup>>> GetAllQuery => new GetAllDiscountGroupsQuery();
        public DiscountGroupsPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new DiscountGroupModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(2));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddUpdatedAtSorting(2);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteDiscountGroupCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static DiscountGroupsPage _instance;
        public static DiscountGroupsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DiscountGroupsPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
