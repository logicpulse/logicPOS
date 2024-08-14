using Gtk;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages.GridViews
{
    public static class Columns
    {
        public static TreeViewColumn CreateColumn(string title,
                                                  int sortColumnId,
                                                  TreeCellDataFunc renderFunction,
                                                  CellRenderer cellRenderer = null,
                                                  bool resizable = true,
                                                  bool clickable = true)
        {
            TreeViewColumn column = new TreeViewColumn();
            column.Title = title;
            column.Resizable = resizable;
            column.Clickable = clickable;
            column.SortColumnId = sortColumnId;
            column.SortIndicator = true;
            column.SortOrder = SortType.Descending;

            var label = new Label(column.Title);
            label.ModifyFont(CellRenderers.TitleFont);
            label.Show();
            column.Widget = label;

            cellRenderer = cellRenderer == null ?  CellRenderers.Text() : cellRenderer;
            column.PackStart(cellRenderer, true);

            column.SetCellDataFunc(cellRenderer, renderFunction);

            return column;
        }

        public static TreeViewColumn CreateCodeColumn(TreeCellDataFunc renderFunction)
        {
            var title = GeneralUtils.GetResourceByName("global_record_code");

            var column = CreateColumn(title,
                                      0,
                                      renderFunction,
                                      CellRenderers.Code());
            column.MinWidth = 60;
            column.MaxWidth = 100;

            return column;
        }

        public static TreeViewColumn CreateDesignationColumn(TreeCellDataFunc renderFunction,
                                                             int sortColumnId = 1)
        {
            var title = GeneralUtils.GetResourceByName("global_designation");

            var column = CreateColumn(title,sortColumnId,renderFunction);
            column.MinWidth = 250;
            column.MaxWidth = 800;

            return column;
        }

    }
}
