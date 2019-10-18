using DevExpress.Xpo;
using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Enums.GenericTreeView;
using logicpos.Classes.Gui.Gtk.BackOffice;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.resources.Resources.Localization;
using System;
using System.Collections.Generic;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    class GenericTreeViewXPO : GenericTreeView<XPCollection, XPGuidObject>
    {
        private Type _xpoGuidObjectType;
        public Type XPObjectType
        {
            get { return _xpoGuidObjectType; }
            set { _xpoGuidObjectType = value; }
        }
        //Protected Records
        protected List<Guid> _protectedRecords = new List<Guid>();
        public List<Guid> ProtectedRecords
        {
            get { return _protectedRecords; }
            set { _protectedRecords = value; }
        }

        //Public Parametless Constructor Required by Generics
        public GenericTreeViewXPO() { }

        public GenericTreeViewXPO(Window pSourceWindow)
            : base(pSourceWindow)
        {
            _sourceWindow = pSourceWindow;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Override Super/Base Class Methods

        //Object Initializer
        public override void InitObject(
          Window pSourceWindow,
          XPGuidObject pXpoDefaultValue,
          GenericTreeViewMode pGenericTreeViewMode,
          GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode,
          List<GenericTreeViewColumnProperty> pColumnProperties,
          XPCollection pXpoCollection,
          Type pDialogType
        )
        {
            if (_debug) _log.Debug("InitObject Begin(" + pSourceWindow + "," + pColumnProperties + "," + pXpoCollection + "," + pXpoDefaultValue + "," + pDialogType + "," + pGenericTreeViewMode + "," + pGenericTreeViewNavigatorMode);

            //Parameters
            _sourceWindow = pSourceWindow;
            _dataSource = pXpoCollection;

            if (_dataSource.Count > 0)
            {
                _dataSourceRow = pXpoDefaultValue;
            }
            _guidDefaultValue = (_dataSourceRow != null) ? _dataSourceRow.Oid : default(Guid);
            _xpoGuidObjectType = pXpoCollection.ObjectType;
            _dialogType = pDialogType;
            _columnProperties = pColumnProperties;
            _treeViewMode = pGenericTreeViewMode;
            _navigatorMode = pGenericTreeViewNavigatorMode;

            //Get First Custom Field Position ex OID
            _modelFirstCustomFieldIndex = (_treeViewMode == GenericTreeViewMode.Default) ? 1 : 2;

            //Add default Sorting, if not defined in XPCollection - Use Ord, XPObject must have Ord, else it Throw Exception Intentionally, this way developer detect error
            if (pXpoCollection.Sorting.Count == 0)
            {
                try
                {
                    pXpoCollection.Sorting = FrameworkUtils.GetXPCollectionDefaultSortingCollection();
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                    throw;
                }
            }

            //DropIdentityMap Removed, gives problems, Avoid used DropIdentityMap
            //_dataSource.Session.DropIdentityMap();
            //Force Reload Objects Without Cache
            _dataSource.Reload();

            //InitDataModel
            InitDataModel(_dataSource, _columnProperties, _treeViewMode);

            //ReIndex and count Rows
            _listStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));

            //Initialize UI
            if (_debug) _log.Debug("InitObject Before InitUI");
            InitUI();

            //Prepare CRUD Privileges
            //Require to use Object Name Without Prefixs (Remove Prefixs PFX_)
            string objectNameWithoutPrefix = _xpoGuidObjectType.UnderlyingSystemType.Name.Substring(4, _xpoGuidObjectType.UnderlyingSystemType.Name.Length - 4);
            string tokenAllowDelete = string.Format("{0}_{1}", string.Format(SettingsApp.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "DELETE").ToUpper();
            string tokenAllowInsert = string.Format("{0}_{1}", string.Format(SettingsApp.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "CREATE").ToUpper();
            string tokenAllowUpdate = string.Format("{0}_{1}", string.Format(SettingsApp.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "EDIT").ToUpper();
            string tokenAllowView = string.Format("{0}_{1}", string.Format(SettingsApp.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "VIEW").ToUpper();

            // Help to Debug some Kind of Types Privileges
            //if (this.GetType().Equals(typeof(TreeViewConfigurationInputReader)))
            //{
            //    _log.Debug($"BREAK {typeof(TreeViewConfigurationInputReader)}");
            //}

            //Assign CRUD permissions to private members, Overriding Defaults
            if (GlobalFramework.LoggedUserPermissions != null)
            {
                _allowRecordDelete = FrameworkUtils.HasPermissionTo(tokenAllowDelete);
                _allowRecordInsert = FrameworkUtils.HasPermissionTo(tokenAllowInsert);
                _allowRecordUpdate = FrameworkUtils.HasPermissionTo(tokenAllowUpdate);
                _allowRecordView = FrameworkUtils.HasPermissionTo(tokenAllowView);
            }

            //Update Navigator Permissions
            InitNavigatorPermissions();

            //Init Protected Record Events
            InitProtectedRecordsEvents();

            //Always have a valid cursor, in first Record or in pDefaultValue
            SetInitialCursorPosition();

            if (_debug) _log.Debug("InitObject End");
        }


        public void InitDashboard(
          Window pSourceWindow,
          GenericTreeViewMode pGenericTreeViewMode,
          GenericTreeViewNavigatorMode pGenericTreeViewNavigatorMode
        )
        {
            if (_debug) _log.Debug("InitObject Begin(" + pSourceWindow + ","  + pGenericTreeViewMode + "," + pGenericTreeViewNavigatorMode);

            //Parameters
            _sourceWindow = pSourceWindow;
            _dialogType = null;
            _treeViewMode = pGenericTreeViewMode;
            _navigatorMode = pGenericTreeViewNavigatorMode;

            //Get First Custom Field Position ex OID
            _modelFirstCustomFieldIndex = (_treeViewMode == GenericTreeViewMode.Default) ? 1 : 2;

            //Initialize UI
            //if (_debug) _log.Debug("InitObject Before InitUI");
            InitUiDashBoard();

            //Update Navigator Permissions
            InitNavigatorPermissions();

            //Init Protected Record Events
            InitProtectedRecordsEvents();

            //Always have a valid cursor, in first Record or in pDefaultValue
            //SetInitialCursorPosition();

            if (_debug) _log.Debug("InitObject End");
        }

        /// <summary>
        /// Generates a XPCollection with Criteria Operator and create a TreeView Model, from a parameter XPGuidObject Class Type. Used to generate TreeView Models to GenericTreeViewModel Class.
        /// </summary>
        /// <param name="XPCollection"> used to generate TreeView Model</param>
        /// <param name="ColumnProperties"> TreeView Column Properties, used to select which XPOCollection fields are used to generate TreeView ListStore Model.</param>
        /// <param name="TreeViewMode"> TreeView Mode Default or CheckBox</param>
        /// <returns>Returns ListStore, a model ready to be used in GenericTreeViewModel.</returns>
        public override void InitDataModel(XPCollection pDataSource, List<GenericTreeViewColumnProperty> pColumnProperties, GenericTreeViewMode pGenericTreeViewMode)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            //Parameters
            XPCollection _XpCollection = pDataSource;
            List<GenericTreeViewColumnProperty> _columnProperties = pColumnProperties;
            GenericTreeViewMode _treeViewMode = pGenericTreeViewMode;

            //Initialize Model and Column Properties
            ListStore model = GenericTreeViewModel.InitModel(_columnProperties, pGenericTreeViewMode);

            //Init ColumnValues Object Array
            System.Object[] columnValues = new System.Object[_columnProperties.Count];

            //Start Render Model Values from Collection
            String fieldName;

            //Loop Records
            Int32 rowIndex = -1;
            foreach (XPGuidObject dataRow in (_XpCollection as XPCollection))
            {
                // Increment RownIndex
                rowIndex++;
                // reset undefinedRecord
                bool undefinedRecord = false;

                // This will reload current dataRow, without this XPO wil use cached values, and never be in sync when we change data outside
                dataRow.Reload();

                //Loop Fields : Generate Abstract Values to use in Model
                for (int i = 0; i < _columnProperties.Count; i++)
                {
                    //Default FieldName
                    fieldName = _columnProperties[i].Name;

                    //if (fieldName == "Ord" || fieldName == "Code")
                    //{
                    //    _log.Debug("BREAK");
                    //}

                    try
                    {
                        //Dont extract value from Collection if is the CheckBox, its not in Collection, and is always Disabled
                        if (_columnProperties[i].PropertyType == GenericTreeViewColumnPropertyType.CheckBox)
                        {
                            //Initial Checked Value
                            columnValues[i] = false;
                        }
                        else if (fieldName == "RowIndex")
                        {
                            columnValues[i] = rowIndex;
                        }
                        //Query
                        else if (_columnProperties[i].Query != null && _columnProperties[i].Query != string.Empty)
                        {
                            columnValues[i] = ColumnPropertyGetQuery(_columnProperties[i].Query, dataRow.GetMemberValue("Oid"));
                            // Decrypt Before use Format
                            if (_columnProperties[i].DecryptValue)
                            {
                                columnValues[i] = XPGuidObject.DecryptIfNeeded(columnValues[i]);
                            }
                            //Format String using Column FormatProvider              
                            if (_columnProperties[i].FormatProvider != null && Convert.ToString(columnValues[i]) != string.Empty)
                            {
                                columnValues[i] = string.Format(_columnProperties[i].FormatProvider, "{0}", columnValues[i]);
                            }
                        }
                        //If detect XPGuidObject Value Type (Value is a XPObject, Child Object), Get Value from its Chield Field (Related Table)
                        else if (dataRow.GetMemberValue(_columnProperties[i].Name) != null &&
                          dataRow.GetMemberValue(_columnProperties[i].Name).GetType().BaseType == typeof(XPGuidObject))
                        {
                            columnValues[i] = GetXPGuidObjectChildValue(dataRow.GetMemberValue(_columnProperties[i].Name), fieldName, _columnProperties[i].ChildName);
                        }
                        //Get Default Value from Field Name
                        else
                        {
                            // Check/Detect if is a undefinedRecord
                            if (fieldName == "Oid")
                            {
                                undefinedRecord = ((new Guid(dataRow.GetMemberValue(_columnProperties[i].Name).ToString())).Equals(SettingsApp.XpoOidUndefinedRecord));
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
                                    columnValues[i] = (booleanValue) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_true") : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_false");
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
                                }
                            }
                            //If Field is NULL or Empty always assign string.Empty to prevent re-use last record value
                            else
                            {
                                columnValues[i] = string.Empty;
                            }
                        }

                        // ResourceString : Extract ResourceManager from Final Value
                        if (_columnProperties[i].ResourceString == true && columnValues[i] != null)
                        {
                            // Try to get ResourceString Value, this is required to replace value, but only if it a valid resourceString (not replaced yet after update)
                            // After an Update and Refresh it turns into a string(non resource token), this protection prevents the replace double with a null resourceString, 
                            // leaving tree cell value with an empty value
                            bool checkIfResourceStringExist = (resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], columnValues[i].ToString()) != null) ? true : false;
                            // Only Replace resourceString if value is resourceString is not Yet been replaced, ex after an update
                            //_log.Debug(string.Format("columnValues[i]#1: [{0}]", columnValues[i]));
                            if (checkIfResourceStringExist)
                            {
                                columnValues[i] = resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], columnValues[i].ToString());
                            }
                            //_log.Debug(string.Format("columnValues[i]#2: [{0}]", columnValues[i]));
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(string.Format("XPCollectionToModel(): {0}", ex.Message), ex);
                        columnValues[i] = string.Format("Invalid Field {0}", fieldName);
                    }
                };
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
            _dataSourceRow = FrameworkUtils.GetXPGuidObject(
              GlobalFramework.SessionXpo,
              _dataSource.ObjectType,
              new Guid(Convert.ToString(_treeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)))
            );
        }

        /// <summary>
        /// Get Current DataRow Column Value, usefull to work in DataSourceRowToModelRow
        /// </summary>
        /// <param name="DataSourceRow"></param>
        /// <param name="ColumnName"></param>
        /// <returns>Column Value</returns>
        public override object DataSourceRowGetColumnValue(XPGuidObject pDataSourceRow, int pColumnIndex, string pFieldName = "")
        {
            string fieldName = pFieldName;
            object fieldValue = null;

            //XPGuidObject - If detect XPGuidObject Type, Extract value from its Chield, ex ArticleFamily[Field].Article[Chield].Designation[FieldName]
            if (pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name) != null
              && pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name).GetType().BaseType == typeof(XPGuidObject))
            {
                fieldValue = GetXPGuidObjectChildValue(pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name), fieldName, _columnProperties[pColumnIndex].ChildName);
            }
            //Other Non XPGuidObject Types, Like String, Int, Decimal, DateTimes, Booleans etc
            else
            {
                try
                {
                    //Boolean Fields
                    if (pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name) != null &&
                      pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name).GetType().Name.Equals("Boolean"))
                    {
                        bool boolValue = Convert.ToBoolean(pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name));
                        fieldValue = (boolValue) ? resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_true") : resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_treeview_false");
                    }
                    //All Other Fields
                    else
                    {
                        fieldValue = Convert.ToString(pDataSourceRow.GetMemberValue(_columnProperties[pColumnIndex].Name));
                    }
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message, ex);
                };
            };

            if (_debug) _log.Debug(string.Format("GetDataRowColumnValue: fieldName:[{0}], fieldValue:[{1}], fieldType:[{2}]", fieldName, fieldValue, fieldValue.GetType()));

            return fieldValue;
        }

        public override XPGuidObject DataSourceRowGetNewRecord()
        {
            XPGuidObject newXPGuidObject = (XPGuidObject)Activator.CreateInstance(_xpoGuidObjectType);

            foreach (GenericTreeViewColumnProperty column in _columnProperties)
            {
                //if (_debug) _log.Debug(string.Format("column.Name: [{0}], column.Type: [{1}]", column.Name, column.Type));
                if (column.InitialValue != null)
                {
                    //If is a XPGuidObject
                    if (column.InitialValue.GetType().BaseType == typeof(XPGuidObject))
                    {
                        //Get Fresh Object else "object belongs to a different session"
                        var xInitialValue = FrameworkUtils.GetXPGuidObject(newXPGuidObject.Session, column.InitialValue.GetType(), (column.InitialValue as XPGuidObject).Oid);
                        newXPGuidObject.SetMemberValue(column.Name, xInitialValue);
                    }
                    //Default Values
                    else
                    {
                        newXPGuidObject.SetMemberValue(column.Name, column.InitialValue);
                    }
                }
            }

            return newXPGuidObject;
        }

        public override void DataSourceRowDelete<T>(T pDataSourceRow)
        {
            //_dataSource.Remove(pDataSourceRow);
            //FIX for dataSource.Remove(pDataSourceRow), it wont work!!!!, now we use Session.Delete to ByPass this Problem
            (pDataSourceRow as XPGuidObject).Session.Delete(pDataSourceRow);
            //Assign to Null to prevent UPDATE OR DELETE in a Deleted Object
            _dataSourceRow = null;
            //Remove from Model
            //FIX DOUBLEDELETE in Cloned Documents (Search for FIX DOUBLEDELETE)
            _listStoreModel.Remove(ref _treeIterModel);
            if (_debug) _log.Debug(string.Format("XPCollection Count: [{0}]", _dataSource.Count));
        }

        public override bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode)
        {
            bool result = false;

            try
            {
                //Require Reload: Else We have The XPO "Deleted Object" Problem
                (pDataObject as XPGuidObject).Reload();

                result = base.ShowDialog<T>(pDataObject, pDialogMode);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }

            return (result);
        }

        public override void Refresh()
        {
            //Store Current treePath to set cursor after refresh Work
            _treePath = _listStoreModelFilterSort.GetPath(_treeIter);

            //Removed, gives problems, Avoid used DropIdentityMap
            //_dataSource.Session.DropIdentityMap();
            //Force reload without XPO Cache
            _dataSource.Reload();

            //InitModel
            InitDataModel(_dataSource, _columnProperties, _treeViewMode);

            //Update ModelFilter from Changes in Model
            UpdateChildModelsAfterCRUDChanges();

            //Try to Restore Current Cursor
            try
            {
                //_treeView.SetCursor(_treePath, null, false);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
            }
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Protected Records

        //Create Automatic Events if not created by User (BeforeDelete and BeforeUpdate)
        public void InitProtectedRecordsEvents()
        {
            if (!HasEventRecordBeforeDelete())
            {
                //_log.Debug("Create RecordBeforeDelete Event");

                RecordBeforeDelete += delegate
                {
                    //_log.Debug("Create RecordBeforeDelete Triggered");

                    //Prevent Delete Protected Records, assigning TreeView Base SkipRecordDelete
                    _skipRecordDelete = (_protectedRecords.Count > 0 && _protectedRecords.Contains(_dataSourceRow.Oid));
                    //Show Message
                    if (_skipRecordDelete)
                    {
                        Utils.ShowMessageTouchProtectedDeleteRecordMessage(_sourceWindow);
                    }
                };
            }

            if (!HasEventRecordBeforeUpdate())
            {
                //_log.Debug("Create RecordBeforeUpdate Event");

                RecordBeforeUpdate += delegate
                {
                    //_log.Debug("Create RecordBeforeUpdate Triggered");

                    //Prevent Update Protected Records, assigning TreeView Base SkipRecordUpdate
                    _skipRecordUpdate = (_protectedRecords.Count > 0 && _protectedRecords.Contains(_dataSourceRow.Oid));
                    //Show Message
                    if (_skipRecordUpdate)
                    {
                        Utils.ShowMessageTouchProtectedUpdateRecordMessage(_sourceWindow);
                    }
                };
            }
        }
    }
}
