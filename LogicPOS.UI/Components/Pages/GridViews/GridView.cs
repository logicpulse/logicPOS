using DevExpress.Data.Filtering;
using Gtk;
using logicpos;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Modules.StockManagement;
using LogicPOS.Settings;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace LogicPOS.UI.Components
{
    internal abstract class GridView<T1, T2> : Box, IGridView
    {
        protected Window _parentWindow;
        protected GridViewMode _treeViewMode;
        protected GridViewNavigatorMode _navigatorMode;
        protected int _currentRowIndex = 0;
        protected int _reindexRowIndex = 0;
        protected int _totalRows = 0;
        protected int _modelFirstCustomFieldIndex;
        protected int _modelCheckBoxFieldIndex = 1;
        protected Guid _guidDefaultValue;
        public TreeIter _treeIter;
        public TreePath _treePath;
        public TreeViewColumn _treeViewColumn;
        private SortType _columnSortType = SortType.Ascending;
        private int _columnSortColumnId = 0;
        private bool _columnSortIndicator = true;

        public Type DialogType { get; set; }
        public Dialog Dialog { get; set; }
        public TreeIter TreeIterModel;
        public TreePath TreePathModel { get; set; }
        public TreeView TreeView { get; set; }
        public GridViewNavigator<T1, T2> Navigator { get; set; }
        public ListStore ListStoreModel { get; set; }
        public TreeModelFilter ListStoreModelFilter { get; set; }
        public TreeModelSort ListStoreModelFilterSort { get; set; }
        public T1 Entities { get; set; }
        public T2 Entity { get; set; }
        public List<GridViewColumn> Columns { get; set; }
        public bool AllowNavigate { get; set; } = true;


        protected bool _allowRecordInsert = true;
        public bool AllowRecordInsert
        {
            get { return _allowRecordInsert; }
            set { _allowRecordInsert = value; Navigator.ButtonInsert.Sensitive = value; }
        }

        protected bool _allowRecordUpdate = true;
        public bool AllowRecordUpdate
        {
            get { return _allowRecordUpdate; }
            set { _allowRecordUpdate = value; Navigator.ButtonUpdate.Sensitive = value; }
        }

        protected bool _allowRecordDelete = true;
        public bool AllowRecordDelete
        {
            get { return _allowRecordDelete; }
            set { _allowRecordDelete = value; Navigator.ButtonDelete.Sensitive = value; }
        }

        protected bool _allowRecordView = true;
        public bool AllowRecordView
        {
            get { return _allowRecordView; }
            set { _allowRecordView = value; Navigator.ButtonView.Sensitive = value; }
        }

        protected bool SkipRecordInsert { get; set; } = false;
        protected bool SkipRecordUpdate { get; set; } = false;
        protected bool SkipRecordDelete { get; set; } = false;

        public int MarkedCheckBoxs { get; set; } = 0;

        protected bool _readOnly = false;
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _allowRecordInsert = !value;
                _allowRecordUpdate = !value;
                _allowRecordDelete = !value;
                Navigator.ButtonDelete.Sensitive = !value;
                Navigator.ButtonInsert.Sensitive = !value;
                Navigator.ButtonUpdate.Sensitive = !value;
                _readOnly = value;
            }
        }
        public int CurrentPageNumber { get; set; } = 1;

        public event EventHandler CursorChanged;
        public event EventHandler CheckBoxToggled;
        public event EventHandler RecordBeforeInsert;
        public event EventHandler RecordBeforeUpdate;
        public event EventHandler RecordBeforeDelete;
        public event EventHandler RecordBeforeView;
        public event EventHandler RecordAfterInsert;
        public event EventHandler RecordAfterUpdate;
        public event EventHandler RecordAfterDelete;
        public event EventHandler RecordAfterView;

        public GridView(Window parentWindow = null)
        {
            _parentWindow = parentWindow;
        }

     
        public abstract void InitObject(
          Window parentWindow,
          T2 pDefaultValue,
          GridViewMode pGenericTreeViewMode,
          GridViewNavigatorMode navigatorMode,
          List<GridViewColumn> pColumnProperties,
          T1 pDataSource,
          Type pDialogType
        );

        public abstract void InitDataModel(
          T1 pDataSource,
          List<GridViewColumn> pColumnProperties,
          GridViewMode pGenericTreeViewMode
        );

        public abstract void GetDataRow();

        public abstract object DataSourceRowGetColumnValue(T2 pDataSourceRow, int pColumnIndex, string pFieldName = "");

        public abstract T2 DataSourceRowGetNewRecord();

        public virtual void DataSourceRowInsert<T>(T pDataSourceRow) { }

        public virtual void DataSourceRowDelete<T>(T pDataSourceRow) { }

        public virtual void ToggleCheckBox(bool pOldValue) { }

        public virtual void Refresh() { }

        public virtual void DeleteRecords() { }

        protected void InitUI()
        {
            ScrolledWindow scrolledWindow = new ScrolledWindow();
            scrolledWindow.ShadowType = ShadowType.EtchedIn;
            scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            Viewport viewport = new Viewport() { ShadowType = ShadowType.None };
            VBox vbox = new VBox(false, 1);

            ListStoreModelFilter = new TreeModelFilter(ListStoreModel, null);
            ListStoreModelFilterSort = new TreeModelSort(ListStoreModelFilter);
            if (_columnSortIndicator) ListStoreModelFilterSort.SetSortColumnId(_columnSortColumnId, _columnSortType);
            TreeView = new TreeView(ListStoreModelFilterSort);

            TreeView.RulesHint = true;
            TreeView.ModifyBase(StateType.Active, new Gdk.Color(215, 215, 215));

            AddColumns();

            Navigator = new GridViewNavigator<T1, T2>(_parentWindow, this, _navigatorMode);

            viewport.Add(TreeView);
            scrolledWindow.Add(viewport);

            vbox.PackStart(scrolledWindow, true, true, 0);
            if (_navigatorMode == GridViewNavigatorMode.Default) vbox.PackStart(Navigator, false, false, 0);

            PackStart(vbox);

            TreeView.CursorChanged += _treeView_CursorChanged;
            TreeView.RowActivated += delegate
            {
                if (_treeViewMode == GridViewMode.CheckBox)
                {
                    ToggleCheckBox(_treePath);
                }
                else
                {
                    if (Entity != null && _allowRecordUpdate) Update();
                }
            };
            TreeView.Vadjustment.ValueChanged += delegate { UpdatePages(); };
            TreeView.Vadjustment.Changed += delegate { UpdatePages(); };
        }


        protected void AddColumns()
        {
            bool assignValue;
            CellRendererText currentCellRendererProperties;
            GridViewColumn currentTreeViewColumnProperty;
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
                pisTreeViewColumnProperties = typeof(GridViewColumn).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
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

        public object[] DataSourceRowToModelRow(T2 pDataSourceRow)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

            PropertyInfo[] pisTreeViewColumnProperties;
            GridViewColumn currentTreeViewColumnProperty;
            object pInfoValue;

            object[] columnValues = new object[Columns.Count];


            for (int i = 0; i < Columns.Count; i++)
            {
                pisTreeViewColumnProperties = typeof(GridViewColumn).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                string currentFieldName;
                foreach (PropertyInfo pInfo in pisTreeViewColumnProperties)
                {
                    currentFieldName = Columns[i].Name;

                    currentTreeViewColumnProperty = Columns[i];
                    pInfoValue = pInfo.GetValue(currentTreeViewColumnProperty, null);

                    if (pInfo.Name == "Name" /*&& (string)pInfoValue != "Oid"*/)
                    {
                        if (Columns[i].Name == "RowIndex")
                        {
                        }
                        else
                        {
                            if (Columns[i].Query != null && Columns[i].Query != string.Empty)
                            {
                                columnValues[i] = ColumnPropertyGetQuery(
                                    Columns[i].Query,
                                    DataSourceRowGetColumnValue(pDataSourceRow,
                                        GetGenericTreeViewColumnPropertyIndex(Columns, "Oid")
                                   )
                                );
                            }
                            else
                            {
                                columnValues[i] = DataSourceRowGetColumnValue(pDataSourceRow, i, currentFieldName);
                            }

                            if (Columns[i].FormatProvider != null)
                            {
                                columnValues[i] = string.Format(Columns[i].FormatProvider, "{0}", columnValues[i]);
                            };
                        };
                    };
                };
                if (columnValues[i] == null) columnValues[i] = string.Empty;
            };


            return columnValues;
        }

        public void ClearDataModel()
        {
            ListStoreModel = GridViewModel.InitModel(Columns, _treeViewMode);
            ListStoreModelFilter = new TreeModelFilter(ListStoreModel, null);
            ListStoreModelFilterSort = new TreeModelSort(ListStoreModelFilter);
            TreeView.Model = ListStoreModelFilterSort;
            _currentRowIndex = -1;
            _totalRows = 0;
            _treePath = null;
            _treeIter = new TreeIter();
        }

        public void ModelRowInsert(T2 pDataSourceRow)
        {
            object[] modelValues = DataSourceRowToModelRow(pDataSourceRow);

            modelValues[0] = _totalRows;
            _totalRows++;

            TreeIterModel = ListStoreModel.AppendValues(modelValues);

            UpdateChildModelsAfterCRUDChanges();

            _currentRowIndex = Convert.ToInt16(modelValues[0]);

            ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTaskStopAtCurrentRowIndex));
        }

        public void Insert()
        {
            OnRecordBeforeInsert();

            if (SkipRecordInsert == false && _allowRecordInsert)
            {

                T2 newDataSourceRow = DataSourceRowGetNewRecord();

                if (ShowDialog(newDataSourceRow, DialogMode.Insert))
                {
                    DataSourceRowInsert(newDataSourceRow);

                    ModelRowInsert(newDataSourceRow);
                    Entity = newDataSourceRow;

                    OnRecordAfterInsert();

                };

            }
        }

        public void Update()
        {
            Update(DialogMode.Update);
        }

        public void Update(DialogMode pDialogMode)
        {
            if (pDialogMode == DialogMode.Update) OnRecordBeforeUpdate();
            if (pDialogMode == DialogMode.Insert) OnRecordBeforeInsert();
            if (pDialogMode == DialogMode.View) OnRecordBeforeView();

            if (
                pDialogMode == DialogMode.Update && _allowRecordUpdate && !SkipRecordUpdate ||
                pDialogMode == DialogMode.Insert && _allowRecordInsert && !SkipRecordInsert ||
                pDialogMode == DialogMode.View && _allowRecordView
                )
            {
                if (ShowDialog(Entity, pDialogMode))
                {
                    object[] modelValues = DataSourceRowToModelRow(Entity);

                    ListStoreModel.SetValues(TreeIterModel, modelValues);

                    TreeView.GetCursor(out _treePath, out _treeViewColumn);

                    UpdateChildModelsAfterCRUDChanges();

                    TreeView.SetCursor(_treePath, _treeViewColumn, false);
                    TreeView.ScrollToCell(ListStoreModelFilterSort.GetPath(_treeIter), TreeView.Columns[0], false, 0, 0);

                    if (pDialogMode == DialogMode.Update) OnRecordAfterUpdate();
                    if (pDialogMode == DialogMode.Insert) OnRecordAfterInsert();
                    if (pDialogMode == DialogMode.View) OnRecordAfterView();

                }
            }
        }

        public void Delete()
        {
            bool itemHasReferences = CheckItemForReferences();

            if (itemHasReferences) { return; }

            OnRecordBeforeDelete();

            if (!SkipRecordDelete && _allowRecordDelete)
            {
                ResponseType response = response = Utils.ShowMessageTouch(
                      GlobalApp.BackOffice,
                      DialogFlags.DestroyWithParent | DialogFlags.Modal,
                      MessageType.Question,
                      ButtonsType.YesNo,
                      GeneralUtils.GetResourceByName("window_title_dialog_delete_record"),
                      GeneralUtils.GetResourceByName("dialog_message_delete_record"))
                    ;

                if (response == ResponseType.Yes)
                {
                    try
                    {
                        if (Entity.GetType() == typeof(fin_article))
                        {
                            (Entity as fin_article).Disabled = true;
                            (Entity as fin_article).DeletedAt = DateTime.Now;
                            (Entity as fin_article).Designation = Utils.RandomString();
                            (Entity as fin_article).Code = Utils.RandomString();
                            (Entity as fin_article).Save();

                        }
                        else if (Entity.GetType() == typeof(fin_warehouse))
                        {
                            (Entity as fin_warehouse).Disabled = true;
                            (Entity as fin_warehouse).DeletedAt = DateTime.Now;
                            (Entity as fin_warehouse).Save();


                        }
                        else if (Entity.GetType() == typeof(fin_articlewarehouse))
                        {
                            (Entity as fin_articlewarehouse).Disabled = true;
                            (Entity as fin_articlewarehouse).DeletedAt = DateTime.Now;
                            (Entity as fin_articlewarehouse).Save();
                        }
                        else
                        {
                            DataSourceRowDelete(Entity);

                        }

                        UpdateChildModelsAfterCRUDChanges();

                        OnRecordAfterDelete();

                        Refresh();
                    }
                    catch (Exception ex)
                    {
                        string message = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_constraint_violation_exception"), Entity.GetType().Name);
                        Utils.ShowMessageTouch(_parentWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), message);
                    }
                }
            }
        }
      
        private bool CheckItemForReferences()
        {
            try
            {
                bool registerHasReferences = false;

                string className = Entity.GetType().Name;
                Guid oid = Guid.Empty;
                string code = string.Empty;
                int countResult = 0;

                switch (className)
                {
                    case "erp_customer":
                        erp_customer customer = Entity as erp_customer;
                        oid = customer.Oid;
                        code = string.Format("fin_documentfinancemaster");

                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[EntityOid] = ?", oid));
                        break;

                    case "fin_articlefamily":
                        fin_articlefamily articleFamily = Entity as fin_articlefamily;
                        oid = articleFamily.Oid;
                        code = "fin_articlesubfamily";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Family] = ?", oid));
                        break;

                    case "fin_articlesubfamily":
                        fin_articlesubfamily articleSubFamily = Entity as fin_articlesubfamily;
                        oid = articleSubFamily.Oid;
                        code = "fin_article";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[SubFamily] = ?", oid));
                        break;

                    case "fin_article":
                        fin_article article = Entity as fin_article;
                        oid = article.Oid;
                        code = "fin_article";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancedetail), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Article] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlecomposition), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Article] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articleserialnumber), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Article] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlecomposition), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[ArticleChild] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlecomposition), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Article] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlestock), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Article] = ?", oid));
                        break;

                    case "fin_warehouse":
                        fin_warehouse warehouse = Entity as fin_warehouse;
                        oid = warehouse.Oid;
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_warehouselocation), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Warehouse] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlewarehouse), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Warehouse] = ?", oid));
                        break;

                    case "fin_articleclass":
                        fin_articleclass articleClass = Entity as fin_articleclass;
                        oid = articleClass.Oid;
                        code = "fin_article";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Class] = ?", oid));
                        break;

                    case "fin_documentfinancetype":
                        fin_documentfinancetype docType = Entity as fin_documentfinancetype;
                        oid = docType.Oid;
                        code = "fin_documentfinancemaster / fin_documentfinancepayment";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[DocumentType] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[DocumentType] = ?", oid));
                        break;
                    case "pos_configurationplace":
                        pos_configurationplace configPlace = Entity as pos_configurationplace;
                        oid = configPlace.Oid;
                        code = configPlace.Code.ToString();
                        if (code == "10")
                        {
                            countResult = 1;
                            code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record"));
                            break;
                        }
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplacetable), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Place] = ?", oid));
                        code = "pos_configurationplacetable";
                        break;

                    case "pos_configurationplacetable":
                        pos_configurationplacetable configPlaceTable = Entity as pos_configurationplacetable;
                        oid = configPlaceTable.Oid;
                        code = configPlaceTable.Code.ToString();
                        if (code == "10")
                        {
                            countResult = 1;
                            code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record"));
                            break;
                        }
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_documentordermain), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[PlaceTable] = ?"));
                        code = "fin_documentordermain";
                        break;

                    case "pos_configurationplaceterminal":
                        pos_configurationplaceterminal terminal = Entity as pos_configurationplaceterminal;
                        oid = terminal.Oid;
                        code = terminal.Code.ToString();
                        
                        if (TerminalSettings.LoggedTerminal.Oid == oid)
                        {
                            countResult = 1;
                            code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record")); ;
                            break;
                        }
                       
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CreatedWhere] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancepayment), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CreatedWhere] = ?", oid));
                        code = "fin_documentfinancemaster / fin_documentfinancepayment";
                        break;

                    case "sys_configurationprinterstemplates":
                        sys_configurationprinterstemplates printerTemplate = Entity as sys_configurationprinterstemplates;
                        oid = printerTemplate.Oid;
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Template] = ?", oid));
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Template] = ?", oid));
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_articlefamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Template] = ?", oid));
                        code = "fin_article / fin_articlesubfamily / fin_articlefamily";
                        break;

                    case "sys_configurationprinters":
                        sys_configurationprinters printer = Entity as sys_configurationprinters;
                        oid = printer.Oid;
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Printer] = ?", oid));
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Printer] = ?", oid));
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_articlefamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Printer] = ?", oid));
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancetype), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Printer] = ?", oid));
                        if (!printer.PrinterType.ThermalPrinter)
                        {
                            countResult += (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplaceterminal), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Printer] = ?", oid));
                        }
                        else
                        {
                            countResult += (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplaceterminal), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[ThermalPrinter] = ?", oid));
                        }
                        code = "fin_article / fin_articlesubfamily / fin_articlefamily / pos_configurationplaceterminal";
                        break;

                    case "sys_configurationpoledisplay":
                        sys_configurationpoledisplay poleDisplay = Entity as sys_configurationpoledisplay;
                        oid = poleDisplay.Oid;
                        code = "pos_configurationplaceterminal";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplaceterminal), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[PoleDisplay] = ?", oid));
                        break;

                    case "sys_configurationweighingmachine":
                        sys_configurationweighingmachine weighingMachine = Entity as sys_configurationweighingmachine;
                        oid = weighingMachine.Oid;
                        code = "pos_configurationplaceterminal"; countResult = (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplaceterminal), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[WeighingMachine] = ?", oid));
                        break;

                    case "sys_configurationinputreader":
                        sys_configurationinputreader inputReader = Entity as sys_configurationinputreader;
                        oid = inputReader.Oid;
                        code = "pos_configurationplaceterminal"; countResult = (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplaceterminal), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[BarcodeReader] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(pos_configurationplaceterminal), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CardReader] = ?", oid));
                        break;

                    case "sys_userprofile":
                        sys_userprofile userProfile = Entity as sys_userprofile;
                        oid = userProfile.Oid;
                        code = "sys_userdetail / sys_userpermissionprofile";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(sys_userdetail), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[Profile] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(sys_userpermissionprofile), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[UserProfile] = ?", oid));
                        break;

                    case "erp_customertype":
                        erp_customertype customerType = Entity as erp_customertype;
                        oid = customerType.Oid;
                        code = customerType.Code.ToString();
                        if (code == "10")
                        {
                            countResult = 1;
                            code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record")); ;
                            break;
                        }
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(erp_customer), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CustomerType] = ?", oid));
                        code = "erp_customer";
                        break;

                    case "fin_configurationpaymentcondition":
                        fin_configurationpaymentcondition cfgPaymentContition = Entity as fin_configurationpaymentcondition;
                        oid = cfgPaymentContition.Oid;
                        code = cfgPaymentContition.Code.ToString();
                        
                        if (code == "10")
                        {
                            countResult = 1;
                            code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record")); ;
                            break;
                        }
                        
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[PaymentCondition] = ?", oid));
                        code = "fin_documentfinancemaster";
                        break;

                    case "fin_configurationvatrate":
                        fin_configurationvatrate cfgVateRate = Entity as fin_configurationvatrate;
                        oid = cfgVateRate.Oid;
                        code = cfgVateRate.Code.ToString();
                        
                        if (code == "10")
                        {
                            countResult = 1;
                            code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record"));
                            break;
                        }
                       
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[VatOnTable] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[VatDirectSelling] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[VatOnTable] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[VatDirectSelling] = ?", oid));
                        code = "fin_article / fin_articlesubfamily";
                        break;

                    case "fin_configurationvatexemptionreason":
                        
                        fin_configurationvatexemptionreason cfgVateExReason = Entity as fin_configurationvatexemptionreason;
                        oid = cfgVateExReason.Oid;
                        code = cfgVateExReason.Code.ToString();
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[VatExemptionReason] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_documentorderdetail), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[VatExemptionReason] = ?", oid));
                        code = "fin_article / fin_documentorderdetail";
                        break;

                    case "fin_configurationpaymentmethod":
                        fin_configurationpaymentmethod payMethod = Entity as fin_configurationpaymentmethod;
                        oid = payMethod.Oid;
                        code = payMethod.Code.ToString();
                        //If payment Method = Numerario/MB/CD/Cheque/CC/ContaCorrente
                        switch (code)
                        {
                            case "10":
                            case "20":
                            case "30":
                            case "80":
                            case "90":
                            case "300":
                                countResult = 1;
                                code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record"));
                                break;
                            default: break;
                        }
                        /* If payment Method is referenced on documents */
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_documentfinancemaster), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[PaymentMethod] = ?", oid));
                        code = "fin_documentfinancemaster";
                        break;

                    case "sys_userdetails":
                        /* If user details is referenced on system print */
                        sys_userdetail userDetail = Entity as sys_userdetail;
                        oid = userDetail.Oid;
                        code = "sys_systemprint";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(sys_systemprint), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[UserDetail] = ?", oid));
                        break;

                    case "pos_usercommissiongroup":
                        /* If user commission group is referenced on articles / articles family / articles sub-family */
                        pos_usercommissiongroup commissionGrp = Entity as pos_usercommissiongroup;
                        oid = commissionGrp.Oid;
                        code = "fin_article / fin_articlesubfamily / fin_articlefamily";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CommissionGroup] = ?", oid));
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_articlefamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CommissionGroup] = ?", oid));
                        countResult += (int)XPOSettings.Session.Evaluate(typeof(fin_articlesubfamily), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[CommissionGroup] = ?", oid));
                        break;

                    case "erp_customerdiscountgroup":
                        /* If customer discount group is referenced on articles  */
                        erp_customerdiscountgroup discountGrp = Entity as erp_customerdiscountgroup;
                        oid = discountGrp.Oid;
                        code = "fin_article";
                        countResult = (int)XPOSettings.Session.Evaluate(typeof(fin_article), CriteriaOperator.Parse("Count()"), CriteriaOperator.Parse("[DiscountGroup] = ?", oid));
                        break;

                    case "cfg_configurationcountry":
                    case "cfg_configurationcurrency":
                    case "cfg_configurationholidays":
                    case "cfg_configurationunitmeasure":
                    case "cfg_configurationunitsize":
                        /* Tables protected records */
                        code = string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_protected_record"));
                        countResult = 1;
                        break;

                    default:
                        break;
                }
                if (countResult > 0)
                {
                    registerHasReferences = true;

                    Utils.ShowMessageTouch(_parentWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_error"), string.Format(GeneralUtils.GetResourceByName("dialog_message_delete_record_show_referenced_record_message"), className, code));
                }
                return registerHasReferences;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public virtual bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode)
        {

            if (DialogType == null) return false;


            object[] dialogConstructor = new object[5] { _parentWindow, this, DialogFlags.DestroyWithParent, pDialogMode, pDataObject };

            if (DialogType.BaseType == typeof(EditDialog))
            {
                Dialog = (EditDialog)Activator.CreateInstance(DialogType, dialogConstructor);
            }
            else if (DialogType.BaseType == typeof(PosBaseDialogGenericTreeView<T>))
            {
                Dialog = (PosBaseDialogGenericTreeView<T>)Activator.CreateInstance(DialogType, dialogConstructor);
            }
            else if (DialogType == typeof(PosArticleStockDialog))
            {
                ProcessArticleStockParameter res = PosArticleStockDialog.GetProcessArticleStockParameter(_parentWindow);

                if (res != null)
                {
                    if (res.ArticleCollectionSimple.Count > 0)
                    {
                        foreach (var item in res.ArticleCollectionSimple)
                        {
                            res.Quantity = item.Value;
                            res.Article = item.Key;
                            ProcessArticleStock.Add(ProcessArticleStockMode.In, res);
                        }
                        return true;
                    }
                }
                else return false;

            }

            //After Dialog Constructor
            Dialog.WindowPosition = WindowPosition.Center;

            //If DialogMode in View Mode, Disable All Widgets : Assign ReadOnly to All Fields
            if (pDialogMode == DialogMode.View)
            {
                //Protection for Null CrudWidgetList, ex Non BackOffice Dialogs BOBaseDialog
                if (Dialog as EditDialog != null)
                {
                    foreach (GenericCRUDWidgetXPO item in (Dialog as EditDialog).InputFields)
                    {
                        item.Widget.Sensitive = false;
                    }
                }
            }

            //Run Dialog
            ResponseType response = (ResponseType)Dialog.Run();

            //Handle Response
            if (response == ResponseType.Ok || response == ResponseType.Cancel || response == ResponseType.DeleteEvent)
            {
                Dialog.Destroy();
                if (response == ResponseType.Ok)
                {
                    foreach (var column in Columns)
                    {
                        if (column.ResourceString)
                        {

                            string columnValue = pDataObject.GetType().GetProperty(column.Name).GetValue(pDataObject).ToString();
                            pDataObject.GetType().GetProperty(column.Name).SetValue(pDataObject, CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, columnValue).ToString());

                            if (columnValue == "prefparam_culture")
                            {
                                string cultureFromDb;
                                try
                                {
                                    string sql = "SELECT value FROM cfg_configurationpreferenceparameter where token = 'CULTURE';";
                                    cultureFromDb = XPOSettings.Session.ExecuteScalar(sql).ToString();
                                }
                                catch
                                {
                                    cultureFromDb = CultureSettings.CurrentCultureName;

                                }
                                if (CultureSettings.OSHasCulture(cultureFromDb) == false)
                                {
                                    CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(AppSettings.Instance.customCultureResourceDefinition);
                                    CultureResources.UpdateLanguage(AppSettings.Instance.customCultureResourceDefinition);
                                }
                                else
                                {
                                    CultureSettings.CurrentCulture = new System.Globalization.CultureInfo(cultureFromDb);
                                    AppSettings.Instance.customCultureResourceDefinition = cultureFromDb;
                                    CultureResources.UpdateLanguage(cultureFromDb);
                                }
                                Utils.ShowMessageBox(GlobalApp.BackOffice, DialogFlags.Modal, new System.Drawing.Size(600, 400), MessageType.Warning, ButtonsType.Ok, GeneralUtils.GetResourceByName("global_language"), string.Format(GeneralUtils.GetResourceByName("dialog_message_culture_change"), CultureSettings.CurrentCultureName));

                            }
                            //IN009296 ENDS
                        }
                    }
                    if (_parentWindow.GetType() == typeof(DialogArticleStock))
                    {
                        (_parentWindow as DialogArticleStock).TreeViewXPO_ArticleDetails.Refresh();
                        (_parentWindow as DialogArticleStock).TreeViewXPO_ArticleHistory.Refresh();
                        (_parentWindow as DialogArticleStock).TreeViewXPO_ArticleWarehouse.Refresh();
                        (_parentWindow as DialogArticleStock).TreeViewXPO_StockMov.Refresh();
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

        protected void InitNavigatorPermissions()
        {

            Navigator.ButtonInsert.Sensitive = _allowRecordInsert;
            Navigator.ButtonUpdate.Sensitive = _allowRecordUpdate;
            Navigator.ButtonDelete.Sensitive = _allowRecordDelete;
            Navigator.ButtonView.Sensitive = _allowRecordView;
            Navigator.ButtonNextRecord.Sensitive = AllowNavigate;
            Navigator.ButtonPrevRecord.Sensitive = AllowNavigate;
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

        public void PrevRecord()
        {
            _treePath.Prev();
            TreeView.SetCursor(_treePath, null, false);
        }

        public void NextRecord()
        {
            _treePath.Next();
            TreeView.SetCursor(_treePath, null, false);
        }

        public object GetCurrentModelCheckBoxValue()
        {
            if (_treeViewMode == GridViewMode.CheckBox)
            {
                return ListStoreModel.GetValue(TreeIterModel, 1);
            }
            else
            {
                return null;
            }
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

        protected void SetInitialCursorPosition()
        {
            //If _treePath is Null, Default when we dont have Initial Default Value, Assigned in FIRST ReIndex and count Rows, check TreeModelForEachTask method
            if (_treeIter.Equals(default(TreeIter)))
            {
                ListStoreModelFilterSort.GetIterFirst(out _treeIter);
                _treePath = ListStoreModelFilterSort.GetPath(_treeIter);
                TreeView.SetCursor(_treePath, null, false);
                //Fix: Require using use _sourceWindow.ExposeEvent to prevent scrollToCell move ActionButtons down problem (Occurs sometimes)
                //Required if (_treeView.Columns.Length > 0), to prevent close Dialog ExposeEvent
                _parentWindow.ExposeEvent += delegate { if (TreeView.Columns.Length > 0) TreeView.ScrollToCell(ListStoreModelFilterSort.GetPath(_treeIter), TreeView.Columns[0], false, 0, 0); };
            }
            else
            {
                _treePath = ListStoreModel.GetPath(_treeIter);
                TreeView.SetCursor(_treePath, null, false);

                _parentWindow.ExposeEvent += delegate { if (TreeView.Columns.Length > 0) TreeView.ScrollToCell(_treePath, TreeView.Columns[0], false, 0, 0); };

            }
        }

        protected void UpdateChildModelsAfterCRUDChanges()
        {
            //Always ReIndex RowIndex Column to be in SYNC with 3 Models (Model, ModelFilter and ModelFilterOrder), required for DELETE Operations
            _totalRows = 0;
            _reindexRowIndex = 0;
            ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));

            //Assign ModelFilter from Model
            ListStoreModelFilter = new TreeModelFilter(ListStoreModel, null);

            //IMPORTANT NOTE: REQUIRED to Update TreeViewSearch.ListStoreModelFilter to the new REFERENCE, else we Lost REFERENCE, and we cant FILTER
            //IMPORTANT NOTE: in TreeViewSearch SETTER we ReAssign VisibleFunc again to the new REFERENCE ex.: _listStoreModelFilter.VisibleFunc = FilterTree;
            Navigator.TreeViewSearch.Filter = ListStoreModelFilter;
            //ReFilter after Assign to TreeViewSearch
            ListStoreModelFilter.Refilter();

            //Assign ModelFilterSort from ModelFilter
            ListStoreModelFilterSort = new TreeModelSort(ListStoreModelFilter);

            //Restore old Sorting
            if (_columnSortIndicator) ListStoreModelFilterSort.SetSortColumnId(_columnSortColumnId, _columnSortType);

            //Assign Model to TreeView - Must be After Refilter
            TreeView.Model = ListStoreModelFilterSort;
        }

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

        protected bool TreeModelForEachTaskStopAtCurrentRowIndex(TreeModel model, TreePath path, TreeIter iter)
        {
            //Always ReIndex RowIndex Column to be in SYNC with Both Models, required for DELETE Operations
            if (_currentRowIndex == (int)model.GetValue(iter, 0))
            {
                //true to stop
                _treePath = path;
                _treeIter = iter;
                TreeView.SetCursor(_treePath, TreeView.Columns[0], false);
                TreeView.ScrollToCell(ListStoreModelFilterSort.GetPath(_treeIter), TreeView.Columns[0], false, 0, 0);
                return true;
            }
            else
            {
                //false to continue
                return false;
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

        public void UnCheckAll()
        {
            if (_treeViewMode == GridViewMode.CheckBox)
            {
                MarkedCheckBoxs = 0;
                ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask_UnMarkCheckBoxs));
            }
        }

        private bool TreeModelForEachTask_UnMarkCheckBoxs(TreeModel model, TreePath path, TreeIter iter)
        {
            bool itemChecked = Convert.ToBoolean(model.GetValue(iter, _modelCheckBoxFieldIndex));
            if (itemChecked)
            {
                model.SetValue(iter, _modelCheckBoxFieldIndex, false);
            }

            return false;
        }

        private void OnCheckBoxToggled()
        {
            CheckBoxToggled?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordBeforeInsert()
        {
            RecordBeforeInsert?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordBeforeUpdate()
        {
            RecordBeforeUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordBeforeDelete()
        {
            RecordBeforeDelete?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordBeforeView()
        {
            RecordBeforeView?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordAfterInsert()
        {
            RecordAfterInsert?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordAfterUpdate()
        {
            RecordAfterUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordAfterDelete()
        {
            RecordAfterDelete?.Invoke(this, EventArgs.Empty);
        }

        private void OnRecordAfterView()
        {
            RecordAfterView?.Invoke(this, EventArgs.Empty);
        }

        public bool HasEventRecordBeforeUpdate()
        {
            return RecordBeforeUpdate != null;
        }

        public bool HasEventRecordBeforeDelete()
        {
            return RecordBeforeDelete != null;
        }

        protected static int GetGenericTreeViewColumnPropertyIndex(List<GridViewColumn> pColumnProperties, string pFieldName)
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

        public void FormatColumnPropertiesForTouch()
        {
            //Settings
            string fontGenericTreeViewSelectRecordColumnTitle = AppSettings.Instance.fontGenericTreeViewSelectRecordColumnTitle;
            string fontGenericTreeViewSelectRecordColumn = AppSettings.Instance.fontGenericTreeViewSelectRecordColumn.ToString();

            FormatColumnPropertiesForTouch(fontGenericTreeViewSelectRecordColumnTitle, fontGenericTreeViewSelectRecordColumn);
        }

        public void FormatColumnPropertiesForTouch(string pFontTitle, string pFontData)
        {
            //Prepare FontDesc
            Pango.FontDescription fontDescTitle = Pango.FontDescription.FromString(pFontTitle);
            Pango.FontDescription fontDescData = Pango.FontDescription.FromString(pFontData);

            //Assign FontDesc, Bypass Default Fonts
            foreach (var item in Columns)
            {
                //Modify Title Column Labels
                item.Column.Widget.ModifyFont(fontDescTitle);
                //Modify Column Data - Replace custom Cellrender Font
                item.CellRenderer.FontDesc = fontDescData;
            };
        }

        public string ColumnPropertyGetQuery(string pSql, object pKey)
        {
            string sql = string.Format(pSql, pKey);
            var result = Convert.ToString(XPOSettings.Session.ExecuteScalar(sql));

            return result;
        }
    }
}
