using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Tables.DeleteTable;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using MediatR;
using System.Collections.Generic;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage : Page<Table>
    {
        public TablesPage(Window parent) : base(parent)
        {
        }

        protected override IRequest<ErrorOr<IEnumerable<Table>>> GetAllQuery => new GetAllTablesQuery();

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new TableModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }
        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(2));
            GridView.AppendColumn(CreatePlaceColumn());
            GridView.AppendColumn(Columns.CreateUpdatedAtColumn(4));
        }
        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(2);
            AddPlaceSorting();
            AddUpdatedAtSorting(4);
        }

        protected override DeleteCommand GetDeleteCommand()
        {
            return new DeleteTableCommand(SelectedEntity.Id);
        }

        #region Signleton
        private static TablesPage _instance;
        public static TablesPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TablesPage(BackOfficeWindow.Instance);
                }

                return _instance;
            }
        }
        #endregion
    }
}
