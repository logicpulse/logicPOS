using DevExpress.Data.Filtering;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Domain.Entities;
using LogicPOS.Settings;
using LogicPOS.UI.Buttons;
using LogicPOS.UI.Components;
using LogicPOS.UI.Dialogs;
using LogicPOS.Utility;
using System;
using System.Data;
using System.Drawing;

namespace logicpos.Classes.Gui.Gtk.Pos.Dialogs
{
    //T1: DataSource (XPCollection|DataTable)
    //T2: DataSourceRow (XPGuidObject|DataRow)
    //T3: GenericTreeType
    internal class PosSelectRecordDialog<T1, T2, T3> : BaseDialog
        //Generic Types Constrained to Classes that Implement IGenericTreeView
        //where T : IGenericTreeView, new()
      where T3 : GridView<T1, T2>, new()
    {
        //UI
        private IconButtonWithText _buttonOk;
        private IconButtonWithText _buttonCancel;

        internal T3 GenericTreeView { get; set; }
        public GridViewMode GenericTreeViewMode { get; set; }

        public ActionAreaButtons ActionAreaButtons { get; set; }

        //PublicEvents from GenericTreeview, Capure here to have access to Objects
        public event EventHandler CursorChanged;
        public event EventHandler CheckBoxToggled;

        //XPO Mode: Prepare GenericTreeView to Use in InitObject : Constructor OverLoad 2
        public PosSelectRecordDialog(
          Window parentWindow,
          DialogFlags pDialogFlags,
          string pWindowsTitle,
          Size pSize,
          Entity pDefaultValue,
          CriteriaOperator pXpoCriteria,
          GridViewMode pGenericTreeViewMode,
          ActionAreaButtons pActionAreaButtons
        )
            : base(parentWindow, pDialogFlags)
        {

            //We can use InitObject Here because we dont have pColumnProperties, pDataSource and pDialogType
            //We must create instance with Activator to use Generic parameter constructors, and Send it to InitObject
            GenericTreeView = (T3)Activator.CreateInstance(typeof(T3), new object[] {
                    parentWindow,
                    pDefaultValue,
                    pXpoCriteria,
                    null,          //DialogType
                    pGenericTreeViewMode,
                    GridViewNavigatorMode.HideToolBar
                });

            //InitObject
            InitObject(parentWindow, pDialogFlags, pWindowsTitle, pSize, GenericTreeView, pGenericTreeViewMode, pActionAreaButtons);

        }

        //DataTable Mode: Prepare GenericTreeView to Use in InitObject : Constructor OverLoad 2
        public PosSelectRecordDialog(
            Window parentWindow,
            DialogFlags pDialogFlags,
            string pWindowsTitle,
            Size pSize,
            //Type pTreeViewType, 
            DataRow pDefaultValue,
            //CriteriaOperator pXpoCriteria, //Absent in DataTable Mode: To Implement in future, if Required
            //Type pDialogType,              //Absent in DataTable Mode: To Implement in future, if Required
            GridViewMode pGenericTreeViewMode,
            ActionAreaButtons pActionAreaButtons
        )
            : base(parentWindow, pDialogFlags)
        {
            //We can use InitObject Here because we dont have pColumnProperties, pDataSource and pDialogType
            //We must create instance with Activator to use Generic parameter constructors, and Send it to InitObject

            GenericTreeView = (T3)Activator.CreateInstance(typeof(T3), new object[]
            {
                    parentWindow,
                    pDefaultValue, 
                    //null,         //Not Used in DataTable
                    null,           //DialogType
                    pGenericTreeViewMode,
                    GridViewNavigatorMode.HideToolBar
            });

            //InitObject
            InitObject(parentWindow, pDialogFlags, pWindowsTitle, pSize, GenericTreeView, pGenericTreeViewMode, pActionAreaButtons);
        }

        public void InitObject(
            Window parentWindow,
            DialogFlags pDialogFlags,
            string pWindowsTitle,
            Size pSize,
            //Type pTreeViewType, 
            //CriteriaOperator pCriteria, 
            T3 pGenericTreeView,
            GridViewMode pGenericTreeViewMode,
            ActionAreaButtons pActionAreaButtons
        )
        {
            //Init private Vars from Parameters
            string windowTitle = pWindowsTitle;
            Size windowSize = pSize;// new Size(900, 700);
            GenericTreeView = pGenericTreeView;
            GenericTreeViewMode = pGenericTreeViewMode;
            ActionAreaButtons = (pActionAreaButtons != null) ? pActionAreaButtons : GetDefaultActionAreaButtons();
            //_actionAreaButtons = pActionAreaButtons;

            //Init Local Vars
            string fileDefaultWindowIcon = PathsSettings.ImagesFolderLocation + @"Icons\Windows\icon_window_select_record.png";
            Size usefullAreaSize = new Size(windowSize.Width - 14, windowSize.Height - 124);

            //Modify default genericTreeView properties
            GenericTreeView.SetSizeRequest(usefullAreaSize.Width, usefullAreaSize.Height);
            GenericTreeView.AllowRecordUpdate = false;
            //Format Columns FontSizes for Touch
            GenericTreeView.FormatColumnPropertiesForTouch();


            //Init Content
            Fixed fixedContent = new Fixed();
            fixedContent.Put(GenericTreeView, 0, 0);

            //Events
            this.KeyReleaseEvent += PosSelectRecordDialog_KeyReleaseEvent;

            //Capture EventHandlers from GenericTreeView
            GenericTreeView.CursorChanged += _genericTreeView_CursorChanged;
            GenericTreeView.CheckBoxToggled += _genericTreeView_CheckBoxToggled;

            //Init Object
            this.Initialize(this,
                            pDialogFlags,
                            fileDefaultWindowIcon,
                            windowTitle,
                            windowSize,
                            fixedContent,
                            GenericTreeView.Navigator.TreeViewSearch,
                            ActionAreaButtons);
        }

        public ActionAreaButtons GetDefaultActionAreaButtons()
        {
            _buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            _buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            _buttonOk.Sensitive = false;

            ActionAreaButtons actionAreaButtons = new ActionAreaButtons
            {
                new ActionAreaButton(_buttonOk, ResponseType.Ok),
                new ActionAreaButton(_buttonCancel, ResponseType.Cancel)
            };
            return actionAreaButtons;
        }

        private void _genericTreeView_CursorChanged(object sender, EventArgs e)
        {
            CursorChanged?.Invoke(sender, e);

            if (GenericTreeView.Navigator.TreeViewSearch.ShowMoreButton)
            {
                GenericTreeView.Navigator.TreeViewSearch.ShowMoreButton = false;
                GenericTreeView.CurrentPageNumber++;
                Respond((int)DialogResponseType.LoadMore);
            }

            if (GenericTreeView.Navigator.TreeViewSearch.ShowFilterButton)
            {
                GenericTreeView.Navigator.TreeViewSearch.ShowFilterButton = false;
                Respond((int)DialogResponseType.Filter);
            }

            if (_buttonOk != null && GenericTreeView.Entity != null) _buttonOk.Sensitive = true;
        }

        private void _genericTreeView_CheckBoxToggled(object sender, EventArgs e)
        {
            CheckBoxToggled?.Invoke(sender, e);
        }

        private void PosSelectRecordDialog_KeyReleaseEvent(object o, KeyReleaseEventArgs args)
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
        private static readonly log4net.ILog _loggerStatic = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //Class Members Related to This Gelper Method
        private static PosSelectRecordDialog<T1, T2, T3> _dialogSelectRecord;
        private static DataTable _resultDataTable;

   
        public static DataTable GetSelected(Window parentWindow)
        {
            //Default ActionArea Buttons
            IconButtonWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Ok);
            buttonOk.Sensitive = false;
            IconButtonWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(DialogButtonType.Cancel);
            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonOk = new ActionAreaButton(buttonOk, ResponseType.Ok);
            ActionAreaButton actionAreaButtonCancel = new ActionAreaButton(buttonCancel, ResponseType.Cancel);
            actionAreaButtons.Add(actionAreaButtonOk);
            actionAreaButtons.Add(actionAreaButtonCancel);

            _dialogSelectRecord =
              new PosSelectRecordDialog<T1, T2, T3>(
                parentWindow,
                DialogFlags.DestroyWithParent,
                GeneralUtils.GetResourceByName("window_title_dialog_select_record"),
                GlobalApp.MaxWindowSize,
                null, //pDefaultValue : Require to Send a DataRow
                GridViewMode.CheckBox,
                actionAreaButtons
              );

            //CheckBox Capture CursorChanged/CheckBoxToggled Event, And enable/disable Buttons based on Valid Selection, Must be Here, Where we have a refence to Buttons
            _dialogSelectRecord.CheckBoxToggled += delegate
            {
                //Use inside delegate to have accesss to local references, ex dialogPartialPayment, actionAreaButtonOk
                if (_dialogSelectRecord.GenericTreeViewMode == GridViewMode.Default)
                {
                    //DataTableMode else use XPGuidObject
                    if (_dialogSelectRecord.GenericTreeView.Entity != null) actionAreaButtonOk.Button.Sensitive = true;
                }
                else if (_dialogSelectRecord.GenericTreeViewMode == GridViewMode.CheckBox)
                {
                    actionAreaButtonOk.Button.Sensitive = (_dialogSelectRecord.GenericTreeView.MarkedCheckBoxs > 0);

                    //This Code may be Usefull in a near future to Update TitleBar
                    //Get Indexes
                    //int indexColumnCheckBox = (_dialogSelectRecord.GenericTreeView.DataSource as DataTable).Columns.IndexOf("CheckBox");
                    //int indexColumnDesignation = (_dialogSelectRecord.GenericTreeView.DataSource as DataTable).Columns.IndexOf("Designation");
                    //Update Dialog Title
                    //bool itemChecked = (bool)(_dialogSelectRecord.GenericTreeView.DataSourceRow as DataRow).ItemArray[indexColumnCheckBox];
                    //string designation = (string)(_dialogSelectRecord.GenericTreeView.DataSourceRow as DataRow).ItemArray[indexColumnDesignation];
                    //_dialogSelectRecord.WindowTitle = 
                    //    (_dialogSelectRecord.GenericTreeView.MarkedCheckBoxs > 0) 
                    //    ? string.Format("{0} : MarkedCheckBoxs:[{1}] : Last:[{2}]", CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_select_record, _dialogSelectRecord.GenericTreeView.MarkedCheckBoxs, designation) 
                    //    : CultureResources.GetCustomResources(LogicPOS.Settings.CultureSettings.CurrentCultureName, "window_title_dialog_select_record
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
                    _resultDataTable = (_dialogSelectRecord.GenericTreeView.Entities as DataTable).Clone();

                    //Single Record Mode - Default - USED HERE ONLY TO TEST Both Dialogs Modes (Default and CheckBox)
                    if (dialog.GenericTreeViewMode == GridViewMode.Default)
                    {
                        //use dialog.GenericTreeView.DataTableRow.ItemArray
                    }
                    //Multi Record Mode - CheckBox - ACTIVE MODE
                    else if (dialog.GenericTreeViewMode == GridViewMode.CheckBox)
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
            DataTable dataTable = (_dialogSelectRecord.GenericTreeView.Entities as DataTable);

            try
            {
                int itemIndex = Convert.ToInt32(model.GetValue(iter, columnIndexIndex).ToString());
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));

                if (itemChecked)
                {
                    //_loggerStatic.Debug(string.Format("{0}:{1}:{2}", itemIndex, itemChecked, new Guid(dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")].ToString())));
                    _resultDataTable.Rows.Add(dataTable.Rows[itemIndex].ItemArray);
                }
            }
            catch (Exception ex)
            {
                _loggerStatic.Error(ex.Message, ex);
            }
            return false;
        }
    }
}
