using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.financial;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using System;
using System.Data;
using System.Drawing;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs.DocumentFinanceDialog;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //T1: DataSource (XPCollection|DataTable)
    //T2: DataSourceRow (XPGuidObject|DataRow)
    //T3: GenericTreeType
    class PosSelectRecordDialog<T1, T2, T3> : PosBaseDialog
        //Generic Types Constrained to Classes that Implement IGenericTreeView
        //where T : IGenericTreeView, new()
      where T3 : GenericTreeView<T1, T2>, new()
    {
        //UI
        private TouchButtonIconWithText _buttonOk;
        private TouchButtonIconWithText _buttonCancel;
        //Add button more for default dialog windows
        //private ResponseType _responseTypeLoadMoreDocuments = (ResponseType)80;
        //private ResponseType _responseTypeFilter = (ResponseType)81;

        //Public
        //Store GerericTreeView Type
        private T3 _genericTreeView;
        internal T3 GenericTreeView
        {
            get { return _genericTreeView; }
            set { _genericTreeView = value; }
        }
        private GenericTreeViewMode _genericTreeViewMode;
        public GenericTreeViewMode GenericTreeViewMode
        {
            get { return _genericTreeViewMode; }
            set { _genericTreeViewMode = value; }
        }
        private ActionAreaButtons _actionAreaButtons;
        public ActionAreaButtons ActionAreaButtons
        {
            get { return _actionAreaButtons; }
            set { _actionAreaButtons = value; }
        }

        //PublicEvents from GenericTreeview, Capure here to have access to Objects
        public event EventHandler CursorChanged;
        public event EventHandler CheckBoxToggled;

        //XPO Mode: Prepare GenericTreeView to Use in InitObject : Constructor OverLoad 2
        public PosSelectRecordDialog(
          Window pSourceWindow,
          DialogFlags pDialogFlags,
          string pWindowsTitle,
          Size pSize,
          XPGuidObject pDefaultValue,
          CriteriaOperator pXpoCriteria,
          GenericTreeViewMode pGenericTreeViewMode,
          ActionAreaButtons pActionAreaButtons
        )
            : base(pSourceWindow, pDialogFlags)
        {
            try
            {
                //We can use InitObject Here because we dont have pColumnProperties, pDataSource and pDialogType
                //We must create instance with Activator to use Generic parameter constructors, and Send it to InitObject
                _genericTreeView = (T3)Activator.CreateInstance(typeof(T3), new object[] {
                    pSourceWindow,
                    pDefaultValue,
                    pXpoCriteria,
                    null,          //DialogType
                    pGenericTreeViewMode,
                    GenericTreeViewNavigatorMode.HideToolBar
                });

                //InitObject
                InitObject(pSourceWindow, pDialogFlags, pWindowsTitle, pSize, _genericTreeView, pGenericTreeViewMode, pActionAreaButtons);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //DataTable Mode: Prepare GenericTreeView to Use in InitObject : Constructor OverLoad 2
        public PosSelectRecordDialog(
            Window pSourceWindow,
            DialogFlags pDialogFlags,
            string pWindowsTitle,
            Size pSize,
            //Type pTreeViewType, 
            DataRow pDefaultValue,
            //CriteriaOperator pXpoCriteria, //Absent in DataTable Mode: To Implement in future, if Required
            //Type pDialogType,              //Absent in DataTable Mode: To Implement in future, if Required
            GenericTreeViewMode pGenericTreeViewMode,
            ActionAreaButtons pActionAreaButtons
        )
            : base(pSourceWindow, pDialogFlags)
        {
            //We can use InitObject Here because we dont have pColumnProperties, pDataSource and pDialogType
            //We must create instance with Activator to use Generic parameter constructors, and Send it to InitObject
            try
            {
                _genericTreeView = (T3)Activator.CreateInstance(typeof(T3), new object[]
                {
                    pSourceWindow,
                    pDefaultValue, 
                    //null,         //Not Used in DataTable
                    null,           //DialogType
                    pGenericTreeViewMode,
                    GenericTreeViewNavigatorMode.HideToolBar
                });

                //InitObject
                InitObject(pSourceWindow, pDialogFlags, pWindowsTitle, pSize, _genericTreeView, pGenericTreeViewMode, pActionAreaButtons);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        public void InitObject(
            Window pSourceWindow,
            DialogFlags pDialogFlags,
            string pWindowsTitle,
            Size pSize,
            //Type pTreeViewType, 
            //CriteriaOperator pCriteria, 
            T3 pGenericTreeView,
            GenericTreeViewMode pGenericTreeViewMode,
            ActionAreaButtons pActionAreaButtons
        )
        {
            //Init private Vars from Parameters
            String windowTitle = pWindowsTitle;
            Size windowSize = pSize;// new Size(900, 700);
            _genericTreeView = pGenericTreeView;
            _genericTreeViewMode = pGenericTreeViewMode;
            _actionAreaButtons = (pActionAreaButtons != null) ? pActionAreaButtons : GetDefaultActionAreaButtons();
            //_actionAreaButtons = pActionAreaButtons;

            //Init Local Vars
            String fileDefaultWindowIcon = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\Windows\icon_window_select_record.png");
            Size usefullAreaSize = new Size(windowSize.Width - 14, windowSize.Height - 124);

            //Modify default genericTreeView properties
            _genericTreeView.SetSizeRequest(usefullAreaSize.Width, usefullAreaSize.Height);
            _genericTreeView.AllowRecordUpdate = false;
            //Format Columns FontSizes for Touch
            _genericTreeView.FormatColumnPropertiesForTouch();


            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(_genericTreeView, 0, 0);

            //Events
            this.KeyReleaseEvent += PosSelectRecordDialog_KeyReleaseEvent;

            //Capture EventHandlers from GenericTreeView
            _genericTreeView.CursorChanged += _genericTreeView_CursorChanged;
            _genericTreeView.CheckBoxToggled += _genericTreeView_CheckBoxToggled;

            //Init Object
            this.InitObject(this, pDialogFlags, fileDefaultWindowIcon, windowTitle, windowSize, fixedContent, _genericTreeView.Navigator.TreeViewSearch, _actionAreaButtons);
        }

        public ActionAreaButtons GetDefaultActionAreaButtons()
        {
            //string fileActionMore = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_more.png");
            //string fileActionFilter = FrameworkUtils.OSSlash(GlobalFramework.Path["images"] + @"Icons\icon_pos_filter.png");
            //TouchButtonIconWithText _buttonMore = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.More, "touchButtonMore_Grey", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_more, SettingsApp.PaginationRowsPerPage), fileActionMore);
            //TouchButtonIconWithText _buttonFilter = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Filter, "touchButtonMore_Green", string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_button_label_filter, SettingsApp.PaginationRowsPerPage), fileActionFilter);
            //_buttonMore.Clicked += _genericTreeView_ButtonMoreClicked;
            
            //Default ActionArea Buttons
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //actionAreaButtons.Add(new ActionAreaButton(_buttonMore, _responseTypeLoadMoreDocuments));
            //actionAreaButtons.Add(new ActionAreaButton(_buttonFilter, _responseTypeFilter));
            actionAreaButtons.Add(new ActionAreaButton(_buttonOk, ResponseType.Ok));
            actionAreaButtons.Add(new ActionAreaButton(_buttonCancel, ResponseType.Cancel));
            return actionAreaButtons;
        }

        void _genericTreeView_CursorChanged(object sender, EventArgs e)
        {
            if (CursorChanged != null) CursorChanged(sender, e);

            //If button more clicked
            if (_genericTreeView.Navigator.TreeViewSearch.Button_MoreResponse())
            {
                _genericTreeView.Navigator.TreeViewSearch.flagMore = false;
                _genericTreeView.Navigator.TreeViewSearch.Button_FilterResponse();
                GenericTreeView.CurrentPageNumber++;
                Respond((int)DialogResponseType.LoadMore);
            }

            //If filter more clicked
            if (_genericTreeView.Navigator.TreeViewSearch.Button_FilterResponse())
            {
                _genericTreeView.Navigator.TreeViewSearch.flagFilter = false;
                _genericTreeView.Navigator.TreeViewSearch.Button_FilterResponse();
                Respond((int)DialogResponseType.Filter);
            }

            //if Default Action Area Buttons with buttonOk, else we catch event outside and enable buttons outside
            if (_buttonOk != null && _genericTreeView.DataSourceRow != null) _buttonOk.Sensitive = true;
        }

        //public void _genericTreeView_ButtonMoreClicked(object sender, EventArgs e)
        //{
        //    GenericTreeView.CurrentPageNumber++;
        //    GenericTreeView.DataSource.TopReturnedObjects = (SettingsApp.PaginationRowsPerPage * _currentPage);
        //    GenericTreeView.Refresh();
        //}

        void _genericTreeView_CheckBoxToggled(object sender, EventArgs e)
        {
            if (CheckBoxToggled != null) CheckBoxToggled(sender, e);
        }

        void PosSelectRecordDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            //Required a valid _buttonOk Refrence to Work, Some Dialogs dont Have _buttonOk, Have Other Actions
            if (args.Event.Key == Gdk.Key.Return && (_buttonOk != null && _buttonOk.Sensitive))
            {
                Respond(ResponseType.Ok);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Static Helper to Get a DataTable of CheckBoxs from a Query

        //Log4Net
        private static log4net.ILog _logStatic = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Class Members Related to This Gelper Method
        private static PosSelectRecordDialog<T1, T2, T3> _dialogSelectRecord;
        private static DataTable _resultDataTable;

        //Todo
        //Improve in future to Work with XPOObjects Too, when needed, already wotk with generics, only need minor changes to XPObjects

        //T1: DataSource (XPCollection|DataTable)
        //T2: DataSourceRow (XPGuidObject|DataRow)
        //T3: GenericTreeType
        public static DataTable GetSelected(Window pSourceWindow)
        {
            //Default ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            buttonOk.Sensitive = false;
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonOk = new ActionAreaButton(buttonOk, ResponseType.Ok);
            ActionAreaButton actionAreaButtonCancel = new ActionAreaButton(buttonCancel, ResponseType.Cancel);
            actionAreaButtons.Add(actionAreaButtonOk);
            actionAreaButtons.Add(actionAreaButtonCancel);

            _dialogSelectRecord =
              new PosSelectRecordDialog<T1, T2, T3>(
                pSourceWindow,
                DialogFlags.DestroyWithParent,
                resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_select_record"),
                GlobalApp.MaxWindowSize,
                null, //pDefaultValue : Require to Send a DataRow
                GenericTreeViewMode.CheckBox,
                actionAreaButtons
              );

            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            _dialogSelectRecord.CheckBoxToggled += delegate
            {
                //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
                if (_dialogSelectRecord.GenericTreeViewMode == GenericTreeViewMode.Default)
                {
                    //DataTableMode else use XPGuidObject
                    if (_dialogSelectRecord.GenericTreeView.DataSourceRow != null) actionAreaButtonOk.Button.Sensitive = true;
                }
                else if (_dialogSelectRecord.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                {
                    actionAreaButtonOk.Button.Sensitive = (_dialogSelectRecord.GenericTreeView.MarkedCheckBoxs > 0) ? true : false;

                    //This Code may be Usefull in a near future to Update TitleBar
                    //Get Indexes
                    //int indexColumnCheckBox = (_dialogSelectRecord.GenericTreeView.DataSource as DataTable).Columns.IndexOf("CheckBox");
                    //int indexColumnDesignation = (_dialogSelectRecord.GenericTreeView.DataSource as DataTable).Columns.IndexOf("Designation");
                    //Update Dialog Title
                    //bool itemChecked = (bool)(_dialogSelectRecord.GenericTreeView.DataSourceRow as DataRow).ItemArray[indexColumnCheckBox];
                    //string designation = (string)(_dialogSelectRecord.GenericTreeView.DataSourceRow as DataRow).ItemArray[indexColumnDesignation];
                    //_dialogSelectRecord.WindowTitle = 
                    //    (_dialogSelectRecord.GenericTreeView.MarkedCheckBoxs > 0) 
                    //    ? string.Format("{0} : MarkedCheckBoxs:[{1}] : Last:[{2}]", resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_select_record, _dialogSelectRecord.GenericTreeView.MarkedCheckBoxs, designation) 
                    //    : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_select_record
                    //;
                }
            };

            //Events
            _dialogSelectRecord.Response += dialogSelectRecord_Response;

            //Call Dialog
            int response = _dialogSelectRecord.Run();
            //Always Destroy Dialog
            _dialogSelectRecord.Destroy();

            //Return DataTable with Selectd (One) or CheckItems, or Empty DataTable
            if (response == -6)
            {
                _resultDataTable = new DataTable();
            }

            return _resultDataTable;
        }

        private static void dialogSelectRecord_Response(object o, ResponseArgs args)
        {
            PosSelectRecordDialog<T1, T2, T3>
              dialog = (PosSelectRecordDialog<T1, T2, T3>)o;

            if (args.ResponseId != ResponseType.Cancel)
            {
                if (args.ResponseId == ResponseType.Ok)
                {
                    //Init _resultDataTable, Clone Structure from _dialogSelectRecord.GenericTreeView.DataSource
                    _resultDataTable = (_dialogSelectRecord.GenericTreeView.DataSource as DataTable).Clone();

                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GenericTreeViewMode.Default)
                    {
                        //use dialog.GenericTreeView.DataTableRow.ItemArray
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GenericTreeViewMode.CheckBox)
                    {
                        //Required to use ListStoreModel and not ListStoreModelFilterSort, we only loop the visible filtered rows, and not The hidden Checked Rows
                        dialog.GenericTreeView.ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));
                    }
                }
            }
        }

        private static bool TreeModelForEachTask(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexIndex = 0;
            int columnIndexCheckBox = 1;
            //Reference Alias Only
            DataTable dataTable = (_dialogSelectRecord.GenericTreeView.DataSource as DataTable);

            try
            {
                Int32 itemIndex = Convert.ToInt32(model.GetValue(iter, columnIndexIndex).ToString());
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));

                if (itemChecked)
                {
                    //_logStatic.Debug(string.Format("{0}:{1}:{2}", itemIndex, itemChecked, new Guid(dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")].ToString())));
                    _resultDataTable.Rows.Add(dataTable.Rows[itemIndex].ItemArray);
                }
            }
            catch (Exception ex)
            {
                _logStatic.Error(ex.Message, ex);
            }
            return false;
        }
    }
}
