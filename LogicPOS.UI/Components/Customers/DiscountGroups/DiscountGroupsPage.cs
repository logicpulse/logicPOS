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
    public class DiscountGroupsPage : Page<DiscountGroup>
    {
       
        protected override IRequest<ErrorOr<IEnumerable<DiscountGroup>>> GetAllQuery => new GetAllDiscountGroupsQuery();
        public DiscountGroupsPage(Window parent) : base(parent)
        {
        }

        public override bool DeleteEntity()
        {
            throw new NotImplementedException();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new DiscountGroupModal(mode, SelectedEntity as DiscountGroup);
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
