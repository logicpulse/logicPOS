using System;
using Gtk;
using System.Reflection;
using System.Collections.Generic;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using logicpos.datalayer.DataLayer.Xpo;
using System.Data;
using logicpos.financial;
using logicpos.Classes.Enums.GenericTreeView;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    /// <summary>
    /// GenericTreeViewModel Model Static Helper Class
    /// </summary>
    abstract class GenericTreeViewModel
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Show SystemColumns in TreeView ex rowIndex, Oid
        private static bool _showSystemColumns = false;

        /// <summary>
        /// Helper to debug models
        /// </summary>
        /// <param name="store"> the ListStore to Use</param>
        /// <param name="col"> Target column to output</param>
        public static void DumpColumnValues(ListStore store, int col)
        {
            foreach (object[] row in store) Console.WriteLine("Value of column {0} is {1}", col, row[col]);
        }

        /*
            /// <summary>Convert a XPObject fields to Object Array of Values, used to Insert, Append in Model</summary>
            /// <param name="XpoObject">XPObject Object </param>
            /// <param name="ColumnProperties">GenericTreeView Column Properties</param>
            public static System.Object[] XPObjectToModelValues(XPGuidObject pXpoObject, List<GenericTreeViewColumnProperty> pColumnProperties)
            {
              //Parameters
              List<GenericTreeViewColumnProperty> _columnProperties = pColumnProperties;
              //Used to Store all PropertyInfos of TreeViewColumnProperty Object 
              PropertyInfo[] pisTreeViewColumnProperties;
              //Used to store current TreeViewColumProperty same as _columnProperties[i]
              GenericTreeViewColumnProperty currentTreeViewColumnProperty;
              //Used to Store Current TreeViewColumnProperty Value
              System.Object pInfoValue;

              //Used to Store columnValues to Append/Insert to Model and to Store Return
              System.Object[] columnValues = new System.Object[_columnProperties.Count];

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
                  //If is Name property and its not XPObject Oid Field Process it
                  if (pInfo.Name == "Name")
                  {
                    //Se detectar XPObject Value Extract value from it, from ChildName
                    if (pXpoObject.GetMemberValue(_columnProperties[i].Name) != null
                      && pXpoObject.GetMemberValue(_columnProperties[i].Name).GetType().BaseType == typeof(XPObject))
                    {
                      if (_columnProperties[i].ChildName != null & _columnProperties[i].ChildName != string.Empty)
                      {
                        currentFieldName += "." + _columnProperties[i].ChildName;
                        dynamic xpoObjectFieldValue = pXpoObject.GetMemberValue(_columnProperties[i].Name);
                        columnValues[i] = Convert.ToString(xpoObjectFieldValue.GetType().GetProperty(_columnProperties[i].ChildName).GetValue(xpoObjectFieldValue, null));
                      }
                      //Get Default Value from Field Name
                      else
                      {
                        columnValues[i] = string.Format("Detected XPObject! You must define ChildName for Field {0}", currentFieldName);
                      };
                    }
                    else
                    {
                      try
                      {
                        columnValues[i] = Convert.ToString(pXpoObject.GetMemberValue(_columnProperties[i].Name));
                        //Console.WriteLine(string.Format("pInfoValue: {0}, xpObjectValue: {1}", pInfoValue, pXpoObject.GetMemberValue((string)pInfoValue)));
                      }
                      catch (Exception ex)
                      {
                        _log.Error(ex.Message, ex);
                      };
                    };
                  };
                };
              };

              return columnValues;
            }
        */
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        // Shared For Both Methods XPOMode and DataTable Mode

        /// <summary>
        /// Initialize ListStore Model with Columns for Both methods, XPOMode and DataTable Mode
        /// </summary>
        /// <param name="pColumnProperties"></param>
        /// <param name="pGenericTreeViewMode"></param>
        /// <returns></returns>
        public static ListStore InitModel(List<GenericTreeViewColumnProperty> pColumnProperties, GenericTreeViewMode pGenericTreeViewMode)
        {
            //Parameters
            List<GenericTreeViewColumnProperty> _columnProperties = pColumnProperties;
            GenericTreeViewMode _treeViewMode = pGenericTreeViewMode;

            ////Only Add System Columns RowIndex, RowCheckBox, and Oid if Not Created First Time, Skip when ReCreate Models
            bool addSystemColumns = (_columnProperties[0].Name != "RowIndex") ? true : false;

            if (addSystemColumns)
            {
                //Insert RowIndex before all Fields
                _columnProperties.Insert(0, new GenericTreeViewColumnProperty("RowIndex") { Type = typeof(Int32), Visible = _showSystemColumns });

                //Hide Oid Field, Required Fields, Used to access collection XPGuidObjects with Lookup(Oid)
                Boolean hasOid = _columnProperties.Contains(new GenericTreeViewColumnProperty("Oid"));

                //Prepare ViewMode
                switch (_treeViewMode)
                {
                    case GenericTreeViewMode.Default:
                        //If miss Oid in ColumnProperties Definition add it After CheckBox
                        if (!hasOid) _columnProperties.Insert(1, new GenericTreeViewColumnProperty("Oid") { Visible = _showSystemColumns });
                        break;
                    //Add Column CheckBox
                    case GenericTreeViewMode.CheckBox:
                        _columnProperties.Insert(1, new GenericTreeViewColumnProperty("RowCheckBox") { Title = string.Empty, PropertyType = GenericTreeViewColumnPropertyType.CheckBox, MinWidth = 50, MaxWidth = 50 });
                        //If miss Oid in ColumnProperties Definition add it After CheckBox
                        if (!hasOid) _columnProperties.Insert(2, new GenericTreeViewColumnProperty("Oid") { Visible = _showSystemColumns });
                        break;
                    default:
                        break;
                }
            }

            //Define array de Types
            Type[] columnTypes = new Type[_columnProperties.Count];

            //Generate columnTypes for ListStore : typeof(string) (G_TYPE_INT, G_TYPE_STRING, GDK_TYPE_PIXBUF)
            for (int i = 0; i < _columnProperties.Count; i++)
            {
                if (_columnProperties[i].PropertyType == GenericTreeViewColumnPropertyType.CheckBox)
                {
                    columnTypes[i] = typeof(bool);
                }
                else
                {
                    //Required to Get Type Int32 from Column, else we cant use it has Int
                    if (_columnProperties[i].Name == "RowIndex")
                    {
                        columnTypes[i] = _columnProperties[i].Type;
                    }
                    else
                    {
                        columnTypes[i] = typeof(string);
                    }
                };
            };

            ListStore resultListStore = new ListStore(columnTypes);

            return resultListStore;
        }
    }
}
