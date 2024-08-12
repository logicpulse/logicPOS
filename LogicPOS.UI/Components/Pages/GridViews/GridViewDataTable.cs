using Gtk;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Domain.Entities;
using LogicPOS.Globalization;
using LogicPOS.UI.Components;
using LogicPOS.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    internal class GridViewDataTable : GridView<DataTable, DataRow>
    {
        public GridViewDataTable() { }

        public GridViewDataTable(Window parentWindow)
            : base(parentWindow)
        {
            _parentWindow = parentWindow;
        }

        public override void InitObject(
          Window parentWindow,
          DataRow pDefaultValue,
          GridViewMode pGenericTreeViewMode,
          GridViewNavigatorMode navigatorMode,
          List<GridViewColumn> pColumnProperties,
          DataTable pDataSource,
          Type pDialogType
        )
        {

            //Parameters
            _parentWindow = parentWindow;
            Entities = pDataSource;
            if (Entities.Rows.Count > 0) Entity = Entities.Rows[_currentRowIndex];
            _guidDefaultValue = default;
            DialogType = pDialogType;
            Columns = pColumnProperties;
            _treeViewMode = pGenericTreeViewMode;
            _navigatorMode = navigatorMode;

            _modelFirstCustomFieldIndex = (_treeViewMode == GridViewMode.Default) ? 1 : 2;

            InitDataModel(Entities, Columns, _treeViewMode);

            ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));

            InitUI();

            InitNavigatorPermissions();

            Navigator.ButtonRefresh.Visible = false;
            Navigator.ButtonRefresh.Destroy();

            SetInitialCursorPosition();
        }

        public override void InitDataModel(DataTable pDataSource, List<GridViewColumn> pColumnProperties, GridViewMode pGenericTreeViewMode)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Parameters
            DataTable dataTable = pDataSource;
            List<GridViewColumn> _columnProperties = pColumnProperties;
            GridViewMode _treeViewMode = pGenericTreeViewMode;

            //Initialize Model and Column Properties
            ListStore model = GridViewModel.InitModel(_columnProperties, pGenericTreeViewMode);

            //Init ColumnValues Object Array
            object[] columnValues = new object[_columnProperties.Count];

            //Start Render Model Values from Collection
            string fieldName;

            //Always add Index to Rows, NOT USED: Check if existe before Add to prevent Refresh Add Again
            dataTable.Columns.Add("Index", typeof(int)).SetOrdinal(0);//Put in Index 0, First Column

            //If Detect CheckBox Mode, Insert CheckBox Column in DataTable
            if (_treeViewMode == GridViewMode.CheckBox)
            {
                //NOT USED: Check if existe before Add to prevent Refresh Add Again "if (dataTable.Columns.IndexOf("CheckBox") < 0)"
                dataTable.Columns.Add("CheckBox", typeof(bool)).SetOrdinal(1);//Put in Index 1, After Index
            }

            //Loop Records
            int rowIndex = -1;
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
                    else if (dataRow.ItemArray[i] != null && dataRow.ItemArray[i].GetType().BaseType == typeof(Entity))
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
                        columnValues[i] = DataConversionUtils.FormatDataTableFieldFromType(dataRow.ItemArray[i].ToString(), dataRow.ItemArray[i].GetType().Name);
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

            ListStoreModel = model;
        }

        /// <summary>
        /// Get Current DataRow Object
        /// </summary>
        /// <returns></returns>
        public override void GetDataRow()
        {
            Entity = Entities.Rows[_currentRowIndex];
        }

        /// <summary>
        /// Get Current DataRow Column Value, usefull to work in DataSourceRowToModelRow
        /// </summary>
        /// <param name="DataSourceRow"></param>
        /// <param name="ColumnIndex"></param>
        /// <returns>Column Value</returns>
        public override object DataSourceRowGetColumnValue(DataRow pDataSourceRow, int pColumnIndex, string pFieldName = "")
        {
            string fieldName = pFieldName;
            object fieldValue = null;

            if (pDataSourceRow.Table.Columns.Contains(fieldName))
            {
                fieldValue = pDataSourceRow[fieldName];

                //XPGuidObject - If detect XPGuidObject Type, Extract value from its Chield, ex ArticleFamily[Field].Article[Chield].Designation[FieldName]
                if (fieldValue != null && fieldValue.GetType().BaseType == typeof(Entity))
                {
                    fieldValue = GetXPGuidObjectChildValue(fieldValue, fieldName, Columns[pColumnIndex].ChildName);
                }
                //Other Non XPGuidObject Types, Like String, Int, Decimal, DateTimes, Booleans etc
                else
                    if (fieldValue.GetType() == typeof(bool))
                {
                    bool boolValue = Convert.ToBoolean(fieldValue);
                    fieldValue = (boolValue) ? CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_treeview_true") : CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_treeview_false");
                }
                else
                {
                    fieldValue = DataConversionUtils.FormatDataTableFieldFromType(fieldValue.ToString(), fieldValue.GetType().Name);
                }
            }
            //_logger.Debug(string.Format("GetDataRowColumnValue: fieldName:[{0}], fieldValue:[{1}], fieldType:[{2}]", fieldName, fieldValue, fieldValue.GetType()));
            return fieldValue;
        }

        public override DataRow DataSourceRowGetNewRecord()
        {
            DataTable dataTableScheme = GridViewColumn.ColumnPropertiesToDataTableScheme(Columns);

            object[] rowArray = new object[Columns.Count];
            int i = -1;
            foreach (GridViewColumn column in Columns)
            {
                i++;
                object defaultFieldValue;

                if (column.Type == typeof(string))
                {
                    defaultFieldValue = string.Empty;
                }
                else if (column.Type == typeof(Enum))
                {
                    defaultFieldValue = 0;
                }
                else
                {
                    defaultFieldValue = Activator.CreateInstance(column.Type);
                }

                defaultFieldValue = (column.Type == typeof(string)) ? string.Empty : Activator.CreateInstance(column.Type);

                rowArray[i] = (column.InitialValue != null)
                  ? column.InitialValue
                  : defaultFieldValue;
            }


            DataRow dataRow = Entities.NewRow();
            dataRow.ItemArray = rowArray;

            return dataRow;
        }

        public override void DataSourceRowInsert<T>(T pDataSourceRow)
        {
            Entities.Rows.Add(pDataSourceRow as DataRow);
        }

        public override void DataSourceRowDelete<T>(T pDataSourceRow)
        {
            Entities.Rows.Remove(pDataSourceRow as DataRow);

            if ((pDataSourceRow as Entity) != null)
            {
                (pDataSourceRow as Entity).DeletedAt = DateTime.Now;
                (pDataSourceRow as Entity).Disabled = true;
                (pDataSourceRow as Entity).Save();
            }

            Entity = null;

            ListStoreModel.Remove(ref TreeIterModel);

            _totalRows = 0;
            _reindexRowIndex = 0;
            ListStoreModel.Foreach(TreeModelForEachTask);

        }


        public override void ToggleCheckBox(bool pOldValue)
        {
            Entity[_modelCheckBoxFieldIndex] = !pOldValue;
        }

        public override bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode)
        {
            return base.ShowDialog<T>(pDataObject, pDialogMode);
        }

        
        public override void Refresh()
        {
            ClearDataModel();
            foreach (DataRow row in Entities.Rows)
            {
                ModelRowInsert(row);
            }
        }

        //Clear DataModel
        public override void DeleteRecords()
        {
            //Call Base ClearModel, to get a fresh Model
            ClearDataModel();
            //Clear T DataSource
            Entities.Clear();
        }

        public int GetColumnModelIndex(string pFieldName)
        {
            int resultIndex = -1;

            resultIndex = Entity.Table.Columns[pFieldName].Ordinal;


            return resultIndex;
        }

        public object GetColumnModelValue(string pFieldName)
        {
            object resultValue = null;
            resultValue = ListStoreModel.GetValue(TreeIterModel, GetColumnModelIndex(pFieldName));

            return resultValue;
        }

        public bool SetColumnModelValue(string pFieldName, object pValue)
        {
            bool result = false;

            ListStoreModel.SetValue(TreeIterModel, GetColumnModelIndex(pFieldName), pValue);
            result = true;

            return result;
        }
    }
}
