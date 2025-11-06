using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;

namespace LogicPOS.UI.Components.Pages
{
    public partial class PermissionsPage
    {
        private TreeViewColumn CreatePermissiontemColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var permissionItem = (PermissionItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = permissionItem.Designation;
            }

            var title = GeneralUtils.GetResourceByName("global_privilege_property");
            var itemDesignationCol = Columns.CreateColumn(title, 0, RenderDesignation);

            return itemDesignationCol;
        }

        private TreeViewColumn CreateGrantedColumn()
        {
            TreeViewColumn grantedColumn = new TreeViewColumn();
            grantedColumn.Title = GeneralUtils.GetResourceByName("global_privilege_active");

            var label = new Label(grantedColumn.Title);
            label.ModifyFont(CellRenderers.TitleFont);
            label.Show();
            grantedColumn.Widget = label;

            var grantedCellRenderer = new CellRendererToggle();
            grantedColumn.PackStart(grantedCellRenderer, true);

            grantedCellRenderer.Toggled += CheckBox_Clicked;

            void RenderGranted(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                (cell as CellRendererToggle).Active = (bool)model.GetValue(iter, 1);
            }

            grantedColumn.SetCellDataFunc(grantedCellRenderer, RenderGranted);



            return grantedColumn;
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting(0);
            AddDesignationSorting(1);
        }

        protected override void AddColumns()
        {
            GridView.AppendColumn(Columns.CreateCodeColumn(0));
            GridView.AppendColumn(Columns.CreateDesignationColumn(1));
        }

        private void InitializePermissionItemsGrid()
        {
            InitializePermissionItemsGridFilter();

            _gridPermissionItems = new TreeView();
            _gridPermissionItems.Model = PermissionItemsGridViewSettings.Filter;
            _gridPermissionItems.EnableSearch = true;
            _gridPermissionItems.SearchColumn = 0;

            TreeViewColumn itemDesignationColumn = CreatePermissiontemColumn();
            _gridPermissionItems.AppendColumn(itemDesignationColumn);

            TreeViewColumn grantedColumn = CreateGrantedColumn();
            _gridPermissionItems.AppendColumn(grantedColumn);
        }

        private void InitializePermissionItemsGridFilter()
        {
            PermissionItemsGridViewSettings.Filter = new TreeModelFilter(PermissionItemsGridViewSettings.Model, null);
            PermissionItemsGridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.Trim().ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                var entity = model.GetValue(iterator, 0) as PermissionItem;

                if (entity != null)
                {
                    if (entity.Designation.ToLower().Contains(search))
                    {
                        return true;
                    }
                }

                return false;
            };
        }

        private void InitializeUserProfilesGridView()
        {
            GridViewSettings.Model = new ListStore(typeof(UserProfile));

            InitializeGridViewModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();
            AddGridViewEventHandlers();
        }
    }
}
