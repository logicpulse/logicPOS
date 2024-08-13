using Gtk;
using logicpos.App;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems;
using LogicPOS.Api.Features.Users.Permissions.Profiles;
using LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles;
using LogicPOS.Api.Features.Users.Profiles;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    internal class PermissionsPage : Page
    {
        private TreeView _gridPermissionItems;
        private List<PermissionProfile> _permissionProfiles = new List<PermissionProfile>();
        private List<PermissionItem> _permissionItems = new List<PermissionItem>();
        private List<UserProfile> _userProfiles = new List<UserProfile>();

        public PermissionsPage(Window parentWindow) : base(parentWindow)
        {
            LoadEntities();

            InitializeUserProfilesGrid();

            InitializePermissionItemsGrid();

            Design();

            AddUserProfilesToModel();

            AddPermissionItemsToModel();

            ShowAll();
        }

        private void LoadEntities()
        {
            LoadPermissionItems();
            LoadPermissionProfiles();
            LoadUserProfiles();
        }

        private void LoadPermissionItems()
        {
            var getPermissionItemsResult = _mediator.Send(new GetAllPermissionItemsQuery()).Result;

            if (getPermissionItemsResult.IsError)
            {
                ShowApiErrorAlert();
                return;
            }

            _permissionItems.Clear();
            _permissionItems.AddRange(getPermissionItemsResult.Value);
        }

        private void LoadPermissionProfiles()
        {
            var getPermissionProfiles = _mediator.Send(new GetAllPermissionProfilesQuery()).Result;

            if (getPermissionProfiles.IsError)
            {
                ShowApiErrorAlert();
                return;
            }

            _permissionProfiles.Clear();
            _permissionProfiles.AddRange(getPermissionProfiles.Value);
        }

        private void LoadUserProfiles()
        {
            var getUserProfilesResult = _mediator.Send(new GetAllUserProfilesQuery()).Result;

            if (getUserProfilesResult.IsError)
            {
                ShowApiErrorAlert();
                return;
            }

            _userProfiles.Clear();
            _userProfiles.AddRange(getUserProfilesResult.Value);
        }

        private void AddUserProfilesToModel()
        {
            var model = (ListStore)GridViewSettings.Model;
            _userProfiles.ForEach(profile => model.AppendValues(profile));
        }

        private void AddPermissionItemsToModel()
        {
            var model = (ListStore)_gridPermissionItems.Model;
            _permissionItems.ForEach(item => model.AppendValues(item, false));
        }

        public void ShowUserProfilePermissions(UserProfile userProfile)
        {
            var permissionItems = (ListStore)_gridPermissionItems.Model;

            permissionItems.Foreach((model, path, iterator) =>
            {
                var permissionItem = (PermissionItem)model.GetValue(iterator, 0);
                var granted = _permissionProfiles.Any(x => x.UserProfileId == userProfile.Id && x.PermissionItemId == permissionItem.Id);
                model.SetValue(iterator, 1, granted);
                return false;
            });
        }

        private void Design()
        {
            VBox verticalBox = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            scrolledWindow.Add(GridView);

            HBox horizontalBox = new HBox(false, 1);
            horizontalBox.PackStart(scrolledWindow);

            verticalBox.PackStart(horizontalBox, true, true, 0);
            verticalBox.PackStart(Navigator, false, false, 0);

            PackStart(verticalBox);

            ScrolledWindow scrolledWindowPermissionItem = new ScrolledWindow() { WidthRequest = 500 };
            scrolledWindowPermissionItem.Add(_gridPermissionItems);

            horizontalBox.Add(scrolledWindowPermissionItem);
        }

        private void InitializePermissionItemsGrid()
        {
            _gridPermissionItems = new TreeView();
            _gridPermissionItems.Model = new ListStore(typeof(PermissionItem), typeof(bool));

            TreeViewColumn itemDesignationColumn = CreatePermissiontemColumn();
            _gridPermissionItems.AppendColumn(itemDesignationColumn);

            TreeViewColumn grantedColumn = CreateGrantedColumn();
            _gridPermissionItems.AppendColumn(grantedColumn);
        }

        private void InitializeUserProfilesGrid()
        {
            GridViewSettings.Model = new ListStore(typeof(UserProfile));

            InitializeUserProfilesGridModel();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddUserProfilesGridColumns();
            AddUserProfilesGridEventHandlers();
        }

        private void AddUserProfilesGridColumns()
        {
            var codeColumn = CreateCodeColumn();
            GridView.AppendColumn(codeColumn);

            var designationColumn = CreateDesignationColumn();
            GridView.AppendColumn(designationColumn);
        }

        private void InitializeUserProfilesGridModel()
        {
            InitializeProfilesGridFilter();
            InitializeUserProfilesGridSort();
        }

        private void InitializeUserProfilesGridSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);
            GridViewSettings.Sort.SetSortFunc(0, (model, a, b) =>
            {
                var userProfileA = (UserProfile)model.GetValue(a, 0);
                var userProfileB = (UserProfile)model.GetValue(b, 0);
                return userProfileA.Code.CompareTo(userProfileB.Code);
            });

            GridViewSettings.Sort.SetSortFunc(1, (model, a, b) =>
            {
                var userProfileA = (UserProfile)model.GetValue(a, 0);
                var userProfileB = (UserProfile)model.GetValue(b, 0);
                return userProfileA.Designation.CompareTo(userProfileB.Designation);
            });
        }

        private void InitializeProfilesGridFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                search = search.Trim().ToLower();
                var userProfile = (UserProfile)model.GetValue(iterator, 0);

                if (userProfile.Designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        private TreeViewColumn CreatePermissiontemColumn()
        {

            TreeViewColumn itemDesignationCol = new TreeViewColumn();
            itemDesignationCol.Title = GeneralUtils.GetResourceByName("global_privilege_property");

            var label = new Label(itemDesignationCol.Title);
            label.ModifyFont(CellRenderers.TitleFont);
            label.Show();
            itemDesignationCol.Widget = label;

            var designationCellRenderer = CellRenderers.Text();
            itemDesignationCol.PackStart(designationCellRenderer, true);

            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var permissionItem = (PermissionItem)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = permissionItem.Designation;
            }

            itemDesignationCol.SetCellDataFunc(designationCellRenderer, RenderDesignation);

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

        private TreeViewColumn CreateCodeColumn()
        {
            TreeViewColumn codeColumn = new TreeViewColumn();
            codeColumn.Title = GeneralUtils.GetResourceByName("global_record_code");
            codeColumn.Resizable = true;
            codeColumn.MinWidth = 60;
            codeColumn.MaxWidth = 100;
            codeColumn.Clickable = true;
            codeColumn.SortColumnId = 0;
            codeColumn.SortIndicator = true;
            codeColumn.SortOrder = SortType.Ascending;


            var label = new Label(codeColumn.Title);
            label.ModifyFont(CellRenderers.TitleFont);
            label.Show();
            codeColumn.Widget = label;

            var codeCellRenderer = CellRenderers.Code();
            codeColumn.PackStart(codeCellRenderer, true);

            void RenderCode(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var userProfile = (UserProfile)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = userProfile.Code;
            }

            codeColumn.SetCellDataFunc(codeCellRenderer, RenderCode);

            return codeColumn;
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            TreeViewColumn designationColumn = new TreeViewColumn();
            designationColumn.Title = GeneralUtils.GetResourceByName("global_designation");
            designationColumn.Resizable = true;
            designationColumn.MinWidth = 250;
            designationColumn.MaxWidth = 800;
            designationColumn.Expand = true;
            designationColumn.Clickable = true;
            designationColumn.SortIndicator = true;
            designationColumn.SortOrder = SortType.Ascending;
            designationColumn.SortColumnId = 1;

            var label = new Label(designationColumn.Title);
            label.ModifyFont(CellRenderers.TitleFont);
            label.Show();
            designationColumn.Widget = label;

            var designationCellRenderer = CellRenderers.Text();
            designationColumn.PackStart(designationCellRenderer, true);

            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var userProfile = (UserProfile)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = userProfile.Designation;
            }

            designationColumn.SetCellDataFunc(designationCellRenderer, RenderDesignation);

            return designationColumn;
        }

        private void AddUserProfilesGridEventHandlers()
        {
            GridView.CursorChanged += GridUserProfilesRow_Changed;
            GridView.RowActivated += delegate { UpdateEntity(); };
            GridView.Vadjustment.ValueChanged += delegate { Navigator.Update(); };
            GridView.Vadjustment.Changed += delegate { Navigator.Update(); };
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            TreeIter iterator;
            var path = new TreePath(args.Path);

            if (_gridPermissionItems.Model.GetIter(out iterator, path))
            {
                var currentValue = (bool)_gridPermissionItems.Model.GetValue(iterator, 1);
                _gridPermissionItems.Model.SetValue(iterator, 1, !currentValue);
            }
        }

        protected void GridUserProfilesRow_Changed(object sender, EventArgs e)
        {
            TreeSelection selection = GridView.Selection;

            if (selection.GetSelected(out TreeModel model, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = model.GetPath(GridViewSettings.Iterator);
                Navigator.CurrentRecord = Convert.ToInt16(GridViewSettings.Path.ToString());
                SelectedEntity = model.GetValue(GridViewSettings.Iterator, 0);
                ShowUserProfilePermissions((UserProfile)SelectedEntity);
            };

            Navigator.Update();
        }

        internal override void Refresh()
        {
            LoadEntities();
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            AddUserProfilesToModel();
        }

        internal override void ViewEntity() => RunModal(EntityModalMode.View);

        internal override void UpdateEntity() => RunModal(EntityModalMode.Update);

        internal override void DeleteEntity()
        {
            throw new NotImplementedException();
        }

        internal override void InsertEntity() => RunModal(EntityModalMode.Insert);

        private void RunModal(EntityModalMode mode)
        {
            var modal = new UserProfileModal(mode, SelectedEntity as UserProfile);
            var response =  modal.Run();
            modal.Destroy();
        }
    }
}
