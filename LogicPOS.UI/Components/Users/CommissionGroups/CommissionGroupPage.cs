using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.CommissionGroups.GetAllCommissionGroups;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicPOS.UI.Components.Pages
{
    public class CommissionGroupPage : Page<CommissionGroup>
    {
       
        protected override IRequest<ErrorOr<IEnumerable<CommissionGroup>>> GetAllQuery => new GetAllCommissionGroupsQuery();
        public CommissionGroupPage(Window parent) : base(parent)
        {
        }


        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new CommissionGroupModal(mode, SelectedEntity as CommissionGroup);
            modal.Run();
            modal.Destroy();
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
    }
}
