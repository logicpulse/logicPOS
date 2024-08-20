using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems;
using LogicPOS.Api.Features.Users.Permissions.Profiles.AddPermissionProfile;
using LogicPOS.Api.Features.Users.Permissions.Profiles.DeletePermissionProfile;
using LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.Utility;
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
        }

        protected override void LoadEntities()
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

        protected override void Design()
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

        private void InitializeUserProfilesGridView()
        {
            GridViewSettings.Model = CreateGridViewModel();

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

        private TreeViewColumn CreateCodeColumn()
        {
            void RenderCode(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var userProfile = (UserProfile)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = userProfile.Code;
            }

            return Columns.CreateCodeColumn(RenderCode);
        }

        private TreeViewColumn CreateDesignationColumn()
        {
            void RenderDesignation(TreeViewColumn column, CellRenderer cell, TreeModel model, TreeIter iter)
            {
                var userProfile = (UserProfile)model.GetValue(iter, 0);
                (cell as CellRendererText).Text = userProfile.Designation;
            }

            return Columns.CreateDesignationColumn(RenderDesignation);
        }

        private void CheckBox_Clicked(object o, ToggledArgs args)
        {
            TreeIter iterator;
            var path = new TreePath(args.Path);

            if (_gridPermissionItems.Model.GetIter(out iterator, path))
            {
                var currentValue = (bool)_gridPermissionItems.Model.GetValue(iterator, 1);
                _gridPermissionItems.Model.SetValue(iterator, 1, !currentValue);

                var permissionItem = _gridPermissionItems.Model.GetValue(iterator, 0) as PermissionItem; 
                
                if (currentValue)
                {
                    DeleteUserProfilePermission(permissionItem);
                }
                else
                {
                    AddUserProfilePermission(permissionItem);
                }
            }
        }

        private void DeleteUserProfilePermission(PermissionItem permissionItem)
        {
            var userProfile = (UserProfile)SelectedEntity;

            if (userProfile is null)
            {
                return;
            }

            var permissionProfile = _permissionProfiles.FirstOrDefault(x => x.UserProfileId == userProfile.Id && x.PermissionItemId == permissionItem.Id);

            if (permissionProfile is null)
            {
                return;
            }

            var deleteResult = _mediator.Send(new DeletePermissionProfileCommand
            {
                Id = permissionProfile.Id
            }).Result;

            if (deleteResult.IsError)
            {
                ShowApiErrorAlert();
            }
        }

        private void AddUserProfilePermission(PermissionItem permissionItem)
        {
            var userProfile  = (UserProfile)SelectedEntity;

            if(userProfile is null)
            {
                return;
            }

            var addResult = _mediator.Send(new AddPermissionProfileCommand
            {
                PermissionItemId = permissionItem.Id,
                UserProfileId = userProfile.Id
            }).Result;

            if (addResult.IsError)
            {
                ShowApiErrorAlert();
            }
        }

        protected override void GridViewRow_Changed(object sender, EventArgs e)
        {
            base.GridViewRow_Changed(sender, e);
            ShowUserProfilePermissions((UserProfile)SelectedEntity);
        }

        public override void Refresh()
        {
            LoadEntities();
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            AddUserProfilesToModel();
        }

        public override void DeleteEntity()
        {
            throw new NotImplementedException();
        }
   
        protected override void RunModal(EntityModalMode mode)
        {
            var modal = new UserProfileModal(mode, SelectedEntity as UserProfile);
            modal.Run();
            modal.Destroy();
        }

        protected override void InitializeGridView()
        {
            InitializeUserProfilesGridView();
            InitializePermissionItemsGrid();
        }

        protected override void AddEntitiesToModel()
        {
            AddUserProfilesToModel();
            AddPermissionItemsToModel();
        }

        protected override ListStore CreateGridViewModel()
        {
            return new ListStore(typeof(UserProfile));
        }

        protected override void InitializeSort()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Filter);

            AddCodeSorting();
            AddDesignationSorting();
        }

        private void AddDesignationSorting()
        {
            GridViewSettings.Sort.SetSortFunc(1, (model, a, b) =>
            {
                var userProfileA = (UserProfile)model.GetValue(a, 0);
                var userProfileB = (UserProfile)model.GetValue(b, 0);

                if (userProfileA == null || userProfileB == null)
                {
                    return 0;
                }

                return userProfileA.Designation.CompareTo(userProfileB.Designation);
            });
        }

        private void AddCodeSorting()
        {
            GridViewSettings.Sort.SetSortFunc(0, (model, a, b) =>
            {
                var userProfileA = (UserProfile)model.GetValue(a, 0);
                var userProfileB = (UserProfile)model.GetValue(b, 0);

                if (userProfileA == null || userProfileB == null)
                {
                    return 0;
                }

                return userProfileA.Code.CompareTo(userProfileB.Code);
            });
        }

        protected override void InitializeFilter()
        {
            GridViewSettings.Filter = new TreeModelFilter(GridViewSettings.Model, null);
            GridViewSettings.Filter.VisibleFunc = (model, iterator) =>
            {
                var search = Navigator.SearchBox.SearchText.ToLower();
                if (string.IsNullOrWhiteSpace(search))
                {
                    return true;
                }

                search = search.Trim();
                var userProfile = (UserProfile)model.GetValue(iterator, 0);

                if (userProfile.Designation.ToLower().Contains(search))
                {
                    return true;
                }

                return false;
            };
        }

        protected override void AddColumns()
        {
            var codeColumn = CreateCodeColumn();
            GridView.AppendColumn(codeColumn);

            var designationColumn = CreateDesignationColumn();
            GridView.AppendColumn(designationColumn);
        }
    }
}
