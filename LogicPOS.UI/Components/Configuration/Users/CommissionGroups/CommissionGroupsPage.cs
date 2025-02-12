using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Users.CommissionGroups.DeleteCommissionGroup;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CommissionGroupsPage : Page<CommissionGroup>
    {
        protected override IRequest<ErrorOr<IEnumerable<CommissionGroup>>> GetAllQuery => new GetAllCommissionGroupsQuery();
        public CommissionGroupsPage(Window parent) : base(parent)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new CommissionGroupModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
            GridView.AppendColumn(CreateCommissionColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(3));
        }

        private TreeViewColumn CreateCommissionColumn()
        {
            void RenderCommission(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var commissionGroup = (CommissionGroup)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = commissionGroup.Commission.ToString();
            }

            var title = GeneralUtils.GetResourceByName("global_commission");
            return Columns.CreateColumn(title, 2, RenderCommission);

        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
            AddCommissionSorting();
            AddUpdatedAtSorting(3);
        }

        private void AddCommissionSorting()
        {
            GridViewSettings.Sort.SetSortFunc(2, (model, left, right) =>
            {
                var leftCommissionGroup = (CommissionGroup)model.GetValue(left, 0);
                var rightCommissionGroup = (CommissionGroup)model.GetValue(right, 0);

                if (leftCommissionGroup == null || rightCommissionGroup == null)
                {
                    return 0;
                }

                return leftCommissionGroup.Commission.CompareTo(rightCommissionGroup.Commission);
            });
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteCommissionGroupCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static CommissionGroupsPage _instance;
        public static CommissionGroupsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CommissionGroupsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
