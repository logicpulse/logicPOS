using Gtk;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using System;
using System.Collections.Generic;
using System.Data;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    //Start with Template

    //Define/Change object types ex <T1,T2>, currently using DataTable, DataRow
    //DataTable pDataSource, 
    //DataRow pDefaultValue, 

    class GenericTreeViewTemplate : GenericTreeView<DataTable, DataRow>
    {
        //Public Parametless Constructor Required by Generics
        public GenericTreeViewTemplate() { }

        public GenericTreeViewTemplate(Window pSourceWindow)
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
            //Parameters
            _sourceWindow = pSourceWindow;
            _columnProperties = pColumnProperties;
            _dataSource = pDataSource;
            _dataSourceRow = _dataSource.Rows[_currentRowIndex];
            _guidDefaultValue = default(Guid);
            _treeViewMode = pGenericTreeViewMode;
            _navigatorMode = pGenericTreeViewNavigatorMode;

            //Get First Custom Field Position ex OID
            _modelFirstCustomFieldIndex = (_treeViewMode == GenericTreeViewMode.Default) ? 1 : 2;

            //InitDataModel
            InitDataModel(_dataSource, _columnProperties, _treeViewMode);

            //ReIndex and count Rows
            _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));

            InitUI();

            //Always have a valid cursor, in first Record or in pDefaultValue
            SetInitialCursorPosition();
        }

        public override void InitDataModel(DataTable pDataSource, List<GenericTreeViewColumnProperty> pColumnProperties, GenericTreeViewMode pGenericTreeViewMode)
        {
            _listStoreModel = null;
        }

        public override void GetDataRow()
        {
            _dataSourceRow = _dataSource.Rows[_currentRowIndex];
        }

        public override object DataSourceRowGetColumnValue(DataRow pDataSourceRow, int pColumnIndex, string pFieldName)
        {
            return null;
        }

        public override DataRow DataSourceRowGetNewRecord()
        {
            return null;
        }

        public override void ToggleCheckBox(bool pOldValue)
        {
            _dataSourceRow[_modelCheckBoxFieldIndex] = !pOldValue;
        }

        public override bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode)
        {
            return false;
        }

        public override void Refresh()
        {
            throw new NotImplementedException();
        }

        /*
        public override void Insert()
        {
          try
          {
            //Fire Event
            OnRecordBeforeInsert();

            //Implementation Goes Here
            _log.Debug("Insert(): WIP");

            //Fire Event 
            OnRecordAfterInsert();
          }
          catch (Exception ex)
          {
            _log.Error(ex.Message, ex);
          }
        }

        public override void Update()
        {
          try
          {
            //Fire Event
            OnRecordBeforeUpdate();

            //Implementation Goes Here
            _log.Debug("Update(): WIP");

            //Fire Event 
            OnRecordAfterUpdate();
          }
          catch (Exception ex)
          {
            _log.Error(ex.Message, ex);
          }
        }
        */

        /*
        public override void Delete()
        {
          try
          {
            //Fire Event
            OnRecordBeforeDelete();

            //Implementation Goes Here
            _log.Debug("Delete(): WIP");

            //Fire Event 
            OnRecordAfterDelete();
          }
          catch (Exception ex)
          {
            _log.Error(ex.Message, ex);
          }
        }
        */
    }
}
