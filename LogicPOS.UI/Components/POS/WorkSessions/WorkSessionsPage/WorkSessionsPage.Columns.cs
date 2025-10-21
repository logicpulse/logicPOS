using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class WorkSessionsPage
    {
        private TreeViewColumn CreateSelectColumn()
        {
            TreeViewColumn selectColumn = new TreeViewColumn();

            var selectCellRenderer = new CellRendererToggle();
            selectColumn.PackStart(selectCellRenderer, true);

            selectCellRenderer.Toggled += CheckBox_Clicked;

            void RenderSelect(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var day = (WorkSessionPeriod)model.GetValue(iter, 0);
                (cell as CellRendererToggle).Active = SelectedEntity == day;
            }

            selectColumn.SetCellDataFunc(selectCellRenderer, RenderSelect);

            return selectColumn;
        }
        private TreeViewColumn CreateStartDateColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var days = (WorkSessionPeriod)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = days.StartDate.ToString();
                cell.Xalign = 1;
                cell.Width = 50;
            }

            var title = GeneralUtils.GetResourceByName("global_date_start");
            var col= Columns.CreateColumn(title, 2, RenderValue);
            col.Expand = false;
            col.IsFloating = false;
            col.MaxWidth = 150;
            col.MinWidth = 150;
            col.FixedWidth = 150;
            col.Alignment = 1;
            return col;
        }

        private TreeViewColumn CreateEndDateColumn()
        {
            void RenderValue(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var days = (WorkSessionPeriod)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = days.EndDate.ToString();
                cell.Xalign = 1;
                cell.Width = 50;
                
            }

            var title = GeneralUtils.GetResourceByName("global_date_end");
            var col= Columns.CreateColumn(title, 3, RenderValue);
            col.Expand = false;
            col.IsFloating = false;
            col.MaxWidth = 150;
            col.MinWidth = 150;
            col.FixedWidth= 150;
            col.Alignment = 1;
            return col;
        }
    }
}
