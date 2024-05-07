using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.Classes.Gui.Gtk.Widgets.Buttons;
using System;
using System.Data;
using LogicPOS.Settings.Extensions;
using LogicPOS.Globalization;

namespace logicpos.Classes.Gui.Gtk.Samples
{
    //Simple example of how to use one dialog with CheckBox TreeViews(DataTable Mode)
    //
    //How to Use Call with
    //UsingSelectRecordTreeViewWithCheckBox usingSelectRecordTreeViewWithCheckBox = new UsingSelectRecordTreeViewWithCheckBox(this);
    //DataTable dataTable = usingSelectRecordTreeViewWithCheckBox.SelectRecordDialog();
    //_logger.Debug(string.Format("dataTable.Rows.Count: [{0}]", dataTable.Rows.Count));

    internal class UsingSelectRecordTreeViewWithCheckBox
    {
        //Log4Net
        private readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Window _sourceWindow;
        private PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries> _dialogSelectRecord;
        private DataTable _resultDataTable = new DataTable();

        //Constructor
        public UsingSelectRecordTreeViewWithCheckBox(Window pSourceWindow)
        {
            _sourceWindow = pSourceWindow;
        }

        public DataTable SelectRecordDialog()
        {
            //Default ActionArea Buttons
            TouchButtonIconWithText buttonOk = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Ok);
            TouchButtonIconWithText buttonCancel = ActionAreaButton.FactoryGetDialogButtonType(PosBaseDialogButtonType.Cancel);
            //ActionArea Buttons
            ActionAreaButtons actionAreaButtons = new ActionAreaButtons();
            //Add references to Send to Event CursorChanged
            ActionAreaButton actionAreaButtonOk = new ActionAreaButton(buttonOk, ResponseType.Ok);
            ActionAreaButton actionAreaButtonCancel = new ActionAreaButton(buttonCancel, ResponseType.Cancel);
            actionAreaButtons.Add(actionAreaButtonOk);
            actionAreaButtons.Add(actionAreaButtonCancel);

            _dialogSelectRecord =
              new PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries>(
                _sourceWindow,
                DialogFlags.DestroyWithParent,
                CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_select_record"),
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
                    actionAreaButtonOk.Button.Sensitive = (_dialogSelectRecord.GenericTreeView.MarkedCheckBoxs > 0);

                    //Get Indexes
                    int indexColumnCheckBox = _dialogSelectRecord.GenericTreeView.DataSource.Columns.IndexOf("CheckBox");
                    int indexColumnDesignation = _dialogSelectRecord.GenericTreeView.DataSource.Columns.IndexOf("Designation");

                    //Update Dialog Title
                    bool itemChecked = (bool)_dialogSelectRecord.GenericTreeView.DataSourceRow.ItemArray[indexColumnCheckBox];
                    string designation = (string)_dialogSelectRecord.GenericTreeView.DataSourceRow.ItemArray[indexColumnDesignation];
                    _dialogSelectRecord.WindowTitle =
                        (_dialogSelectRecord.GenericTreeView.MarkedCheckBoxs > 0)
                        ? string.Format("{0} : MarkedCheckBoxs:[{1}] : Last:[{2}]", CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_select_record"), _dialogSelectRecord.GenericTreeView.MarkedCheckBoxs, designation)
                        : CultureResources.GetResourceByLanguage(LogicPOS.Settings.GeneralSettings.Settings.GetCultureName(), "window_title_dialog_select_record")
                    ;
                }
            };

            //Events
            _dialogSelectRecord.Response += dialogSelectRecord_Response;

            //Call Dialog
            int response = _dialogSelectRecord.Run();
            _dialogSelectRecord.Destroy();

            //Return DataTable with CheckItems
            return _resultDataTable;
        }

        private void dialogSelectRecord_Response(object o, ResponseArgs args)
        {
            PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries>
              dialog = (PosSelectRecordDialog<DataTable, DataRow, TreeViewTerminalSeries>)o;

            if (args.ResponseId != ResponseType.Cancel)
            {
                if (args.ResponseId == ResponseType.Ok)
                {
                    //Init _resultDataTable, Clone Structure from _dialogSelectRecord.GenericTreeView.DataSource
                    _resultDataTable = _dialogSelectRecord.GenericTreeView.DataSource.Clone();

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

        private bool TreeModelForEachTask(TreeModel model, TreePath path, TreeIter iter)
        {
            int columnIndexIndex = 0;
            int columnIndexCheckBox = 1;
            //Reference Alias Only
            DataTable dataTable = _dialogSelectRecord.GenericTreeView.DataSource;

            try
            {
                int itemIndex = Convert.ToInt32(model.GetValue(iter, columnIndexIndex).ToString());
                bool itemChecked = Convert.ToBoolean(model.GetValue(iter, columnIndexCheckBox));

                if (itemChecked)
                {
                    Guid itemGuid = new Guid(dataTable.Rows[itemIndex].ItemArray[dataTable.Columns.IndexOf("Oid")].ToString());
                    _logger.Debug(string.Format("{0}:{1}:{2}", itemIndex, itemChecked, itemGuid));
                    _resultDataTable.Rows.Add(dataTable.Rows[itemIndex].ItemArray);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return false;
        }
    }
}
