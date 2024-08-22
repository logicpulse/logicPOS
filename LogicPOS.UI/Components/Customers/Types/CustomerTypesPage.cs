using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Customers.Types.GetAllCustomerTypes;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class CustomerTypesPage : Page<CustomerType>
    {
        protected override IRequest<ErrorOr<IEnumerable<CustomerType>>> GetAllQuery => new GetAllCustomerTypesQuery();

        public CustomerTypesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
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

        public override void RunModal(EntityModalMode mode)
        {
            var modal = new CustomerTypeModal(mode, SelectedEntity);
            modal.Run();
            modal.Destroy();
        }
    }
}
