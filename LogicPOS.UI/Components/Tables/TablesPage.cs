using ErrorOr;
using Gtk;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Tables.DeleteTable;
using LogicPOS.Api.Features.Tables.GetAllTables;
using LogicPOS.UI.Components.BackOffice.Windows;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using Table = LogicPOS.Api.Entities.Table;

namespace LogicPOS.UI.Components.Pages
{
    public class TablesPage : Page<Table>
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

        private TreeViewColumn CreatePlaceColumn()
        {
            void RenderPlace(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var table = (Table)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = table.Place.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_places");
            return Columns.CreateColumn(title, 3, RenderPlace);
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(2);
            AddPlaceSorting();
            AddUpdatedAtSorting(4);
        }

        private void AddPlaceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftTable = (Table)model.GetValue(left, 0);
                var rightTable = (Table)model.GetValue(right, 0);

                if (leftTable == null || rightTable == null)
                {
                    return 0;
                }

                return leftTable.Place.Designation.CompareTo(rightTable.Place.Designation);
            });
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
