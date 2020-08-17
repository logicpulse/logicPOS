using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using DevExpress.Data.Filtering;
using logicpos.resources;
using System.Configuration;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    /// <summary>Class used to render TreeView from generated liststore model, and column properties</summary>
    /// T1 DataSource Generic Type, T2 DataSourceRow Generic Type
    abstract class GenericTreeView<T1, T2> : Box, IGenericTreeView
    {
        //Log4Net
        protected static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected bool _debug = false;

        //Parameters
        protected Window _sourceWindow;
        //DialogType, the Type Dialog ex ArticleDialog
        protected GenericTreeViewMode _treeViewMode;
        protected bool _showStatusBar = false;
        protected GenericTreeViewNavigatorMode _navigatorMode;
        protected int _currentRowIndex = 0;
        //Used when Update ModelFilter, to ReIndex Index
        protected int _reindexRowIndex = 0;
        protected int _totalRows = 0;
        //Used to Store First Custom User Field, usefull to get for ex OID Position (1=NonCheckBoxMode or 2=CheckBoxMode)
        protected int _modelFirstCustomFieldIndex;
        protected int _modelCheckBoxFieldIndex = 1;
        //Just used to Store Initial DataTable Selected Value, and to use it to Compare when Init in ReIndex
        protected Guid _guidDefaultValue;
        //UI
        //Used for _listStoreModelFilterSort (#3 model)
        protected TreeIter _treeIter;
        protected TreePath _treePath;
        protected TreeViewColumn _treeViewColumn;
        //Columns
        private SortType _columnSortType = SortType.Ascending;
        private int _columnSortColumnId = 0;
        private bool _columnSortIndicator = true;
        protected Statusbar _statusbar;
        //Public Properties
        //Use Generic GTK Dialog, to Work on Both bases (BOBaseDialog|PosBaseDialog)
        protected Type _dialogType;
        public Type DialogType
        {
            get { return _dialogType; }
            set { _dialogType = value; }
        }
        protected Dialog _dialog;
        public Dialog Dialog
        {
            get { return _dialog; }
            set { _dialog = value; }
        }
        //Used for _listStoreModel, require CRUD (#1 model)
        protected TreeIter _treeIterModel;
        public TreeIter TreeIterModel
        {
            get { return _treeIterModel; }
            set { _treeIterModel = value; }
        }
        private TreePath _treePathModel;
        public TreePath TreePathModel
        {
            get { return _treePathModel; }
            set { _treePathModel = value; }
        }
        protected TreeView _treeView;
        public TreeView TreeView
        {
            get { return _treeView; }
            set { _treeView = value; }
        }
        protected GenericTreeViewNavigator<T1, T2> _navigator;
        public GenericTreeViewNavigator<T1, T2> Navigator
        {
            get { return _navigator; }
            set { _navigator = value; }
        }
        //DataModel : Base Model
        protected ListStore _listStoreModel;
        public ListStore ListStoreModel
        {
            get { return _listStoreModel; }
            set { _listStoreModel = value; }
        }
        //DataModel : Base Model > Filter Model
        protected TreeModelFilter _listStoreModelFilter;
        public TreeModelFilter ListStoreModelFilter
        {
            get { return _listStoreModelFilter; }
            set { _listStoreModelFilter = value; }
        }
        //DataModel : Base Model > Filter Model > Sort Model
        protected TreeModelSort _listStoreModelFilterSort;
        public TreeModelSort ListStoreModelFilterSort
        {
            get { return _listStoreModelFilterSort; }
            set { _listStoreModelFilterSort = value; }
        }
        //DataSource Collection (XPCollection | DataTable)
        protected T1 _dataSource;
        public T1 DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }
        //DataSourceRow Row/Store Cursor|Default Value (XPGuidObject | DataRow)
        protected T2 _dataSourceRow;
        public T2 DataSourceRow
        {
            get { return _dataSourceRow; }
            set { _dataSourceRow = value; }
        }
        //TreeView Column Properties
        protected List<GenericTreeViewColumnProperty> _columnProperties;
        public List<GenericTreeViewColumnProperty> Columns
        {
            get { return _columnProperties; }
            set { _columnProperties = value; }
        }
        protected bool _allowNavigate = true;
        public bool AllowNavigate
        {
            get { return _allowNavigate; }
            set { _allowNavigate = value; }
        }
        //CRUD Privileges
        protected bool _allowRecordInsert = true;
        public bool AllowRecordInsert
        {
            get { return _allowRecordInsert; }
            set { _allowRecordInsert = value; _navigator.ButtonInsert.Sensitive = value; }
        }
        protected bool _allowRecordUpdate = true;
        public bool AllowRecordUpdate
        {
            get { return _allowRecordUpdate; }
            set { _allowRecordUpdate = value; _navigator.ButtonUpdate.Sensitive = value; }
        }
        protected bool _allowRecordDelete = true;
        public bool AllowRecordDelete
        {
            get { return _allowRecordDelete; }
            set { _allowRecordDelete = value; _navigator.ButtonDelete.Sensitive = value; }
        }
        protected bool _allowRecordView = true;
        public bool AllowRecordView
        {
            get { return _allowRecordView; }
            set { _allowRecordView = value; _navigator.ButtonView.Sensitive = value; }
        }
        //Skip Action : Usefull to Use Outside code to Prevent Inserts,Updates and Deletes based on Custom Logic Code
        protected bool _skipRecordInsert = false;
        public bool SkipRecordInsert
        {
            get { return _skipRecordInsert; }
            set { _skipRecordInsert = value; }
        }
        protected bool _skipRecordUpdate = false;
        public bool SkipRecordUpdate
        {
            get { return _skipRecordUpdate; }
            set { _skipRecordUpdate = value; }
        }
        protected bool _skipRecordDelete = false;
        public bool SkipRecordDelete
        {
            get { return _skipRecordDelete; }
            set { _skipRecordDelete = value; }
        }
        //Other
        protected int _markedCheckBoxs = 0;
        public int MarkedCheckBoxs
        {
            get { return _markedCheckBoxs; }
            set { _markedCheckBoxs = value; }
        }
        //ReadOnly/Disable INS/DEL/UPD
        protected bool _readOnly = false;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _allowRecordInsert = !value;
                _allowRecordUpdate = !value;
                _allowRecordDelete = !value;
                _navigator.ButtonDelete.Sensitive = !value;
                _navigator.ButtonInsert.Sensitive = !value;
                _navigator.ButtonUpdate.Sensitive = !value;
                _readOnly = value;
            }
        }

        protected int _currentPageNumber = 1;

        public int CurrentPageNumber
        {
            get { return _currentPageNumber; }
            set { _currentPageNumber = value; }
        }
        //Public EventHandlers
        public event EventHandler CursorChanged;
        public event EventHandler CheckBoxToggled;
        //Public Custom CRUD Events
        public event EventHandler RecordBeforeInsert;
        public event EventHandler RecordBeforeUpdate;
        public event EventHandler RecordBeforeDelete;
        public event EventHandler RecordBeforeView;
        public event EventHandler RecordAfterInsert;
        public event EventHandler RecordAfterUpdate;
        public event EventHandler RecordAfterDelete;
        public event EventHandler RecordAfterView;

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Constructor

        //Parameterless Constructor
        public GenericTreeView()
        {
        }

        public GenericTreeView(Window pSourceWindow)
        {
            _sourceWindow = pSourceWindow;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Abstract/Virtual Methods to be Implemented by SubClass/Child Classes

        public abstract void InitObject(
          Window pSourceWindow,
          T2 pDefaultValue,
          GenericTreeViewMode pGenericTreeViewMode,
          GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode,
          List<GenericTreeViewColumnProperty> pColumnProperties,
          T1 pDataSource,
          Type pDialogType
        );

        //Must Be Overridden 
        public abstract void InitDataModel(
          T1 pDataSource,
          List<GenericTreeViewColumnProperty> pColumnProperties,
          GenericTreeViewMode pGenericTreeViewMode
        );

        //Must Be Overridden
        public abstract void GetDataRow();

        //Must be Overridden : Used inside DataSourceRowToModelRow, to get T2 Column Values
        public abstract object DataSourceRowGetColumnValue(T2 pDataSourceRow, int pColumnIndex, string pFieldName = "");

        //Must be Overridden : Used in Inserts, to Get Fresh T2 Record DataSourceRow 
        public abstract T2 DataSourceRowGetNewRecord();

        //Optional Overridden
        public virtual void DataSourceRowInsert<T>(T pDataSourceRow) { }

        //Optional Overridden
        public virtual void DataSourceRowDelete<T>(T pDataSourceRow) { }

        //Optional Overridden : Currently only DataTable Implements it, With dont persist CheckBoxs in XPOGuidObjects
        public virtual void ToggleCheckBox(bool pOldValue) { }

        //Optional Overridden
        //public virtual bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode) { return false; }

        //Optional Overridden
        public virtual void Refresh() { }

        //Optional Overridden
        public virtual void DeleteRecords() { }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        /// <summary>Initialize GenericTreeView User Interface</summary>
        protected void InitUI()
        {
            //Tree Containers
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            VBox vbox = new VBox(false, 1);

            //StatusBar
            if (_showStatusBar)
            {
                _statusbar = new Statusbar() { HasResizeGrip = false };
                _statusbar.Push(0, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_statusbar"));
            };

            //1) Add Model to TreeModelFilter
            _listStoreModelFilter = new TreeModelFilter(_listStoreModel, null);
            //2) Add FilterModel to Sort
            _listStoreModelFilterSort = new TreeModelSort(_listStoreModelFilter);
            if (_columnSortIndicator) _listStoreModelFilterSort.SetSortColumnId(_columnSortColumnId, _columnSortType);
            //3) Finally Add FilterSortModel to TreeView
            _treeView = new TreeView(_listStoreModelFilterSort);

            _treeView.RulesHint = true;
            _treeView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));
            //_treeView.ModifyCursor(new Gdk.Color(50, 50, 50), new Gdk.Color(200, 200, 200));
            //_treeView.EnableSearch = false;
            //_treeView.Selection.Mode = SelectionMode.Single;
            //_treeView.SearchColumn = 1;

            //Add Columns
            AddColumns();

            //Navigator
            _navigator = new GenericTreeViewNavigator<T1, T2>(_sourceWindow, this, _navigatorMode);

            //Pack components
            viewport.Add(_treeView);
            scrolledWindow.Add(viewport);
            //Pack VBox
            vbox.PackStart(scrolledWindow, true, true, 0);
            if (_navigatorMode == GenericTreeViewNavigatorMode.Default) vbox.PackStart(_navigator, false, false, 0);
            if (_showStatusBar) vbox.PackStart(_statusbar, false, false, 0);

            //Final Pack      
            PackStart(vbox);

            //Events
            _treeView.CursorChanged += _treeView_CursorChanged;
            _treeView.RowActivated += delegate
            {
                // CheckBox Mode on DoubleClick
                if (_treeViewMode == GenericTreeViewMode.CheckBox)
                {
                    ToggleCheckBox(_treePath);
                }
                // Non CheckBox Mode on DoubleClick
                else
                {
                    //DoubleClick Edit only Works if has Privileges
                    if (_dataSourceRow != null && _allowRecordUpdate) Update();
                }
            };
            _treeView.Vadjustment.ValueChanged += delegate { UpdatePages(); };
            _treeView.Vadjustment.Changed += delegate { UpdatePages(); };
        }

        protected void InitUiDashBoard()
        {
            //Tree Containers
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            VBox vbox = new VBox(false, 1);


            //StatusBar
            if (_showStatusBar)
            {
                _statusbar = new Statusbar() { HasResizeGrip = false };
                _statusbar.Push(0, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_statusbar"));
            };

            //Navigator
            _navigator = new GenericTreeViewNavigator<T1, T2>(_sourceWindow, this, _navigatorMode);

            //Pack components
            viewport.Add(_treeView);
            scrolledWindow.Add(viewport);
            //Pack VBox
            vbox.PackStart(scrolledWindow, true, true, 0);
            if (_navigatorMode == GenericTreeViewNavigatorMode.Default) vbox.PackStart(_navigator, false, false, 0);
            if (_showStatusBar) vbox.PackStart(_statusbar, false, false, 0);

            //Final Pack      
            PackStart(vbox);

        }

        /// <summary>
        /// Add Dynamic columns TreeViewColumnProperty object to TreeViewColumn
        /// </summary>
        protected void AddColumns()
        {
            //Declare Vars
            Boolean assignValue;
            //Create Working currentCellRendererProperties
            CellRendererText currentCellRendererProperties;
            //Used to store current TreeViewColumProperty same as _columnProperties[i]
            GenericTreeViewColumnProperty currentTreeViewColumnProperty;
            //Used to Store all PropertyInfos of TreeViewColumnProperty Object 
            PropertyInfo[] pisTreeViewColumnProperties;
            //Used to Store Current TreeViewColumnProperty Value, Used as Source to Assign to Target
            System.Object pInfoValue;
            //Used to Store PropertyInfo of TreeViewColumn, Used as Target for Assign values
            PropertyInfo piTreeViewColumn;

            //Loop Custom TreeViewColumnProperty properties and extract PropertyInfos to work (Reflection)
            for (int i = 0; i < _columnProperties.Count; i++)
            {
                //Assign DefaultProperties CellRenderer
                currentCellRendererProperties = _columnProperties[i].CellRenderer;

                //Add a new Default TreeViewColumn
                if (_columnProperties[i].PropertyType == GenericTreeViewColumnPropertyType.Text)
                {
                    //Instantiate a new TreeViewColumn in TreeViewColumnProperty object
                    _columnProperties[i].Column = new TreeViewColumn(_columnProperties[i].Name, currentCellRendererProperties, "text", i) { Clickable = true };
                    _columnProperties[i].Column.Clicked += Column_Clicked;
                }
                //Add a new CheckBox TreeViewColumn, used for multiselection with CheckBoxs
                else if (_columnProperties[i].PropertyType == GenericTreeViewColumnPropertyType.CheckBox)
                {
                    CellRendererToggle currentCellRendererToggle = new CellRendererToggle() { Activatable = true };
                    //Toggle CheckBox
                    currentCellRendererToggle.Toggled += CurrentCellRendererToggle_Toggled;
                    _columnProperties[i].Column = new TreeViewColumn(_columnProperties[i].Name, currentCellRendererToggle, "active", i);
                }

                //Extract All property Infos from TreeViewColumnProperty Class to Loop and Check for Assigned Properties to assign to Column
                currentTreeViewColumnProperty = _columnProperties[i];
                pisTreeViewColumnProperties = typeof(GenericTreeViewColumnProperty).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                pInfoValue = new System.Object();
                //Loop TreeViewColumnProperty
                foreach (PropertyInfo pInfo in pisTreeViewColumnProperties)
                {
                    assignValue = false;
                    //Source value
                    pInfoValue = pInfo.GetValue(currentTreeViewColumnProperty, null);

                    if (pInfoValue != null)
                    {
                        //Source : TreeViewColumnProperty
                        //if (_debug) _log.Debug("Source Property:{0}, Value:{1}, Type:{2}", p.Name, tempSourceValue, tempSourceValue.GetType());

                        //Target : TreeViewColumn
                        piTreeViewColumn = _columnProperties[i].Column.GetType().GetProperty(pInfo.Name);
                        //if (piTreeViewColumn != null) _log.Debug(string.Format("piTreeViewColumn.GetValue[{0}]:[{1}]", pInfo.Name, piTreeViewColumn.GetValue(_columnProperties[i].Column, null).ToString()));

                        //Check if is Valid property and Assign assignValue to true, to post processing
                        switch (pInfoValue.GetType().Name)
                        {
                            case "Boolean":
                                assignValue = true;
                                break;
                            case "Int32":
                                if ((Int32)pInfoValue > 0) { assignValue = true; };
                                break;
                            case "Single":
                                if ((float)pInfoValue > 0) { assignValue = true; };
                                break;
                            case "String":
                                if ((String)pInfoValue != string.Empty) { assignValue = true; };
                                break;
                            case "FontDescription":
                                Label labelTitle = new Label(_columnProperties[i].Title);
                                labelTitle.Show();
                                labelTitle.ModifyFont((Pango.FontDescription)pInfoValue);
                                _columnProperties[i].Column.Widget = labelTitle;
                                break;
                            case "TreeViewColumn":
                                break;
                            case "CellRendererText":
                                break;
                            default:
                                //if (_debug) _log.Debug(string.Format("Undetected Column FieldProperty Type: [{0}]", pInfoValue.GetType().Name));
                                break;
                        }

                        if (assignValue && pInfo.Name != "Name" && piTreeViewColumn != null)
                        {
                            try
                            {
                                //if (_debug) _log.Debug("Name:{0} piTreeViewColumn.SetValue({1}, {2})", _columnProperties[i].Name, _columnProperties[i].Column, pInfoValue);
                                piTreeViewColumn.SetValue(_columnProperties[i].Column, pInfoValue);
                            }
                            catch (Exception ex)
                            {
                                _log.Error(string.Format("AddColumns(): {0}", ex.Message), ex);
                            }
                        }
                    }
                }

                //Only here we can change Column Title Label Font, After Column is Created "new TreeViewColumn"
                _columnProperties[i].Column.Widget.ModifyFont(_columnProperties[i].FontDescTitle);

                //Allways add Sort Column ID
                _columnProperties[i].Column.SortColumnId = i;

                //Now add Column to TreeView
                _treeView.AppendColumn(_columnProperties[i].Column);
            }
        }

        /// <summary>
        /// Convert a T2 DataSourceRow to Object Array of Values, used to Work in Insert, Append TreeView Models
        /// </summary>
        protected System.Object[] DataSourceRowToModelRow(T2 pDataSourceRow)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Used to Store all PropertyInfos of TreeViewColumnProperty Object 
            PropertyInfo[] pisTreeViewColumnProperties;
            //Used to store current TreeViewColumProperty same as _columnProperties[i]
            GenericTreeViewColumnProperty currentTreeViewColumnProperty;
            //Used to Store Current TreeViewColumnProperty Value
            System.Object pInfoValue;

            //Used to Store columnValues to Append/Insert to Model and to Store Return 
            System.Object[] columnValues = new System.Object[_columnProperties.Count];

            try
            {
                for (int i = 0; i < _columnProperties.Count; i++)
                {
                    pisTreeViewColumnProperties = typeof(GenericTreeViewColumnProperty).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                    String currentFieldName;
                    foreach (PropertyInfo pInfo in pisTreeViewColumnProperties)
                    {
                        //Default FieldName
                        currentFieldName = _columnProperties[i].Name;

                        currentTreeViewColumnProperty = _columnProperties[i];
                        //Get currentTreeViewColumnProperty Value ex Code, Designation, CreatedAt etc
                        pInfoValue = pInfo.GetValue(currentTreeViewColumnProperty, null);
                        //If is Name property and its not XPGuidObject Oid Field Process it
                        if (pInfo.Name == "Name" /*&& (string)pInfoValue != "Oid"*/)
                        {
                            //Skip Columns
                            if (_columnProperties[i].Name == "RowIndex")
                            {
                            }
                            //Non Skip Columns
                            else
                            {
                                //Query
                                if (_columnProperties[i].Query != null && _columnProperties[i].Query != string.Empty)
                                {
                                    columnValues[i] = ColumnPropertyGetQuery(
                                        _columnProperties[i].Query,
                                        DataSourceRowGetColumnValue(pDataSourceRow,
                                            GetGenericTreeViewColumnPropertyIndex(_columnProperties, "Oid")
                                       )
                                    );
                                }
                                //All Others
                                else
                                {
                                    //SHARED : Calls SubClass Implementations of GetDataRowColumnValue, to Get the Field Value
                                    columnValues[i] = DataSourceRowGetColumnValue(pDataSourceRow, i, currentFieldName);
                                }

                                //Use FormatProvider, if Defined for non Skip Columns
                                if (_columnProperties[i].FormatProvider != null)
                                {
                                    columnValues[i] = string.Format(_columnProperties[i].FormatProvider, "{0}", columnValues[i]);
                                };
                            };
                        };
                    };
                    //If Gets here with null value, ex XPOComboBox -- Indef -- assign "" to it to update tree liststore model, that dont work with nulls, cant update
                    if (columnValues[i] == null) columnValues[i] = string.Empty;
                };
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return columnValues;
        }

        /// <summary>
        /// Clear all model Records, Usefull to Clean Model and Manually inserts, very usefull for DataTable Mode, to Clean and Re-Init Model without Destroy TreeView 
        /// </summary>
        public void ClearDataModel()
        {
            //Initialize a New Fresh Model and Column Properties
            _listStoreModel = GenericTreeViewModel.InitModel(_columnProperties, _treeViewMode);
            //1) Add Model to TreeModelFilter
            _listStoreModelFilter = new TreeModelFilter(_listStoreModel, null);
            //2) Add FilterModel to Sort
            _listStoreModelFilterSort = new TreeModelSort(_listStoreModelFilter);
            //Require to ReAssign, if we change Reference it doesnt do Nothing
            _treeView.Model = _listStoreModelFilterSort;
            //Reset Current Row and Total Rows
            _currentRowIndex = -1;
            _totalRows = 0;
            //Reset Path And Iter
            _treePath = null;
            _treeIter = new TreeIter();
        }

        /// <summary>
        /// Insert DataSourceRow into TreeView LisStore Model : Helper for Insert(), usefull to create external inserts to Tree, Create a DataRow and Send to Object
        /// </summary>
        public void ModelRowInsert(T2 pDataSourceRow)
        {
            //Get TreeView Model Row Values
            System.Object[] modelValues = DataSourceRowToModelRow(pDataSourceRow);

            //Assign next RowIndex to modelValues, comes empty from XPGuidObjectToModelValues
            modelValues[0] = _totalRows;
            _totalRows++;

            //Append Values to Model
            _treeIterModel = _listStoreModel.AppendValues(modelValues);

            //Update ModelFilter from Changes in Model
            UpdateChildModelsAfterCRUDChanges();

            //Cursor Work - INSERT
            _currentRowIndex = Convert.ToInt16(modelValues[0]);

            //Force Update Current Cursor to _currentRowIndex - REQUIRED, to solve SORT and FILTER Cursor Problems
            //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
            _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTaskStopAtCurrentRowIndex));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Generic CRUD

        /// <summary>
        /// Insert Record
        /// </summary>
        public void Insert()
        {
            //Fire Event
            OnRecordBeforeInsert();

            if (!_skipRecordInsert && _allowRecordInsert)
            {
                try
                {
                    //Get a Fresh Object With Default Ready to INSERT
                    T2 newDataSourceRow = DataSourceRowGetNewRecord();

                    //Send New Record Reference to Dialog, will be changed in Dialog :)
                    if (ShowDialog(newDataSourceRow, DialogMode.Insert))
                    {
                        //SubClass Implementation : Insert DataSourceRow to DataSource, Currently only DataTable Implements It, XPO Dont Required
                        DataSourceRowInsert(newDataSourceRow);

                        ModelRowInsert(newDataSourceRow);

                        /* BLOCK CAN BE REMOVED 
                        //MOVED TO ModelRowInsert, to be Called from Outside, Creating Manually Records in Tree Model

                        //Get TreeView Model Row Values
                        System.Object[] modelValues = DataSourceRowToModelRow(newDataSourceRow);

                        //Assign next RowIndex to modelValues, comes empty from XPGuidObjectToModelValues
                        modelValues[0] = _totalRows;
                        _totalRows++;

                        //Append Values to Model
                        _treeIterModel = _listStoreModel.AppendValues(modelValues);

                        //Update ModelFilter from Changes in Model
                        UpdateChildModelsAfterCRUDChanges();

                        //Cursor Work - INSERT
                        _currentRowIndex = Convert.ToInt16(modelValues[0]);

                        //Force Update Current Cursor to _currentRowIndex - REQUIRED, to solve SORT and FILTER Cursor Problems
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTaskStopAtCurrentRowIndex));
                        */

                        //Assign current XPGuidObject
                        _dataSourceRow = newDataSourceRow;

                        //Fire Event 
                        OnRecordAfterInsert();

                        //Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), "***Record Inserted***");
                    };
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    //Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), "***Cant Insert Record***");
                }
            }
        }

        /// <summary>
        /// Update Selected Record With DataSourceRow Data
        /// </summary>
        public void Update()
        {
            Update(DialogMode.Update);
        }

        public void Update(DialogMode pDialogMode)
        {
            //Fire Events
            if (pDialogMode == DialogMode.Update) OnRecordBeforeUpdate();
            if (pDialogMode == DialogMode.Insert) OnRecordBeforeInsert();
            if (pDialogMode == DialogMode.View) OnRecordBeforeView();

            if (
                (pDialogMode == DialogMode.Update && _allowRecordUpdate && !_skipRecordUpdate) ||
                (pDialogMode == DialogMode.Insert && _allowRecordInsert && !_skipRecordInsert) ||
                (pDialogMode == DialogMode.View && _allowRecordView)
                )
            {
                if (ShowDialog(_dataSourceRow, pDialogMode))
                {
                    try
                    {
                        //Initialize modelValues from DataSourceRow
                        System.Object[] modelValues = DataSourceRowToModelRow(_dataSourceRow);

                        //_listStoreModel.SetValues(_treeIter, modelValues);
                        _listStoreModel.SetValues(_treeIterModel, modelValues);

                        //Required to Store _treePath to SetCursor after UpdateModelsAfterChanges()
                        _treeView.GetCursor(out _treePath, out _treeViewColumn);

                        //Update ModelFilter after changes in Base Model
                        UpdateChildModelsAfterCRUDChanges();

                        //Cursor Work - Must be After Assign UpdateModelsAfterChanges()
                        _treeView.SetCursor(_treePath, _treeViewColumn, false);
                        _treeView.ScrollToCell(_listStoreModelFilterSort.GetPath(_treeIter), _treeView.Columns[0], false, 0, 0);

                        //Fire Events
                        if (pDialogMode == DialogMode.Update) OnRecordAfterUpdate();
                        if (pDialogMode == DialogMode.Insert) OnRecordAfterInsert();
                        if (pDialogMode == DialogMode.View) OnRecordAfterView();

                        //Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), "***Record Updated***");
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex.Message, ex);
                        //Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), "***Cant Update Record ***");
                    }
                }
            }
        }

        public void Delete()
        {
            /* IN009250, IN009217, IN009216 */
            bool itemHasReferences = CheckItemForReferences();

            if (itemHasReferences) { return; }

            //Fire Event
            OnRecordBeforeDelete();

            if (!_skipRecordDelete && _allowRecordDelete)
            {
                ResponseType response = response = Utils.ShowMessageTouch(
                      GlobalApp.WindowBackOffice,
                      DialogFlags.DestroyWithParent | DialogFlags.Modal,
                      MessageType.Question,
                      ButtonsType.YesNo,
                      resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_delete_record"),
                      resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_delete_record"))
                    ;

                if (response == ResponseType.Yes)
                {
                    try
                    {
                        //Delete
                        DataSourceRowDelete(_dataSourceRow);

                        //FIX DOUBLEDELETE in Cloned Documents (Search for FIX DOUBLEDELETE) : Line Above Commented, Model.Remove inside DataSourceRowDelete for both modes XPO and DataTable
                        //_listStoreModel.Remove(ref _treeIterModel);

                        //Update ModelFilter after changes in Base Model
                        UpdateChildModelsAfterCRUDChanges();

                        //Fire Event
                        OnRecordAfterDelete();

                        //Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_information"), "***Record Deleted***");
                    }
                    catch (Exception ex)
                    {
                        _log.Error("void Delete() :: Class '" + _dataSourceRow.GetType().Name + "' : " + ex.Message, ex);
                        String message = string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_delete_record_constraint_violation_exception"), _dataSourceRow.GetType().Name);
                        Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), message);
                    }
                }
            }
        }
        /// <summary>
        /// Checks for database dependencies, based on types:
        /// - erp_customer
        /// - fin_articlefamily
        /// - fin_articlesubfamily
        /// - fin_article
        /// </summary>
        /// <returns></returns>
        private bool CheckItemForReferences()
        {
            try
            {


                bool registerHasReferences = false;

                String className = _dataSourceRow.GetType().Name;
                Guid oid = Guid.Empty;
                String code = String.Empty;
                int countResult = 0;

                switch (className)
                {
                    case "erp_customer":
                        /* erp_customer has Documents */
                        logicpos.datalayer.DataLayer.Xpo.erp_customer customer = (_dataSourceRow as logicpos.datalayer.DataLayer.Xpo.erp_customer);
                        oid = customer.Oid;
                        code = customer.Code.ToString();
                        countResult = (int)GlobalFramework.SessionXpo.Evaluate(typeof(logicpos.datalayer.DataLayer.Xpo.fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[EntityOid] = ?", oid));
                        break;

                    case "fin_articlefamily":
                        /* Family has subfamily */
                        logicpos.datalayer.DataLayer.Xpo.fin_articlefamily articleFamily = (_dataSourceRow as logicpos.datalayer.DataLayer.Xpo.fin_articlefamily);
                        oid = articleFamily.Oid;
                        code = articleFamily.Code.ToString();
                        countResult = (int)GlobalFramework.SessionXpo.Evaluate(typeof(logicpos.datalayer.DataLayer.Xpo.fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Family] = ?", oid));
                        break;

                    case "fin_articlesubfamily":
                        /* Subfamily has article */
                        logicpos.datalayer.DataLayer.Xpo.fin_articlesubfamily articleSubFamily = (_dataSourceRow as logicpos.datalayer.DataLayer.Xpo.fin_articlesubfamily);
                        oid = articleSubFamily.Oid;
                        code = articleSubFamily.Code.ToString();
                        countResult = (int)GlobalFramework.SessionXpo.Evaluate(typeof(logicpos.datalayer.DataLayer.Xpo.fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[SubFamily] = ?", oid));
                        break;

                    case "fin_article":
                        /* Has an article an invoice issued for it? */
                        logicpos.datalayer.DataLayer.Xpo.fin_article article = (_dataSourceRow as logicpos.datalayer.DataLayer.Xpo.fin_article);
                        oid = article.Oid;
                        code = article.Code;
                        countResult = (int)GlobalFramework.SessionXpo.Evaluate(typeof(logicpos.datalayer.DataLayer.Xpo.fin_documentfinancedetail), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Article] = ?", oid));
                        break;
                    case "pos_configurationplace":
                        /* Has an article an invoice issued for it? */
                        logicpos.datalayer.DataLayer.Xpo.pos_configurationplace configPlace = (_dataSourceRow as logicpos.datalayer.DataLayer.Xpo.pos_configurationplace);
                        oid = configPlace.Oid;
                        code = configPlace.Code.ToString();
                        if(code == "10")
                        {
                            countResult = 1;
                        }
                        //countResult = (int)GlobalFramework.SessionXpo.Evaluate(typeof(logicpos.datalayer.DataLayer.Xpo.pos_configurationplace), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Oid] = ?"));
                        break;
                    case "pos_configurationplacetable":
                        /* Has an article an invoice issued for it? */
                        logicpos.datalayer.DataLayer.Xpo.pos_configurationplacetable configPlaceTable = (_dataSourceRow as logicpos.datalayer.DataLayer.Xpo.pos_configurationplacetable);
                        oid = configPlaceTable.Oid;
                        code = configPlaceTable.Code.ToString();
                        if (code == "10")
                        {
                            countResult = 1;
                        }
                        //countResult = (int)GlobalFramework.SessionXpo.Evaluate(typeof(logicpos.datalayer.DataLayer.Xpo.pos_configurationplace), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Oid] = ?"));
                        break;

                    default:
                        break;
                }

                if ((int)countResult > 0)
                {
                    registerHasReferences = true;

                    _log.Error("void bool CheckItemForReferences() :: '" + _dataSourceRow.GetType().FullName + "' [" + oid + "] : " + resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_delete_record_constraint_violation_exception"));
                    Utils.ShowMessageTouch(_sourceWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_delete_record_show_referenced_record_message"), className, code));
                }
                return registerHasReferences;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
        }

        public virtual bool ShowDialog<T>(T pDataObject, Enums.Dialogs.DialogMode pDialogMode)
        {
            try
            {
                //Extra Protection to prevent Null Type Dialogs, if not Assigned in Constructor dont ShowDialog
                if (_dialogType == null) return false;

                //Dynamic create Abstract Dialog from Type With Constructor
                //object[] dialogConstructor = new object[4] { _sourceWindow, DialogFlags.DestroyWithParent, pDialogMode, pDataObject };
                object[] dialogConstructor = new object[5] { _sourceWindow, this, DialogFlags.DestroyWithParent, pDialogMode, pDataObject };

                //Work with Both Modes (BOBaseDialog|PosBaseDialogGenericTreeView), with Shared
                if (_dialogType.BaseType == typeof(BOBaseDialog))
                {
                    _dialog = (BOBaseDialog)Activator.CreateInstance(_dialogType, dialogConstructor);
                }
                else if (_dialogType.BaseType == typeof(PosBaseDialogGenericTreeView<T>))
                {
                    _dialog = (PosBaseDialogGenericTreeView<T>)Activator.CreateInstance(_dialogType, dialogConstructor);
                }

                //After Dialog Constructor
                _dialog.WindowPosition = WindowPosition.Center;

                //If DialogMode in View Mode, Disable All Widgets : Assign ReadOnly to All Fields
                if (pDialogMode == DialogMode.View)
                {
                    //Protection for Null CrudWidgetList, ex Non BackOffice Dialogs BOBaseDialog
                    if ((_dialog as BOBaseDialog) != null)
                    {
                        foreach (GenericCRUDWidgetXPO item in (_dialog as BOBaseDialog).CrudWidgetList)
                        {
                            item.Widget.Sensitive = false;
                        }
                    }
                }

                //Run Dialog
                ResponseType response = (ResponseType)_dialog.Run();

                //Handle Response
                if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
                {
                    _dialog.Destroy();
                    if (response == ResponseType.Ok)
                    {
                        // OnSave/Update column, must Update ResourceString Columns[?].ResourceString, else we see the resourceString Token and not the Value From Resources
                        // ex prefparam_report_footer_line1 and we need the "Value Report Footer Line 1", this is REQUIRED, else we need to refresh to update Column Tree Value
                        foreach (var column in _columnProperties)
                        {
                            if (column.ResourceString)
                            {
                                // Reflection : Get Property from Column.Name and Update its Value with reflection propertyInfo
                                // This is the Trick to update Column display ResourceString after we Change/Update records
                                string columnValue = pDataObject.GetType().GetProperty(column.Name).GetValue(pDataObject).ToString();
                                pDataObject.GetType().GetProperty(column.Name).SetValue(pDataObject, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], columnValue).ToString());
                                if (Debugger.IsAttached)
                                {
                                    _log.Debug($"GenericTreeView: Replaced ResourceString column name [{column.Name}] with value [{columnValue}] after Update Record...");
                                }
                                //IN009296 BackOffice - Mudar o idioma da aplicação
                                if (columnValue == "prefparam_culture")
                                {
                                    string getCultureFromDB;
                                    try
                                    {
                                        string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
                                        getCultureFromDB = GlobalFramework.SessionXpo.ExecuteScalar(sql).ToString();
                                    }
                                    catch
                                    {
                                        getCultureFromDB = GlobalFramework.Settings["customCultureResourceDefinition"];

                                    }
                                    if (!Utils.getCultureFromOS(getCultureFromDB))
                                    {
                                        GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(ConfigurationManager.AppSettings["customCultureResourceDefinition"]);
                                        GlobalFramework.Settings["customCultureResourceDefinition"] = ConfigurationManager.AppSettings["customCultureResourceDefinition"];
                                        CustomResources.UpdateLanguage(ConfigurationManager.AppSettings["customCultureResourceDefinition"]);
                                    }
                                    else
                                    {
                                        GlobalFramework.CurrentCulture = new System.Globalization.CultureInfo(getCultureFromDB);
                                        GlobalFramework.Settings["customCultureResourceDefinition"] = getCultureFromDB;
                                        CustomResources.UpdateLanguage(getCultureFromDB);
                                    }
                                    Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.Modal, new System.Drawing.Size(600, 400), MessageType.Warning, ButtonsType.Ok, CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_language"), string.Format(CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_culture_change"), GlobalFramework.Settings["customCultureResourceDefinition"]));

                                }
                                //IN009296 ENDS
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Navigator 

        protected void InitNavigatorPermissions()
        {
            //_navigator.buttonNextPage.Sensitive = _allowNavigate;
            //_navigator.buttonPrevPage.Sensitive = _allowNavigate;
            //_navigator.buttonFirstPage.Sensitive = _allowNavigate;
            //_navigator.buttonLastPage.Sensitive = _allowNavigate; 
            _navigator.ButtonInsert.Sensitive = _allowRecordInsert;
            _navigator.ButtonUpdate.Sensitive = _allowRecordUpdate;
            _navigator.ButtonDelete.Sensitive = _allowRecordDelete;
            _navigator.ButtonView.Sensitive = _allowRecordView;
            _navigator.ButtonNextRecord.Sensitive = _allowNavigate;
            _navigator.ButtonPrevRecord.Sensitive = _allowNavigate;
        }

        protected void UpdatePages()
        {
            //string output;
            _navigator.CurrentPage = (int)Math.Floor(_treeView.Vadjustment.Value / _treeView.Vadjustment.PageSize) + 1;
            _navigator.TotalPages = (int)Math.Floor(_treeView.Vadjustment.Upper / _treeView.Vadjustment.PageSize);
            if (_treeView.Model != null)
            {
                _navigator.TotalRecords = _treeView.Model.IterNChildren() - 1;
            }
            else
            {
                _navigator.TotalRecords = 0;
            };
            _navigator.UpdateButtons(_treeView);
            //output = string.Format("Value:{0} PageSize{1} Lower:{2} Upper:{3} currentPage/totalPages:{4}/{5}", _treeView.Vadjustment.Value, _treeView.Vadjustment.PageSize, _treeView.Vadjustment.Lower, _treeView.Vadjustment.Upper, _navigator.CurrentPage, _navigator.TotalPages);
            //if (_showStatusBar) _statusbar.Push(0, output);
            //_log.Debug(output);
        }

        public void FirstPage()
        {
            _treeView.Vadjustment.Value = _treeView.Vadjustment.Lower;
        }

        public void PrevPage()
        {
            if ((_treeView.Vadjustment.Value - _treeView.Vadjustment.PageSize) > _treeView.Vadjustment.Lower)
                _treeView.Vadjustment.Value -= _treeView.Vadjustment.PageSize;
            else
            {
                _treeView.Vadjustment.Value = _treeView.Vadjustment.Lower;
            }
        }

        public void NextPage()
        {
            if ((_treeView.Vadjustment.Value + _treeView.Vadjustment.PageSize) < _treeView.Vadjustment.Upper)
            {
                _treeView.Vadjustment.Value += _treeView.Vadjustment.PageSize;
            };
        }

        public void LastPage()
        {
            _treeView.Vadjustment.Value = (_navigator.TotalPages - 1) * _treeView.Vadjustment.PageSize;
        }

        public void PrevRecord()
        {
            try
            {
                _treePath.Prev();
                _treeView.SetCursor(_treePath, null, false);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void NextRecord()
        {
            try
            {
                _treePath.Next();
                _treeView.SetCursor(_treePath, null, false);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Models

        //Used to get get Current Row Values
        public object GetCurrentModelRowValue(int pColumnIndex)
        {
            return _listStoreModel.GetValue(_treeIterModel, pColumnIndex);
        }

        public object GetCurrentModelCheckBoxValue()
        {
            if (_treeViewMode == GenericTreeViewMode.CheckBox)
            {
                return _listStoreModel.GetValue(_treeIterModel, 1);
            }
            else
            {
                return null;
            }
        }

        //Used to GetValue when Detect a XPGuidObject Column Value ex Article, ChildName="Designation", Get Value from ChildName (Designation), and Not XPGuidObject Value
        public object GetXPGuidObjectChildValue(object pFieldValue, string pFieldName, string pChildName)
        {

            object fieldValue = null;

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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Cursor Work

        /// <summary>
        /// Set TreeView Cursor in First Row or Selected Record/Default Value
        /// </summary>
        protected void SetInitialCursorPosition()
        {
            //If _treePath is Null, Default when we dont have Initial Default Value, Assigned in FIRST ReIndex and count Rows, check TreeModelForEachTask method
            if (_treeIter.Equals(default(TreeIter)))
            {
                _listStoreModelFilterSort.GetIterFirst(out _treeIter);
                _treePath = _listStoreModelFilterSort.GetPath(_treeIter);
                _treeView.SetCursor(_treePath, null, false);
                //Fix: Require using use _sourceWindow.ExposeEvent to prevent scrollToCell move ActionButtons down problem (Occurs sometimes)
                //Required if (_treeView.Columns.Length > 0), to prevent close Dialog ExposeEvent
                _sourceWindow.ExposeEvent += delegate { if (_treeView.Columns.Length > 0) _treeView.ScrollToCell(_listStoreModelFilterSort.GetPath(_treeIter), _treeView.Columns[0], false, 0, 0); };
            }
            else
            {
                _treePath = _listStoreModel.GetPath(_treeIter);
                _treeView.SetCursor(_treePath, null, false);
                try
                {
                    //Fix: Require using use _sourceWindow.ExposeEvent to prevent scrollToCell move ActionButtons down problem (Occurs sometimes)
                    //Required if (_treeView.Columns.Length > 0), to prevent close Dialog ExposeEvent
                    _sourceWindow.ExposeEvent += delegate { if (_treeView.Columns.Length > 0) _treeView.ScrollToCell(_treePath, _treeView.Columns[0], false, 0, 0); };
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    _log.Error(string.Format("Error! Required sourceWindow for ExposeEvent, current value is sourceWindow: [{0}]", _sourceWindow));
                }
            }
        }

        /// <summary>
        /// Used when we change base listStoreModel, ex When we Change MODEL from CRUD etc, we need to Update ModelFilter and ModelFilterSort to be in SYNC with Model
        /// </summary>
        protected void UpdateChildModelsAfterCRUDChanges()
        {
            //Always ReIndex RowIndex Column to be in SYNC with 3 Models (Model, ModelFilter and ModelFilterOrder), required for DELETE Operations
            _totalRows = 0;
            _reindexRowIndex = 0;
            _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));

            //Assign ModelFilter from Model
            _listStoreModelFilter = new TreeModelFilter(_listStoreModel, null);

            //IMPORTANT NOTE: REQUIRED to Update TreeViewSearch.ListStoreModelFilter to the new REFERENCE, else we Lost REFERENCE, and we cant FILTER
            //IMPORTANT NOTE: in TreeViewSearch SETTER we ReAssign VisibleFunc again to the new REFERENCE ex.: _listStoreModelFilter.VisibleFunc = FilterTree;
            _navigator.TreeViewSearch.ListStoreModelFilter = _listStoreModelFilter;
            //ReFilter after Assign to TreeViewSearch
            _listStoreModelFilter.Refilter();

            //Assign ModelFilterSort from ModelFilter
            _listStoreModelFilterSort = new TreeModelSort(_listStoreModelFilter);

            //Restore old Sorting
            if (_columnSortIndicator) _listStoreModelFilterSort.SetSortColumnId(_columnSortColumnId, _columnSortType);

            //Assign Model to TreeView - Must be After Refilter
            _treeView.Model = _listStoreModelFilterSort;
        }

        /// <summary>
        /// Used ReIndex Rows and count TotalRows for Generated Models
        /// </summary>
        protected bool TreeModelForEachTask(TreeModel model, TreePath path, TreeIter iter)
        {
            //Always ReIndex RowIndex Column to be in SYNC with Both Models, required for DELETE Operations
            model.SetValue(iter, 0, _reindexRowIndex);
            _reindexRowIndex++;

            //Prepare _treeIter to method SetInitialCursorPosition();
            //If First TreeModelForEachTask ReIndex (_treePath == null), when we Init TreeView First Time
            if (_treePath == null)
            {
                Guid currentEachOid = new Guid(model.GetValue(iter, _modelFirstCustomFieldIndex).ToString());

                //Detect Initial Default Value
                if (_guidDefaultValue == currentEachOid)
                {
                    //First Time Assign _treeIter, used to First Time Assign curentRow/Initial Selected Value
                    _treeIter = iter;
                    //Assign _treePath this way the first record found is the one is used to be selected
                    _treePath = model.GetPath(iter);
                    _currentRowIndex = Convert.ToInt16(model.GetValue(iter, 0));
                }
            }

            _totalRows++;
            //false to continue TreeModelForEachTask
            return false;
        }

        /// <summary>
        /// Used to Stop in _currentRowIndex, and update _treeIter and _treePath, usefull for setcursor when InsertRecord
        /// </summary>
        protected bool TreeModelForEachTaskStopAtCurrentRowIndex(TreeModel model, TreePath path, TreeIter iter)
        {
            //Always ReIndex RowIndex Column to be in SYNC with Both Models, required for DELETE Operations
            if (_currentRowIndex == (int)model.GetValue(iter, 0))
            {
                //true to stop
                _treePath = path;
                _treeIter = iter;
                _treeView.SetCursor(_treePath, _treeView.Columns[0], false);
                _treeView.ScrollToCell(_listStoreModelFilterSort.GetPath(_treeIter), _treeView.Columns[0], false, 0, 0);
                return true;
            }
            else
            {
                //false to continue
                return false;
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Events

        //Detect cursor changes, from mouse or keyboard
        protected void _treeView_CursorChanged(object sender, EventArgs e)
        {
            TreeView treeView = (TreeView)sender;
            //(obj as TreeView).Selection
            TreeSelection selection = treeView.Selection;
            TreeModel model;
            String output = string.Empty;

            // The _treeIter will point to the selected row
            if (selection.GetSelected(out model, out _treeIter))
            {
                _currentRowIndex = Convert.ToInt16(_listStoreModelFilterSort.GetValue(_treeIter, 0));
                _treePath = model.GetPath(_treeIter);
                _navigator.CurrentRecord = Convert.ToInt16(_treePath.ToString());

                //Assign iter and path to ListStoreModel - Iters from both models MUST BE SINCRONIZED
                //string stringTreeIter = model.GetStringFromIter(_treeIter);
                _listStoreModel.GetIterFromString(out _treeIterModel, _currentRowIndex.ToString());
                _treePathModel = _listStoreModel.GetPath(_treeIterModel);

                //Update CurrentData from Childs Implementation T1 (XPGuidObject | DataRow)
                GetDataRow();

                if (_showStatusBar) _statusbar.Push(0, output);
            };

            UpdatePages();

            //Dont Delete this Comment Block
            //If in CheckBox mode Enable CheckBox when Click in Row
            //Warning must be before than Share Event, to have _markedCheckBoxs Updated before Capture event from Outside
            //Check the Checkbox Mode #2 : Check CheckBox when Click in Row - DISABLED - in Favor of Mode #1 - We can Only one method else one check and the other uncheck it 
            //if (_treeViewMode == GenericTreeViewMode.CheckBox) { ToggleCheckBox(_treePath); }

            //Check MODELS Sync from _listStoreModel <> _listStoreModelFilterSort
            //_log.Debug(string.Format("_currentRowIndex: [{0}/{1}]", _currentRowIndex, _totalRows));
            //_log.Debug(string.Format("_listStoreModelFilterSort CurrentValue(0): [{0}]", _listStoreModelFilterSort.GetValue(_treeIter, 0)));
            //_log.Debug(string.Format("_listStoreModelFilterSort CurrentValue(3): [{0}]", _listStoreModelFilterSort.GetValue(_treeIter, 3)));
            //_log.Debug(string.Format("_listStoreModel CurrentValue(0): [{0}]", _listStoreModel.GetValue(_treeIterModel, 0)));
            //_log.Debug(string.Format("_listStoreModel CurrentValue(3): [{0}]", _listStoreModel.GetValue(_treeIterModel, 3)));
            //_log.Debug(string.Empty);

            //Use fire/share event handler only if is used (!= null), else do nothing            
            if (CursorChanged != null) CursorChanged(this, e);
        }

        /// <summary>
        /// Store Current Clicked Column ID and Sort Order
        /// </summary>
        void Column_Clicked(object sender, EventArgs e)
        {
            TreeViewColumn column = (TreeViewColumn)sender;
            _columnSortColumnId = column.SortColumnId;
            _columnSortType = column.SortOrder;
            _columnSortIndicator = column.SortIndicator;
            //_log.Debug(string.Format("_columnSortColumnId: [{0}], _columnSortType: [{1}], _columnSortIndicator: [{2}]", _columnSortColumnId, _columnSortType, _columnSortIndicator));
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //CheckBox Mode

        //Check the Checkbox Mode #1 : Click in CheckBox
        private void CurrentCellRendererToggle_Toggled(object o, ToggledArgs args)
        {
            //Required to force call CursorChanged
            _treePath = new TreePath(args.Path);
            _treeView.SetCursor(_treePath, null, false);
            ToggleCheckBox(new TreePath(args.Path));
        }

        //Shared ToggleCheckBox 
        private void ToggleCheckBox(TreePath pTreePath)
        {
            //Required to get _treeIterModel from _currentRowIndex (Assigned in CursorChanged from ModelFilterOrder)
            if (_listStoreModel.GetIterFromString(out _treeIterModel, _currentRowIndex.ToString()))
            {
                //Update Model
                bool old = (bool)_listStoreModel.GetValue(_treeIterModel, _modelCheckBoxFieldIndex);
                if (!old) { _markedCheckBoxs++; } else { _markedCheckBoxs--; };
                _listStoreModel.SetValue(_treeIterModel, _modelCheckBoxFieldIndex, !old);

                //Use Child Implementations : Currently only used implemented in DataTable
                ToggleCheckBox(old);
            }

            //Fire Toggle Event
            OnCheckBoxToggled();
        }

        /// <summary>
        /// Uncheck all checked CheckBoxs
        /// </summary>
        public void UnCheckAll()
        {
            if (_treeViewMode == GenericTreeViewMode.CheckBox)
            {
                MarkedCheckBoxs = 0;
                _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_UnMarkCheckBoxs));
            }
        }

        private bool TreeModelForEachTask_UnMarkCheckBoxs(TreeModel model, TreePath path, TreeIter iter)
        {
            try
            {
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, _modelCheckBoxFieldIndex));
                if (itemChecked)
                {
                    model.SetValue(iter, _modelCheckBoxFieldIndex, false);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return false;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom Events

        private void OnCheckBoxToggled()
        {
            if (CheckBoxToggled != null) { CheckBoxToggled(this, EventArgs.Empty); }
        }

        private void OnRecordBeforeInsert()
        {
            if (RecordBeforeInsert != null) { RecordBeforeInsert(this, EventArgs.Empty); }
        }

        private void OnRecordBeforeUpdate()
        {
            if (RecordBeforeUpdate != null) { RecordBeforeUpdate(this, EventArgs.Empty); }
        }

        private void OnRecordBeforeDelete()
        {
            if (RecordBeforeDelete != null) { RecordBeforeDelete(this, EventArgs.Empty); }
        }

        private void OnRecordBeforeView()
        {
            if (RecordBeforeView != null) { RecordBeforeView(this, EventArgs.Empty); }
        }

        private void OnRecordAfterInsert()
        {
            if (RecordAfterInsert != null) { RecordAfterInsert(this, EventArgs.Empty); }
        }

        private void OnRecordAfterUpdate()
        {
            if (RecordAfterUpdate != null) { RecordAfterUpdate(this, EventArgs.Empty); }
        }

        private void OnRecordAfterDelete()
        {
            if (RecordAfterDelete != null) { RecordAfterDelete(this, EventArgs.Empty); }
        }

        private void OnRecordAfterView()
        {
            if (RecordAfterView != null) { RecordAfterView(this, EventArgs.Empty); }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Public Methods to Check if Event has a Listener from Outside

        public bool HasEventRecordBeforeInsert()
        {
            return RecordBeforeInsert != null;
        }

        public bool HasEventRecordBeforeUpdate()
        {
            return RecordBeforeUpdate != null;
        }

        public bool HasEventRecordBeforeDelete()
        {
            return RecordBeforeDelete != null;
        }

        public bool HasEventRecordAfterInsert()
        {
            return RecordAfterInsert != null;
        }

        public bool HasEventRecordAfterUpdate()
        {
            return RecordAfterUpdate != null;
        }

        public bool HasEventRecordAfterDelete()
        {
            return RecordAfterDelete != null;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Column Properties

        /// <summary>
        /// Get Column Properity Field Index from ColumnProperties List
        /// Can use a Alias with "Func<List<GenericTreeViewColumnProperty>, string, int> GetIndex = GenericTreeView.GetGenericTreeViewColumnPropertyIndex;"
        /// And Call it With "int index = GetIndex(_columnProperties, "Code");"
        /// </summary>
        /// <param name="pColumnProperties"></param>
        /// <param name="pFieldName"></param>
        /// <returns></returns>
        protected static int GetGenericTreeViewColumnPropertyIndex(List<GenericTreeViewColumnProperty> pColumnProperties, string pFieldName)
        {
            //int i = 0;
            //foreach (GenericTreeViewColumnProperty item in pColumnProperties)
            //{
            //    if (item.Name == pFieldName) return i;
            //    i++;
            //}
            //return -1;

            //Replaced With LINQ
            return pColumnProperties.FindIndex(item => item.Name == pFieldName);
        }

        /// <summary>
        /// Simple Method to Style Columns and Cells Font Sizes for Touch GenericTreeView
        /// </summary>
        public void FormatColumnPropertiesForTouch()
        {
            //Settings
            string fontGenericTreeViewSelectRecordColumnTitle = GlobalFramework.Settings["fontGenericTreeViewSelectRecordColumnTitle"];
            string fontGenericTreeViewSelectRecordColumn = GlobalFramework.Settings["fontGenericTreeViewSelectRecordColumn"];

            FormatColumnPropertiesForTouch(fontGenericTreeViewSelectRecordColumnTitle, fontGenericTreeViewSelectRecordColumn);
        }

        public void FormatColumnPropertiesForTouch(string pFontTitle, string pFontData)
        {
            //Prepare FontDesc
            Pango.FontDescription fontDescTitle = Pango.FontDescription.FromString(pFontTitle);
            Pango.FontDescription fontDescData = Pango.FontDescription.FromString(pFontData);

            //Assign FontDesc, Bypass Default Fonts
            foreach (var item in this.Columns)
            {
                //Modify Title Column Labels
                item.Column.Widget.ModifyFont(fontDescTitle);
                //Modify Column Data - Replace custom Cellrender Font
                item.CellRenderer.FontDesc = fontDescData;
            };
        }

        /// <summary>
        /// Query Property : Get Column Result Query, used in all modes (DataTable and Xpo) to get the ExecuteScalar value for Column
        /// </summary>
        /// <param name="Sql"></param>
        /// <returns></returns>
        public string ColumnPropertyGetQuery(string pSql, object pKey)
        {
            string result = string.Empty;
            try
            {
                string sql = string.Format(pSql, pKey);
                result = Convert.ToString(GlobalFramework.SessionXpo.ExecuteScalar(sql));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
            return result;
        }
    }
}
