using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Warehouses.DeleteWarehouse;
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

        public WarehousesPage(Window parent, Dictionary<string,string> options = null) : base(parent, options)
        {
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new WarehouseModal(mode, SelectedEntity);
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
            return new DeleteWarehouseCommand(SelectedEntity.Id);
        }

        #region Singleton
        private static WarehousesPage _instance;
        public static WarehousesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WarehousesPage(null);
                }
                return _instance;
            }
        }
        #endregion
    }
}
