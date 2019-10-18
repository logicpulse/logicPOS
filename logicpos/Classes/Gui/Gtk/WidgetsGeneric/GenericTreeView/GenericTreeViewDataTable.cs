using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericTreeViewDataTable : GenericTreeView<DataTable, DataRow>
    {
        //Public Parametless Constructor Required by Generics
        public GenericTreeViewDataTable() { }

        public GenericTreeViewDataTable(Window pSourceWindow)
            : base(pSourceWindow)
        {
            _sourceWindow = pSourceWindow;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //IGenericTreeViewMode Implementation

        //Object Initializer
        public override void InitObject(
          Window pSourceWindow,
          DataRow pDefaultValue,
          GenericTreeViewMode pGenericTreeViewMode,
          GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode,
          List<GenericTreeViewColumnProperty> pColumnProperties,
          DataTable pDataSource,
          Type pDialogType
        )
        {
            if (_debug) _log.Debug("InitObject Begin(" + pSourceWindow + "," + pDataSource + "," + pColumnProperties + "," + pGenericTreeViewMode + "," + pGenericTreeViewNavigatorMode);

            //Parameters
            _sourceWindow = pSourceWindow;
            _dataSource = pDataSource;
            if (_dataSource.Rows.Count > 0) _dataSourceRow = _dataSource.Rows[_currentRowIndex];
            _guidDefaultValue = default(Guid);
            _dialogType = pDialogType;
            _columnProperties = pColumnProperties;
            _treeViewMode = pGenericTreeViewMode;
            _navigatorMode = pGenericTreeViewNavigatorMode;

            //Get First Custom Field Position ex OID
            _modelFirstCustomFieldIndex = (_treeViewMode == GenericTreeViewMode.Default) ? 1 : 2;

            //InitDataModel
            InitDataModel(_dataSource, _columnProperties, _treeViewMode);

            //ReIndex and count Rows
            _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));

            //Initialize UI
            if (_debug) _log.Debug("InitObject Before InitUI");
            InitUI();

            //Update Navigator Permissions
            InitNavigatorPermissions();

            //Change Navigator
            _navigator.ButtonRefresh.Visible = false;
            _navigator.ButtonRefresh.Destroy();

            //Always have a valid cursor, in first Record or in pDefaultValue
            SetInitialCursorPosition();

            if (_debug) _log.Debug("InitObject End");
        }

        public override void InitDataModel(DataTable pDataSource, List<GenericTreeViewColumnProperty> pColumnProperties, GenericTreeViewMode pGenericTreeViewMode)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Parameters
            DataTable dataTable = pDataSource;
            List<GenericTreeViewColumnProperty> _columnProperties = pColumnProperties;
            GenericTreeViewMode _treeViewMode = pGenericTreeViewMode;

            //Initialize Model and Column Properties
            ListStore model = GenericTreeViewModel.InitModel(_columnProperties, pGenericTreeViewMode);

            //Init ColumnValues Object Array
            System.Object[] columnValues = new System.Object[_columnProperties.Count];

            //Start Render Model Values from Collection
            String fieldName;

            //Always add Index to Rows, NOT USED: Check if existe before Add to prevent Refresh Add Again
            dataTable.Columns.Add("Index", typeof(Int32)).SetOrdinal(0);//Put in Index 0, First Column

            //If Detect CheckBox Mode, Insert CheckBox Column in DataTable
            if (_treeViewMode == GenericTreeViewMode.CheckBox)
            {
                //NOT USED: Check if existe before Add to prevent Refresh Add Again "if (dataTable.Columns.IndexOf("CheckBox") < 0)"
                dataTable.Columns.Add("CheckBox", typeof(Boolean)).SetOrdinal(1);//Put in Index 1, After Index
            }

            //Loop Records
            Int32 rowIndex = -1;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                //Increase RowIndex
                rowIndex++;

                //Loop Fields : Generate Abstract Values to use in Model
                for (int i = 0; i < _columnProperties.Count; i++)
                {
                    fieldName = _columnProperties[i].Name;

                    //If is in first Field (Index) assign rowIndex to Model and DataRow
                    if (fieldName == "RowIndex")
                    {
                        columnValues[i] = rowIndex;
                        dataRow[i] = rowIndex;
                    }
                    //Query
                    else if (_columnProperties[i].Query != null && _columnProperties[i].Query != string.Empty)
                    {
                        columnValues[i] = ColumnPropertyGetQuery(_columnProperties[i].Query, dataRow.ItemArray[i]);
                    }
                    //If detect XPGuidObject Value Type (Value is a XPObject, Child Object), Get Value from its Chield Field (Related Table)
                    else if (dataRow.ItemArray[i] != null && dataRow.ItemArray[i].GetType().BaseType == typeof(XPGuidObject))
                    {
                        columnValues[i] = GetXPGuidObjectChildValue(dataRow.ItemArray[i], fieldName, _columnProperties[i].ChildName);
                    }
                    //If is second field Assign false to checkbox to Model and DataRow
                    else if (fieldName == "RowCheckBox")
                    {
                        columnValues[i] = false;
                        dataRow[i] = false;
                    }
                    //Else All others Fields assign Value from dataRow
                    else
                    {
                        columnValues[i] = FrameworkUtils.FormatDataTableFieldFromType(dataRow.ItemArray[i].ToString(), dataRow.ItemArray[i].GetType().Name);
                        //Format String using Column FormatProvider              
                        if (_columnProperties[i].FormatProvider != null)
                        {
                            columnValues[i] = string.Format(_columnProperties[i].FormatProvider, "{0}", columnValues[i]);
                        }
                    }
                }

                //Add Column Value to Model
                model.AppendValues(columnValues);
            }

            _listStoreModel = model;
        }

        /// <summary>
        /// Get Current DataRow Object
        /// </summary>
        /// <returns></returns>
        public override void GetDataRow()
        {
            _dataSourceRow = _dataSource.Rows[_currentRowIndex];
        }

        /// <summary>
        /// Get Current DataRow Column Value, usefull to work in DataSourceRowToModelRow
        /// </summary>
        /// <param name="DataSourceRow"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns>Column Value</returns>
        public override object DataSourceRowGetColumnValue(DataRow pDataSourceRow, int pColumnIndex, string pFieldName = "")
        {
            String fieldName = pFieldName;
            object fieldValue = null;

            if (pDataSourceRow.Table.Columns.Contains(fieldName))
            {
                fieldValue = pDataSourceRow[fieldName];

                //XPGuidObject - If detect XPGuidObject Type, Extract value from its Chield, ex ArticleFamily[Field].Article[Chield].Designation[FieldName]
                if (fieldValue != null && fieldValue.GetType().BaseType == typeof(XPGuidObject))
                {
                    fieldValue = GetXPGuidObjectChildValue(fieldValue, fieldName, _columnProperties[pColumnIndex].ChildName);
                }
                //Other Non XPGuidObject Types, Like String, Int, Decimal, DateTimes, Booleans etc
                else
                    if (fieldValue.GetType() == typeof(bool))
                    {
                        bool boolValue = Convert.ToBoolean(fieldValue);
                        fieldValue = (boolValue) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_true") : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_false");
                    }
                    else
                    {
                        fieldValue = FrameworkUtils.FormatDataTableFieldFromType(fieldValue.ToString(), fieldValue.GetType().Name);
                    }
            }
            //_log.Debug(string.Format("GetDataRowColumnValue: fieldName:[{0}], fieldValue:[{1}], fieldType:[{2}]", fieldName, fieldValue, fieldValue.GetType()));
            return fieldValue;
        }

        public override DataRow DataSourceRowGetNewRecord()
        {
            //Initialize DataTableScheme 
            DataTable dataTableScheme = GenericTreeViewColumnProperty.ColumnPropertiesToDataTableScheme(_columnProperties);
            //Initialize DataRow
            System.Object[] rowArray = new System.Object[_columnProperties.Count];
            //Create rowArray to add To Fresh DataTable, to be the DataSourceRow[0]
            object defaultFieldValue = default(object);

            int i = -1;//Leave 0 for Column Propertiy Index
            foreach (GenericTreeViewColumnProperty column in _columnProperties)
            {
                i++;
                //_log.Debug(string.Format("column.Name: [{0}], column.Type: [{1}]", column.Name, column.Type));

                //Always add a Typed Default value, ex String, Guid, Enum etc
                if (column.Type == typeof(String))
                {
                    //String dont have parameterless constructor
                    defaultFieldValue = string.Empty;
                }
                else if (column.Type == typeof(Enum))
                {
                    //Always default to first index of Enum
                    defaultFieldValue = 0;
                }
                else
                {
                    defaultFieldValue = Activator.CreateInstance(column.Type);
                }

                //String dont have a parameterless constructor
                defaultFieldValue = (column.Type == typeof(String)) ? string.Empty : Activator.CreateInstance(column.Type);

                rowArray[i] = (column.InitialValue != null)
                  ? column.InitialValue
                    //Always Create From Type ;)
                  : defaultFieldValue;
            }

            //Add Row
            //Gives the problem fixed above
            //DataRow dataRow = dataTableScheme.Rows.Add(rowArray);

            //Fixed Delete Problem : The given DataRow is not in the current DataRowCollection.
            //Create a NewRow from _dataSource, and assign it to ItemArray
            DataRow dataRow = _dataSource.NewRow();
            dataRow.ItemArray = rowArray;

            //Returns the Row, Not the DataTable
            return dataRow;
        }

        public override void DataSourceRowInsert<T>(T pDataSourceRow)
        {
            //_dataSource.Rows[0][12].GetType() == (pDataSourceRow as DataRow).ItemArray[12].GetType()
            _dataSource.Rows.Add(pDataSourceRow as DataRow);

            if (_debug) _log.Debug(string.Format("DataTable Count: [{0}]", _dataSource.Rows.Count));
        }

        public override void DataSourceRowDelete<T>(T pDataSourceRow)
        {
            _dataSource.Rows.Remove(pDataSourceRow as DataRow);
            //Assign to Null to prevent UPDATE OR DELETE in a Deleted Object
            _dataSourceRow.Delete();
            _dataSourceRow = null;

            //Remove from Model and re-Index RowIndex and TotalRows
            _listStoreModel.Remove(ref _treeIterModel);
            //Reset _totalRows and _reindexRowIndex must be here else problems arrise in Indexs
            _totalRows = 0;
            _reindexRowIndex = 0;
            _listStoreModel.Foreach(TreeModelForEachTask);

            if (_debug) _log.Debug(string.Format("DataTable Count: [{0}]", _dataSource.Rows.Count));
        }

        /// <summary>
        /// Toggle DataSourceRow CheckBox
        /// </summary>
        public override void ToggleCheckBox(bool pOldValue)
        {
            _dataSourceRow[_modelCheckBoxFieldIndex] = !pOldValue;
        }

        public override bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode)
        {
            //_log.Debug(string.Format("pDataObject: [{0}]", pDataObject));

            return base.ShowDialog<T>(pDataObject, pDialogMode);
        }

        //Dont Have Refresh Implemented, Assign values to DataSourceRow and to ListStoreModelFilterSort with SetValue to Update Trees
        //ex#01.: UPDATE
        //dialog.GenericTreeView.DataSourceRow["Title"] = dialogEditClassified.AdsTitle;
        //dialog.GenericTreeView.ListStoreModel.SetValue(dialog.GenericTreeView.TreeIterModel, 4, dialogEditClassified.AdsTitle);
        //
        //Ex#02.: DELETE Use DataSourceRowDelete or 
        //Delete Row from DataSource
        //dialog.GenericTreeView.DataSourceRow.Delete();
        //Delete Row from TreeView
        //TreeIter treeIter = dialog.GenericTreeView.TreeIterModel;
        //dialog.GenericTreeView.ListStoreModel.Remove(ref treeIter);
        //ReIndex and count Rows
        //dialog.GenericTreeView.ListStoreModel.Foreach(dialog.GenericTreeView.TreeModelForEachTask);
        //
        //Ex#03.: Refresh/Recreate Treeview from DataTable. Info: Clear and Recreate from DataTable
        //treeView.ClearDataModel();
        //treeView.foreach (DataRow item in treeView.DataSource.Rows) {
        //  treeView.ModelRowInsert(item);
        //};
        //
        //Refresh From DataTable
        public override void Refresh()
        {
            ClearDataModel();
            foreach (DataRow row in _dataSource.Rows) {
              ModelRowInsert(row);
            }
        }

        //Clear DataModel
        public override void DeleteRecords()
        {
            //Call Base ClearModel, to get a fresh Model
            ClearDataModel();
            //Clear T DataSource
            _dataSource.Clear();
        }

        //Helper Method to get Model Column Index, to prevent using static Index like "...GetValue(TreeIter, 5)"
        public int GetColumnModelIndex(string pFieldName)
        {
            int resultIndex = -1;

            try
            {
                resultIndex = _dataSourceRow.Table.Columns[pFieldName].Ordinal;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return resultIndex;
        }

        //Helper Method to Set Model Column Value from Model
        public object GetColumnModelValue(string pFieldName)
        {
            object resultValue = null;

            try
            {
                resultValue = _listStoreModel.GetValue(_treeIterModel, GetColumnModelIndex(pFieldName));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return resultValue;
        }

        //Helper Method to Set Model Column Value from Model
        //Tips if Assign a boolean Column send "True" in pValue
        public bool SetColumnModelValue(string pFieldName, object pValue)
        {
            bool result = false;

            try
            {
                _listStoreModel.SetValue(_treeIterModel, GetColumnModelIndex(pFieldName), pValue);
                result = true;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return result;
        }
    }
}
