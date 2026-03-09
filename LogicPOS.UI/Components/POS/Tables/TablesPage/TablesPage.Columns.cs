
using Gtk;
using LogicPOS.Api.Features.POS.Tables.Common;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class TablesPage
    {
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

        private void AddPlaceSorting()
        {
            GridViewSettings.Sort.SetSortFunc(3, (model, left, right) =>
            {
                var leftTable = (TableViewModel)model.GetValue(left, 0);
                var rightTable = (TableViewModel)model.GetValue(right, 0);

                if (leftTable == null || rightTable == null)
                {
                    return 0;
                }

                return leftTable.Place.CompareTo(rightTable.Place);
            });
        }

        private TreeViewColumn CreatePlaceColumn()
        {
            void RenderPlace(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var table = (TableViewModel)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = table.Place;
            }

            var title = GeneralUtils.GetResourceByName("global_places");
            return Columns.CreateColumn(title, 3, RenderPlace);
        }
    }
}
