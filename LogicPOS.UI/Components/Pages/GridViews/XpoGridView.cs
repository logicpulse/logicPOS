using DevExpress.Xpo;
using Gtk;
using logicpos;
using logicpos.Classes.Enums.Dialogs;
using LogicPOS.Data.XPO.Settings;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.Domain.Entities;
using LogicPOS.Domain.Enums;
using LogicPOS.Globalization;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using System;
using System.Collections.Generic;

namespace LogicPOS.UI.Components
{
    internal class XpoGridView : GridView<XPCollection, Entity>
    {
        public Type XPObjectType { get; set; }
        //Protected Records
        protected List<Guid> _protectedRecords = new List<Guid>();
        public List<Guid> ProtectedRecords
        {
            get { return _protectedRecords; }
            set { _protectedRecords = value; }
        }

        public XpoGridView() { }

        public XpoGridView(Window parentWindow)
            : base(parentWindow)
        {
            _parentWindow = parentWindow;
        }

        public override void InitObject(
          Window parentWindow,
          Entity pXpoDefaultValue,
          GridViewMode pGenericTreeViewMode,
          GridViewNavigatorMode navigatorMode,
          List<GridViewColumn> pColumnProperties,
          XPCollection pXpoCollection,
          Type pDialogType
        )
        {

            //Parameters
            _parentWindow = parentWindow;
            Entities = pXpoCollection;

            if (Entities != null && Entities.Count > 0)
            {
                Entity = pXpoDefaultValue;
            }
            _guidDefaultValue = Entity != null ? Entity.Oid : default;
            XPObjectType = pXpoCollection.ObjectType;
            DialogType = pDialogType;
            Columns = pColumnProperties;
            _treeViewMode = pGenericTreeViewMode;
            _navigatorMode = navigatorMode;

            _modelFirstCustomFieldIndex = _treeViewMode == GridViewMode.Default ? 1 : 2;

            if (pXpoCollection.Sorting.Count == 0)
            {
                pXpoCollection.Sorting = XPOUtility.GetXPCollectionDefaultSortingCollection();
            }

            Entities.Reload();

            InitDataModel(Entities, Columns, _treeViewMode);

            ListStoreModel.Foreach(new TreeModelForeachFunc(TreeModelForEachTask));


            InitUI();

            string objectNameWithoutPrefix = XPObjectType.UnderlyingSystemType.Name.Substring(4, XPObjectType.UnderlyingSystemType.Name.Length - 4);
            string tokenAllowDelete = string.Format("{0}_{1}", string.Format(LogicPOSSettings.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "DELETE").ToUpper();
            string tokenAllowInsert = string.Format("{0}_{1}", string.Format(LogicPOSSettings.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "CREATE").ToUpper();
            string tokenAllowUpdate = string.Format("{0}_{1}", string.Format(LogicPOSSettings.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "EDIT").ToUpper();
            string tokenAllowView = string.Format("{0}_{1}", string.Format(LogicPOSSettings.PrivilegesBackOfficeCRUDOperationPrefix, objectNameWithoutPrefix), "VIEW").ToUpper();


            if (GeneralSettings.LoggedUserPermissions != null)
            {
                _allowRecordDelete = GeneralSettings.LoggedUserHasPermissionTo(tokenAllowDelete);
                _allowRecordInsert = GeneralSettings.LoggedUserHasPermissionTo(tokenAllowInsert);
                _allowRecordUpdate = GeneralSettings.LoggedUserHasPermissionTo(tokenAllowUpdate);
                _allowRecordView = GeneralSettings.LoggedUserHasPermissionTo(tokenAllowView);
            }

            InitNavigatorPermissions();

            InitProtectedRecordsEvents();

            SetInitialCursorPosition();
        }

        public override void InitDataModel(XPCollection pDataSource, List<GridViewColumn> pColumnProperties, GridViewMode pGenericTreeViewMode)
        {
            XPCollection _XpCollection = pDataSource;
            List<GridViewColumn> _columnProperties = pColumnProperties;

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
                                        columnValues[i] = Entity.DecryptIfNeeded(columnValues[i]);
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
                                    columnValues[i] = Entity.DecryptIfNeeded(columnValues[i]);
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
                                        columnValues[i] = Entity.DecryptIfNeeded(columnValues[i]);
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


        public override void GetDataRow()
        {
            Entity = XPOUtility.GetXPGuidObject(
              Entities.ObjectType,
              new Guid(Convert.ToString(TreeView.Model.GetValue(_treeIter, _modelFirstCustomFieldIndex)))
            );
        }

        public override object DataSourceRowGetColumnValue(Entity pDataSourceRow, int pColumnIndex, string pFieldName = "")
        {
            string fieldName = pFieldName;
            object fieldValue = null;

            if (pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name) != null
              && pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name).GetType().BaseType == typeof(Entity))
            {
                fieldValue = GetXPGuidObjectChildValue(pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name), fieldName, Columns[pColumnIndex].ChildName);
            }
            else
            {

                if (pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name) != null &&
                  pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name).GetType().Name.Equals("Boolean"))
                {
                    bool boolValue = Convert.ToBoolean(pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name));
                    fieldValue = boolValue ? CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_true") : CultureResources.GetResourceByLanguage(CultureSettings.CurrentCultureName, "global_treeview_false");
                }
                else
                {
                    fieldValue = Convert.ToString(pDataSourceRow.GetMemberValue(Columns[pColumnIndex].Name));
                }

            };

            return fieldValue;
        }

        public override Entity DataSourceRowGetNewRecord()
        {
            Entity newXPGuidObject = (Entity)Activator.CreateInstance(XPObjectType);

            foreach (GridViewColumn column in Columns)
            {
                //if (_debug) _logger.Debug(string.Format("column.Name: [{0}], column.Type: [{1}]", column.Name, column.Type));
                if (column.InitialValue != null)
                {
                    //If is a XPGuidObject
                    if (column.InitialValue.GetType().BaseType == typeof(Entity))
                    {
                        //Get Fresh Object else "object belongs to a different session"
                        var xInitialValue = XPOUtility.GetXPGuidObject(newXPGuidObject.Session, column.InitialValue.GetType(), (column.InitialValue as Entity).Oid);
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
            if (pDataSourceRow as Entity != null)
            {
                (pDataSourceRow as Entity).DeletedAt = DateTime.Now;
                (pDataSourceRow as Entity).DeletedBy = XPOSettings.LoggedUser;
                (pDataSourceRow as Entity).Disabled = true;
                (pDataSourceRow as Entity).Save();
            }

            Entity = null;

            ListStoreModel.Remove(ref TreeIterModel);
        }

        public override bool ShowDialog<T>(T pDataObject, DialogMode pDialogMode)
        {
            bool result = false;

            (pDataObject as Entity).Reload();

            result = base.ShowDialog(pDataObject, pDialogMode);

            return result;
        }

        public override void Refresh()
        {
            _treePath = ListStoreModelFilterSort.GetPath(_treeIter);

            Entities.Reload();

            InitDataModel(Entities, Columns, _treeViewMode);

            UpdateChildModelsAfterCRUDChanges();
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Protected Records

        //Create Automatic Events if not created by User (BeforeDelete and BeforeUpdate)
        public void InitProtectedRecordsEvents()
        {
            if (!HasEventRecordBeforeDelete())
            {
                //_logger.Debug("Create RecordBeforeDelete Event");

                RecordBeforeDelete += delegate
                {
                    //_logger.Debug("Create RecordBeforeDelete Triggered");

                    //Prevent Delete Protected Records, assigning TreeView Base SkipRecordDelete
                    SkipRecordDelete = _protectedRecords.Count > 0 && _protectedRecords.Contains(Entity.Oid);
                    //Show Message
                    if (SkipRecordDelete)
                    {
                        Utils.ShowMessageTouchProtectedDeleteRecordMessage(_parentWindow);
                    }
                };
            }

            if (!HasEventRecordBeforeUpdate())
            {
                //_logger.Debug("Create RecordBeforeUpdate Event");

                RecordBeforeUpdate += delegate
                {
                    //_logger.Debug("Create RecordBeforeUpdate Triggered");

                    //Prevent Update Protected Records, assigning TreeView Base SkipRecordUpdate
                    SkipRecordUpdate = _protectedRecords.Count > 0 && _protectedRecords.Contains(Entity.Oid);
                    //Show Message
                    if (SkipRecordUpdate)
                    {
                        Utils.ShowMessageTouchProtectedUpdateRecordMessage(_parentWindow);
                    }
                };
            }
        }
    }
}
