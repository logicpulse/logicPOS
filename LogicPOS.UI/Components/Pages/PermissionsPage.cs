using Gtk;
using logicpos.App;
using LogicPOS.Api.Errors;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems;
using LogicPOS.Api.Features.Users.Permissions.PermissionItems.GetAllPermissionItems;
using LogicPOS.Api.Features.Users.Permissions.Profiles;
using LogicPOS.Api.Features.Users.Permissions.Profiles.GetAllPermissionProfiles;
using LogicPOS.Api.Features.Users.Profiles;
using LogicPOS.Api.Features.Users.Profiles.GetAllUserProfiles;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components.GridViews;
using LogicPOS.Utility;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components.Pages
{
    internal class PermissionsPage : Page
    {
        public UserProfile Profile { get; set; }
        public int MarkedCheckBoxs { get; set; }
        private readonly ISender _mediator = DependencyInjection.Services.GetRequiredService<ISender>();

        private readonly TreeView _gridViewPermissionItems = new TreeView();
        private readonly ListStore _permissionItemModel = new ListStore(typeof(string), typeof(string), typeof(bool));
        private readonly CellRendererToggle _permissionItemCellRenderer = new CellRendererToggle();

        private List<PermissionProfile> _permissionProfiles = new List<PermissionProfile>();
        private List<PermissionItem> _permissionItems = new List<PermissionItem>();
        private List<UserProfile> _userProfiles = new List<UserProfile>();

        public event EventHandler CursorChanged;
        public event EventHandler CheckBoxToggled;

        public PermissionsPage(Window parentWindow) : base(parentWindow)
        {
            LoadEntities();

            Design();

            AddUserProfilesToModel(GridViewSettings.Model);

            ListPermissions();

            LoadRefreshView();

            ShowAll();
        }

        private void ShowApiErrorAlert()
        {
            SimpleAlerts.Error()
                .WithParent(_parentWindow)
                .WithTitle("API")
                .WithMessage(ApiErrors.CommunicationError.Description)
                .Show();
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

            _userProfiles.AddRange(getUserProfilesResult.Value);
        }

        public void AddUserProfilesToModel(TreeModel model)
        {
            var list = (ListStore)model;
            _userProfiles.ForEach(userProfile => list.AppendValues(userProfile));
        }

        public void ListPermissions()
        {
            foreach (var item in _permissionItems)
            {
                _permissionItemModel.AppendValues(item.Id, item.Designation, false);
            }
        }

        private void Design()
        {
            VBox verticalBox = new VBox(false, 1);

            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            DesignUserProfilesGrid();

            InitializeNavigator();

            IconButtonWithText buttonApplyPrivileges = Navigator.CreateButton("touchButtonApplyPrivileges_DialogActionArea", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_user_apply_privileges"), @"Icons/icon_pos_nav_refresh.png");

            buttonApplyPrivileges.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_USER_PRIVILEGES_APPLY");

            buttonApplyPrivileges.Clicked += delegate
            {
                GlobalApp.BackOfficeMainWindow.Accordion.UpdateMenuPrivileges();
                GlobalApp.PosMainWindow.TicketList.UpdateTicketListButtons();
            };

            Navigator.ExtraButtonSpace.PackStart(buttonApplyPrivileges, false, false, 0);

            scrolledWindow.Add(GridView);

            HBox horizontalBox = new HBox(false, 1);
            horizontalBox.PackStart(scrolledWindow);

            verticalBox.PackStart(horizontalBox, true, true, 0);
            verticalBox.PackStart(Navigator, false, false, 0);

            PackStart(verticalBox);

            InitializePermissionItemsGridView();

            ScrolledWindow scrolledWindowPermissionItem = new ScrolledWindow() { WidthRequest = 500 };
            scrolledWindowPermissionItem.Add(_gridViewPermissionItems);

            horizontalBox.Add(scrolledWindowPermissionItem);
        }

        private void InitializePermissionItemsGridView()
        {
            _gridViewPermissionItems.Model = _permissionItemModel;

            TreeViewColumn idColumn = _gridViewPermissionItems.AppendColumn("ID", new CellRendererText(), "text", 0);
            idColumn.Visible = false;

            TreeViewColumn permissionItemColumn = _gridViewPermissionItems.AppendColumn(GeneralUtils.GetResourceByName("global_privilege_property"),
                                                                                  new CellRendererText() { FontDesc = GridViewSettings.CellFont },
                                                                                  "text", 1);


            Label labelPropertyTitle = new Label(permissionItemColumn.Title);
            labelPropertyTitle.Show();
            labelPropertyTitle.ModifyFont(GridViewSettings.ColumnTitleFont);
            permissionItemColumn.Widget = labelPropertyTitle;

            TreeViewColumn grantedColumn = _gridViewPermissionItems.AppendColumn(GeneralUtils.GetResourceByName("global_privilege_active"), _permissionItemCellRenderer, "active", 2);
            grantedColumn.MaxWidth = 100;

            Label labelGranted = new Label(grantedColumn.Title);
            labelGranted.Show();
            labelGranted.ModifyFont(GridViewSettings.ColumnTitleFont);
            grantedColumn.Widget = labelGranted;

            _permissionItemCellRenderer.Toggled += CheckBox_Click;
        }

        private void DesignUserProfilesGrid()
        {
            GridViewSettings.Model = new ListStore(typeof(UserProfile));

            InitializeSorting();

            GridView = new TreeView();
            GridView.Model = GridViewSettings.Sort;
            GridView.EnableSearch = true;
            GridView.SearchColumn = 1;

            GridView.RulesHint = true;
            GridView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddUserProfilesGridColumns();
            AssignUserProfilesGridEvents();
        }

        private void AddUserProfilesGridColumns()
        {
            var codeColumn = CreateCodeColumn();
            GridView.AppendColumn(codeColumn);

            var designationColumn = CreateDesignationColumn();
            GridView.AppendColumn(designationColumn);
        }

        private void InitializeSorting()
        {
            GridViewSettings.Sort = new TreeModelSort(GridViewSettings.Model);
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

        private TreeViewColumn CreateCodeColumn()
        {
            TreeViewColumn codeColumn = new TreeViewColumn();
            codeColumn.Title  = GeneralUtils.GetResourceByName("global_record_code");
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

        private void AssignUserProfilesGridEvents()
        {
            GridView.CursorChanged += OnRowChanged;
            GridView.RowActivated += delegate
            {
                SimpleAlerts.Information()
                            .WithParent(_parentWindow)
                            .WithTitle("Teste")
                            .WithMessage(nameof(GridView.RowActivated))
                            .Show();
            };

            GridView.Vadjustment.ValueChanged += delegate { UpdatePages(); };
            GridView.Vadjustment.Changed += delegate { UpdatePages(); };
        }

        private void InitializeNavigator()
        {
            Navigator = new PageNavigator(_parentWindow, this, GridViewNavigatorMode.Default);
        }

        private void CheckBox_Click(object o, ToggledArgs args)
        {
            TreeIter iterator;
            var path = new TreePath(args.Path);

            if (_permissionItemModel.GetIter(out iterator, path))
            {
                _permissionItemModel.SetValue(iterator, 2, !_permissionItemCellRenderer.Active);
                UpdatePermissionProfile("" + _permissionItemModel.GetValue(iterator, 0), _permissionItemCellRenderer.Active);
            }
        }

        private void UpdatePermissionProfile(string permissionProfileId,
                                             bool granted)
        {
            SimpleAlerts.Information()
                        .WithParent(_parentWindow)
                        .WithTitle("Teste")
                        .WithMessage("Update Permission Profile")
                        .Show();
        }

        protected void OnRowChanged(object sender, EventArgs e)
        {
            TreeView grid = (TreeView)sender;
            TreeSelection selection = grid.Selection;
            GridViewSettings.Sort = new TreeModelSort(GridView.Model);


            if (selection.GetSelected(out TreeModel treeModel, out GridViewSettings.Iterator))
            {
                GridViewSettings.Path = treeModel.GetPath(GridViewSettings.Iterator);
                Navigator.CurrentRecord = Convert.ToInt16(GridViewSettings.Path.ToString());
            };

            UpdatePages();

            CursorChanged?.Invoke(this, e);

            LoadRefreshView();
        }

        private void LoadRefreshView()
        {
            _permissionItemModel.Clear();
            ListPermissions();
        }

        protected void UpdatePages()
        {
            Navigator.CurrentPage = (int)Math.Floor(GridView.Vadjustment.Value / GridView.Vadjustment.PageSize) + 1;
            Navigator.TotalPages = (int)Math.Floor(GridView.Vadjustment.Upper / GridView.Vadjustment.PageSize);
            if (GridView.Model != null)
            {
                Navigator.TotalRecords = GridView.Model.IterNChildren() - 1;
            }
            else
            {
                Navigator.TotalRecords = 0;
            };
            Navigator.UpdateButtons(GridView);
        }

        private void CellToggle(object o, ToggledArgs args)
        {
            //Required to force call CursorChanged
            GridViewSettings.Path = new TreePath(args.Path);
            GridView.SetCursor(GridViewSettings.Path, null, false);
            ToggleCheckBox(new TreePath(args.Path));
        }

        private void ToggleCheckBox(TreePath pTreePath)
        {
            //if (GridViewSettings.Model.GetIterFromString(out GridViewSettings.ModelIterator, GridViewSettings.CurrentRowIndex.ToString()))
            //{
            //    bool old = (bool)GridViewSettings.Model.GetValue(GridViewSettings.ModelIterator, GridViewSettings.ModelCheckBoxFieldIndex);
            //    if (!old) { MarkedCheckBoxs++; } else { MarkedCheckBoxs--; };
            //    GridViewSettings.Model.SetValue(GridViewSettings.ModelIterator, GridViewSettings.ModelCheckBoxFieldIndex, !old);

            //    ToggleCheckBox(old);
            //}

            //OnCheckBoxToggled();
        }

        private void OnCheckBoxToggled()
        {
            CheckBoxToggled?.Invoke(this, EventArgs.Empty);
        }

        public virtual void ToggleCheckBox(bool pOldValue) { }
    }
}
