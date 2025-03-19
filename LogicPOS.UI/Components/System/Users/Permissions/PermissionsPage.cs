using ErrorOr;
using Gtk;
using LogicPOS.Api.Entities;
using LogicPOS.Api.Features.Common;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems;
using LogicPOS.Api.Features.Users.Permissions.Profiles.AddPermissionProfile;
using LogicPOS.Api.Features.Users.Permissions.Profiles.DeletePermissionProfile;
using LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.UI.Components.Modals;
using LogicPOS.UI.Components.Pages.GridViews;
using LogicPOS.UI.Components.Windows;
using LogicPOS.Utility;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPOS.UI.Components.Pages
{
    internal class PermissionsPage : Page<UserProfile>
    {
        private TreeView _gridPermissionItems;
        private List<PermissionProfile> _permissionProfiles = new List<PermissionProfile>();
        private List<PermissionItem> _permissionItems = new List<PermissionItem>();
        protected override IRequest<ErrorOr<IEnumerable<UserProfile>>> GetAllQuery => new GetAllUserProfilesQuery();

        public PermissionsPage(Window parentWindow) : base(parentWindow)
        {
        }

        protected override void LoadEntities()
        {
            base.LoadEntities();
            LoadPermissionItems();
            LoadPermissionProfiles();
        }

        private void LoadPermissionItems()
        {
            var getPermissionItemsResult = _mediator.Send(new GetAllPermissionItemsQuery()).Result;

            if (getPermissionItemsResult.IsError)
            {
                HandleErrorResult(getPermissionItemsResult);
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
                HandleErrorResult(getPermissionProfiles);
                return;
            }

            _permissionProfiles.Clear();
            _permissionProfiles.AddRange(getPermissionProfiles.Value);
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
            var userProfile = SelectedEntity;

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
                HandleErrorResult(deleteResult);
            }
        }

        private void AddUserProfilePermission(PermissionItem permissionItem)
        {
            var userProfile = SelectedEntity;

            if (userProfile is null)
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
                HandleErrorResult(addResult);
            }
        }

        protected override void GridViewRow_Changed(object sender, EventArgs e)
        {
            base.GridViewRow_Changed(sender, e);
            ShowUserProfilePermissions(SelectedEntity);
        }

        public override void Refresh()
        {
            LoadEntities();
            var model = (ListStore)GridViewSettings.Model;
            model.Clear();
            base.AddEntitiesToModel();
        }

        public override int RunModal(EntityEditionModalMode mode)
        {
            var modal = new UserProfileModal(mode, SelectedEntity);
            var response = modal.Run();
            modal.Destroy();
            return response;
        }

        protected override void InitializeGridView()
        {
            InitializeUserProfilesGridView();
            InitializePermissionItemsGrid();
        }

        protected override void AddEntitiesToModel()
        {
            base.AddEntitiesToModel();
            AddPermissionItemsToModel();
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

        protected override DeleteCommand GetDeleteCommand()
        {
            return null;
        }

        #region Singleton
        private static PermissionsPage _instance;
        public static PermissionsPage Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PermissionsPage(BackOfficeWindow.Instance);
                }
                return _instance;
            }
        }
        #endregion
    }
}
