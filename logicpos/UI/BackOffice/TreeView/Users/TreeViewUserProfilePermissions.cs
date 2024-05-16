using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.datalayer.Xpo;
using System;
using System.Collections.Generic;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;
using LogicPOS.Settings;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewUserProfilePermissions : GenericTreeViewXPO
    {
        //Settings
        private readonly string _fontGenericTreeViewColumnTitle = LogicPOS.Settings.GeneralSettings.Settings["fontGenericTreeViewColumnTitle"];
        private readonly string _fontGenericTreeViewColumn = LogicPOS.Settings.GeneralSettings.Settings["fontGenericTreeViewColumn"];

        private readonly TreeView _treeViewPermissionItem = new TreeView();
        private readonly ListStore _listStoreModelPermissionItem = new ListStore(typeof(string), typeof(string), typeof(bool));
        private readonly CellRendererToggle _cellRendererTogglePermissionItem = new CellRendererToggle();

        private readonly Type _xpObjectTypeUserPermissionItem = typeof(sys_userpermissionitem);
        private readonly Type _xpObjectTypeUserPermissionProfile = typeof(sys_userpermissionprofile);
        private XPCollection _xpCollectionUserPermissionItem;
        private XPCollection _xpCollectionUserPermissionProfile;

        private readonly Pango.FontDescription _fontDescTitle;
        private readonly Pango.FontDescription _fontDesc;

        //Public Parametless Constructor Required by Generics
        public TreeViewUserProfilePermissions() { }

        public TreeViewUserProfilePermissions(Window pSourceWindow)
        {
            _fontDescTitle = Pango.FontDescription.FromString(_fontGenericTreeViewColumnTitle);
            _fontDesc = Pango.FontDescription.FromString(_fontGenericTreeViewColumn);

            CriteriaOperator criteria = null;

            bool showStatusBar = false;
            Type xPGuidObjectType = typeof(sys_userprofile);
            Type typeDialogClass = typeof(DialogUserProfile);

            Type xpObjectTypeUserProfile = typeof(sys_userprofile);

            XPCollection xpCollectionUserProfile = new XPCollection(XPOSettings.Session, xpObjectTypeUserProfile, criteria);

            List<GenericTreeViewColumnProperty> columnPropertiesUserProfile = new List<GenericTreeViewColumnProperty>
            {
                new GenericTreeViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code") },
                new GenericTreeViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_user_profiles"), MaxWidth = 150 }
            };

            InitObject(pSourceWindow, xpCollectionUserProfile, xPGuidObjectType, typeDialogClass, columnPropertiesUserProfile, showStatusBar);
            this.ShowAll();
        }

        protected void InitObject(Window pSourceWindow, XPCollection pXPCollection, Type pXPGuidObjectType, Type pDialogType, List<GenericTreeViewColumnProperty> pColumnProperties, bool pShowStatusBar = false)
        {
            SortProperty sortPropertyPermissionItem = new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending);

            CriteriaOperator criteriaOperatorPermissionItem = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _xpCollectionUserPermissionItem = new XPCollection(XPOSettings.Session, _xpObjectTypeUserPermissionItem, criteriaOperatorPermissionItem, sortPropertyPermissionItem);
            _xpCollectionUserPermissionProfile = new XPCollection(XPOSettings.Session, _xpObjectTypeUserPermissionProfile, null);

            //Parameters
            _sourceWindow = pSourceWindow;
            DialogType = pDialogType;
            _columnProperties = pColumnProperties;
            _showStatusBar = pShowStatusBar;
            _dataSource = pXPCollection;

            //Get First Custom Field Position ex OID
            _modelFirstCustomFieldIndex = (_treeViewMode == GenericTreeViewMode.Default) ? 1 : 2;

            //Sorting
            _dataSource.Sorting = XPOHelper.GetXPCollectionDefaultSortingCollection();
            //Prepare listStoreModel 
            //_listStoreModel = GenericTreeViewModel.XPCollectionToModel(_dataSource, _columnProperties);
            InitDataModel(_dataSource, _columnProperties, GenericTreeViewMode.Default);

            //Assign ListStoreModelFilter with ListStoreModel
            _listStoreModelFilter = new TreeModelFilter(ListStoreModel, null);
            //Assign ListStoreModelFilterSort with ListStoreModelFilter
            _listStoreModelFilterSort = new TreeModelSort(_listStoreModelFilter);

            InitUI();

            LoadRefreshView();
        }

        new protected void InitUI()
        {
            VBox vbox = new VBox(false, 1);

            //ScrolledWindow
            ScrolledWindow scrolledWindowProfile = new ScrolledWindow();
            scrolledWindowProfile.ShadowType = ShadowType.EtchedIn;
            scrolledWindowProfile.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            //StatusBar
            if (_showStatusBar)
            {
                _statusbar = new Statusbar() { HasResizeGrip = false };
                _statusbar.Push(0, CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_statusbar"));
            };

            //Treeview
            TreeView = new TreeView(_listStoreModelFilterSort);

            //_treeView.RulesHint = true;
            TreeView.EnableSearch = true;
            TreeView.SearchColumn = 1;
            TreeView.ModifyBg(StateType.Normal, new Gdk.Color(255, 0, 0));
            TreeView.ModifyCursor(new Gdk.Color(100, 100, 100), new Gdk.Color(200, 200, 200));
            //Add Columns
            AddColumns();

            //Navigator
            Navigator = new GenericTreeViewNavigator<XPCollection, XPGuidObject>(_sourceWindow, this, _navigatorMode);

            //TODO:THEME
            //if (GlobalApp.ScreenSize.Width >= 800)
            //{
            TouchButtonIconWithText buttonApplyPrivileges = Navigator.GetNewButton("touchButtonApplyPrivileges_DialogActionArea", CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_user_apply_privileges"), @"Icons/icon_pos_nav_refresh.png");
            //buttonApplyPrivileges.WidthRequest = 110;
            //Apply Permissions
            buttonApplyPrivileges.Sensitive = GeneralSettings.HasPermissionTo("BACKOFFICE_MAN_USER_PRIVILEGES_APPLY");
            //Event
            buttonApplyPrivileges.Clicked += delegate
            {
                GlobalApp.BackOfficeMainWindow.Accordion.UpdateMenuPrivileges();
                //Force Update MainWindow Pos Privilegs ex TollBar Buttons etc
                GlobalApp.PosMainWindow.TicketList.UpdateTicketListButtons();
            };
            //Add to Extra Slot
            Navigator.ExtraSlot.PackStart(buttonApplyPrivileges, false, false, 0);
            //}

            //Pack components
            scrolledWindowProfile.Add(TreeView);

            HBox hbox = new HBox(false, 1);
            hbox.PackStart(scrolledWindowProfile);

            vbox.PackStart(hbox, true, true, 0);
            vbox.PackStart(Navigator, false, false, 0);

            if (_showStatusBar)
            {
                vbox.PackStart(_statusbar, false, false, 0);
            }

            //Final Pack      
            PackStart(vbox);

            //Required Always Start in First Record, and get Iter XPGuidObject, Ready for Update Action
            try
            {
                if (_dataSource.Count > 0)
                {
                    TreeView.Model.GetIterFirst(out _treeIter);
                    _treePath = ListStoreModelFilter.GetPath(_treeIter);
                    TreeView.SetCursor(_treePath, null, false);
                    _dataSourceRow = (sys_userprofile)_dataSource.Lookup(new Guid(Convert.ToString(TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex))));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            //Events
            TreeView.CursorChanged += _treeView_CursorChanged;
            TreeView.RowActivated += delegate { Update(); };
            TreeView.Vadjustment.ValueChanged += delegate { UpdatePages(); };
            TreeView.Vadjustment.Changed += delegate { UpdatePages(); };

            //****************************************************************************

            _treeViewPermissionItem.Model = _listStoreModelPermissionItem;

            TreeViewColumn tmpColId = _treeViewPermissionItem.AppendColumn("ID", new CellRendererText(), "text", 0);
            tmpColId.Visible = false;

            TreeViewColumn tmpColProperty = _treeViewPermissionItem.AppendColumn(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_privilege_property"), new CellRendererText() { FontDesc = _fontDesc }, "text", 1);
            //Config Column Title
            Label labelPropertyTitle = new Label(tmpColProperty.Title);
            labelPropertyTitle.Show();
            labelPropertyTitle.ModifyFont(_fontDescTitle);
            tmpColProperty.Widget = labelPropertyTitle;

            TreeViewColumn tmpColActivo = _treeViewPermissionItem.AppendColumn(CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_privilege_active"), _cellRendererTogglePermissionItem, "active", 2);
            tmpColActivo.MaxWidth = 100;
            //Config Column Title
            Label labelActivoTitle = new Label(tmpColActivo.Title);
            labelActivoTitle.Show();
            labelActivoTitle.ModifyFont(_fontDescTitle);
            tmpColActivo.Widget = labelActivoTitle;
            //Event
            _cellRendererTogglePermissionItem.Toggled += ToggleEvent;

            ScrolledWindow scrolledWindowPermissionItem = new ScrolledWindow() { WidthRequest = 500 };
            scrolledWindowPermissionItem.Add(_treeViewPermissionItem);

            hbox.Add(scrolledWindowPermissionItem);
        }

        private void ToggleEvent(object o, ToggledArgs args)
        {
            TreeIter iter;
            if (_listStoreModelPermissionItem.GetIter(out iter, new TreePath(args.Path)))
            {
                _listStoreModelPermissionItem.SetValue(iter, 2, !_cellRendererTogglePermissionItem.Active);
                UpdateState("" + _listStoreModelPermissionItem.GetValue(iter, 0), _cellRendererTogglePermissionItem.Active);
            }
        }

        private void UpdateState(string pOid, bool pNewValue)
        {
            //UserProfile _currentXPObject = (UserProfile)_dataSource.Lookup(new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));
            sys_userprofile _currentXPObject = (sys_userprofile)XPOHelper.GetXPGuidObject(XPOSettings.Session, typeof(sys_userprofile), new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));
            bool needToInsert = true;
            for (int i = 0; i < _xpCollectionUserPermissionItem.Count; i++)
            {
                sys_userpermissionitem tmpUserPermissionItem = ((sys_userpermissionitem)_xpCollectionUserPermissionItem[i]);

                try
                {
                    if (tmpUserPermissionItem.Oid == new Guid("" + pOid))
                    {
                        for (int j = 0; j < _xpCollectionUserPermissionProfile.Count; j++)
                        {
                            sys_userpermissionprofile tmpUserPermissionProfile = ((sys_userpermissionprofile)_xpCollectionUserPermissionProfile[j]);

                            if ((tmpUserPermissionProfile.UserProfile != null) && (tmpUserPermissionProfile.PermissionItem != null))
                            {
                                if ((tmpUserPermissionProfile.UserProfile.Oid == _currentXPObject.Oid) &&
                                   (tmpUserPermissionProfile.PermissionItem.Oid == tmpUserPermissionItem.Oid))
                                {
                                    needToInsert = true;
                                    //((UserPermissionProfile)_xpCollection3[j]).Disabled = !pNewValue;
                                    //((UserPermissionProfile)_xpCollectionUserPermissionProfile[j]).Delete();
                                    //Mario Fix: Get Fresh Object else Gives Object Deleted Stress
                                    tmpUserPermissionProfile = (sys_userpermissionprofile)XPOHelper.GetXPGuidObject(XPOSettings.Session, typeof(sys_userpermissionprofile), tmpUserPermissionProfile.Oid);
                                    tmpUserPermissionProfile.Delete();
                                    _xpCollectionUserPermissionProfile.Reload();
                                }
                            }
                        }

                        if (needToInsert)
                        {
                            sys_userpermissionprofile tmpUserPermissionProfileUpdate = (sys_userpermissionprofile)Activator.CreateInstance(_xpObjectTypeUserPermissionProfile, XPOSettings.Session);
                            tmpUserPermissionProfileUpdate.Reload();
                            tmpUserPermissionProfileUpdate.UserProfile = _currentXPObject;
                            //Mario Fix: Get Fresh Object else Gives Object Deleted Stress
                            tmpUserPermissionItem = (sys_userpermissionitem)XPOHelper.GetXPGuidObject(XPOSettings.Session, typeof(sys_userpermissionitem), tmpUserPermissionItem.Oid);
                            tmpUserPermissionProfileUpdate.PermissionItem = tmpUserPermissionItem;
                            tmpUserPermissionProfileUpdate.Granted = !pNewValue;
                            tmpUserPermissionProfileUpdate.Save();
                            _xpCollectionUserPermissionProfile.Reload();
                        }
                        else
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }
        }

        // Detect cursor changes, from mouse or keyboard
        new protected void _treeView_CursorChanged(object sender, EventArgs e)
        {
            base._treeView_CursorChanged(sender, e);
            LoadRefreshView();
        }

        private void LoadRefreshView()
        {
            _listStoreModelPermissionItem.Clear();

            //UserProfile _currentXPObject = (UserProfile)_dataSource.Lookup(new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));
            sys_userprofile _currentXPObject = (sys_userprofile)XPOHelper.GetXPGuidObject(XPOSettings.Session, typeof(sys_userprofile), new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));

            for (int i = 0; i < _xpCollectionUserPermissionItem.Count; i++)
            {
                bool tmpCurrentValue = false;
                sys_userpermissionitem tmpUserPermissionItem = ((sys_userpermissionitem)_xpCollectionUserPermissionItem[i]);

                try
                {
                    for (int j = 0; j < _xpCollectionUserPermissionProfile.Count; j++)
                    {
                        sys_userpermissionprofile tmpUserPermissionProfile = ((sys_userpermissionprofile)_xpCollectionUserPermissionProfile[j]);
                        if (tmpUserPermissionProfile.UserProfile != null && tmpUserPermissionProfile.PermissionItem != null)
                        {
                            if ((tmpUserPermissionProfile.UserProfile.Oid == _currentXPObject.Oid) &&
                                (tmpUserPermissionProfile.PermissionItem.Oid == tmpUserPermissionItem.Oid))
                            {
                                tmpCurrentValue = tmpUserPermissionProfile.Granted;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
                _listStoreModelPermissionItem.AppendValues("" + tmpUserPermissionItem.Oid, tmpUserPermissionItem.Designation, tmpCurrentValue);
            }
        }
    }
}
