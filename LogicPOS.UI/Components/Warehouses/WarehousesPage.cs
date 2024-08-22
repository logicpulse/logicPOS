using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Warehouses.GetAllWarehouses;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using MediatR;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    public class WarehousesPage : Page<Warehouse>
    {
        protected override IRequest<ErrorOr<IEnumerable<Warehouse>>> GetAllQuery => new GetAllWarehousesQuery();

        public WarehousesPage(Window parent) : base(parent)
        {
        }

        public override void DeleteEntity()
        {
            throw new System.NotImplementedException();
        }

        public override void RunModal(EntityModalMode mode)
        {
            throw new System.NotImplementedException();
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
