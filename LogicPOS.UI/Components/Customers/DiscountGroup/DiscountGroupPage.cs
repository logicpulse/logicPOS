using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.DiscountGroups.GetAllDiscountGroups;
using LogicPOS.Domain.Entities;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages;
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
    public class DiscountGroupPage : Page<DiscountGroup>
    {
       
        protected override IRequest<ErrorOr<IEnumerable<DiscountGroup>>> GetAllQuery => new GetAllDiscountGroupsQuery();
        public DiscountGroupPage(Window parent) : base(parent)
        {
        }


        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new DiscountGroupModal(mode, SelectedEntity as DiscountGroup);
            modal.Run();
            modal.Destroy();
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

    }
}
