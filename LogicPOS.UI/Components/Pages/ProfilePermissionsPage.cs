using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Components.Pages;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class ProfilePermissionsPage : Page
    {
        // Forged Fields
        private  Window _parentWindow;
        public Type DialogType { get; set; }
        public XPCollection Entities { get; private set; }
        public TreeModelSort ListStoreModelFilterSort { get; private set; }
        public TreeModel ListStoreModel { get; private set; }
        public PageNavigator Navigator { get; private set; }
        public sys_userprofile Profile { get; private set; }
        public TreePath TreePathModel { get; private set; }
        public int MarkedCheckBoxs { get; private set; }

        public event EventHandler CursorChanged;
        public event EventHandler CheckBoxToggled;

        private int _modelFirstCustomFieldIndex;


        //Settings
        private readonly string _fontGenericTreeViewColumnTitle = AppSettings.Instance.fontGenericTreeViewColumnTitle;
        private readonly string _fontGenericTreeViewColumn = AppSettings.Instance.fontGenericTreeViewColumn;

        private readonly TreeView _treeViewPermissionItem = new TreeView();
        private readonly ListStore _listStoreModelPermissionItem = new ListStore(typeof(string), typeof(string), typeof(bool));
        private readonly CellRendererToggle _cellRendererTogglePermissionItem = new CellRendererToggle();

        private readonly Type _xpObjectTypeUserPermissionItem = typeof(sys_userpermissionitem);
        private readonly Type _xpObjectTypeUserPermissionProfile = typeof(sys_userpermissionprofile);
        private XPCollection _xpCollectionUserPermissionItem;
        private XPCollection _xpCollectionUserPermissionProfile;
        private GridViewMode _treeViewMode;
        private GridViewNavigatorMode _navigatorMode;
        private short _currentRowIndex;
        private TreePath _treePath;
        private TreeIter _treeIter;
        private TreeIter TreeIterModel;
        private int _columnSortColumnId;
        private SortType _columnSortType;
        private bool _columnSortIndicator;
        private int _modelCheckBoxFieldIndex;
        private readonly Pango.FontDescription _fontDescTitle;
        private readonly Pango.FontDescription _fontDesc;

      
        public ProfilePermissionsPage(Window parentWindow)
        {
            _fontDescTitle = Pango.FontDescription.FromString(_fontGenericTreeViewColumnTitle);
            _fontDesc = Pango.FontDescription.FromString(_fontGenericTreeViewColumn);

            CriteriaOperator criteria = null;

            bool showStatusBar = false;
            Type xPGuidObjectType = typeof(sys_userprofile);
            Type typeDialogClass = typeof(DialogUserProfile);

            Type xpObjectTypeUserProfile = typeof(sys_userprofile);

            XPCollection xpCollectionUserProfile = new XPCollection(XPOSettings.Session, xpObjectTypeUserProfile, criteria);

            List<GridViewColumnProperty> columnPropertiesUserProfile = new List<GridViewColumnProperty>
            {
                new GridViewColumnProperty("Code") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_record_code") },
                new GridViewColumnProperty("Designation") { Title = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_user_profiles"), MaxWidth = 150 }
            };

            InitObject(parentWindow, xpCollectionUserProfile, xPGuidObjectType, typeDialogClass, columnPropertiesUserProfile, showStatusBar);
            ShowAll();
        }

        protected void InitObject(Window parentWindow,
                                  XPCollection pXPCollection,
                                  Type pXPGuidObjectType,
                                  Type pDialogType,
                                  List<GridViewColumnProperty> pColumnProperties,
                                  bool pShowStatusBar = false)
        {
            SortProperty sortPropertyPermissionItem = new SortProperty("Ord", DevExpress.Xpo.DB.SortingDirection.Ascending);

            CriteriaOperator criteriaOperatorPermissionItem = CriteriaOperator.Parse("(Disabled IS NULL OR Disabled  <> 1)");
            _xpCollectionUserPermissionItem = new XPCollection(XPOSettings.Session, _xpObjectTypeUserPermissionItem, criteriaOperatorPermissionItem, sortPropertyPermissionItem);
            _xpCollectionUserPermissionProfile = new XPCollection(XPOSettings.Session, _xpObjectTypeUserPermissionProfile, null);


            _parentWindow = parentWindow;
            DialogType = pDialogType;
            Columns = pColumnProperties;
            Entities = pXPCollection;

            _modelFirstCustomFieldIndex = (_treeViewMode == GridViewMode.Default) ? 1 : 2;

            Entities.Sorting = XPOUtility.GetXPCollectionDefaultSortingCollection();

            InitDataModel(Entities, Columns, GridViewMode.Default);


            ListStoreModelFilter = new TreeModelFilter(ListStoreModel, null);
          
            ListStoreModelFilterSort = new TreeModelSort(ListStoreModelFilter);

            InitUI();

            LoadRefreshView();
        }

        public void InitDataModel(XPCollection pDataSource,
                                           List<GridViewColumnProperty> pColumnProperties,
                                           GridViewMode pGenericTreeViewMode)
        {
            XPCollection _XpCollection = pDataSource;
            List<GridViewColumnProperty> _columnProperties = pColumnProperties;

            ListStore model = GridViewModel.InitModel(_columnProperties, pGenericTreeViewMode);

            object[] columnValues = new object[_columnProperties.Count];

            string fieldName;

            int rowIndex = -1;
            foreach (Entity dataRow in _XpCollection as XPCollection)
            {
                rowIndex++;

                bool undefinedRecord = false;

                dataRow.Reload();

                for (int i = 0; i < _columnProperties.Count; i++)
                {
                    fieldName = _columnProperties[i].Name;

                    try
                    {
                        if (_columnProperties[i].PropertyType == GridViewPropertyType.CheckBox)
                        {
                            columnValues[i] = false;
                        }
                        else if (fieldName == "RowIndex")
                        {
                            columnValues[i] = rowIndex;
                        }
                        else if (_columnProperties[i].Query != null && _columnProperties[i].Query != string.Empty)
                        {
                            if (dataRow.GetType() == typeof(fin_articleserialnumber))
                            {
                                if ((dataRow as fin_articleserialnumber).StockMovimentIn != null)
                                {
                                    var Oid = new Guid();
                                    if (_columnProperties[i].Query == "SELECT Name as Result FROM erp_customer WHERE Oid = '{0}';")
                                    {
                                        Oid = (dataRow as fin_articleserialnumber).StockMovimentIn.Customer.Oid;
                                    }

                                    else if (_columnProperties[i].Query == "SELECT DocumentNumber as Result FROM fin_documentfinancemaster WHERE Oid = '{0}';")
                                    {
                                        if ((dataRow as fin_articleserialnumber).StockMovimentOut != null)
                                        {
                                            Oid = (dataRow as fin_articleserialnumber).StockMovimentOut.DocumentMaster.Oid;
                                        }
                                    }
                                    else if (_columnProperties[i].Query != string.Empty && dataRow.GetType() == typeof(fin_articleserialnumber))
                                    {
                                        Oid = (dataRow as fin_articleserialnumber).ArticleWarehouse.Location.Oid;
                                    }

                                    columnValues[i] = ColumnPropertyGetQuery(_columnProperties[i].Query, Oid);
                                    // Decrypt Before use Format
                                    if (_columnProperties[i].DecryptValue)
                                    {
                                        columnValues[i] = LogicPOS.Domain.Entities.Entity.DecryptIfNeeded(columnValues[i]);
                                    }
                                    //Format String using Column FormatProvider              
                                    if (_columnProperties[i].FormatProvider != null && Convert.ToString(columnValues[i]) != string.Empty)
                                    {
                                        columnValues[i] = string.Format(_columnProperties[i].FormatProvider, "{0}", columnValues[i]);
                                    }
                                }
                                else
                                {
                                    columnValues[i] = string.Empty;
                                }
                            }
                            else
                            {
                                columnValues[i] = ColumnPropertyGetQuery(_columnProperties[i].Query, dataRow.GetMemberValue("Oid"));
                                // Decrypt Before use Format
                                if (_columnProperties[i].DecryptValue)
                                {
                                    columnValues[i] = LogicPOS.Domain.Entities.Entity.DecryptIfNeeded(columnValues[i]);
                                }
                                //Format String using Column FormatProvider              
                                if (_columnProperties[i].FormatProvider != null && Convert.ToString(columnValues[i]) != string.Empty)
                                {
                                    columnValues[i] = string.Format(_columnProperties[i].FormatProvider, "{0}", columnValues[i]);
                                }
                            }
                        }

                        //If detect XPGuidObject Value Type (Value is a XPObject, Child Object), Get Value from its Chield Field (Related Table)
                        else if (dataRow.GetMemberValue(_columnProperties[i].Name) != null &&
                          dataRow.GetMemberValue(_columnProperties[i].Name).GetType().BaseType == typeof(Entity))
                        {
                            var value = GetXPGuidObjectChildValue(dataRow.GetMemberValue(_columnProperties[i].Name), fieldName, _columnProperties[i].ChildName);
                            if (value != null)
                            {
                                if (GetXPGuidObjectChildValue(dataRow.GetMemberValue(_columnProperties[i].Name), fieldName, _columnProperties[i].ChildName).Equals("False") || GetXPGuidObjectChildValue(dataRow.GetMemberValue(_columnProperties[i].Name), fieldName, _columnProperties[i].ChildName).Equals("True"))
                                {
                                    bool booleanValue = Convert.ToBoolean(GetXPGuidObjectChildValue(dataRow.GetMemberValue(_columnProperties[i].Name), fieldName, _columnProperties[i].ChildName));
                                    columnValues[i] = booleanValue ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_true") : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_false");
                                }
                                else
                                {
                                    columnValues[i] = GetXPGuidObjectChildValue(dataRow.GetMemberValue(_columnProperties[i].Name), fieldName, _columnProperties[i].ChildName);
                                }
                            }
                        }
                        //Get Default Value from Field Name
                        else
                        {
                            // Check/Detect if is a undefinedRecord
                            if (fieldName == "Oid")
                            {
                                undefinedRecord = new Guid(dataRow.GetMemberValue(_columnProperties[i].Name).ToString()).Equals(XPOSettings.XpoOidUndefinedRecord);
                            }

                            //TODO (Muga): melhorar isto para ser generico e contemplar os outros campos
                            if (dataRow.GetMemberValue(_columnProperties[i].Name) != null)
                            {
                                // Detect UndefinedRecord and if is a Int Field, and Replace with string.Empty, this ways we Dont Show 0 in Null Undefined Records
                                if (undefinedRecord && dataRow.GetMemberValue(_columnProperties[i].Name).GetType().Name.Equals("UInt32"))
                                {
                                    columnValues[i] = string.Empty;
                                }
                                //Boolean Fields
                                else if (dataRow.GetMemberValue(_columnProperties[i].Name).GetType().Name.Equals("Boolean"))
                                {
                                    bool booleanValue = Convert.ToBoolean(dataRow.GetMemberValue(_columnProperties[i].Name));
                                    columnValues[i] = booleanValue ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_true") : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_false");
                                }
                                else if (dataRow.GetMemberValue(_columnProperties[i].Name).Equals("False") || dataRow.GetMemberValue(_columnProperties[i].Name).Equals("True"))
                                {
                                    bool booleanValue = Convert.ToBoolean(dataRow.GetMemberValue(_columnProperties[i].Name));
                                    columnValues[i] = booleanValue ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_true") : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_false");
                                }
                                //Enum
                                else if (dataRow.GetType() == typeof(fin_articleserialnumber) && _columnProperties[i].Name == "Status")
                                {
                                    columnValues[i] = Convert.ToString(Enum.GetName(typeof(ArticleSerialNumberStatus), Convert.ToInt32(dataRow.GetMemberValue(_columnProperties[i].Name))));
                                }

                                //Other Fields
                                else
                                {
                                    columnValues[i] = Convert.ToString(dataRow.GetMemberValue(_columnProperties[i].Name));
                                    //Format String using Column FormatProvider              
                                    if (_columnProperties[i].FormatProvider != null)
                                    {
                                        columnValues[i] = string.Format(_columnProperties[i].FormatProvider, "{0}", columnValues[i]);
                                    }
                                    if (_columnProperties[i].DecryptValue)
                                    {
                                        columnValues[i] = LogicPOS.Domain.Entities.Entity.DecryptIfNeeded(columnValues[i]);
                                    }
                                }
                            }
                            else
                            {
                                columnValues[i] = string.Empty;
                            }
                        }

                        if (_columnProperties[i].ResourceString == true && columnValues[i] != null)
                        {
                            bool checkIfResourceStringExist = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, columnValues[i].ToString()) != null;
                            if (checkIfResourceStringExist)
                            {
                                columnValues[i] = CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, columnValues[i].ToString());
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        columnValues[i] = string.Format("Invalid Field {0}", fieldName);
                    }
                };
                //Add Column Value to Model
                model.AppendValues(columnValues);
            }
            ListStoreModel = model;
        }


        protected void InitUI()
        {
            VBox vbox = new VBox(false, 1);

            ScrolledWindow scrolledWindowProfile = new ScrolledWindow();
            scrolledWindowProfile.ShadowType = ShadowType.EtchedIn;
            scrolledWindowProfile.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);

            //Treeview
            TreeView = new TreeView(ListStoreModelFilterSort);

            //_treeView.RulesHint = true;
            TreeView.EnableSearch = true;
            TreeView.SearchColumn = 1;
            TreeView.ModifyBg(StateType.Normal, new Gdk.Color(255, 0, 0));
            TreeView.ModifyCursor(new Gdk.Color(100, 100, 100), new Gdk.Color(200, 200, 200));
            //Add Columns
            AddColumns();

            //Navigator
            Navigator = new PageNavigator(_parentWindow, this, _navigatorMode);

            //TODO:THEME
            //if (GlobalApp.ScreenSize.Width >= 800)
            //{
            IconButtonWithText buttonApplyPrivileges = Navigator.GetNewButton("touchButtonApplyPrivileges_DialogActionArea", CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_user_apply_privileges"), @"Icons/icon_pos_nav_refresh.png");
            //buttonApplyPrivileges.WidthRequest = 110;
            //Apply Permissions
            buttonApplyPrivileges.Sensitive = GeneralSettings.LoggedUserHasPermissionTo("BACKOFFICE_MAN_USER_PRIVILEGES_APPLY");
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

            //Final Pack      
            PackStart(vbox);

            //Required Always Start in First Record, and get Iter XPGuidObject, Ready for Update Action
            if (Entities.Count > 0)
            {
                TreeView.Model.GetIterFirst(out _treeIter);
                _treePath = ListStoreModelFilter.GetPath(_treeIter);
                TreeView.SetCursor(_treePath, null, false);
                Profile = (sys_userprofile)Entities.Lookup(new Guid(Convert.ToString(TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex))));
            }

            //Events
            TreeView.CursorChanged += _treeView_CursorChanged;
            TreeView.RowActivated += delegate { throw new NotImplementedException(); };
            TreeView.Vadjustment.ValueChanged += delegate { UpdatePages(); };
            TreeView.Vadjustment.Changed += delegate { UpdatePages(); };

            //****************************************************************************

            _treeViewPermissionItem.Model = _listStoreModelPermissionItem;

            TreeViewColumn tmpColId = _treeViewPermissionItem.AppendColumn("ID", new CellRendererText(), "text", 0);
            tmpColId.Visible = false;

            TreeViewColumn tmpColProperty = _treeViewPermissionItem.AppendColumn(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_privilege_property"), new CellRendererText() { FontDesc = _fontDesc }, "text", 1);
            //Config Column Title
            Label labelPropertyTitle = new Label(tmpColProperty.Title);
            labelPropertyTitle.Show();
            labelPropertyTitle.ModifyFont(_fontDescTitle);
            tmpColProperty.Widget = labelPropertyTitle;

            TreeViewColumn tmpColActivo = _treeViewPermissionItem.AppendColumn(CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_privilege_active"), _cellRendererTogglePermissionItem, "active", 2);
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
            sys_userprofile _currentXPObject = XPOUtility.GetEntityById<sys_userprofile>(new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));
            bool needToInsert = true;
            for (int i = 0; i < _xpCollectionUserPermissionItem.Count; i++)
            {
                sys_userpermissionitem tmpUserPermissionItem = ((sys_userpermissionitem)_xpCollectionUserPermissionItem[i]);

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
                                tmpUserPermissionProfile = XPOUtility.GetEntityById<sys_userpermissionprofile>(tmpUserPermissionProfile.Oid);
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
                        tmpUserPermissionItem = XPOUtility.GetEntityById<sys_userpermissionitem>(tmpUserPermissionItem.Oid);
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
        }

        protected void _treeView_CursorChanged(object sender, EventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            TreeSelection selection = treeView.Selection;
            TreeModel model;
            string output = string.Empty;

            if (selection.GetSelected(out model, out _treeIter))
            {
                _currentRowIndex = Convert.ToInt16(ListStoreModelFilterSort.GetValue(_treeIter, 0));
                _treePath = model.GetPath(_treeIter);
                Navigator.CurrentRecord = Convert.ToInt16(_treePath.ToString());


                ListStoreModel.GetIterFromString(out TreeIterModel, _currentRowIndex.ToString());
                TreePathModel = ListStoreModel.GetPath(TreeIterModel);

                GetDataRow();
            };

            UpdatePages();

            CursorChanged?.Invoke(this, e);

            LoadRefreshView();
        }

        private void LoadRefreshView()
        {
            _listStoreModelPermissionItem.Clear();

            //UserProfile _currentXPObject = (UserProfile)_dataSource.Lookup(new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));
            sys_userprofile _currentXPObject = XPOUtility.GetEntityById<sys_userprofile>(new Guid("" + TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)));

            for (int i = 0; i < _xpCollectionUserPermissionItem.Count; i++)
            {
                bool tmpCurrentValue = false;
                sys_userpermissionitem tmpUserPermissionItem = ((sys_userpermissionitem)_xpCollectionUserPermissionItem[i]);

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

                _listStoreModelPermissionItem.AppendValues("" + tmpUserPermissionItem.Oid, tmpUserPermissionItem.Designation, tmpCurrentValue);
            }
        }

        public string ColumnPropertyGetQuery(string pSql, object pKey)
        {
            string sql = string.Format(pSql, pKey);
            var result = Convert.ToString(XPOSettings.Session.ExecuteScalar(sql));

            return result;
        }

        public object GetXPGuidObjectChildValue(object pFieldValue, string pFieldName, string pChildName)
        {

            object fieldValue;
            if (pChildName != null & pChildName != string.Empty)
            {
                pFieldName += "." + pChildName;
                dynamic dynamicFieldValue = pFieldValue;
                fieldValue = Convert.ToString(dynamicFieldValue.GetType().GetProperty(pChildName).GetValue(dynamicFieldValue, null));
            }
            //XPGuidObject - If detect XPGuidObject Type and dont have a valid ChieldName, Send Warning Value to Alert Developer
            else
            {
                fieldValue = string.Format("Detected XPGuidObject! You must define ChildName for Field {0}", pFieldName);
            };

            return fieldValue;
        }

        protected void AddColumns()
        {
            bool assignValue;
            CellRendererText currentCellRendererProperties;
            GridViewColumnProperty currentTreeViewColumnProperty;
            PropertyInfo[] pisTreeViewColumnProperties;
            object pInfoValue;
            PropertyInfo piTreeViewColumn;

            for (int i = 0; i < Columns.Count; i++)
            {
                currentCellRendererProperties = Columns[i].CellRenderer;

                if (Columns[i].PropertyType == GridViewPropertyType.Text)
                {
                    Columns[i].Column = new TreeViewColumn(Columns[i].Name, currentCellRendererProperties, "text", i) { Clickable = true };
                    Columns[i].Column.Clicked += Column_Clicked;
                }
                else if (Columns[i].PropertyType == GridViewPropertyType.CheckBox)
                {
                    CellRendererToggle currentCellRendererToggle = new CellRendererToggle() { Activatable = true };
                    currentCellRendererToggle.Toggled += CurrentCellRendererToggle_Toggled;
                    Columns[i].Column = new TreeViewColumn(Columns[i].Name, currentCellRendererToggle, "active", i);
                }

                currentTreeViewColumnProperty = Columns[i];
                pisTreeViewColumnProperties = typeof(GridViewColumnProperty).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                pInfoValue = new object();

                foreach (PropertyInfo pInfo in pisTreeViewColumnProperties)
                {
                    assignValue = false;
                    pInfoValue = pInfo.GetValue(currentTreeViewColumnProperty, null);

                    if (pInfoValue != null)
                    {
                        piTreeViewColumn = Columns[i].Column.GetType().GetProperty(pInfo.Name);

                        switch (pInfoValue.GetType().Name)
                        {
                            case "Boolean":
                                assignValue = true;
                                break;
                            case "Int32":
                                if ((int)pInfoValue > 0) { assignValue = true; };
                                break;
                            case "Single":
                                if ((float)pInfoValue > 0) { assignValue = true; };
                                break;
                            case "String":
                                if ((string)pInfoValue != string.Empty) { assignValue = true; };
                                break;
                            case "FontDescription":
                                Label labelTitle = new Label(Columns[i].Title);
                                labelTitle.Show();
                                labelTitle.ModifyFont((Pango.FontDescription)pInfoValue);
                                Columns[i].Column.Widget = labelTitle;
                                break;
                            case "TreeViewColumn":
                                break;
                            case "CellRendererText":
                                break;
                            default:
                                break;
                        }

                        if (assignValue && pInfo.Name != "Name" && piTreeViewColumn != null)
                        {
                            piTreeViewColumn.SetValue(Columns[i].Column, pInfoValue);
                        }
                    }
                }

                Columns[i].Column.Widget.ModifyFont(Columns[i].FontDescriptionTitle);

                Columns[i].Column.SortColumnId = i;

                TreeView.AppendColumn(Columns[i].Column);
            }
        }

        protected void UpdatePages()
        {
            Navigator.CurrentPage = (int)Math.Floor(TreeView.Vadjustment.Value / TreeView.Vadjustment.PageSize) + 1;
            Navigator.TotalPages = (int)Math.Floor(TreeView.Vadjustment.Upper / TreeView.Vadjustment.PageSize);
            if (TreeView.Model != null)
            {
                Navigator.TotalRecords = TreeView.Model.IterNChildren() - 1;
            }
            else
            {
                Navigator.TotalRecords = 0;
            };
            Navigator.UpdateButtons(TreeView);
        }

        public  void GetDataRow()
        {
            Profile =(sys_userprofile) XPOUtility.GetXPGuidObject(
              Entities.ObjectType,
              new Guid(Convert.ToString(TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)))
            );
        }

        private void Column_Clicked(object sender, EventArgs e)
        {
            TreeViewColumn column = (TreeViewColumn)sender;
            _columnSortColumnId = column.SortColumnId;
            _columnSortType = column.SortOrder;
            _columnSortIndicator = column.SortIndicator;
        }

        private void CurrentCellRendererToggle_Toggled(object o, ToggledArgs args)
        {
            //Required to force call CursorChanged
            _treePath = new TreePath(args.Path);
            TreeView.SetCursor(_treePath, null, false);
            ToggleCheckBox(new TreePath(args.Path));
        }

        private void ToggleCheckBox(TreePath pTreePath)
        {
            //Required to get _treeIterModel from _currentRowIndex (Assigned in CursorChanged from ModelFilterOrder)
            if (ListStoreModel.GetIterFromString(out TreeIterModel, _currentRowIndex.ToString()))
            {
                //Update Model
                bool old = (bool)ListStoreModel.GetValue(TreeIterModel, _modelCheckBoxFieldIndex);
                if (!old) { MarkedCheckBoxs++; } else { MarkedCheckBoxs--; };
                ListStoreModel.SetValue(TreeIterModel, _modelCheckBoxFieldIndex, !old);

                //Use Child Implementations : Currently only used implemented in DataTable
                ToggleCheckBox(old);
            }

            //Fire Toggle Event
            OnCheckBoxToggled();
        }

        private void OnCheckBoxToggled()
        {
            CheckBoxToggled?.Invoke(this, EventArgs.Empty);
        }

        public virtual void ToggleCheckBox(bool pOldValue) { }
    }
}
