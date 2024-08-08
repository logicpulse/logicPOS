using Gtk;
using logicpos.Classes.Gui.Gtk.WidgetsGeneric;
using System;
using System.Collections.Generic;
using System.Data;
using LogicPOS.Globalization;
using LogicPOS.Data.XPO.Utility;
using LogicPOS.UI.Components;

namespace logicpos.Classes.Gui.Gtk.BackOffice
{
    internal class TreeViewTerminalSeries : GridViewDataTable
    {
        //Public Parametless Constructor Required by Generics
        public TreeViewTerminalSeries() { }

        public TreeViewTerminalSeries(Window parentWindow)
            : this(parentWindow, null, null) { }

        //DataTable Mode
        public TreeViewTerminalSeries(Window parentWindow, DataRow pDefaultValue, Type pDialogType, GridViewMode pGenericTreeViewMode = GridViewMode.Default, GridViewNavigatorMode navigatorMode = GridViewNavigatorMode.Default)
        {
            //Init Vars
            DataRow defaultValue = (pDefaultValue != null) ? pDefaultValue : null;
            //Override Default DialogType with Parameter Dialog Type, this way we can have diferent DialogTypes for GenericTreeView
            Type typeDialogClass = (pDialogType != null) ? pDialogType : null;

            //Configure columnProperties
            List<GridViewColumnProperty> columnProperties = new List<GridViewColumnProperty>
            {
                /*00*/
                new GridViewColumnProperty("Oid") { Type = typeof(Guid), Visible = false },
                /*01*/
                new GridViewColumnProperty("Code") { Type = typeof(uint), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_record_code") },
                /*02*/
                new GridViewColumnProperty("Designation") { Type = typeof(string), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_designation"), Expand = true },
                /*03*/
                new GridViewColumnProperty("HardwareId") { Type = typeof(string), Title = CultureResources.GetResourceByLanguage(LogicPOS.Settings.CultureSettings.CurrentCultureName, "global_hardware_id"), MinWidth = 200 }
            };

            //init DataTable
            DataTable dataTable = GetDataTable(columnProperties);

            //Call Base Initializer
            base.InitObject(
              parentWindow,                    //Pass parameter 
              pDefaultValue,                    //Pass parameter 
              pGenericTreeViewMode,             //Pass parameter 
              navigatorMode,    //Pass parameter 
              columnProperties,                 //Created Here
              dataTable,                        //Created Here
              typeDialogClass                   //Created Here
            );
        }

        private DataTable GetDataTable(List<GridViewColumnProperty> pColumnProperties)
        {
            //Init Local Vars
            string sql = @"
                SELECT 
                    Oid,Code,Designation,HardwareId
                FROM 
                    pos_configurationplaceterminal 
                WHERE 
                    (Disabled IS NULL OR Disabled  <> 1)
                ORDER BY 
                    Ord;
            ";
            DataTable resultDataTable = XPOUtility.GetDataTableFromQuery(sql);

            return resultDataTable;
        }
    }
}
